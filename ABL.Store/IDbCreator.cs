using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace ABL.Store
{
    public interface IDbCreator
    {
        DbParameter CreateParameter(string name, object value);

        DbParameter CreateParameter();

        DbCommandBuilder CreateCommandBuilder();

        DbDataAdapter CreateDataAdapter();

        DbCommand CreateCommand();

        DbConnection CreateConnection();
    }
}
