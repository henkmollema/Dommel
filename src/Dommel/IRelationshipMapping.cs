using System;
using System.Collections.Generic;
using System.Text;

namespace Dommel
{
    public interface IRelationshipMapping
    {
        Type RelatedType { get; set; }
        string JoinClause { get; set; }
        string JoinType { get; set; }
        string ForeignKeyPropertyName { get; set; }
        ForeignKeyRelation Relation { get; set; }
        string SplitId { get; set; }
    }
}
