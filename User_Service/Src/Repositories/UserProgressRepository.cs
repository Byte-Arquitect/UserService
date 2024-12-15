using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User_Service.Src.Models;
using User_Service.Src.Repositories.Interfaces;

namespace User_Service.Src.Repositories
{
    public class UserProgressRepository : GenericRepository<UserProgress>, IUserProgressRepository
    {
        public UserProgressRepository(DataContext context)
            : base(context) { }
    }
}
