using System.Threading;
using System.Threading.Tasks;

namespace App.Core.Queries
{
    public interface IQueryProcessor
    {
        Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query,
            CancellationToken cancellationToken = default);
    }
}