using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Flowar
{
    public class ModelCard
    {
        public Case[,] Cases { get; set; }

        public int[,] DrawingCaseValue { get; set; }

		public Point Center;

		public int Width { get; set; }
		public int Height { get; set; }        
		
		public ModelCard()
        {
            this.Cases = new Case[2, 4];
            this.DrawingCaseValue = new int[2, 4];
        }

		public void CalcDimensions()
		{
			//--- Détermine la taille réelle de la carte sélectionnée
			int width = Cases.GetUpperBound(0) + 1;
			int height = Cases.GetUpperBound(1) + 1;

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (Cases[x, y] != null)
					{
						if (Width < x)
							Width = x;
						if (Height < y)
							Height = y;
					}
				}
			}
			//---

			//--- Incrémente de 1 les dimensions
			//Width++;
			//Height++;
			//---
		}
    }
}
