using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;

using Documents.Data;
using Documents.Common;
using Documents.Utils;
using Documents.DataAccess;
using System.Collections.Generic;
using System.Threading;

namespace Documents.Services.Tests.Core
{
    [TestClass]
    public abstract class ServiceTestBase<TEntityDto, TIdentity>
        where TEntityDto : class
    {
        #region Constants
        
        private static readonly string _userLogin = "test@test.com";
        private static readonly string _userName = "Test User";
        private static readonly string _userPassword = "test";

        #endregion

        #region Members

        protected readonly ILogger _logger;
        protected readonly IUserContext _userCtx;

        protected IRepositoryService<TEntityDto, TIdentity> _testService;       

        private Lazy<UserDto> _testUser;

        #endregion

        #region Virtual methods

        /// <summary>
        /// Retrive all varinats of keys to 
        /// </summary>
        /// <returns></returns>
        protected virtual List<object> GetRetriveDataVariantKeys()
        {
            return new List<object>() 
            {
                null
            };
        }

        /// <summary>
        /// Validate has children exists by parent identity
        /// </summary>
        /// <param name="parentId">Parent entity identity</param>
        /// <returns></returns>
        protected virtual bool CheckChildrenExists(TIdentity parentId)
        {
            return false;
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Get entity identity from entity
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        protected abstract TIdentity GetIdentity(TEntityDto entityDto);

        /// <summary>
        /// Prepare valid entity
        /// </summary>
        /// <returns></returns>
        protected abstract TEntityDto PrepareValidEntity();

        /// <summary>
        /// Prepare invalid entity
        /// </summary>
        /// <returns></returns>
        protected abstract TEntityDto PrepareInvalidEntity();

        /// <summary>
        /// Prepare valid entities
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<TEntityDto> PrepareValidEntities();

        /// <summary>
        /// Prepare valid entity with children
        /// </summary>
        /// <returns></returns>
        protected abstract TEntityDto PrepareAndCreateValidEntityWithChildren();

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ServiceTestBase()
        {
            if (!DatabaseHelper.SetupDatabaseIfNeed())
                DatabaseHelper.Cleanup(_userLogin);

            _logger = ObjectContainer.Resolve<SimpleLogger>();
            _userCtx = ObjectContainer.Resolve<TestUserContext>();
            _testService = null;
        }
      
        /// <summary>
        /// Create a test user
        /// </summary>
        /// <returns></returns>
        private UserDto CreateTestUser()
        {
            var result = _userCtx
                .GetCurrentUser();

            if (result == null)
            {
                using (var unitOfWork = ObjectContainer.Resolve<UnitOfWork>())
                {
                    var user = unitOfWork
                        .UserRepository
                        .GetAll()
                        .Where(p => p.Login == _userLogin && !p.Deleted)
                        .FirstOrDefault();

                    if (user == null)
                    {
                        user = new User()
                        {
                            CreatedTime = DateTime.Now,
                            ModifiedTime = DateTime.Now,
                            Login = _userLogin,
                            UserName = _userName,
                            Password = _userPassword,
                            Role = Roles.Manager
                        };

                        using (var scope = new TransactionScope())
                        {
                            unitOfWork
                                .UserRepository
                                .Insert(user);

                            unitOfWork
                                .Save();
                            scope.Complete();
                        }
                    }

                    result = user
                        .ToDto();

                    ((TestUserContext)_userCtx)
                        .SetUser(result);
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves a test user
        /// </summary>
        /// <returns></returns>
        protected UserDto GetTestUser()
        {
            if (_testUser == null)
            {
                _testUser = new Lazy<UserDto>(() =>
                {
                    return CreateTestUser();
                });
            }

            return _testUser.Value;
        }

        /// <summary>
        /// Standart test to insert and update entity successfully
        /// </summary>
        protected void SaveEntity_SuccessfullySaved()
        {
            var newData = PrepareValidEntity();

            Assert.IsNotNull(newData);

            var insertedData = _testService.Create(newData);
            
            Assert.IsNotNull(insertedData);

            var insertId = GetIdentity(insertedData);

            var loadedData = _testService.Get(insertId);

            Assert.IsNotNull(loadedData);
            Assert.AreEqual(insertedData, loadedData);

            Thread.Sleep(2000);

            var updatedData = _testService.Update(insertedData);

            Assert.IsNotNull(updatedData);
            Assert.AreNotEqual(insertedData, updatedData);
        }

        /// <summary>
        /// Standart test to insert invalid entity
        /// </summary>
        protected void SaveEntity_ErrorSaved()
        {
            var newData = PrepareInvalidEntity();

            Assert.IsNotNull(newData);

            var savedData = _testService.Create(newData);
            
            Assert.IsNull(savedData);
        }

        /// <summary>
        /// Standart test to retrive entitities by different variants successfully
        /// </summary>
        protected void LoadEntities_SuccessfullyLoaded()
        {
            var newDataItems = PrepareValidEntities();

            Assert.IsNotNull(newDataItems);

            foreach (var newDataItem in newDataItems)
            {
                _testService.Create(newDataItem);
            }

            var keys = GetRetriveDataVariantKeys();

            Assert.IsNotNull(keys);

            IEnumerable<TEntityDto> loadedData;

            foreach (var key in keys)
            {
                loadedData = null;

                if (key == null)
                    loadedData = _testService.GetAll();
                else 
                    loadedData = _testService.GetAll(key);
                
                Assert.IsNotNull(loadedData);

                if (key == null)
                    Assert.AreEqual(newDataItems.Count(), loadedData.Count(),
                         "Number of new items not equal loaded items.");
            }
        }

        /// <summary>
        /// Standart test to delete (simple/cascade) entity successfully
        /// </summary>
        protected void DeleteEntity_SuccessfullyDeleted()
        {
            var savedData = PrepareAndCreateValidEntityWithChildren();

            Assert.IsNotNull(savedData);

            var savedDataId = GetIdentity(savedData);

            Assert.IsTrue(_testService.Delete(savedDataId));

            var loadedData = _testService.Get(savedDataId);

            Assert.IsNull(loadedData);
            Assert.IsFalse(CheckChildrenExists(savedDataId), 
                "Children has exists after that parent was deleted");
        }
    }
}