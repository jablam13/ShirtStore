using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StoreRepository.Interface
{
    public interface IBaseRepository
    {
        IEnumerable<T> Query<T>(string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null, [CallerFilePath] string fromFile = null, [CallerLineNumber] int fromLine = 0);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerFilePath] string fromFile = null, [CallerLineNumber] int fromLine = 0);
        int Execute(string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerFilePath] string fromFile = null, [CallerLineNumber] int fromLine = 0);
        SqlMapper.GridReader QueryMultiple(string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, [CallerFilePath] string fromFile = null, [CallerLineNumber] int fromLine = 0);
    }
}
