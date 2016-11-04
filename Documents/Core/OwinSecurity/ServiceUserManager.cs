using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using Documents.Data;
using Documents.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Documents.Utils;
using Documents.Services.Impl;
using System.Collections.Generic;
using System.Security.Claims;

namespace Documents.Core
{
    /// <summary>
    /// Service user manager for users where the primary key for the User is of type string
    /// </summary>
    public class ServiceUserManager : UserManager<ServiceUser>
    {
        #region Members

        protected readonly ILogger _logger;

        #endregion
       
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="store"></param>
        public ServiceUserManager(ILogger logger, IUserStore<ServiceUser> store)
            : base(store)
        {
            this._logger = logger;
            this.PasswordHasher = new ServiceUserPasswordHasher();
        }

        /// <summary>
        /// Create user manager
        /// </summary>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ServiceUserManager Create(IdentityFactoryOptions<ServiceUserManager> options, IOwinContext context)
        {
            var logger = UnityConfig.Resolve<SimpleLogger>();
            var userService = UnityConfig.Resolve<UserService>();
            var manager = new ServiceUserManager(logger, new ServiceUserStore(logger, userService));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ServiceUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            return manager;
        }

        /// <summary>
        /// Creates a ClaimsIdentity representing the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="authenticationType"></param>
        /// <returns></returns>
        public override Task<ClaimsIdentity> CreateIdentityAsync(ServiceUser user, string authenticationType)
        {
            var task = Task
                .Run<ClaimsIdentity>(() =>
            {
                var result = new ClaimsIdentity(
                    authenticationType,
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                result.AddClaim(
                    new Claim(ClaimTypes.NameIdentifier,
                        user.Id,
                        ClaimValueTypes.String));

                result.AddClaim(
                    new Claim(ClaimsIdentity.DefaultNameClaimType,
                        user.ToDto().Login,
                        ClaimValueTypes.String));

                result.AddClaim(
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                        "OWIN Provider",
                        ClaimValueTypes.String));

                result.AddClaim(
                    new Claim(ClaimsIdentity.DefaultRoleClaimType,
                        user.ToDto().UserRole, 
                        ClaimValueTypes.String));

                return result;
            });

            return task;	
        }

        /// <summary>
        /// Returns true if the user is in the specified role
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public override Task<bool> IsInRoleAsync(string userId, string role)
        {
            var task = Task
                .Run<bool>(() =>
            {
                var result = false;

                Store
                    .FindByIdAsync(userId)
                    .ContinueWith(t => 
                     {
                         var serviceUser = t.Result;

                         if (serviceUser != null && !String.IsNullOrEmpty(role))
                         {
                             var userDto = serviceUser
                                 .ToDto();

                             result = (userDto.UserRole.ToLower() == role.ToLower().Trim());
                         }
                     });

                return result;
            });

            return task;
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public override Task<IdentityResult> UpdateAsync(ServiceUser user)
        {
            var task = Task
                .Run<IdentityResult>(() =>
            {
                IdentityResult result = null;

                try
                {
                    Store
                        .UpdateAsync(user)
                        .Wait();

                    result = IdentityResult
                        .Success;
                }
                catch (Exception e)
                {
                    result = IdentityResult
                        .Failed(e.Message);
                }

                return result;
            });

            return task;
        }

        /// <summary>
        /// Create a user with no password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public override Task<IdentityResult> CreateAsync(ServiceUser user)
        {
            var task = Task
                .Run<IdentityResult>(() =>
            {
                IdentityResult result = null;

                try
                {
                    Store
                        .CreateAsync(user)
                        .Wait();

                    result = IdentityResult
                        .Success;
                }
                catch (Exception e)
                {
                    result = IdentityResult
                        .Failed(e.Message);
                }

                return result;
            });

            return task;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public override Task<IdentityResult> DeleteAsync(ServiceUser user)
        {
            var task = Task
                .Run<IdentityResult>(() =>
            {
                IdentityResult result = null;

                try
                {
                    Store
                        .DeleteAsync(user)
                        .Wait();

                    result = IdentityResult
                        .Success;
                }
                catch (Exception e)
                {
                    result = IdentityResult
                        .Failed(e.Message);
                }

                return result;
            });

            return task;
        }

        /// <summary>
        /// Returns the roles for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public override Task<IList<string>> GetRolesAsync(string userId)
        {
            var task = Task
                .Run<IList<string>>(() =>
            {
                IList<string> result = null;

                var findUserTask = Store
                    .FindByIdAsync(userId);

                findUserTask
                    .Wait();

                var serviceUser = findUserTask
                    .Result;

                if (serviceUser != null)
                {
                    var userDto = serviceUser
                        .ToDto();

                    result = new List<string>()
                    {
                        userDto.UserRole
                    };
                }

                return result;
            });

            return task;
        }

        /// <summary>
        ///  Find a user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public override Task<ServiceUser> FindByIdAsync(string userId)
        {
            return Store
                .FindByIdAsync(userId);
        }

        /// <summary>
        /// Find a user by user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override Task<ServiceUser> FindByNameAsync(string userName)
        {
            return Store
                .FindByNameAsync(userName);
        }
        
        /// <summary>
        ///  Return a user with the specified username and password or null if there is 
        ///  no match.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override Task<ServiceUser> FindAsync(string userName, string password)
        {
            var task = Task
                .Run<ServiceUser>(() =>
            {
                ServiceUser result = null;

                var findUserTask = Store
                    .FindByNameAsync(userName);

                findUserTask
                    .Wait();

                var user = findUserTask
                    .Result;

                if (user != null && 
                    PasswordHasher.VerifyHashedPassword(user.ToDto().Password, 
                        password) == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    result = user;
                }

                return result;
            });

            return task;
        }
    }
}