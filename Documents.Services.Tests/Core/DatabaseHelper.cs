using Documents.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Documents.Services.Tests.Core
{
    /// <summary>
    /// Database helper class
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Has database created
        /// </summary>
        private static bool _hasSetuped = false;

        /// <summary>
        /// Install new database if need
        /// </summary>
        public static bool SetupDatabaseIfNeed()
        {
            var result = false;

            if (!_hasSetuped)
            {
                _hasSetuped = true;

                using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
                {
                    unitOfWork.CreateNewDatabase();

                    using (var scope = new TransactionScope())
                    {
                        unitOfWork.Save();
                        scope.Complete();
                    }
                }

                result = true;
            }

            return result;
        }

        /// <summary>
        /// Clean up all data except test user
        /// </summary>
        /// <param name="testUserLogin"></param>
        public static void Cleanup(string testUserLogin)
        {
            using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
            {
                using (var scope = new TransactionScope())
                {
                    var deletedDocuments = unitOfWork
                        .DocumentRepository
                        .GetAll()
                        .ToList();

                    deletedDocuments.ForEach(d => unitOfWork
                        .DocumentRepository
                        .Delete(d));

                    var deletedUsers = unitOfWork
                        .UserRepository
                        .GetAll()
                        .Where(u => u.Login != testUserLogin)
                        .ToList();

                    deletedUsers.ForEach(u => unitOfWork
                        .UserRepository
                        .Delete(u));

                    unitOfWork.Save();
                    scope.Complete();
                }
            }
        }
    }
}