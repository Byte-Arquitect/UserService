using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User_Service.Src.Repositories.Interfaces;

namespace User_Service.Src.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IUserRepository _usersRepository = null!;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        private readonly DataContext _context;
        private bool _disposed = false;


        public IUserRepository UsersRepository
        {
            get
            {
                _usersRepository ??= new UserRepository(_context);
                return _usersRepository;
            }
        }
        
    }
}