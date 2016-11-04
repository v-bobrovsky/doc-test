using System;
using Microsoft.AspNet.Identity;

using Documents.Data;
using Documents.Services;

namespace Documents.Core
{
    /// <summary>
    /// Override password hashing to save password as clean text
    /// </summary>
    public class ServiceUserPasswordHasher : PasswordHasher
    {
        /// <summary>
        /// Hash a password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public override string HashPassword(string password)
        {
            return password;
        }

        /// <summary>
        /// Verify that a password matches the hashedPassword
        /// </summary>
        /// <param name="hashedPassword"></param>
        /// <param name="providedPassword"></param>
        /// <returns></returns>
        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var result = PasswordVerificationResult.Failed;

            if (hashedPassword == providedPassword)
                result = PasswordVerificationResult.SuccessRehashNeeded;

            return result;
        }
    }
}