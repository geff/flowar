using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HexHit
{
    public partial class Form1 : Form
    {
        List<Cell> ListCell;
        double size = 20;
        Point[] pointHexa = new Point[6];

        public Form1()
        {
            InitializeComponent();
        }

        private void Init()
        {
            this.Location = new Point(700, 500);

            CreatePointHExa();
            CreateCellsHex();
        }

        private void CreatePointHExa()
        {
            pointHexa = new Point[6];

            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * (double)i;
                pointHexa[i] = new Point((int)(size * Math.Cos(angle)), (int)(size * Math.Sin(angle)));
            }
        }

        private void CreateCellsCircle()
        {
            ListCell = new List<Cell>();
            int numberOfLayer = 5;

            for (int i = 0; i < numberOfLayer; i++)
            {
                double angle = 0;
                if (i > 0)
                    angle = Math.PI / (3 * i);

                for (int j = 0; j <= i * 6; j++)
                {
                    int x = (int)(Math.Cos(angle * (double)j) * (double)i * size);
                    int y = (int)(Math.Sin(angle * (double)j) * (double)i * size);

                    Cell cell = new Cell(x, y);

                    ListCell.Add(cell);
                }
            }
        }

        private void CreateCellsHex()
        {
            ListCell = new List<Cell>();

            int maxX = 7;
            int maxY = 10;

            float d = (float)Math.Sqrt(0.75);
            float r = 20;

            int nb = 0;

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    float fx = (float)x;
                    float fy = (float)y;

                    Cell cell1 = new Cell(
                        (int)((1 + fx * 3) * r),
                        (int)((0.5f + fy) * (2 * d * r)));

                    if (y == 0 || y == maxY - 1 || x == 0)
                        cell1.IsBorder = true;

                    cell1.Coordinate = new Point(x, y);
                    ListCell.Add(cell1);


                    Cell cell2 = new Cell(
                         (int)((2.5f + fx * 3) * r),
                         (int)((fy) * (2 * d * r)));

                    if (y == 0 || y == maxY - 1 || x == maxX - 1)
                        cell2.IsBorder = true;

                    //cell2.Coordinate = new Point(x, y);

                    ListCell.Add(cell2);
                }
            }
        }

        private Cell GetSelectedCell(Point point)
        {
            //float s = 10;
            float s = (float)size;
            float r = s * (float)Math.Cos(Math.PI / 6f);
            float h = s * (float)Math.Sin(Math.PI / 6f);
            float HexYSpacing = 2f * r;
            float HexXSpacing = s + h;

            // NOTE:  HexCoord(0,0)'s x() and y() just define the origin
            //        for the coordinate system; replace with your own
            //        constants.  (HexCoord(0,0) is the origin in the hex
            //        coordinate system, but it may be offset in the x/y
            //        system; that's why I subtract.)
            double x = 1.0 * (point.X - 0) / HexXSpacing + 0.5f;
            double y = 1.0 * (point.Y - 0) / HexYSpacing;
            //double z = -0.5f * x - y;
            double z = -0.5 * x - y;
            y = -0.5f * x + y;
            int ix = (int)Math.Floor(x + 0.5f);
            int iy = (int)Math.Floor(y + 0.5f);
            int iz = (int)Math.Floor(z + 0.5f);
            int sum = ix + iy + iz;
            if (sum > 0)
            {
                double abs_dx = Math.Abs(ix - x);
                double abs_dy = Math.Abs(iy - y);
                double abs_dz = Math.Abs(iz - z);
                if (abs_dx >= abs_dy && abs_dx >= abs_dz)
                    ix -= sum;
                else if (abs_dy >= abs_dx && abs_dy >= abs_dz)
                    iy -= sum;
                else
                    iz -= sum;
            }
            Point coord = new Point(ix, (iy - iz + (1 - ix % 2)) / 2);

            return null;
        }

        private void Draw()
        {
            Graphics g = this.CreateGraphics();
            g.Clear(Color.White);

            foreach (Cell cell in ListCell)
            {
                Point[] p = new Point[6];
                for (int i = 0; i < 6; i++)
                {
                    p[i] = pointHexa[i];
                    p[i].Offset(cell.Position);
                }

                g.DrawPolygon(Pens.LightGray, p);

                if (cell.IsBorder)
                    g.FillPolygon(Brushes.LemonChiffon, p);

                Point cellCornerPosition = cell.Position;
                cellCornerPosition.Offset(-10, -5);

                g.DrawString(cell.Coordinate.X.ToString() + ";" + cell.Coordinate.Y.ToString(), this.Font, Brushes.Silver, cellCornerPosition);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                Init();
            }
            if (e.Button == MouseButtons.Left)
            {
                Cell cell = GetSelectedCell(e.Location);
            }
            else if (e.Button == MouseButtons.Right)
            {
            }

            Draw();
        }
    }
}
