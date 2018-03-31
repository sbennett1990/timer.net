using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Windows.Forms;
using Timer.Utils;

namespace Timer {
    public partial class MainForm : Form {
        private const string DEFAULT_TIME_LABEL = "00:00:00";
        private static readonly TimeSpan ONE_SECOND =
            new TimeSpan(TimeSpan.TicksPerSecond);
        private static readonly TimeSpan DEFAULT_TIME = TimeSpan.Zero;
        private static readonly SystemSound DEFAULT_SOUND = SystemSounds.Beep;

        private SoundPlayer alarmSound;
        private TimeSpan timeLeft;
        private bool started;

        public MainForm() {
            InitializeComponent();

            // Load the alarm sound file
            try {
                DirectoryInfo folder = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
                FileInfo[] soundFiles = folder.GetFiles("Alarm04.wav", SearchOption.TopDirectoryOnly);
                string soundPath = soundFiles.Select(x => x.FullName).FirstOrDefault();
                alarmSound = new SoundPlayer(soundPath);
                alarmSound.Load();
            }
            catch {
                alarmSound = null;
            }

            timeLeft = DEFAULT_TIME;
            started = false;
        }

        private void MainForm_Load(object sender, EventArgs e) {
            this.lblRemaining.Text = DEFAULT_TIME_LABEL;
        }

        private void clockTimer_Tick(object sender, EventArgs e) {
            timeLeft = timeLeft.Subtract(ONE_SECOND);
            this.lblRemaining.Text = timeLeft.ToString(@"hh\:mm\:ss");

            if (timeLeft.CompareTo(TimeSpan.Zero) <= 0) {
                // Timer done!
                this.clockTimer.Enabled = false;
                this.btnStart.Text = "Start";
                timeLeft = DEFAULT_TIME;
                this.lblRemaining.Text = DEFAULT_TIME_LABEL;
                this.txtHours.Enabled = true;
                this.txtMinutes.Enabled = true;
                this.txtSeconds.Enabled = true;
                started = false;

                // Bring the alarm to the foreground
                if (this.WindowState != FormWindowState.Minimized) {
                    this.WindowState = FormWindowState.Minimized;
                }
                this.Show();
                this.WindowState = FormWindowState.Normal;

                using (DoneBox done = new DoneBox()) {
                    if (alarmSound != null) {
                        alarmSound.PlayLooping();
                    }
                    else {
                        DEFAULT_SOUND.Play();
                    }

                    done.ShowDialog();
                }

                if (alarmSound != null) {
                    alarmSound.Stop();
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e) {
            if (!started) {
                // Act as a "Start" button

                if (this.lblRemaining.Text.Equals(DEFAULT_TIME_LABEL)) {
                    int hours = 0;
                    if (!string.IsNullOrWhiteSpace(this.txtHours.Text)) {
                        hours = this.txtHours.Text.ToInt() ?? -1;
                        if (hours < 0) {
                            // TODO: show a warning
                            this.txtHours.Text = string.Empty;
                        }
                    }

                    int minutes = 0;
                    if (!string.IsNullOrWhiteSpace(this.txtMinutes.Text)) {
                        minutes = this.txtMinutes.Text.ToInt() ?? -1;
                        if (minutes < 0) {
                            // TODO: show a warning
                            this.txtMinutes.Text = string.Empty;
                        }
                    }

                    int seconds = 0;
                    if (!string.IsNullOrWhiteSpace(this.txtSeconds.Text)) {
                        seconds = this.txtSeconds.Text.ToInt() ?? -1;
                        if (seconds < 0) {
                            // TODO: show a warning
                            this.txtSeconds.Text = string.Empty;
                        }
                    }

                    timeLeft = new TimeSpan(hours, minutes, seconds);
                }

                if (timeLeft.CompareTo(TimeSpan.Zero) <= 0) {
                    // TODO: display a warning
                    this.txtHours.Text = string.Empty;
                    this.txtMinutes.Text = string.Empty;
                    this.txtSeconds.Text = string.Empty;
                }
                else {
                    started = true;
                    this.btnStart.Text = "Stop";
                    this.clockTimer.Enabled = true;
                    this.lblRemaining.Text = timeLeft.ToString(@"hh\:mm\:ss");

                    this.txtHours.Enabled = false;
                    this.txtMinutes.Enabled = false;
                    this.txtSeconds.Enabled = false;
                }
            }
            else {
                // Act as a "Stop" button

                started = false;
                this.btnStart.Text = "Start";
                this.clockTimer.Enabled = false;
            }
        }

        private void btnReset_Click(object sender, EventArgs e) {
            this.clockTimer.Enabled = false;
            this.btnStart.Text = "Start";
            this.txtHours.Text = string.Empty;
            this.txtMinutes.Text = string.Empty;
            this.txtSeconds.Text = string.Empty;
            timeLeft = DEFAULT_TIME;
            this.lblRemaining.Text = DEFAULT_TIME_LABEL;
            this.txtHours.Enabled = true;
            this.txtMinutes.Enabled = true;
            this.txtSeconds.Enabled = true;
            started = false;
        }

        private void txtSeconds_KeyPress(object sender, KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case '\r': // Enter key
                    btnStart.PerformClick();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void txtMinutes_KeyPress(object sender, KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case '\r': // Enter key
                    btnStart.PerformClick();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void txtHours_KeyPress(object sender, KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case '\r': // Enter key
                    btnStart.PerformClick();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }
    }
}
