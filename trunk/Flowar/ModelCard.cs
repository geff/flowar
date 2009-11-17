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

        public ModelCard()
        {
            this.Cases = new Case[2, 4];
            this.DrawingCaseValue = new int[2, 4];
        }

		public ModelCard Clone()
		{
			ModelCard modelCard = new ModelCard();

			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 4; y++)
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
    }
}
