using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NewFlowar.Model;

namespace NewFlowar
{
    public class PlayerCard
    {
        //private FlowerType flowerType;
        //public FlowerType FlowerType 
        //{ get
        //{
        //    return flowerType;
        //}
        //    set
        //    {
        //        flowerType = value;
        //        SetFlowerType(value);
        //    }
        //}
        public List<Cell> ListCell { get; set; }
        public FlowerType FlowerType { get; set; }
		public Player Player { get; set; }
        public ModelCard ModelCard { get; set; }
        public int RotationStep { get; set; }

		//public CardType CardType { get; set; }

        //private void SetFlowerType(FlowerType flowerType)
        //{
        //    int width = Cases.GetUpperBound(0) + 1;
        //    int height = Cases.GetUpperBound(1) + 1;

        //    //Cases = new Case[width, height];

        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            if (this.Cases[x, y] != null)
        //            {
        //                this.Cases[x, y].FlowerType = flowerType;
        //            }
        //        }
        //    }
        //}

        //public PlayerCard Clone()
        //{
        //    PlayerCard modelCard = new PlayerCard();

        //    int width = Cases.GetUpperBound(0)+1;
        //    int height = Cases.GetUpperBound(1)+1;

        //    modelCard.Cases = new PlayerCase[width, height];
        //    modelCard.DrawingCaseValue = new int[width, height];

        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            if (this.Cases[x, y] != null)
        //            {
        //                modelCard.Cases[x, y] = new PlayerCase();

        //                modelCard.Cases[x, y].BonusDefenser = this.Cases[x, y].BonusDefenser;
        //                modelCard.Cases[x, y].BonusStricker = this.Cases[x, y].BonusStricker;
        //                modelCard.Cases[x, y].Defenser = this.Cases[x, y].Defenser;
        //                modelCard.Cases[x, y].FlowerType = this.Cases[x, y].FlowerType;
        //                modelCard.Cases[x, y].MalusDefenser = this.Cases[x, y].MalusDefenser;
        //                modelCard.Cases[x, y].MalusStricker = this.Cases[x, y].MalusStricker;
        //                modelCard.Cases[x, y].NumberFlower = this.Cases[x, y].NumberFlower;
        //                modelCard.Cases[x, y].NumberFlowerAdjacent = this.Cases[x, y].NumberFlowerAdjacent;
        //                modelCard.Cases[x, y].Player = this.Cases[x, y].Player;
        //                modelCard.Cases[x, y].Stricker = this.Cases[x, y].Stricker;
        //            }

        //            modelCard.DrawingCaseValue[x, y] = this.DrawingCaseValue[x, y];
        //        }
        //    }

        //    modelCard.Center = new Point(this.Center.X, this.Center.Y);
        //    modelCard.FlowerType = this.FlowerType;
        //    modelCard.Player = this.Player;

        //    return modelCard;
        //}
    }
}
