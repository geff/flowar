using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flowar.Model.Enum
{
    public enum ContextType : int
    {
        None = 0,
        CardSelected = 1,
        CardRotated = 2,
        CardOverMap = 3,
        CardRotatedOverMap = 4,
        PutDownCard = 5,
        GrowingCase = 6,
        NextPlayerToPlay = 7,
    }
}
