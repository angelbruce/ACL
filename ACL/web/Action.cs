using ACL.business.agent;
using ACL.dao;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.web
{
    [Controller("data")]
    public class DataController
    {
        [Action("agents")]
        public Msg GetAgents()
        {
            var datastore = new DataStore();
            var agents = datastore.Fill<AgentInfo>();
            if (agents == null) agents = new List<AgentInfo>();
            return Msg.Succed(agents);
        }


        [Action("save")]
        public Msg Save(Model model)
        {
            var store = new DataStore();
            var configs = store.Fill<FlowConfig>();
            var config = configs.Where(x => x.Id == long.Parse(model.id)).FirstOrDefault();
            if (config == null)
            {
                config = new FlowConfig
                {
                    Id = long.Parse(model.id),
                    Desc = Newtonsoft.Json.JsonConvert.SerializeObject(model),
                    State = ABL.Object.EnumEntityState.Added
                };
            }
            else
            {
                config = new FlowConfig
                {
                    Id = long.Parse(model.id),
                    Desc = Newtonsoft.Json.JsonConvert.SerializeObject(model),
                    State = ABL.Object.EnumEntityState.Modified
                };
            }

            store.Save(config);
            return Msg.Succed(true); ;
        }

        [Action("config")]
        public Msg GetFlowConfig(Model model)
        {
            var cid = long.Parse(model.id);
            var store = new DataStore();
            var configs = store.Fill<FlowConfig>();
            var config = configs.Where(x => x.Id == cid).FirstOrDefault();
            Model data = null;
            if (config == null)
            {
                data = model;
            }
            else
            {
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<Model>(config.Desc);
            }

            return Msg.Succed(data);
        }
    }

    public class Model
    {
        public string id;
        public List<Vertex> vertices { get; set; }
        public List<Edge> edges { get; set; }
    }

    public class Vertex
    {
        public string id { get; set; }
        public string value { get; set; }
        public long width { get; set; }
        public long height { get; set; }
        public long x { get; set; }
        public long y { get; set; }
        public string type { get; set; }
        public long? agent { get; set; }
        public string? prompt { get; set; }
        public List<string> paths { get; set; }
        public string? degree { get; set; }
    }

    public class Edge
    {
        public string id { get; set; }
        public string value { get; set; }
        public string src { get; set; }
        public string target { get; set; }
        public string style { get; set; }
    }
}
