using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flowar
{
    public class ModelCard
    {
        public Case[,] Cases { get; set; }

        public int[,] DrawingCaseValue { get; set; }

        public ModelCard()
        {
            this.Cases = new Case[2, 4];
            this.DrawingCaseValue = new int[2, 4];
        }
    }
}
