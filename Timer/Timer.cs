using System;

namespace Timer {
    public class Timer {
        private static readonly TimeSpan ONE_SECOND =
            new TimeSpan(TimeSpan.TicksPerSecond);
        private static readonly TimeSpan DEFAULT_TIME = TimeSpan.Zero;

        private TimeSpan timer;

        public Timer() {
            timer = DEFAULT_TIME;
            Running = false;
        }

        public Timer(int hours, int minutes, int seconds) {
            setTime(hours, minutes, seconds);
            Running = false;
        }

        public bool Running {
            get;
            private set;
        }

        public bool Done {
            get {
                return timer.CompareTo(TimeSpan.Zero) <= 0;
            }
        }

        public void start() {
            Running = true;
        }

        public void stop() {
            Running = false;
        }

        public void setTime(int hours, int minutes, int seconds) {
            timer = new TimeSpan(hours, minutes, seconds);
        }

        public void zeroOut() {
            timer = DEFAULT_TIME;
        }

        public void decrement() {
            timer = timer.Subtract(ONE_SECOND);
        }

        public string timeRemainingString() {
            return timer.ToString(@"hh\:mm\:ss");
        }
    }
}
