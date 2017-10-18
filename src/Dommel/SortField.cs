using System;
using System.Collections.Generic;
using System.Text;

namespace Dommel
{
    public class SortField : ISortField
    {
        public string FieldName { get; set; }
        public OrderDirection Direction { get; set; }
    }
}
