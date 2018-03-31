﻿using System;
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
        private TimeSpan timer;

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

            timer = DEFAULT_TIME;
            setStarted(false);
        }

        private void MainForm_Load(object sender, EventArgs e) {
            this.lblRemaining.Text = DEFAULT_TIME_LABEL;
        }

        private void clockTimer_Tick(object sender, EventArgs e) {
            timer = timer.Subtract(ONE_SECOND);
            this.lblRemaining.Text = timer.ToString(@"hh\:mm\:ss");

            if (timer.CompareTo(TimeSpan.Zero) <= 0) {
                // Timer done!
                pushStartStopButton(start: false);
                enableInput();
                timer = DEFAULT_TIME;
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

        private void btnStart_Click(object sender, EventArgs e) {
            if (!started()) {
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

                    timer = new TimeSpan(hours, minutes, seconds);
                }

                if (timer.CompareTo(TimeSpan.Zero) <= 0) {
                    clearInput();
                }
                else {
                    pushStartStopButton(start: true);
                    disableInput();
                    this.lblRemaining.Text = timer.ToString(@"hh\:mm\:ss");
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
            timer = DEFAULT_TIME;
            this.lblRemaining.Text = DEFAULT_TIME_LABEL;
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

        private void pushStartStopButton(bool start) {
            if (start) {
                setStarted(true);
                this.btnStart.Text = "Stop";
                this.clockTimer.Enabled = true;
            }
            else { // Stop
                setStarted(false);
                this.btnStart.Text = "Start";
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

        private void setStarted(bool state) {
            this.btnStart.Tag = state;
        }

        private bool started() {
            bool? state = this.btnStart.Tag as bool?;
            if (state == null) {
                return false;
            }
            else {
                return (bool) state;
            }
        }
    }
}
