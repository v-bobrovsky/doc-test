using System;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;

using Documents.Data;
using Documents.Common;
using Documents.DataAccess;
using Documents.Utils;

namespace Documents.Services.Impl
{
    /// <summary>
    /// Provides repository functionality for CRUD operation.
    /// </summary>
    public abstract class RepositoryService<TEntityDto, TIdentity> 
        : IRepositoryService<TEntityDto, TIdentity>
        where TEntityDto : class
    {
        #region Members

        protected readonly ILogger _logger;

        #endregion

        #region Virtual validations methods

        protected virtual bool ValidateNewEntity(TEntityDto entityDto)
        {
            return (entityDto != null);
        }

        protected virtual bool ValidateExistEntity(TEntityDto entityDto)
        {
            return (entityDto != null);
        }

        protected virtual bool ValidateEntityKey(TIdentity id)
        {
            return (id != null);
        }

        #endregion

        #region Abstract methods

        protected abstract TEntityDto OnCreate(TEntityDto entityDto);
        protected abstract TEntityDto OnGet(PermissionsContext ctx, TIdentity id);
        protected abstract IEnumerable<TEntityDto> OnGetAll(PermissionsContext ctx, params object[] args);
        protected abstract TEntityDto OnUpdate(TEntityDto entityDto);
        protected abstract bool OnDelete(TIdentity id);

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public RepositoryService()
        {
            _logger = ObjectContainer.Resolve<SimpleLogger>();
        }

        /// <summary>
        /// Performs the specified action
        /// </summary>
        /// <param name="action">Action to perform</param>
        private void PerformAction(Action action)
        {
            try
            {
                _logger.LogInfo(String.Format("Performing service: {0}.{1}", 
                    this.GetType().Name, action.Method.Name));

                 action();
            }
            catch (Exception e)
            {
                _logger.LogError(e);
                throw e;
            }
        }

        /// <summary>
        /// Creates a new entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// <see cref="TEntityDto"/>s object containing the new entity.
        /// </returns>
        public TEntityDto Create(TEntityDto entityDto)
        {
            TEntityDto result = null;

            PerformAction(() =>
            {
                if (ValidateNewEntity(entityDto))
                {
                    result = OnCreate(entityDto);
                }
                else
                {
                    _logger.LogError("Validation error for entity:");
                    _logger.LogErrorObject(entityDto);
                }                   
            });

            return result;
        }

        /// <summary>
        ///  Retrieves a specific entity
        /// </summary>
        /// <param name="ctx">Contains information of current user</param>
        /// <param name="id">Entity unique identifier</param>
        /// <returns>
        /// <see cref="TEntityDto"/>s object containing the entity.
        /// </returns>
        public TEntityDto Get(PermissionsContext ctx, TIdentity id)
        {
            TEntityDto result = null;

            PerformAction(() =>
            {
                if (ValidateEntityKey(id))
                {
                    result = OnGet(ctx, id);
                }
                else
                {
                    _logger.LogError("Validation error for identity:");
                    _logger.LogErrorObject(id);
                }
                    
            });

            return result;
        }

        /// <summary>
        /// Retrieves list of specific DTO's entities.
        /// </summary>
        /// <param name="ctx">Contains information of current user</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IEnumerable<TEntityDto> GetAll(PermissionsContext ctx, params object[] args)
        {
            IEnumerable<TEntityDto> result = null;

            PerformAction(() =>
            {
                result = OnGetAll(ctx, args);
            });

            return result;
        }

        /// <summary>
        /// Updates a specific DTO
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// <see cref="TEntityDto"/>s object containing the changed entity.
        /// </returns>
        public TEntityDto Update(TEntityDto entityDto)
        {
            TEntityDto result = null;

            PerformAction(() =>
            {
                if (ValidateExistEntity(entityDto))
                {
                    result = OnUpdate(entityDto);
                }
                else
                {
                    _logger.LogError("Validation error for entity:");
                    _logger.LogErrorObject(entityDto);
                }
            });

            return result;
        }

        /// <summary>
        /// Delete a specific DTO
        /// </summary>
        /// <param name="id">Entity unique identifier</param>
        /// <returns></returns>
        public bool Delete(TIdentity id)
        {
            bool result = false;

            PerformAction(() =>
            {
                if (ValidateEntityKey(id))
                {
                    result = OnDelete(id);
                }
                else
                {
                    _logger.LogError("Validation error for identity:");
                    _logger.LogErrorObject(id);
                }
            });

            return result;
        }
    }
}