using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer.Utils;

namespace Timer
{
    public partial class MainForm : Form
    {
        private const string defaultTimeLabel = "00:00:00";
        private static readonly TimeSpan decrementAmount =
            new TimeSpan(TimeSpan.TicksPerSecond);
        private TimeSpan time;
        private bool started;
        private SoundPlayer alarmPlayer;
        private static readonly SystemSound defaultSound = SystemSounds.Beep;

        public MainForm()
        {
            InitializeComponent();

            // Load the alarm sound file
            try
            {
                string location = Assembly.GetExecutingAssembly().Location;
                DirectoryInfo folder = Directory.GetParent(location);
                var soundFiles = folder.GetFiles("Alarm04.wav", SearchOption.TopDirectoryOnly);
                string soundPath = soundFiles.Where(x => x.Name.Equals("Alarm04.wav")).Select(x => x.FullName).FirstOrDefault();
                alarmPlayer = new SoundPlayer(soundPath);
                alarmPlayer.Load();
            }
            catch
            {
                alarmPlayer = null;
            }
            time = defaultTime();
            started = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.lblRemaining.Text = defaultTimeLabel;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!started)
            {
                // Act as a "Start" button

                if (this.lblRemaining.Text.Equals(defaultTimeLabel))
                {
                    int hours = 0;
                    if (!string.IsNullOrWhiteSpace(this.txtHours.Text))
                    {
                        hours = this.txtHours.Text.ToInt() ?? -1;
                        if (hours < 0)
                        {
                            // TODO: show a warning
                            this.txtHours.Text = string.Empty;
                        }
                    }

                    int minutes = 0;
                    if (!string.IsNullOrWhiteSpace(this.txtMinutes.Text))
                    {
                        minutes = this.txtMinutes.Text.ToInt() ?? -1;
                        if (minutes < 0)
                        {
                            // TODO: show a warning
                            this.txtMinutes.Text = string.Empty;
                        }
                    }

                    int seconds = 0;
                    if (!string.IsNullOrWhiteSpace(this.txtSeconds.Text))
                    {
                        seconds = this.txtSeconds.Text.ToInt() ?? -1;
                        if (seconds < 0)
                        {
                            // TODO: show a warning
                            this.txtSeconds.Text = string.Empty;
                        }
                    }

                    time = new TimeSpan(hours, minutes, seconds);
                }

                if (time.CompareTo(TimeSpan.Zero) <= 0)
                {
                    // TODO: display a warning
                    this.txtHours.Text = string.Empty;
                    this.txtMinutes.Text = string.Empty;
                    this.txtSeconds.Text = string.Empty;
                }
                else
                {
                    started = true;
                    this.btnStart.Text = "Stop";
                    this.clockTimer.Enabled = true;
                    this.lblRemaining.Text = time.ToString(@"hh\:mm\:ss");

                    this.txtHours.Enabled = false;
                    this.txtMinutes.Enabled = false;
                    this.txtSeconds.Enabled = false;
                }
            }
            else
            {
                // Act as a "Stop" button

                started = false;
                this.btnStart.Text = "Start";
                this.clockTimer.Enabled = false;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.clockTimer.Enabled = false;
            this.btnStart.Text = "Start";
            this.txtHours.Text = string.Empty;
            this.txtMinutes.Text = string.Empty;
            this.txtSeconds.Text = string.Empty;
            time = defaultTime();
            this.lblRemaining.Text = defaultTimeLabel;
            this.txtHours.Enabled = true;
            this.txtMinutes.Enabled = true;
            this.txtSeconds.Enabled = true;
            started = false;
        }

        private void clockTimer_Tick(object sender, EventArgs e)
        {
            time = time.Subtract(decrementAmount);
            this.lblRemaining.Text = time.ToString(@"hh\:mm\:ss");

            if (time.CompareTo(TimeSpan.Zero) <= 0)
            {
                // timer done!
                this.clockTimer.Enabled = false;
                this.btnStart.Text = "Start";
                time = defaultTime();
                this.lblRemaining.Text = defaultTimeLabel;
                this.txtHours.Enabled = true;
                this.txtMinutes.Enabled = true;
                this.txtSeconds.Enabled = true;
                started = false;

                using (DoneBox done = new DoneBox())
                {
                    if (alarmPlayer != null)
                        alarmPlayer.PlayLooping();
                    else
                        defaultSound.Play();

                    done.ShowDialog();
                }

                if (alarmPlayer != null)
                    alarmPlayer.Stop();
            }
        }

        private static TimeSpan defaultTime()
        {
            return new TimeSpan(hours: 0, minutes: 0, seconds: 1);
        }
    }
}
