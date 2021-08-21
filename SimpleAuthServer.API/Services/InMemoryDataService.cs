using SimpleAuthServer.API.Models;
using SimpleAuthServer.API.Services.Interfaces;
using SimpleAuthServer.API.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Services
{
    public class InMemoryDataService : IUserRepository
    {
        private List<AuthorizedUser> users;
        private readonly ICryptoService cryptoService;
        private readonly IUserStore userStore;

        public InMemoryDataService(ICryptoService cryptoService, IUserStore userStore)
        {
            this.cryptoService = cryptoService;
            this.userStore = userStore;
            this.users = this.userStore.RetrieveUsers() ?? new List<AuthorizedUser>();
        }
        
        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <exception cref="UserAlreadyExistsException">Thrown when the username already exists in the user store.</exception>
        /// <returns></returns>
        public async Task<AuthorizedUser> CreateUser(string username, string password)
        {
            var matchedUser = await FindUserByUserName(username);
            if(matchedUser != null)
            {
                throw new UserAlreadyExistsException();
            }
            else
            {
                AuthorizedUser newUser = new AuthorizedUser()
                {
                    ID = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    PasswordHash = await this.cryptoService.HashPassword(password),
                    Username = username
                };

                users.Add(newUser);
                this.userStore.StoreUsers(users);
                return newUser;
            }
        }

        /// <summary>
        /// Finds a user from the user store by user name.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<AuthorizedUser> FindUserByUserName(string username)
        {
            return users.FirstOrDefault(item => item.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && !item.IsLockedOut);
        }

        /// <summary>
        /// Remove a user from the user store.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> RemoveUser(AuthorizedUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update a user's properties in the user store.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<AuthorizedUser> UpdateUser(AuthorizedUser user)
        {
            throw new NotImplementedException();
        }
    }
}
