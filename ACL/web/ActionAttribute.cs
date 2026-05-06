using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.web
{

    public class Controller: Attribute
    {
        public string Name { get; set; } = string.Empty;


        public Controller() { }

        public Controller(string name)
        {
            Name = name;
        }
    }

    public class ActionAttribute : Attribute
    {
        public string Name { get; set; } = string.Empty;


        public ActionAttribute() { }

        public ActionAttribute(string name)
        {
            Name = name;
        }
    }

    public class Msg
    {
        public bool Success { get; set; } = false;
        public object? Data { get; set; }
        public string? Message { get; set; }


        public static Msg Error(string message)
        {
            return new Msg
            {
                Message = message
            };
        }

        public static Msg Succed(object data)
        {
            return new Msg
            {
                Success = true,
                Data = data
            };
        }
    }

}
