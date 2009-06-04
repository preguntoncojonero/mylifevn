using System;
using System.Threading;
using System.Timers;
using Timer=System.Timers.Timer;

namespace MyLife.Web.Schedulers
{
    public abstract class Task
    {
        private Timer timer;

        protected Task() : this(60000)
        {
        }

        protected Task(double interval)
        {
            Interval = interval;
            Initialize();
        }

        public string Name { get; set; }
        public bool IsRunning { get; set; }
        public bool Enabled { get; set; }
        public DateTime LastRunTime { get; set; }
        public bool IsLastRunSuccessful { get; set; }
        public double Interval { get; set; }
        public bool Stopped { get; set; }

        public void Start()
        {
            Stopped = false;
            StartTask();
        }

        public void Stop()
        {
            Stopped = true;
        }

        private void Initialize()
        {
            Stopped = false;
            Enabled = true;

            timer = new Timer(Interval);
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
        }

        private void StartTask()
        {
            if (!Stopped)
            {
                var thread = new Thread(Execute);
                thread.Start();
            }
        }

        private void Execute()
        {
            try
            {
                IsRunning = true;
                LastRunTime = DateTime.Now;
                ExecuteTask();
                IsLastRunSuccessful = true;
            }
            catch
            {
                IsLastRunSuccessful = false;
            }
            finally
            {
                IsRunning = false;
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsRunning)
                StartTask();
        }

        protected abstract void ExecuteTask();
    }
}