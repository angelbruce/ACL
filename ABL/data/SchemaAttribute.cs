using System;
using System.Collections.Generic;
using System.Text;

namespace ABL.Data
{
    public class SchemaAttribute : Attribute
    {
        public string Name { get; set; } = string.Empty;
    }
}
