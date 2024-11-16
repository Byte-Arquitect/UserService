using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User_Service.Src.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Gets the users repository.
        /// </summary>
        /// <value>A Concrete class for IUsersRepository</value>
        public IUserRepository UsersRepository { get; }
    }
}