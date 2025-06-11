using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using System.Drawing.Drawing2D;

namespace Weather_App
{
    public partial class Form1 : Form
    {
        private bool isDark = false;
        public Form1()
        {
            InitializeComponent();
        }

        string APIKey = "1510ed20dd4115d3090a386670a42b72";
        private void Form1_Load(object sender, EventArgs e)
        {
            RoundControl(btnSearch, 12);
            RoundControl(TBCity, 12);

            btnSearch.MouseEnter += btnSearch_MouseEnter;
            btnSearch.MouseLeave += btnSearch_MouseLeave;

        }
        private void btnToggleTheme_Click(object sender, EventArgs e)
        {
            isDark = !isDark;
            this.BackColor = isDark ? Color.FromArgb(30, 30, 30) : Color.White;

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label || ctrl is Button || ctrl is TextBox)
                {
                    ctrl.ForeColor = isDark ? Color.White : Color.Black;
                    ctrl.BackColor = isDark ? Color.FromArgb(45, 45, 45) : Color.White;
                }
            }
        }

        private void btnSearch_MouseEnter(object sender, EventArgs e)
        {
            btnSearch.BackColor = Color.LightSkyBlue;
        }

        private void btnSearch_MouseLeave(object sender, EventArgs e)
        {
            btnSearch.BackColor = SystemColors.Control;
        }

        private void RoundControl(Control c, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(c.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(c.Width - radius, c.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, c.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            c.Region = new Region(path);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            getWeather();
        }
        void getWeather()
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}", TBCity.Text, APIKey);
                var json = web.DownloadString(url);
                WeatherInfo.root Info = JsonConvert.DeserializeObject<WeatherInfo.root>(json);

                picIcon.ImageLocation = "https://openweathermap.org/img/wn/" + Info.weather[0].icon + "@2x.png";
                labCondition.Text = Info.weather[0].main;
                labDetails.Text = Info.weather[0].description;
                labSunset.Text = convertDataTime(Info.sys.sunset).ToShortTimeString();
                labSunrise.Text = convertDataTime(Info.sys.sunrise).ToShortTimeString();

                labWindSpeed.Text = Info.wind.speed.ToString();
                labPressure.Text = Info.main.pressure.ToString();
            }
        }
        DateTime convertDataTime(long sec)
        {
            DateTime day = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime();
            day = day.AddSeconds(sec).ToLocalTime();

            return day;
        }
    }
}
