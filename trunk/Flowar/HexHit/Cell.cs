using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HexHit
{
    public class Cell
    {
        public Point Position { get; set; }
        public Boolean IsBorder { get; set; }
        public Point Coordinate { get; set; }

        public Dictionary<int,Cell> ListNeighbour { get; set; }

        public Cell(int x, int y)
        {
            this.Position = new Point(x, y);
            this.ListNeighbour = new Dictionary<int, Cell>();
        }
    }
}
