using System;
namespace ABL.Enums
{
    public class DescriptionAttribute : Attribute
    {
        string name = null;
        public DescriptionAttribute(string name) { this.name = name; }
        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}