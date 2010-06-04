using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ThreadAStar.Model;
using System.Threading;

namespace ThreadAStar.UC
{
    public partial class UCMonitoring : UserControl
    {
        delegate void Refresh_UCMonitoringCallback();

        public ThreadMonitor monitor;

        Graphics gImg;
        Graphics g;
        Image img;

        public UCMonitoring()
        {
            InitializeComponent();

            this.monitor = new ThreadMonitor(this);

            InitGraphics();
        }

        private void InitGraphics()
        {
            g = pictureBox.CreateGraphics();
            img = new Bitmap(pictureBox.Width, pictureBox.Height);
            gImg = Graphics.FromImage(img);
        }

        public void StartMonitoring()
        {
            monitor.StartMonitoring();
        }

        public void StopMonitoring()
        {
            monitor.StartMonitoring();
        }


        public void RefreshGraph()
        {
            if (this.pictureBox.InvokeRequired)
            {
                Refresh_UCMonitoringCallback call = new Refresh_UCMonitoringCallback(RefreshGraph);
                pictureBox.Invoke(call);
            }
            else
            {
                gImg.Clear(Color.White);

                int numThread = 0;
                foreach (ThreadData threadData in  monitor.ListThreadData.Values)
                {
                    numThread++;
                    Pen pen = new Pen(Brushes.Black, 3f);
                    gImg.DrawLine(pen, numThread * 5, 0, numThread * 5, (int)threadData.CountRefresh);
                }
                
                g.DrawImage(img, 0, 0);

                label1.Text = DateTime.Now.TimeOfDay.TotalMilliseconds.ToString();
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            RefreshGraph();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            RefreshGraph();
        }
    }
}
