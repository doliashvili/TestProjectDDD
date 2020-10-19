using System;
using System.Threading;
using System.Threading.Tasks;
using App.Core.Helpers;

namespace App.Core.Queries.Implementation
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryProcessor(IServiceProvider serviceProvider) 
            => _serviceProvider = serviceProvider;

        public Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            var service =
                _serviceProvider.GetService(
                    typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse)));

            return (Task<TResponse>) service.Invoke("HandleAsync", query, cancellationToken);
        }
    }
}
