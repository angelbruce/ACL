using ABL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Store
{
    class AttributeSolver : IMapSolver
    {
        public MapInfo Interpret(Type type)
        {
            var map = new MapInfo();
            var attributes = type.GetCustomAttributes(typeof(TableAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                var attr = attributes[0] as TableAttribute;
                map.Table = new TableAttribute(attr.Name, type.Name);
            }

            var props = type.GetProperties().Where(d => !d.IsSpecialName).ToList();
            var fields = new List<ColumnAttribute>();
            foreach (var prop in props)
            {
                var fieldsAttributes = prop.GetCustomAttributes(typeof(ColumnAttribute), true);
                if (fieldsAttributes != null && fieldsAttributes.Length > 0)
                {
                    var attr = fieldsAttributes[0] as ColumnAttribute;
                    if (attr == null) continue;

                    var col = new ColumnAttribute { Name = attr.Name, PropertyName = prop.Name, Type = prop.PropertyType };
                    if (attr.IsPK) map.Pk = col;
                    fields.Add(col);
                }
            }
            map.Fields = fields;
            return map;
        }
    }
}
