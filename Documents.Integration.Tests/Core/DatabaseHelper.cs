using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using Documents.DataAccess;

namespace Documents.Integration.Tests.Core
{
    /// <summary>
    /// Database helper class
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Clean up all data for test user
        /// </summary>
        /// <param name="testUserLogin"></param>
        public static void CleanupForUser(string testUserLogin)
        {
            if (!String.IsNullOrEmpty(testUserLogin))
            {
                using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
                {
                    var delUser = unitOfWork
                        .UserRepository
                        .GetAll()
                        .Where(u => u.Login == testUserLogin)
                        .FirstOrDefault();

                    if (delUser != null)
                    {
                        using (var scope = new TransactionScope())
                        {
                            var deletedDocuments = unitOfWork
                                .DocumentRepository
                                .GetAll()
                                .Where(d => d.UserId == delUser.UserId)
                                .ToList();

                            deletedDocuments.ForEach(d => unitOfWork
                                .DocumentRepository
                                .Delete(d));

                            unitOfWork.Save();
                            scope.Complete();
                        }

                        using (var scope = new TransactionScope())
                        {
                            var deletedComments = unitOfWork
                                .CommentRepository
                                .GetAll()
                                .Where(c => c.UserId == delUser.UserId)
                                .ToList();

                            deletedComments.ForEach(c => unitOfWork
                                .CommentRepository
                                .Delete(c));

                            unitOfWork
                                .UserRepository
                                .Delete(delUser);

                            unitOfWork.Save();
                            scope.Complete();
                        }
                    }
                }
            }
        }
    }
}