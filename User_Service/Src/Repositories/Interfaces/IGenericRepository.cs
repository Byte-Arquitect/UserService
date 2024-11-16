using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace User_Service.Src.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<List<TEntity>> Get(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");

        Task<TEntity?> GetByID(object id);

        Task<TEntity> Insert(TEntity entity);

        Task Delete(object id);

        Task Delete(TEntity entityToDelete);

        Task SoftDelete(object id);

        Task SoftDelete(TEntity entityToDelete);

        Task<TEntity> Update(TEntity entityToUpdate);
    }
}
