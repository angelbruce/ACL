using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ACL.business.llm
{
    public class ModelConfig
    {
        private List<LLMModelInfo>? models;
        private LLMModelFactory factory;
        private LLMModelInfo? current;
        private static ModelConfig? instance;
        private static readonly object lockObj = new object();

        private ModelConfig()
        {
            factory = new LLMModelFactory();
        }


        public static ModelConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new ModelConfig();
                        }
                    }
                }

                return instance;

            }
        }

        public async Task<LLMModelInfo> Current()
        {
            if (current != null)
            {
                return current;
            }

            if (models == null)
            {
                var llmInfos = await factory.GetModelsAsync();
                this.models = llmInfos.ToList();
            }

            var model = this.models.Where(x => x.IsDefault != null && x.IsDefault.Equals("1")).FirstOrDefault();

            if (model == null)
            {
                model = new LLMModelInfo
                {
                    Name = "gemma4-e4b",
                    Version = "1.0",
                    AccessUrl = "http://localhost:8080/v1",
                };
            }

            current = model;
            return current;
        }
    }
}
