using System;
using System.Collections.Generic;
using System.Text;

namespace Dommel
{
    public class RelationshipMapping : IRelationshipMapping
    {
        public Type RelatedType { get; set; }
        public string JoinClause { get; set; }
        public string JoinType { get; set; }
        public string ForeignKeyPropertyName { get; set; }
        public ForeignKeyRelation Relation { get; set; }
        public string SplitId { get; set; }
        public RelationshipMapping()
        {
            Relation = ForeignKeyRelation.OneToOne;
            JoinType = "left";
        }

    }
}
