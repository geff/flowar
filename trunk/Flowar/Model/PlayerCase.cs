using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flowar
{
    public class PlayerCase : BaseCase
    {
        public int Player { get; set; }
        public FlowerType FlowerType { get; set; }

        public int NumberFlower { get; set; }
        public float NumberFlowerAdjacent { get; set; }

        public float Defenser { get; set; }
        public float BonusDefenser { get; set; }
        public float MalusDefenser { get; set; }

        public float Stricker { get; set; }
        public float BonusStricker { get; set; }
        public float MalusStricker { get; set; }

        public int NumberGrowingCase { get; set; }
        public float StartValueGrowingCase { get; set; }
        public float EndValueGrowingCase { get; set; }
        public float PercentageGrowingCase { get; set; }
        public bool GrowingCase { get; set; }
        public bool NewCase { get; set; }
        public TimeSpan GrowingStartTime { get; set; }

        public PlayerCase()
            : base(0)
        {

        }
    }
}
