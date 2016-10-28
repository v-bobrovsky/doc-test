using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;

using Documents.Utils;

namespace Documents.DataAccess
{
    /// <summary>
    /// Unit of Work class responsible for DB transactions
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Get/Set Property for user repository.
        /// </summary>
        GenericRepository<User> UserRepository
        {
            get;
        }

        /// <summary>
        /// Get/Set Property for document repository.
        /// </summary>
        GenericRepository<Document> DocumentRepository
        {
            get;
        }

        /// <summary>
        /// Get/Set Property for comment repository.
        /// </summary>
        GenericRepository<Comment> CommentRepository
        {
            get;
        }

        /// <summary>
        /// Save all changers
        /// </summary>
        void Save();
    }
}