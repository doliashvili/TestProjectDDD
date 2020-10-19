using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.Core.Queries;

namespace App.Core.Repository
{
    public interface IReadModelRepository<TReadModel, TId> 
        where TReadModel : IReadModel
        where TId: IComparable, IEquatable<TId>
    {
        Task<TReadModel> WriteAsync(TReadModel readModel, CancellationToken cancellationToken = default);
        Task<TReadModel> UpdateAsync(TReadModel updatedObj, CancellationToken cancellationToken = default);
        Task<TReadModel> UpdateAsync(TId id, Action<TReadModel> action, CancellationToken cancellationToken = default);
        Task DeleteAsync(TId id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Expression<Func<TReadModel, bool>> selector, CancellationToken cancellationToken = default);
        Task<TReadModel> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        Task<TReadModel> GetFirstOrDefaultAsync(Expression<Func<TReadModel, bool>> selector, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TReadModel>> QueryListAsync(Expression<Func<TReadModel, bool>> filter = null, int page = 0, int pageSize = 10,
            CancellationToken cancellationToken = default);
        IQueryable<TReadModel> GetQueryable();
        IQueryable<TReadModel> GetQueryable(Expression<Func<TReadModel, bool>> filter);
        Task<long> CountAsync(CancellationToken cancellationToken = default);
        Task<long> CountAsync(Expression<Func<TReadModel, bool>> expression,
            CancellationToken cancellationToken = default);
    }
}
