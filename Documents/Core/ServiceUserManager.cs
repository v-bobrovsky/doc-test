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
       
        public ServiceUserManager(ILogger logger, IUserStore<ServiceUser> store)
            : base(store)
        {
            this._logger = logger;
            this.PasswordHasher = new ServiceUserPasswordHasher();
        }

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

            var dataProtectionProvider = options.DataProtectionProvider;
            
            //if (dataProtectionProvider != null)
            //{
            //    manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            //}

            return manager;
        }

        public override Task<bool> IsInRoleAsync(string userId, string role)
        {
            var task = Task<bool>.Factory.StartNew(() =>
            {
                var result = false;

                 var serviceUser = Store
                     .FindByIdAsync(userId)
                     .Result;

                if (serviceUser != null && !String.IsNullOrEmpty(role))
                {
                    var userDto = serviceUser.ToDto();
                    result = (userDto.UserRole.ToLower() == role.ToLower().Trim());
                }

                return result;
            });

            return task;
        }

        public override Task<string> GenerateUserTokenAsync(string purpose, string userId)
        {
            var task = Task<string>.Factory.StartNew(() =>
            {
                var result = string.Empty;
                var id = 0;

                if (!String.IsNullOrEmpty(userId) 
                    && !Int32.TryParse(userId, out id))
                    id = 0;

                if (id > 0)
                {
                    var key = new byte[4];
                    
                    BitConverter
                        .GetBytes(id)
                        .CopyTo(key, 0);

                    var time = BitConverter
                        .GetBytes(
                        DateTime.Now
                        .AddDays(1)
                        .ToBinary());

                    var tokenData = new List<byte>();
                    
                    tokenData.AddRange(key);
                    tokenData.AddRange(time);

                    result = Convert
                        .ToBase64String(
                            tokenData.ToArray());
                }

                return result;
            });

            return task;
        }

        public override Task<bool> VerifyUserTokenAsync(string userId, string purpose, string token)
        {
            var task = Task<bool>.Factory.StartNew(() =>
            {
                var result = false;

                if (!String.IsNullOrEmpty(token) && !String.IsNullOrEmpty(userId))
                {
                    try
                    {
                        byte[] bytes = Convert
                            .FromBase64String(token);

                        var key = new byte[4];
                        var time = new byte[(bytes.Length - 4)];

                        BitConverter.ToInt32(bytes, 0);

                        Array.Copy(bytes, 0, key, 0, key.Length);
                        Array.Copy(bytes, 4, time, 0, time.Length);

                        var id = BitConverter.ToInt32(key, 0);
                        var tokenCreated = DateTime.FromBinary(
                            BitConverter.ToInt64(time, 0));

                        result = (id.ToString() == userId.Trim() && tokenCreated >= DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(String.Format("Failed to decrypt user token: {0}", token));
                        _logger.LogError(ex);
                    }
                }

                return result;
            });

            return task;
        }

        public override Task<IdentityResult> UpdateAsync(ServiceUser user)
        {
            var task = Task<IdentityResult>.Factory.StartNew(() =>
            {
                IdentityResult result = null;

                try
                {
                    Store
                        .UpdateAsync(user)
                        .Wait();
                    result = IdentityResult.Success;
                }
                catch
                {
                    result = IdentityResult.Failed();
                }

                return result;
            });

            return task;
        }

        public override Task<IdentityResult> CreateAsync(ServiceUser user)
        {
            var task = Task<IdentityResult>.Factory.StartNew(() =>
            {
                IdentityResult result = null;

                try
                {
                    Store
                        .CreateAsync(user)
                        .Wait();
                    result = IdentityResult.Success;
                }
                catch
                {
                    result = IdentityResult.Failed();
                }

                return result;
            });

            return task;
        }

        public override Task<IdentityResult> DeleteAsync(ServiceUser user)
        {
            var task = Task<IdentityResult>.Factory.StartNew(() =>
            {
                IdentityResult result = null;

                try
                {
                    Store
                        .DeleteAsync(user)
                        .Wait();

                    result = IdentityResult.Success;
                }
                catch
                {
                    result = IdentityResult.Failed();
                }

                return result;
            });

            return task;
        }

        public override Task<IList<string>> GetRolesAsync(string userId)
        {
            var task = Task<IList<string>>.Factory.StartNew(() =>
            {
                IList<string> result = null;

                var serviceUser = Store
                    .FindByIdAsync(userId)
                    .Result;

                if (serviceUser != null)
                {
                    var userDto = serviceUser.ToDto();

                    result = new List<string>()
                    {
                        userDto.UserRole
                    };
                }

                return result;
            });

            return task;
        }


        public override Task<ServiceUser> FindByIdAsync(string userId)
        {
            return Store.FindByIdAsync(userId);
        }

        public override Task<ServiceUser> FindAsync(string userName, string password)
        {
            var task = Task<ServiceUser>.Factory.StartNew(() =>
            {
                ServiceUser result = null;

                if (PasswordHasher.VerifyHashedPassword(
                    userName, password) == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    result = Store
                        .FindByNameAsync(userName)
                        .Result;
                }

                return result;
            });

            return task;
        }
    }
}