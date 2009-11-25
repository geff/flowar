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

		public ModelCard Clone()
		{
			ModelCard modelCard = new ModelCard();

			int width = Cases.GetUpperBound(0) + 1;
			int height = Cases.GetUpperBound(1) + 1;

			modelCard.Cases = new Case[width, height];
			modelCard.DrawingCaseValue = new int[width, height];

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (this.Cases[x, y] != null)
					{
						modelCard.Cases[x, y] = new Case();

						modelCard.Cases[x, y].BonusDefenser = this.Cases[x, y].BonusDefenser;
						modelCard.Cases[x, y].BonusStricker = this.Cases[x, y].BonusStricker;
						modelCard.Cases[x, y].Defenser = this.Cases[x, y].Defenser;
						modelCard.Cases[x, y].FlowerType = this.Cases[x, y].FlowerType;
						modelCard.Cases[x, y].MalusDefenser = this.Cases[x, y].MalusDefenser;
						modelCard.Cases[x, y].MalusStricker = this.Cases[x, y].MalusStricker;
						modelCard.Cases[x, y].NumberFlower = this.Cases[x, y].NumberFlower;
						modelCard.Cases[x, y].NumberFlowerAdjacent = this.Cases[x, y].NumberFlowerAdjacent;
						modelCard.Cases[x, y].Player = this.Cases[x, y].Player;
						modelCard.Cases[x, y].Stricker = this.Cases[x, y].Stricker;
					}

					modelCard.DrawingCaseValue[x, y] = this.DrawingCaseValue[x, y];
				}
			}

			modelCard.Center = new Point(this.Center.X, this.Center.Y);

			return modelCard;
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
