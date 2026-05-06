using ABL.Access;
using System;
using System.Collections.Generic;
using System.Text;

namespace ABL.Object
{
    public class JsonReader
    {
        public JsonReader() { }

        public JVal? Parse(string value)
        {
            if (value == null || value.Length == 0) throw new JsonParseError();
            var parser = new JsonParser(value);
            return parser.Parse();
        }

        public JsonObject? ParseObject(string value)
        {
            var jval = Parse(value);
            if (jval == null || jval.Value == null || jval.Type != JsonDataType.Object) return null;

            return (JsonObject)jval.Value;
        }
    }

    class JsonLexError : Exception { }
    class JsonParseError : Exception { }

    class JsonParser
    {
        TokenStream stream;
        public JsonParser(string data)
        {
            stream = new TokenStream(data);
        }

        /// <summary>
        /// parse value
        /// </summary>
        /// <returns></returns>
        /// <exception cref="JsonLexError"></exception>
        /// <exception cref="JsonParseError"></exception>
        public JVal? Parse()
        {
            var next = true;
            var state = State.EOF;
            (next, state) = stream.Next();
            for (; next; (next, state) = stream.Next())
            {
                if (state == State.IllegalChar) throw new JsonLexError();
                var token = stream.Token;
                if (token == null) throw new JsonLexError();

                switch (token.TokenType)
                {
                    case JsonTokenType.LCurly:
                        {
                            var obj = new JsonObject();
                            ParseObject(obj);
                            return new JVal
                            {
                                Type = JsonDataType.Object,
                                Value = obj
                            };
                        }
                    case JsonTokenType.LSquare:
                        {
                            var array = new JsonArray();
                            ParseArray(array);
                            return new JVal
                            {
                                Type = JsonDataType.Array,
                                Value = array
                            };
                        }

                    case JsonTokenType.Identifier:
                        {
                            var val = token.ToString();
                            switch (val)
                            {
                                case "true":
                                    {
                                        return new JVal
                                        {
                                            Type = JsonDataType.Boolean,
                                            Value = new JBool(true)
                                        };
                                    }
                                case "false":
                                    {
                                        return new JVal
                                        {
                                            Type = JsonDataType.Boolean,
                                            Value = new JBool(false)
                                        };
                                    }
                                case "null":
                                    {
                                        return new JVal
                                        {
                                            Type = JsonDataType.Null,
                                            Value = new JNull()
                                        };
                                    }
                                default:
                                    throw new JsonParseError();
                            }
                        }

                    case JsonTokenType.Number:
                        {
                            var val = new JNumeric(token.ToString());
                            return new JVal
                            {
                                Type = JsonDataType.Number,
                                Value = val
                            };
                        }

                    case JsonTokenType.String:
                        {
                            var str = token.ToString();
                            str = str.Substring(1, str.Length - 2);
                            var val = new JStr(str);
                            return new JVal
                            {
                                Type = JsonDataType.String,
                                Value = val
                            };
                        }

                    default:
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// parse array
        /// </summary>
        /// <param name="array"></param>
        /// <exception cref="JsonLexError"></exception>
        /// <exception cref="JsonParseError"></exception>
        private void ParseArray(JsonArray array)
        {
            var next = false;
            var state = State.EOF;

            do
            {
                (next, state) = stream.Next();
                if (state == State.IllegalChar) throw new JsonLexError();
                var token = stream.Token;
                if (token == null) throw new JsonLexError();

                if (token.TokenType == JsonTokenType.RSquare) return;
                if (token.TokenType == JsonTokenType.Comma) continue;

                stream.Keep = true;
                var jval = Parse();
                if (jval == null || jval.Value == null) throw new JsonParseError();
                array.Write(jval.Value);

            } while (next);
        }

        /// <summary>
        /// parse object
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="JsonLexError"></exception>
        /// <exception cref="JsonParseError"></exception>
        private void ParseObject(JsonObject obj)
        {
            var next = false;
            var state = State.EOF;
            (next, state) = stream.Next();
            for (; next; (next, state) = stream.Next())
            {
                if (state == State.IllegalChar) throw new JsonLexError();
                var token = stream.Token;
                if (token == null) throw new JsonLexError();

                //} over
                if (token.TokenType == JsonTokenType.RCurly) return;
                //, continue
                if (token.TokenType == JsonTokenType.Comma) continue;
                //校验类型
                Accept(JsonTokenType.String);
                //name of object
                var name = token.ToString();
                name = name.Substring(1, name.Length - 2);
                //colon
                (next, state) = stream.Next();
                if (!next || state == State.IllegalChar) throw new JsonLexError();
                token = stream.Token;
                if (token == null) throw new JsonLexError();
                Accept(JsonTokenType.Colon);

                // value 
                var jval = Parse();
                if (jval == null || jval.Value == null) throw new JsonParseError();
                //write value
                obj.Write(name, jval.Value);
            }
        }

        private void Accept(JsonTokenType type)
        {
            if (stream.Token == null)
                throw new JsonLexError();

            if (type != stream.Token.TokenType)
                throw new JsonParseError();

        }
    }

    public class JVal
    {
        public JsonDataType Type { get; set; }
        public IJsonWriter Value { get; set; } = new JsonObject();
    }

    public class CharStream
    {
        private string data;
        private int offset = -1;
        private int length = 0;


        public CharStream(string data)
        {
            this.data = data;
            length = data.Length;
        }

        /// <summary>
        /// current char, offset , is end 
        /// </summary>
        /// <returns></returns>
        public (char, int, bool) Next()
        {
            offset++;
            if (offset >= length)
            {
                return (' ', -1, true);
            }

            var ret = (data[offset], offset, false);
            return ret;
        }

        public void Reset(int pos)
        {
            offset = pos;
        }

        public void Step(int step = -1)
        {
            offset += step;
        }

        public string Text(int begin, int end)
        {
            return data.Substring(begin, end - begin + 1);
        }
    }


    enum State
    {
        Next,
        EOF,
        IllegalChar
    }

    /// <summary>
    /// lexer token stream
    /// </summary>
    public class TokenStream
    {
        private CharStream stream;

        private JsonToken? token;

        public bool Keep { get; set; } = false;

        public TokenStream(string data)
        {
            stream = new CharStream(data);
        }

        internal (bool, State) Next()
        {
            if (Keep)
            {
                Keep = false;
                return (true, State.Next);
            }

            var end = false;
            char ch = ' ';
            int pos = 0;
            int startIdx = -1;
            int endIdx = -1;
            (ch, pos, end) = stream.Next();
            for (; !end; (ch, pos, end) = stream.Next())
            {

                startIdx = pos;

                if (ch == '{')
                {
                    endIdx = startIdx;

                    token = new JsonToken(stream)
                    {
                        StartIndex = startIdx,
                        EndIndex = endIdx,
                        TokenType = JsonTokenType.LCurly
                    };

                    return (true, State.Next);
                }


                if (ch == '}')
                {
                    endIdx = startIdx;

                    token = new JsonToken(stream)
                    {
                        StartIndex = startIdx,
                        EndIndex = endIdx,
                        TokenType = JsonTokenType.RCurly
                    };

                    return (true, State.Next);
                }

                if (ch == '[')
                {
                    endIdx = startIdx;

                    token = new JsonToken(stream)
                    {
                        StartIndex = startIdx,
                        EndIndex = endIdx,
                        TokenType = JsonTokenType.LSquare
                    };

                    return (true, State.Next);
                }

                if (ch == ']')
                {
                    endIdx = startIdx;

                    token = new JsonToken(stream)
                    {
                        StartIndex = startIdx,
                        EndIndex = endIdx,
                        TokenType = JsonTokenType.RSquare
                    };

                    return (true, State.Next);
                }

                if (ch == ':')
                {
                    endIdx = startIdx;

                    token = new JsonToken(stream)
                    {
                        StartIndex = startIdx,
                        EndIndex = endIdx,
                        TokenType = JsonTokenType.Colon
                    };

                    return (true, State.Next);
                }

                if (ch == '"')
                {
                    char prev = ch;
                    do
                    {
                        (ch, pos, end) = stream.Next();
                        if (ch == '"' && prev != '\\') break;
                        prev = ch;
                    } while (!end);

                    endIdx = pos;

                    token = new JsonToken(stream)
                    {
                        StartIndex = startIdx,
                        EndIndex = endIdx,
                        TokenType = JsonTokenType.String
                    };

                    return (true, State.Next);
                }

                if (ch == 't')
                {
                    var chars = new char[] { 'r', 'u', 'e' };
                    for (int i = 0; i < chars.Length; i++)
                    {
                        (ch, pos, end) = stream.Next();
                        if (ch != chars[i])
                        {
                            return (false, State.IllegalChar);
                        }
                    }

                    endIdx = pos;

                    token = new JsonToken(stream)
                    {
                        StartIndex = startIdx,
                        EndIndex = endIdx,
                        TokenType = JsonTokenType.Identifier
                    };

                    return (true, State.Next);
                }

                if (ch == 'f')
                {
                    var chars = new char[] { 'a', 'l', 's', 'e' };
                    for (int i = 0; i < chars.Length; i++)
                    {
                        (ch, pos, end) = stream.Next();
                        if (ch != chars[i])
                        {
                            return (false, State.IllegalChar);
                        }
                    }

                    endIdx = pos;

                    token = new JsonToken(stream)
                    {
                        StartIndex = startIdx,
                        EndIndex = endIdx,
                        TokenType = JsonTokenType.Identifier
                    };

                    return (true, State.Next);
                }

                if (ch == 'n')
                {
                    var chars = new char[] { 'u', 'l', 'l' };
                    for (int i = 0; i < chars.Length; i++)
                    {
                        (ch, pos, end) = stream.Next();
                        if (ch != chars[i])
                        {
                            return (false, State.IllegalChar);
                        }
                    }

                    endIdx = pos;

                    token = new JsonToken(stream)
                    {
                        StartIndex = startIdx,
                        EndIndex = endIdx,
                        TokenType = JsonTokenType.Identifier
                    };

                    return (true, State.Next);
                }

                if (ch == '-' || ch == '.' || (ch >= '0' && ch <= '9') || ch == 'e' || ch == 'E')
                {
                    char prev = ch;

                    do
                    {
                        (ch, pos, end) = stream.Next();
                        if (!(ch == '-' || ch == '.' || (ch >= '0' && ch <= '9') || ch == 'e' || ch == 'E'))
                        {
                            stream.Step();
                            break;
                        }

                    } while (!end);

                    endIdx = pos - 1;

                    token = new JsonToken(stream)
                    {
                        StartIndex = startIdx,
                        EndIndex = endIdx,
                        TokenType = JsonTokenType.Number
                    };

                    return (true, State.Next);
                }
            }

            return (false, State.EOF);

        }

        public JsonToken? Token
        {
            get { return token; }
        }
    }

    /// <summary>
    /// json token from lexer
    /// </summary>
    public class JsonToken
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; } = -1;
        public JsonTokenType TokenType { get; set; }
        public CharStream Stream { get; private set; }
        public JsonToken(CharStream stream)
        {
            Stream = stream;
        }

        public override string ToString()
        {
            return Stream.Text(StartIndex, EndIndex);
        }
    }

    /// <summary>
    /// json data type
    /// </summary>
    public enum JsonDataType
    {
        String,
        Boolean,
        Number,
        Null,
        Object,
        Array,
    }

    public enum JsonTokenType
    {
        /// <summary>
        /// {
        /// </summary>
        LCurly,
        /// <summary>
        /// }
        /// </summary>
        RCurly,
        /// <summary>
        /// [
        /// </summary>
        LSquare,
        /// <summary>
        /// ]
        /// </summary>
        RSquare,
        /// <summary>
        /// 封闭字符
        /// </summary>
        Identifier,
        /// <summary>
        /// "..."
        /// </summary>
        String,
        /// <summary>
        /// 数字
        /// </summary>
        Number,
        /// <summary>
        /// :
        /// </summary>
        Colon,
        /// <summary>
        /// ,
        /// </summary>
        Comma,
        /// <summary>
        /// end 
        /// </summary>
        EOF
    }
}