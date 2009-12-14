using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flowar
{
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public BaseCase[,] Cases { get; set; }
        public int[,] DrawingCaseValue { get; set; }

        public Map(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            this.Cases = new BaseCase[width, height];
            this.DrawingCaseValue = new int[width, height];

            //InitMap();
        }

        //private void InitMap()
        //{
        //    for (int x = 0; x < Width; x++)
        //    {
        //        for (int y = 0; y < Height; y++)
        //        {
        //            this.Cases[x, y] = new Case();
        //            this.Cases[x, y].FlowerType = FlowerType.None;
        //            this.Cases[x, y].Player = 0;
        //        }
        //    }
        //}
    }
}
