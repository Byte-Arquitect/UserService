using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using User_Service.Src.Models;
using User_Service.Src.Repositories.Interfaces;

namespace User_Service.Src.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly Expression<Func<User, bool>> softDeleteFilter = x => x.DeletedAt == null;

        public UserRepository(DataContext context)
            : base(context)
        {
            InitializeDatabase();
        }

        public void InitializeDatabase()
        {
            context.Database.EnsureCreated();
        }

        public async Task<User?> GetByEmail(string email)
        {
            var user = await dbSet
                .Where(softDeleteFilter)
                .FirstOrDefaultAsync(x => x.Email == email);

            return user;
        }

        public async Task<User?> GetByRut(string rut)
        {
            var user = await dbSet.Where(softDeleteFilter).FirstOrDefaultAsync(x => x.RUT == rut);
            return user;
        }

        public async Task<List<UserProgress?>> GetUserProgress(string id)
        {
            var userProgress = await context
                .UserProgresses.Where(x => x.UserId.ToString() == id)
                .ToListAsync();
            return userProgress;
        }
    }
}
