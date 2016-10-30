using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.DataAccess
{
    /// <summary>
    /// Override functionality for DocumentsContext
    /// </summary>
    public partial class DocumentsContext : DbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="createNewDatabase">Always create a new database</param>
        public DocumentsContext(bool createNewDatabase)
            : base("name=DocumentsContext")
        {
            Configuration.ProxyCreationEnabled = true;
            Configuration.LazyLoadingEnabled = true;

            if (createNewDatabase)
                Database.SetInitializer<DocumentsContext>(new DropCreateDatabaseAlways<DocumentsContext>());
            else
                Database.SetInitializer<DocumentsContext>(new CreateDatabaseIfNotExists<DocumentsContext>());
        }

        /// <summary>
        /// Get instance with new database
        /// </summary>
        /// <returns></returns>
        public static DocumentsContext CreateNewDatabase()
        {
            return new DocumentsContext(true);
        }

        /// <summary>
        /// Get instance with new database if database not exist
        /// </summary>
        /// <returns></returns>
        public static DocumentsContext CreateDatabaseIfNotExists()
        {
            return new DocumentsContext(false);
        }
    }
}