using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User_Service.Src.Models;
using User_Service.Src.Protos;

namespace User_Service.Src.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public Task<User?> GetByEmail(string email);
        public Task<User?> GetByRut(string rut);

        public Task<List<UserProgress?>> GetUserProgress(string id);
    }
}
