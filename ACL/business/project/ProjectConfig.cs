using ABL.Config.Ant;

namespace ACL.business.project
{
    public class ProjectConfig
    {
        private const string PROJECT_CONFIG_KEY = "project-config";

        private static ProjectConfigInfo? current;
        public static ProjectConfigInfo Current
        {
            get
            {
                if (current == null)
                {
                    var config = AntContext.Instance.GetItems(PROJECT_CONFIG_KEY);
                    if (config != null && config.Count > 0)
                    {
                        foreach (var item in config)
                        {
                            if (item is ProjectConfigInfo projectConfig)
                            {
                                current = projectConfig;
                                break;
                            }
                        }
                    }

                    if (current == null)
                    {
                        current = new ProjectConfigInfo();
                    }
                }

                return current;
            }
            set
            {
                if (value != null)
                {
                    current = value;
                    AntContext.Instance.SetItems(typeof(ProjectAnt), PROJECT_CONFIG_KEY, new List<IAntItem>() { current }, true);
                }
            }
        }
    }
}
