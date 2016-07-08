using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyScrum
{
    public partial class Form1 : Form
    {
        private bool inMeeting;
        private bool isOvertime;

        private TimeSpan meetingTimeSpan;
        private TimeSpan individualTimeSpan;
        private TimeSpan overtimeSpan;
        private int currentItemIndex;

        private List<string> attendee;
        private const string timeZero = "00:00:00";
        private readonly TimeSpan standardDailyScrumMeetingTimeSpan = TimeSpan.FromMinutes(15);
        private readonly TimeSpan standardIndividualSpeakingTimeSpan = TimeSpan.FromMinutes(2);
        private readonly TimeSpan standardTotalMeetingTimeSpan = TimeSpan.FromMinutes(30);
        private readonly Color startBackColor;
        private readonly Color endBackColor = Color.Yellow;
        private readonly byte colorSpanR;
        private readonly byte colorSpanG;
        private readonly byte colorSpanB;

        private TimeSpan totalTimeSpan
        {
            get
            {
                return meetingTimeSpan + overtimeSpan;
            }
        }

        public Form1()
        {
            InitializeComponent();

            startBackColor = this.BackColor;
            currentItemIndex = -1;

            colorSpanR = Convert.ToByte(Math.Abs(endBackColor.R - startBackColor.R));
            colorSpanG = Convert.ToByte(Math.Abs(endBackColor.G - startBackColor.G));
            colorSpanB = Convert.ToByte(Math.Abs(endBackColor.B - startBackColor.B));

            meetingTimeSpan = new TimeSpan();

            inMeeting = false;
            isOvertime = false;

            InitalAttendeeList();
            FillListView();
        }

        private void InitalAttendeeList()
        {
            attendee = new List<string>();
            attendee.Add("Brian An");
            attendee.Add("Plum Mei");
            attendee.Add("Jim Ruan");
            attendee.Add("Hensher Han");
            attendee.Add("Sophie Wang");
            attendee.Add("Ivy Wang");
            attendee.Add("Zoe Zhao");
            attendee.Add("Alan Wen");
        }

        private void FillListView()
        {
            this.listView1.Items.RemoveAt(0);
            int no = 1;

            string name = GetNextAttendee();
            for (; name != null; name = GetNextAttendee())
            {
                var item = new ListViewItem(new string[] { no++.ToString(), name, timeZero }, no);
                this.listView1.Items.Add(item);
            }
        }

        private string GetNextAttendee()
        {
            if (attendee == null || attendee.Count == 0)
            {
                return null;
            }

            Random r = new Random();
            int n = r.Next();
            int i = n % attendee.Count;
            string ret = attendee[i];
            attendee.RemoveAt(i);
            return ret;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.inMeeting)
            {
                this.inMeeting = true;
                this.timer.Start();
            }

            if (this.listView1.SelectedItems.Count > 0)
            {
                if (this.currentItemIndex >= 0)
                {
                    this.listView1.Items[this.currentItemIndex].ForeColor = Color.DarkGray;
                }

                individualTimeSpan = new TimeSpan();
                this.listView1.SelectedItems[0].ForeColor = Color.Blue;
                this.currentItemIndex = this.listView1.SelectedItems[0].Index;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.inMeeting)
            {
                meetingTimeSpan = meetingTimeSpan.Add(TimeSpan.FromSeconds(1));
                this.labelTimeSpan.Text = meetingTimeSpan.ToString();

                individualTimeSpan = individualTimeSpan.Add(TimeSpan.FromSeconds(1));
                this.listView1.SelectedItems[0].SubItems[2].Text = individualTimeSpan.ToString();

                if (individualTimeSpan > standardIndividualSpeakingTimeSpan)
                {
                    //this.listView1.SelectedItems[0].UseItemStyleForSubItems = false;
                    this.listView1.SelectedItems[0].ForeColor = Color.Red;
                }
            }
            else if (this.isOvertime)
            {
                overtimeSpan = overtimeSpan.Add(TimeSpan.FromSeconds(1));
                string main = this.labelTimeSpan.Text.Substring(0, 8);
                this.labelTimeSpan.Text = main + string.Format(" ({0})", overtimeSpan);
            }

            if (meetingTimeSpan > standardDailyScrumMeetingTimeSpan)
            {
                this.labelTimeSpan.ForeColor = Color.Red;
            }

            SetBackColor();
        }

        private void SetBackColor()
        {
            if (totalTimeSpan > standardTotalMeetingTimeSpan)
            {
                return;
            }

            double progress = totalTimeSpan.TotalSeconds / standardTotalMeetingTimeSpan.TotalSeconds;
            int r = Convert.ToInt32(startBackColor.R + colorSpanR * progress);
            int g = Convert.ToInt32(startBackColor.G + colorSpanG * progress);
            int b = Convert.ToInt32(startBackColor.B - colorSpanB * progress);

            this.labelTimeSpan.BackColor = Color.FromArgb(255, r, g, b);
        }

        private void labelTotalTimeSpan_Click(object sender, EventArgs e)
        {
            if (this.inMeeting)
            {
                this.inMeeting = false;

                this.isOvertime = true;
                overtimeSpan = new TimeSpan();
            }
            else if (!this.labelTimeSpan.Text.Equals(timeZero))
            {
                this.inMeeting = true;
                this.isOvertime = false;

                this.meetingTimeSpan = this.meetingTimeSpan.Add(this.overtimeSpan);
            }
        }

        private void listView1_MouseEnter(object sender, EventArgs e)
        {
            //this.Opacity = 1.0;
        }

        private void listView1_MouseLeave(object sender, EventArgs e)
        {
            //this.Opacity = 0.75;
        }
    }
}
