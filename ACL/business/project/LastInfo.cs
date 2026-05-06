using ABL.Config.Ant;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.business.project
{
    public class LastInfoAnt : BaseAnt
    {
        public override IAntItem Create()
        {
            return new LastUsedInfo();
        }
    }


    [AntRoot("last_useds")]
    [AntPrefix("last_used")]
    public class LastUsedInfo : IAntItem
    {
        [AntElement("session_id")]
        public string SessionId { get; set; } = string.Empty;

        [AntElement("agent_id")]
        public string AgentId { get; set; } = string.Empty;
    }

    public class LastUsedRecorder
    {
        static string LAST_TAG = "last_used";
        public static LastUsedInfo GetLastUsed()
        {
            var items = AntContext.Instance.GetItems(LAST_TAG);
            if (items == null || items.Count == 0)
            {
                return new LastUsedInfo();
            }
            else
            {
                return items.Select(x => (LastUsedInfo)x).ToList().First();
            }
        }

        public static void SaveLastInfo()
        {
            try
            {
                var last = new LastUsedInfo { };

                var session = Context.Instance.CurrentSession;
                last.SessionId = session?.Id.ToString();

                var agent = Context.Instance.CurrentAgent;
                last.AgentId = agent?.Id.ToString();

                var items = new List<IAntItem>() { last };

                AntContext.Instance.SetItems(typeof(LastInfoAnt), LAST_TAG, items);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
