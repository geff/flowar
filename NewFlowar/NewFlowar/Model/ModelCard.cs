using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace NewFlowar
{
    public class ModelCard
    {
        public int CardValue { get; set; }
        public Vector2 Center { get; set; }

        public ModelCard(int cardValue)
        {
            this.CardValue = cardValue;
        }
    }
}
