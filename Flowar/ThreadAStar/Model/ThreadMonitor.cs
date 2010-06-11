using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using ThreadAStar.UC;

namespace ThreadAStar.Model
{
    public class ThreadMonitor
    {
        volatile public List<TimelineData> ListTimeLineData;// { get; set; }
        volatile public Dictionary<Int32, ThreadData> ListThreadData;// { get; set; }

        private BackgroundWorker _backgroundWorker;
        private Boolean _cancelMonitoring = false;
        private TimeSpan _lastRefresh;
        private Int16 _refreshRate;

        private UCMonitoring _ucMonitoring;
        ConnectionOptions oConn;
        ManagementScope oMs;

        public ThreadMonitor(UCMonitoring ucMonitoring)
        {
            this.ListTimeLineData = new List<TimelineData>();
            this.ListThreadData = new Dictionary<Int32, ThreadData>();
            _refreshRate = 1000;
            _ucMonitoring = ucMonitoring;

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);

            oConn = new ConnectionOptions();
            oConn.Impersonation = ImpersonationLevel.Default;
            oConn.Authentication = AuthenticationLevel.Unchanged;
            
            oMs = new ManagementScope();
        }

        public void StartMonitoring()
        {
            _lastRefresh = DateTime.Now.TimeOfDay;
            _backgroundWorker.RunWorkerAsync();
        }

        public void StopMonitoring()
        {
            _cancelMonitoring = true;
        }

        private void LoopMonitoring()
        {
            while (!_cancelMonitoring)
            {
                TimeSpan newRefresh = DateTime.Now.TimeOfDay;

                if (newRefresh.Subtract(_lastRefresh).TotalMilliseconds >= _refreshRate)
                {
                    _lastRefresh = newRefresh;
                    
                    SurveyThreads();

                    TimeSpan t = DateTime.Now.TimeOfDay.Subtract(newRefresh);

                    _ucMonitoring.RefreshGraph();
                    //_ucMonitoring.label1.Text = "a";
                }
            }
        }

        private void SurveyThreads()
        {
            int processId = Process.GetCurrentProcess().Id;

            ProcessThread f;
            f.tot

            foreach (ProcessThread thread in Process.GetCurrentProcess().Threads)
            {
                string sQuery = String.Format("SELECT PercentProcessorTime FROM win32_PerfFormattedData_PerfProc_Thread WHERE IdProcess={0} and IdThread={1}",
                    processId, thread.Id);

                
                ObjectQuery oQuery = new ObjectQuery(sQuery);
                ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
                ManagementObjectCollection oReturnCollection = oSearcher.Get();

                
                /*
                 * ThreadData threadData = new ThreadData(); ;

                if (ListThreadData.ContainsKey(thread.Id))
                {
                    threadData = ListThreadData[thread.Id];
                }
                else
                {
                    ListThreadData.Add(thread.Id, threadData);
                }

                threadData.ThreadId = thread.Id;

                foreach (ManagementObject oReturn in oReturnCollection)
                {
                    PropertyDataCollection retProperties = oReturn.Properties;

                    UInt64 percentProcessorTime = (UInt64)retProperties["PercentProcessorTime"].Value;

                    threadData.CountRefresh++;

                    if (percentProcessorTime < threadData.CPUMin)
                        threadData.CPUMin = percentProcessorTime;

                    if (percentProcessorTime > threadData.CPUMax)
                        threadData.CPUMax = percentProcessorTime;

                    threadData.CPUAverage = (threadData.CPUAverage * (ulong)(threadData.CountRefresh - 1) + percentProcessorTime) / (ulong)threadData.CountRefresh;
                }
                 * */
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            LoopMonitoring();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }
    }
}
