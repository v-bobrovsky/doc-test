using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

using Documents.Utils;

namespace Documents.Common
{
    /// <summary>
    /// Logger interception behavior
    /// </summary>
    public class LoggerInterceptionBehavior : IInterceptionBehavior
    {
        #region Members

        private readonly ILogger _logger;

        public bool WillExecute
        {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public LoggerInterceptionBehavior(ILogger logger)
        {
            _logger = logger;
        }
 
        public IEnumerable<Type> GetRequiredInterfaces() 
        { 
            return Type.EmptyTypes; 
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)    
        {
            var declaringType = input
                .MethodBase
                .DeclaringType;
          
            var className = declaringType != null 
                ? declaringType.Name 
                : string.Empty;

            var methodName = input
                .MethodBase
                .Name;

            var generic = (declaringType != null && declaringType.IsGenericType)
                ? String.Join<Type>(", ", declaringType.GetGenericArguments()) 
                : String.Empty;
 
            var sbArgs = new StringBuilder();

            for (var i = 0; i < input.Arguments.Count; i++)
            {
                sbArgs.AppendFormat("{0} = {1}", input.Arguments.GetParameterInfo(i).Name,
                    input.Arguments[i] != null ? input.Arguments[i].ToString() : "NULL");
            }

            var methodCall = String
                .Format("{0}{1}.{2}({3})", 
                className, generic, methodName, sbArgs);
 
            _logger.LogInfo(String
                .Format("Performing {0}", methodCall));

            var result = getNext()(input, getNext);

            _logger.LogInfo(String
                .Format("Exited {0}", methodName));

            var e = result
                .Exception;

            if (e is EntityValidationException)
            {
                _logger.LogError(((EntityValidationException)e)
                    .Message);

                _logger.LogErrorObject(
                    ((EntityValidationException)e)
                    .Entity);
            }
            else if (e != null)
            {
                _logger.LogError(String
                    .Format("Exception occurred in: {0}.", methodCall), e);
            }

            return result;
        }
    }
}