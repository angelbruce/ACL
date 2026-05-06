using ABL.Config.Ant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ACL.business.project
{
    public class ProjectAnt : BaseAnt
    {
        public override IAntItem Create()
        {
            return new ProjectConfigInfo();
        }
    }

    [Description("项目基本信息")]
    [AntRoot("projectCollection")]
    [AntPrefix("project")]
    public class ProjectConfigInfo : IAntItem
    {
        [Description("项目名称")]
        [AntElement("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Description("项目所在目录")]
        [AntElement("directory")]
        public string Directory { get; set; } = string.Empty;

        [Description("项目的描述")]
        [AntElement("description")]
        public string Description { get; set; } = string.Empty;
    }
}
