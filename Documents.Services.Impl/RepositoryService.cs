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
            if (!ValidateNewEntity(entityDto))
                throw new EntityValidationException(entityDto);

            return OnCreate(entityDto);
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
            if (!ValidateEntityKey(id))
                throw new EntityValidationException(id);

           return OnGet(ctx, id);
        }

        /// <summary>
        /// Retrieves list of specific DTO's entities.
        /// </summary>
        /// <param name="ctx">Contains information of current user</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IEnumerable<TEntityDto> GetAll(PermissionsContext ctx, params object[] args)
        {
            return OnGetAll(ctx, args);
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
            if (!ValidateExistEntity(entityDto))
                throw new EntityValidationException(entityDto);

            return OnUpdate(entityDto);
        }

        /// <summary>
        /// Delete a specific DTO
        /// </summary>
        /// <param name="id">Entity unique identifier</param>
        /// <returns></returns>
        public bool Delete(TIdentity id)
        {
            if (!ValidateEntityKey(id))
                throw new EntityValidationException(id);
            
            return OnDelete(id);
        }
    }
}