using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Common
{
    /// <summary>
    /// Represents validation exception
    /// </summary>
    public class EntityValidationException : Exception
    {
        /// <summary>
        /// The validation entity.
        /// </summary>
        public object Entity
        {
            get;
            protected set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">The validation entity.</param>
        public EntityValidationException(object entity)
            : this("Validation error", entity)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="msg">The message that describes the error.</param>
        /// <param name="target">The validation entity.</param>
        public EntityValidationException(string msg, object entity)
            : base(msg)
        {
            Entity = entity;
        }
    }
}