using System.Threading;
using System.Threading.Tasks;

namespace App.Core.Queries
{
    public interface IQueryHandler<in TQuery, TResponse> 
        where TQuery: IQuery<TResponse>
    {
        /// <summary>
        /// Handler for query
        /// </summary>
        /// <param name="query">IQuery type</param>
        /// <param name="cancellationToken">CancellationToken(optional)</param>
        /// <returns name="TResponse"></returns>
        Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}