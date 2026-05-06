using ABL.Config.Ant;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ACL.business.llm
{
    public class LLMAnt : BaseAnt
    {
        public override IAntItem Create()
        {
            return new LLMModelInfo();
        }
    }


    [AntPrefix("llm")]
    [AntRoot("llmCollection")]
    public class LLMModelInfo : IAntItem
    {
        [AntElement("name")]
        public string Name { get; set; } = string.Empty;

        [AntElement("version")]
        public string Version { get; set; } = string.Empty;

        [AntElement("description")]
        public string Description { get; set; } = string.Empty;

        [AntElement("accessUrl")]
        public string AccessUrl { get; set; } = string.Empty;

        [AntElement("apiKey")]
        public string ApiKey { get; set; } = string.Empty;

        [AntElement("apiSecret")]
        public string ApiSecret { get; set; } = string.Empty;

        [AntElement("default")]
        public string IsDefault { get; set; } = "0";

    }
}
