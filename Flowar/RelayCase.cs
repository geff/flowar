﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flowar
{
    public class RelayCase : BaseCase
    {
        public RelayType RelayType { get; set; }
        public float Factor { get; set; }

        public RelayCase(RelayType relayType, float factor)
            : base(2000 + (int)relayType)
        {

        }
    }
}
