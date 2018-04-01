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
        private static readonly SystemSound DEFAULT_SOUND = SystemSounds.Beep;

        private SoundPlayer alarmSound;
        private Timer timer;

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

            timer = new Timer();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            this.lblRemaining.Text = DEFAULT_TIME_LABEL;
        }

        private void clockTimer_Tick(object sender, EventArgs e) {
            timer.decrement();
            this.lblRemaining.Text = timer.timeRemainingString();

            if (timer.Done) {
                pushStartStopButton(start: false);
                enableInput();
                timer.zeroOut();
                this.lblRemaining.Text = DEFAULT_TIME_LABEL;

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

        private void btnStartStop_Click(object sender, EventArgs e) {
            if (!timer.Running) {
                // Act as a "Start" button
                if (this.lblRemaining.Text.Equals(DEFAULT_TIME_LABEL)) {
                    int hours = 0;
                    if (this.txtHours.Text.IsNotEmpty()) {
                        hours = this.txtHours.Text.ToInt() ?? 0;
                        if (hours < 1) {
                            // TODO: show a warning
                            this.txtHours.Text = string.Empty;
                        }
                    }

                    int minutes = 0;
                    if (this.txtMinutes.Text.IsNotEmpty()) {
                        minutes = this.txtMinutes.Text.ToInt() ?? 0;
                        if (minutes < 1) {
                            // TODO: show a warning
                            this.txtMinutes.Text = string.Empty;
                        }
                    }

                    int seconds = 0;
                    if (this.txtSeconds.Text.IsNotEmpty()) {
                        seconds = this.txtSeconds.Text.ToInt() ?? 0;
                        if (seconds < 1) {
                            // TODO: show a warning
                            this.txtSeconds.Text = string.Empty;
                        }
                    }

                    timer.setTime(hours, minutes, seconds);
                }

                if (!timer.Done) {
                    pushStartStopButton(start: true);
                    disableInput();
                    this.lblRemaining.Text = timer.timeRemainingString();
                }
                else {
                    clearInput();
                }
            }
            else {
                // Act as a "Stop" button
                pushStartStopButton(start: false);
            }
        }

        private void btnReset_Click(object sender, EventArgs e) {
            pushStartStopButton(start: false);
            enableInput(clearInput: true);
            timer.zeroOut();
            this.lblRemaining.Text = DEFAULT_TIME_LABEL;
        }

        private void txtSeconds_KeyPress(object sender, KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case '\r': // Enter key
                    btnStartStop.PerformClick();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void txtMinutes_KeyPress(object sender, KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case '\r': // Enter key
                    btnStartStop.PerformClick();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void txtHours_KeyPress(object sender, KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case '\r': // Enter key
                    btnStartStop.PerformClick();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void pushStartStopButton(bool start) {
            if (start) {
                timer.start();
                this.btnStartStop.Text = "Stop";
                this.clockTimer.Enabled = true;
            }
            else { // Stop
                timer.stop();
                this.btnStartStop.Text = "Start";
                this.clockTimer.Enabled = false;
            }
        }

        private void enableInput(bool clearInput = false) {
            this.txtHours.Enabled = true;
            this.txtMinutes.Enabled = true;
            this.txtSeconds.Enabled = true;

            if (clearInput) {
                this.clearInput();
            }
        }

        private void disableInput() {
            this.txtHours.Enabled = false;
            this.txtMinutes.Enabled = false;
            this.txtSeconds.Enabled = false;
        }

        private void clearInput() {
            this.txtHours.Text = string.Empty;
            this.txtMinutes.Text = string.Empty;
            this.txtSeconds.Text = string.Empty;
        }
    }
}
