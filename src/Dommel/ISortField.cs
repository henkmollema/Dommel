using System;
using System.Collections.Generic;
using System.Text;

namespace Dommel
{
    public interface ISortField
    {
        string FieldName { get; set; }
        OrderDirection Direction { get; set; }
    }

    public enum OrderDirection
    {
        ASC,
        DESC
    }
}
