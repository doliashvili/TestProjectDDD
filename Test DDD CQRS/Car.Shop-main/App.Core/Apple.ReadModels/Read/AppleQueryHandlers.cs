using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using App.Core.Queries;
using App.Core.Repository;
using App.Infrastructure.RavenDbImplementation;
using Apple.ReadModels.Models;
using Apple.ReadModels.Read.Queries;
using Raven.Client.Documents;

namespace Apple.ReadModels.Read
{
    public class AppleQueryHandlers : 
        IQueryHandler<GetAllApples, IReadOnlyList<AppleReadModel>>
    {
        private readonly IReadModelRepository<AppleReadModel, string> _repo;

        public AppleQueryHandlers(IReadModelRepository<AppleReadModel, string> repo)
        {
            _repo = repo;
        }
        
        public async Task<IReadOnlyList<AppleReadModel>> HandleAsync(GetAllApples query, 
            CancellationToken cancellationToken = default)
        {
            return await _repo.QueryListAsync(filter: null, 0, 50, cancellationToken);
        }
    }
}