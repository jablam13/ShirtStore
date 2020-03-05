using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using Dapper;
using Microsoft.Extensions.Options;
using StoreModel.Generic;
using StoreRepository.Interface;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace StoreRepository
{
    public class BaseRepository : IBaseRepository
    {
        private readonly AppSettings _appSettings;
        private readonly string connectionString;
        protected readonly int siteId;

        public BaseRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            connectionString = _appSettings.ConnectionString.ToString();
            siteId = _appSettings.SiteId;
        }

        internal IDbConnection SqlConnection => new SqlConnection(connectionString);

        public IEnumerable<T> Query<T>(string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null, [CallerFilePath] string fromFile = null, [CallerLineNumber] int fromLine = 0)
        {
            dynamic results;
            var connection = new SqlConnection(connectionString);
            sql = MarkSqlString(sql, fromFile, fromLine);

            try
            {
                results = SqlMapper.Query<T>(connection, sql, param, transaction, buffered, commandTimeout, commandType);
            }
            catch (Exception e)
            {
                var exception = new Exception("Sql Query Error", e);
                exception.Data["Parameters"] = param != null ? ToPropertiesString(param) : "None";
                throw exception;
            }
            return results;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerFilePath] string fromFile = null, [CallerLineNumber] int fromLine = 0)
        {
            dynamic results;
            var connection = new SqlConnection(connectionString);
            sql = MarkSqlString(sql, fromFile, fromLine);

            try
            {
                results = await SqlMapper.QueryAsync<T>(connection, sql, param, transaction, commandTimeout, commandType).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var exception = new Exception("Sql Query Error", e);
                exception.Data["Parameters"] = param != null ? ToPropertiesString(param) : "None";
                throw exception;
            }
            return results;
        }

        public int Execute(string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerFilePath] string fromFile = null, [CallerLineNumber] int fromLine = 0)
        {
            sql = MarkSqlString(sql, fromFile, fromLine);

            try
            {
                return SqlMapper.Execute(SqlConnection, sql, param, transaction, commandTimeout, commandType);
            }
            catch (Exception e)
            {
                var exception = new Exception("Sql Execute Error", e);
                exception.Data["Parameters"] = param != null ? ToPropertiesString(param) : "None";
                throw exception;
            }
        }

        public SqlMapper.GridReader QueryMultiple(string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerFilePath] string fromFile = null, [CallerLineNumber] int fromLine = 0)
        {
            sql = MarkSqlString(sql, fromFile, fromLine);

            SqlMapper.GridReader results;
            try
            {
                results = SqlMapper.QueryMultiple(SqlConnection, sql, param, transaction, commandTimeout, commandType);
            }
            catch (Exception e)
            {
                var exception = new Exception("Sql Query Error", e);
                exception.Data["Parameters"] = param != null ? ToPropertiesString(param) : "None";
                throw exception;
            }
            return results;
        }

        private string MarkSqlString(string sql, string fromFile, int fromLine)
        {
            if (string.IsNullOrEmpty(fromFile) || fromLine == 0) return sql;

            var split = fromFile.LastIndexOf(@"\", StringComparison.Ordinal) - 1;
            if (split < 0) return sql;

            split = fromFile.LastIndexOf(@"\", split, StringComparison.Ordinal);
            if (split < 0) return sql;

            split++;

            var metaComment = string.Format("/* {0}@{1} */{2}", fromFile.Substring(split), fromLine, sql.StartsWith(Environment.NewLine) ? "" : Environment.NewLine);

            return string.Format("{0}{1}", metaComment, sql);
        }

        private static string ToPropertiesString(object obj)
        {
            var sb = new StringBuilder();
            foreach (System.Reflection.PropertyInfo property in obj.GetType().GetProperties())
            {
                sb.Append(property.Name);
                sb.Append(": ");
                if (property.GetIndexParameters().Length > 0)
                {
                    sb.Append("Indexed Property cannot be used");
                }
                else
                {
                    sb.Append(property.GetValue(obj, null));
                }

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
