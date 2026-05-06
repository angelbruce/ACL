using ABL.Data;
using System.Reflection;

namespace ABL.Store
{
    /// <summary>
    /// 数据库结构检查
    /// </summary>
    public class DataBaseSchema
    {
        private DbDriver creator;
        private DataBase database;
        public DataBaseSchema(DbDriver creator)
        {
            this.creator = creator;
            database = new DataBase(creator);
        }
        public bool Initialize()
        {
            return InitializeAsync().Result;
        }

        /// <summary>
        /// initliaze the new data structure with po if needed.
        /// </summary>
        private async Task<bool> InitializeAsync()
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();

            if (asms == null || asms.Length == 0) return await Task.FromResult(true);
            var schemas = new List<Schema>();
            var tableSet = new HashSet<string>();
            var schemaMap = new Dictionary<string, Schema>();
            foreach (var asm in asms)
            {
                await LoadPOSchemas(asm, tableSet, schemaMap, schemas);
            }

            var dbSchemas = creator.FetchSchemas(database);

            await CompareState(dbSchemas, schemas);
            return await SyncSchemas(schemas);
        }

        /// <summary>
        /// compare db schemas & po schemas and set the schemas entry state if needed.
        /// </summary>
        /// <param name="dbSchemas"></param>
        /// <param name="poSchemas"></param>
        /// <returns></returns>
        private async Task CompareState(List<Schema> dbSchemas, List<Schema> poSchemas)
        {
            var dbSchemaMaps = dbSchemas.ToDictionary(x => x.Name, y => y);
            var poSchemaMaps = poSchemas.ToDictionary(x => x.Name, y => y);
            //SCHEMA COMPARE && SET STATE
            foreach (var item in poSchemaMaps)
            {
                var schemaName = item.Key;
                var poSchema = item.Value;
                if (!dbSchemaMaps.ContainsKey(schemaName))
                {
                    poSchema.State = Object.EnumEntityState.Added;
                    foreach (var table in poSchema.Tables)
                    {
                        table.State = Object.EnumEntityState.Added;
                    }

                    continue;
                }

                //TABLE COMPARE && SET STATE
                var dbSchema = dbSchemaMaps[schemaName];
                var poTableMap = poSchema.Tables.ToDictionary(x => x.Name, y => y);
                var dbTableMap = dbSchema.Tables.ToDictionary(x => x.Name, y => y);
                foreach (var poTableItem in poTableMap)
                {
                    var tableName = poTableItem.Key;
                    var poTable = poTableItem.Value;
                    if (!dbTableMap.ContainsKey(tableName))
                    {
                        poTable.State = Object.EnumEntityState.Added;
                        foreach (var col in poTable.Columns)
                        {
                            col.State = Object.EnumEntityState.Added;
                        }

                        continue;
                    }

                    //COLUMNS COMPARE && SET STATE
                    var dbTable = dbTableMap[tableName];
                    var poColumnMap = poTable.Columns.ToDictionary(x => x.Name, y => y);
                    var dbColumnMap = dbTable.Columns.ToDictionary(x => x.Name, y => y);

                    foreach (var poColumnItem in poColumnMap)
                    {
                        var columnName = poColumnItem.Key;
                        var poColumn = poColumnItem.Value;
                        if (!dbColumnMap.ContainsKey(columnName))
                        {
                            poColumn.State = Object.EnumEntityState.Added;
                            continue;
                        }

                        var dbColumn = dbColumnMap[columnName];
                        if (dbColumn.IsPK != poColumn.IsPK
                            || dbColumn.DataType.ToLower() != poColumn.DataType.ToLower()
                            )
                        {
                            dbColumn.State = Object.EnumEntityState.Modified;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// SYNC NEW STRUCTRE TO DATABASE WITH NEW DECLARATION OF PO
        /// </summary>
        /// <param name="schemas"></param>
        /// <returns></returns>
        private async Task<bool> SyncSchemas(List<Schema> schemas)
        {
            var schemaSqlList = new List<string>();
            var tableSqlList = new List<string>();
            var schSet = new HashSet<string>();
            var tableSet = new HashSet<string>();
            foreach (var schema in schemas)
            {
                if (!schSet.Contains(schema.Name) && schema.Name.Length > 0)
                {
                    var schemaDdl = creator.CreateSchemaDDL(schema);
                    schemaSqlList.Add(schemaDdl);
                }

                foreach (var table in schema.Tables)
                {
                    var key = $"{schema.Name}/{table.Name}";
                    if (tableSet.Contains(key)) continue;
                    tableSet.Add(key);
                    var tableDdl = creator.CreateTableDDL(table);
                    tableSqlList.AddRange(tableDdl);
                }
            }

            var schemaCreated = true;
            try
            {
                schemaCreated = await InvokeDDL(schemaSqlList);
            }
            catch
            {
                return false;
            }

            if (schemaCreated)
            {
                try
                {
                    return await InvokeDDL(tableSqlList);
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// INVOKE DDL
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        private async Task<bool> InvokeDDL(List<string> sqls)
        {
            if (sqls == null || sqls.Count == 0) return true;

            return await Task.Run(() =>
            {
                try
                {
                    foreach (var sql in sqls)
                    {
                        database.ExecuteNonQuery(sql);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            });
        }

        /// <summary>
        /// LOAD SCHEMAS FROM PLAIN OBJECT FROM ASSMEBY TYPES WHICH IS PUBLIC
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="tableSet"></param>
        /// <param name="schemaMap"></param>
        /// <param name="schemas"></param>
        /// <returns></returns>
        private async Task<bool> LoadPOSchemas(Assembly assembly, HashSet<string> tableSet, Dictionary<string, Schema> schemaMap, List<Schema> schemas)
        {
            var types = assembly.GetTypes();
            if (types == null || types.Length == 0) return true;

            foreach (var type in types)
            {
                var tableAttr = type.GetCustomAttribute<TableAttribute>();
                if (tableAttr == null) continue;

                string tableName = string.Empty;
                tableName = tableAttr.Name ?? string.Empty;
                if (tableName == null || tableName.Length == 0) continue;

                string schemaName = string.Empty;
                var schemaAttr = type.GetCustomAttribute<SchemaAttribute>();
                if (schemaAttr != null) schemaName = schemaAttr.Name ?? string.Empty;

                var key = $"{schemaName}/{tableName}";
                if (tableSet.Contains(key)) continue;
                tableSet.Add(key);

                var schema = schemaMap.GetValueOrDefault(schemaName, null);
                if (schema == null)
                {
                    schema = new Schema() { Name = schemaName };
                    schemaMap[schemaName] = schema;
                    schemas.Add(schema);
                }


                var columns = LoadTableColumns(type);
                if (columns != null && columns.Count > 0)
                {
                    var table = new Table
                    {
                        Name = tableName,
                        Description = tableAttr.Description,
                        Columns = columns
                    };

                    schema.Tables.Add(table);
                }
            }

            return true;
        }

        /// <summary>
        /// LOAD PLAIN OBJECT TABLE AND COLUMNS.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<Column> LoadTableColumns(Type type)
        {
            var cols = new List<Column>();
            var props = type.GetProperties();

            if (props == null || props.Length == 0) return cols;

            foreach (var prop in props)
            {
                if (prop.IsSpecialName || !prop.CanWrite) continue;
                var fieldType = prop.PropertyType;
                var fieldAttr = prop.GetCustomAttribute<ColumnAttribute>();
                if (fieldAttr == null) continue;

                var fieldName = fieldAttr.Name;
                if (fieldName == null || fieldName.Length == 0) fieldName = prop.Name;

                var dataType = creator.GetDataType(fieldType);

                var column = new Column
                {
                    Name = fieldName,
                    Description = fieldAttr.Description,
                    IsPK = fieldAttr.IsPK,
                    KeyGenerateStrategy = fieldAttr.KeyGenerateStrategy,
                    DataType = dataType
                };

                cols.Add(column);
            }

            return cols;
        }
    }


}
