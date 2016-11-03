using System;
using Microsoft.AspNet.Identity;

using Documents.Data;
using Documents.Services;

namespace Documents.Core
{
    /// <summary>
    /// Implements password hashing methods
    /// </summary>
    public class ServiceUserPasswordHasher : PasswordHasher
    {
        public override string HashPassword(string password)
        {
            return password;
        }

        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var result = PasswordVerificationResult.Failed;

            if (hashedPassword == providedPassword)
                result = PasswordVerificationResult.SuccessRehashNeeded;

            return result;
        }
    }
}