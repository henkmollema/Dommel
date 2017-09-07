using System;
using System.Collections.Generic;
using System.Text;

namespace Dommel
{
    /// <summary>
    /// Describes a foreign key relationship.
    /// </summary>
    public enum ForeignKeyRelation
    {
        /// <summary>
        /// Specifies a one-to-one relationship.
        /// </summary>
        OneToOne,

        /// <summary>
        /// Specifies a one-to-many relationship.
        /// </summary>
        OneToMany,

        /// <summary>
        /// Specifies a many-to-many relationship.
        /// </summary>
        ManyToMany
    }
}
