using System;
using System.Collections.Generic;
using System.Text;

using System.Timers;

namespace DanceDj.Utils
{
    public interface ITimer
    {
        void Start();
        void Stop();
        bool AutoReset { get; set; }
        bool Enabled { get; set; }
        double Interval { get; set; }

        event EventHandler Elapsed;
    }

    public class SystemTimer : ITimer
    {
        public SystemTimer(double duration) {
            Timer = new Timer(duration);
            Timer.Elapsed += (s, e) => Elapsed?.Invoke(s, e);
        }

        public Timer Timer { get; }
        public void Start() => Timer.Start();
        public void Stop() => Timer.Stop();
        public bool AutoReset { get => Timer.AutoReset; set => Timer.AutoReset = value; }
        public bool Enabled { get => Timer.Enabled; set => Timer.Enabled = value; }
        public double Interval { get => Timer.Interval; set => Timer.Interval = value; }

        public event EventHandler Elapsed;
    }

    public class MockTimer : ITimer
    {
        public MockTimer(double duration) {

        }

        public void Start() => Enabled = true;
        public void Stop() => Enabled = false;
        public bool AutoReset { get; set; }
        public bool Enabled { get; set; }
        public double Duration { get; set; }
        public double Interval { get; set; }

        public event EventHandler Elapsed;

        public void Tick() {
            if (Enabled) {
                Elapsed?.Invoke(this, new EventArgs());
                Enabled = AutoReset;
            }
            else {
                throw new ApplicationException("Tick called while timer is stopped.");
            }
        }
    }
}
