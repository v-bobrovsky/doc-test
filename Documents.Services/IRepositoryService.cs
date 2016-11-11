using System;
using System.Collections.Generic;

using Documents.Data;

namespace Documents.Services
{
    /// <summary>
    /// Provides repository functionality for CRUD operation.
    /// </summary>
    public interface IRepositoryService<TEntityDto, in TIdentity>
        where TEntityDto : class
    {
        /// <summary>
        /// Creates a new entity dto
        /// </summary>
        /// <param name="entityDto">Dto entity</param>
        /// <returns>
        /// <see cref="TEntityDto"/>s object containing the new dto entity.
        /// </returns>
        TEntityDto Create(TEntityDto entityDto);

        /// <summary>
        ///  Retrieves a specific entity
        /// </summary>
        /// <param name="ctx">Contains information of current user</param>
        /// <param name="id">Entity unique identifier</param>
        /// <returns>
        /// <see cref="TEntityDto"/>s object containing the entity.
        /// </returns>
        TEntityDto Get(PermissionsContext ctx, TIdentity id);

        /// <summary>
        /// Retrieves list of specific DTO's entities.
        /// </summary>
        /// <param name="ctx">Contains information of current user</param>
        /// <param name="args"></param>
        /// <returns>
        /// <see cref="IEnumerable<TEntityDto>"/>s object containing the list of entitities.
        /// </returns>
        IEnumerable<TEntityDto> GetAll(PermissionsContext ctx, params object[] args);
 
        /// <summary>
        /// Updates a specific DTO
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns>
        /// <see cref="TEntityDto"/>s object containing the changed entity.
        /// </returns>
        TEntityDto Update(TEntityDto entityDto);

        /// <summary>
        /// Delete a specific DTO
        /// </summary>
        /// <param name="id">Entity unique identifier</param>
        /// <returns></returns>
        bool Delete(TIdentity id);
    }
}