using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using User_Service.Src.Exceptions;
using User_Service.Src.Models;
using User_Service.Src.Repositories.Interfaces;

namespace User_Service.Src.Repositories
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected DataContext context;
        protected DbSet<TEntity> dbSet;

        public GenericRepository(DataContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
            InitializeDatabase();
        }

       public void InitializeDatabase()
        {
            context.Database.EnsureCreated();
            if (context.Users.Any())
            {
                return;   // La base de datos ya está poblada
            }

            context.Users.AddRange(
                new User
                {
                    Name = "Jane",
                    FirstLastName = "Doe",
                    SecondLastName = "Smith",
                    RUT = "12345678-9",
                    Email = "jane.doe@example.com",
                    HashedPassword = "hashed_password_123", // Usa un hash real aquí
                    IsEnabled = true,
                    CareerName = "Computer Science",
                    CareerId = 1,
                    RoleName = "User",
                    RoleId = 2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Version = 1
                }
            );
            context.SaveChanges();
        }


        public virtual async Task<List<TEntity>> Get(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;


            if (typeof(BaseModel).IsAssignableFrom(typeof(TEntity)))
            {
#pragma warning disable CS8602 // Already checked in if statement
                query = query.Where(x => (x as BaseModel).DeletedAt == null);
#pragma warning restore CS8602
            }

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy is not null)
            {
                return await orderBy(query).ToListAsync();
            }
            return await query.ToListAsync();
        }

        public virtual async Task<TEntity?> GetByID(object id)
        {
            var entity = await dbSet.FindAsync(id);
            if (entity is BaseModel baseModel && baseModel.DeletedAt is not null)
            {
                return null;
            }
            return entity;

        }

        public virtual async Task<TEntity> Insert(TEntity entity)
        {
            await dbSet.AddAsync(entity);

            await context.SaveChangesAsync();
            return entity;
        }

        public async Task SoftDelete(TEntity entityToDelete)
        {
            if (entityToDelete is BaseModel baseModel)
            {
                if (baseModel.DeletedAt is not null)
                    throw new EntityDeletedException($"Entity: {entityToDelete} cannot be deleted");

                baseModel.DeletedAt = DateTime.UtcNow; // Use UTC
            }

            dbSet.Attach(entityToDelete);
            context.Entry(entityToDelete).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task SoftDelete(object id)
        {
            TEntity? entityToDelete = dbSet.Find(id) ??
                throw new EntityDeletedException($"Entity with Id: {id} cannot be deleted");

            if (entityToDelete is BaseModel baseModel && baseModel.DeletedAt is null)
            {
                throw new EntityDeletedException($"Entity: {entityToDelete} cannot be deleted");
            }
            await SoftDelete(entityToDelete);
        }

        public virtual async Task Delete(object id)
        {
            TEntity? entityToDelete = dbSet.Find(id) ??
                throw new EntityDeletedException($"Entity with Id: {id} cannot be deleted");

            await Delete(entityToDelete);
        }

        public virtual async Task Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);

            var result = await context.SaveChangesAsync() > 0;
            if (!result) throw new EntityDeletedException($"Entity: {entityToDelete} cannot be deleted");
        }

        public virtual async Task<TEntity> Update(TEntity entityToUpdate)
        {
            if (entityToUpdate is BaseModel baseModel)
            {
                baseModel.UpdatedAt = DateTime.UtcNow; // Use UTC
                baseModel.Version++;
            }

            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;

            await context.SaveChangesAsync();
            return entityToUpdate;
        }

    }
}