using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AStar
{
    public partial class Form1 : Form
    {
        public List<Cell> ListCell { get; set; }
        int nmbCell = 300;
        int distanceMax = 60;

        Cell cellStart = null;
        Cell cellEnd = null;

        public Form1()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            CreateCells();

            CalcNeighbourgh();
        }

        private void CreateCells()
        {
            ListCell = new List<Cell>();
            ListCellClose = new List<AStarCell>();
            ListCellOpen = new List<AStarCell>();

            Random rnd = new Random();

            for (int i = 0; i < nmbCell; i++)
            {
                Cell cell = new Cell(rnd.Next(this.Width), rnd.Next(this.Height), (float)rnd.NextDouble());
                ListCell.Add(cell);
            }
        }

        private void CalcNeighbourgh()
        {
            foreach (Cell cell1 in ListCell)
            {
                foreach (Cell cell2 in ListCell)
                {
                    if (cell1 != cell2 && !cell1.ListNeighbour.Contains(cell2))
                    {
                        if (Distance(cell1.Position, cell2.Position) < distanceMax)
                        {
                            cell1.ListNeighbour.Add(cell2);
                            cell2.ListNeighbour.Add(cell1);
                        }
                    }
                }
            }
        }

        List<AStarCell> ListCellOpen = new List<AStarCell>();
        List<AStarCell> ListCellClose = new List<AStarCell>();
        

        private List<Cell> CalcPath(bool useCost, float costMax)
        {
            ListCellOpen = new List<AStarCell>();
            ListCellClose = new List<AStarCell>();
            List<Cell> listCellPath = new List<Cell>();

            //--- Ajout de la case de départ
            AStarCell aStarCell = new AStarCell();
            aStarCell.Cell = cellStart;
            //---

            //---
            ListCellClose.Add(aStarCell);

            bool pathFound = CalcPathFromCell(aStarCell, useCost, costMax);

            if (!pathFound)
                return listCellPath;
            //---

            //--- Reconstitue le chemin
            bool pathCompleted = false;
            Cell cell = cellEnd;

            while (!pathCompleted)
            {
                if (ListCellClose.Count == 0)
                    pathCompleted = true;

                foreach (AStarCell aStarCellChild in ListCellClose)
                {
                    if (aStarCellChild.Cell == cell)
                    {
                        listCellPath.Add(aStarCellChild.Cell);

                        cell = aStarCellChild.ParentCell;

                        if (aStarCellChild.Cell == cellStart)
                        {
                            pathCompleted = true;
                        }
                    }
                }
            }
            //---

            return listCellPath;
        }


        private bool CalcPathFromCell(AStarCell cellParent, bool useCost, float costMax)
        {
            foreach (Cell cell in cellParent.Cell.ListNeighbour)
            {
                //---> La cellule n'est pas dans la liste fermée
                if (!ListCellClose.Exists(c => c.Cell == cell))
                {
                    AStarCell aStarCellPrev = ListCellOpen.Find(c => c.Cell == cell);

                    AStarCell aStarCell = new AStarCell();
                    aStarCell.Cell = cell;
                    aStarCell.ParentCell = cellParent.Cell;
                    aStarCell.G = cellParent.G + Distance(cell.Position, cellParent.Cell.Position);
                    aStarCell.H = Distance(cell.Position, cellEnd.Position);
                    aStarCell.F = aStarCell.G + aStarCell.H;

                    if (useCost && cell.Cost < costMax)
                    {
                        aStarCell.F = (int)((float)aStarCell.F *cell.Cost);
                    }

                    if (aStarCellPrev != null && aStarCell.G < aStarCellPrev.G)
                    {
                        aStarCellPrev.ParentCell = cellParent.Cell;
                        aStarCellPrev.G = aStarCell.G;
                        aStarCellPrev.H = aStarCell.H;
                        aStarCellPrev.F = aStarCell.F;
                    }
                    else if (aStarCellPrev == null)
                    {
                        ListCellOpen.Add(aStarCell);
                    }
                }
            }

            AStarCell aStarChoosenCell = null;

            foreach (AStarCell aStarCell in ListCellOpen)
            {
                if (aStarChoosenCell == null)
                    aStarChoosenCell = aStarCell;

                if (aStarCell.F < aStarChoosenCell.F)
                    aStarChoosenCell = aStarCell;
            }

            //---
            ListCellOpen.Remove(aStarChoosenCell);
            ListCellClose.Add(aStarChoosenCell);
            //---

            //---
            if (aStarChoosenCell == null)
                return false;
            else if (aStarChoosenCell.Cell == cellEnd)
                return true;
            else
                return CalcPathFromCell(aStarChoosenCell, useCost, costMax);
            //---
        }

        private int Distance(Point point1, Point point2)
        {
            int distance = 0;

            Point pointResultat = new Point(point2.X - point1.X, point2.Y - point1.Y);

            distance = (int)Math.Sqrt((double)(pointResultat.X * pointResultat.X + pointResultat.Y * pointResultat.Y));

            return distance;
        }

        private void Draw(params List<Cell>[] listsCellPath)
        {
            Random rnd = new Random();

            Graphics g = this.CreateGraphics();
            g.Clear(Color.White);
            foreach (Cell cell in ListCell)
            {
                foreach (Cell cell2 in cell.ListNeighbour)
                {
                    g.DrawLine(Pens.LightBlue, cell.Position, cell2.Position);
                }

                //g.DrawEllipse(Pens.LightGray, cell.Position.X - cell.Cost * 7, cell.Position.Y - cell.Cost * 7, cell.Cost * 15, cell.Cost * 15);
                g.FillEllipse(Brushes.LightGray, cell.Position.X - cell.Cost * 7, cell.Position.Y - cell.Cost * 7, cell.Cost * 15, cell.Cost * 15);

                if (cell == cellStart)
                {
                    g.FillEllipse(Brushes.LightGreen, cell.Position.X - cell.Cost * 7, cell.Position.Y - cell.Cost * 7, cell.Cost * 14, cell.Cost * 14);
                }

                if (cell == cellEnd)
                {
                    g.FillEllipse(Brushes.LightSalmon, cell.Position.X - cell.Cost * 7, cell.Position.Y - cell.Cost * 7, cell.Cost * 14, cell.Cost * 14);
                }
            }

            foreach (List<Cell> listCellPath in listsCellPath)
            {
                Pen pen = new Pen(Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255)), 2f);

                for (int i = 0; i < listCellPath.Count - 1; i++)
                {
                    g.DrawLine(pen, listCellPath[i].Position, listCellPath[i + 1].Position);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Draw();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                Init();
                Draw();
                return;
            }

            int minDistance = int.MaxValue;
            Cell curCell = null;

            foreach (Cell cell in ListCell)
            {
                int curDistance = Distance(cell.Position, e.Location);
                if (curDistance < minDistance)
                {
                    minDistance = curDistance;
                    curCell = cell;
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                cellStart = curCell;
            }
            else if (e.Button == MouseButtons.Right)
            {
                cellEnd = curCell;
            }

            Draw();
        }

        private void btnCalcPath_Click(object sender, EventArgs e)
        {
            List<Cell> ListCellPathWithoutCost = CalcPath(false,0f);
            List<Cell> ListCellPathWithCost = CalcPath(true, 1f);
            List<Cell> ListCellPathWithCostMax = CalcPath(true, 0.5f);

            Draw(ListCellPathWithoutCost, ListCellPathWithCost, ListCellPathWithCostMax);
        }
    }
}
