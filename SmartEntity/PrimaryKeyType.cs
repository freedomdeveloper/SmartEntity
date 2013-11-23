using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartEntity
{
    public enum PrimaryKeyType:byte
    {
        AutoIncrease = 1,
        NotAutoIncrease = 2,
        Reference = 3,
    }
}
