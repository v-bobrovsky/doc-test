using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;

using Documents.Utils;
using System.Data.Entity;

namespace Documents.DataAccess
{
    /// <summary>
    /// Unit of Work class responsible for DB transactions
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        #region Members

        private ILogger _logger = null;
        private DocumentsContext _context = null;

        private Lazy<GenericRepository<User>> _userRepository;
        private Lazy<GenericRepository<Document>> _documentRepository;
        private Lazy<GenericRepository<Comment>> _commentRepository;

        /// <summary>
        /// Get/Set Property for user repository.
        /// </summary>
        public GenericRepository<User> UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new Lazy<GenericRepository<User>>(() =>
                    {
                        return new GenericRepository<User>(_context);
                    });
                }

                return _userRepository.Value;
            }
        }

        /// <summary>
        /// Get/Set Property for document repository.
        /// </summary>
        public GenericRepository<Document> DocumentRepository
        {
            get
            {
                if (_documentRepository == null)
                {
                    _documentRepository = new Lazy<GenericRepository<Document>>(() =>
                    {
                        return new GenericRepository<Document>(_context);
                    });
                }

                return _documentRepository.Value;
            }
        }


        /// <summary>
        /// Get/Set Property for comment repository.
        /// </summary>
        public GenericRepository<Comment> CommentRepository
        {
            get
            {
                if (_commentRepository == null)
                {
                    _commentRepository = new Lazy<GenericRepository<Comment>>(() =>
                    {
                        return new GenericRepository<Comment>(_context);
                    });
                }

                return _commentRepository.Value;
            }
        }


        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitOfWork()
        {
            _logger = new SimpleLogger();
            _context = DocumentsContext.CreateDatabaseIfNotExists();           
        }

        #region Public

        /// <summary>
        /// Create a new database
        /// </summary>
        public void CreateNewDatabase()
        {
            _userRepository = null;
            _documentRepository = null;
            _commentRepository = null;

            try
            {
                if (_context.Database.Exists())
                {
                    _context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, 
                        "ALTER DATABASE [" 
                        + _context.Database.Connection.Database 
                        + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");

                    _context.Database.Delete();
                }                   

                _context = DocumentsContext.CreateNewDatabase();
            }
            catch (Exception e)
            {
                _logger.LogError("Create new database error!", e);
                throw e;
            }
        }

        /// <summary>
        /// Save all changers
        /// </summary>
        public void Save()
        {
            try
            {
                _context.SaveChanges();                
            }
            catch (DbEntityValidationException e)
            {
                var sbErrors = new StringBuilder();

                foreach (var entityValidationError in e.EntityValidationErrors)
                {
                    sbErrors.AppendFormat(
                        "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:\r\n", 
                        entityValidationError.Entry.Entity.GetType().Name, 
                        entityValidationError.Entry.State);

                    foreach (var validationError in entityValidationError.ValidationErrors)
                    {
                        sbErrors.AppendFormat(" Property name: \"{0}\", Error: \"{1}\"\r\n", 
                            validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                _logger.LogError(sbErrors.ToString());

                throw e;
            }

        }

        #endregion

        #region IDisposable

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _logger.LogInfo("UnitOfWork is being disposed");
                    _context.Dispose();
                }
            }

            this.disposed = true;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}