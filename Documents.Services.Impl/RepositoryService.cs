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
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IUserContext _userCtx;

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

        #region Abstract CRUD methods

        protected abstract TEntityDto OnCreate(TEntityDto entityDto);
        protected abstract TEntityDto OnGet(TIdentity id);
        protected abstract IEnumerable<TEntityDto> OnGetAll(params object[] args);
        protected abstract TEntityDto OnUpdate(TEntityDto entityDto);
        protected abstract bool OnDelete(TIdentity id);

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public RepositoryService(IUserContext userCtx)
        {
            _logger = ObjectContainer.Resolve<SimpleLogger>();
            _unitOfWork = ObjectContainer.Resolve<UnitOfWork>();
            _userCtx = userCtx;
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

            if (ValidateNewEntity(entityDto))
                result = OnCreate(entityDto);
            else
                _logger.LogError("Bad entity structure!");

            return result;
        }

        /// <summary>
        ///  Retrieves a specific entity
        /// </summary>
        /// <param name="id">Entity unique identifier</param>
        /// <returns>
        /// <see cref="TEntityDto"/>s object containing the entity.
        /// </returns>
        public TEntityDto Get(TIdentity id)
        {
            TEntityDto result = null;

            if (ValidateEntityKey(id))
                result = OnGet(id);
            else
                _logger.LogError("Bad entity identity!");

            return result;
        }

        /// <summary>
        /// Retrieves list of specific DTO's entities.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public IEnumerable<TEntityDto> GetAll(params object[] args)
        {
            return OnGetAll(args);
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

            if (ValidateExistEntity(entityDto))
                result = OnUpdate(entityDto);
            else
                _logger.LogError("Bad entity structure!");

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

            if (ValidateEntityKey(id))
                result = OnDelete(id);
            else
                _logger.LogError("Bad entity identity!");

            return result;
        }
    }
}