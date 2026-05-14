using ABL.Config.Ant;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.business.llm
{
    public class LLMModelFactory
    {
        private const string LLM_TAG = "llm";
        public LLMModelFactory()
        {
        }

        public async Task<bool> SaveModelsAsync(List<LLMModelInfo> llms)
        {
            var items = llms.Select(x => x as IAntItem).ToList();
            AntContext.Instance.SetItems(typeof(LLMAnt), LLM_TAG, items);
            return true;
        }

        public async Task<List<LLMModelInfo>> GetModelsAsync()
        {
            var datas = AntContext.Instance.GetItems(LLM_TAG);
            var list = new List<LLMModelInfo>();
            if (datas != null && datas.Count > 0)
            {
                foreach (var data in datas)
                {
                    if (data is LLMModelInfo llm)
                    {
                        list.Add(llm);
                    }
                }
            }

            return list;
        }
    }
}
