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
using System.IO;
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

            if (File.Exists("history.txt"))
            {
                var cities = File.ReadAllLines("history.txt").Distinct();
                foreach (var city in cities)
                {
                    if (!string.IsNullOrWhiteSpace(city))
                    {
                        TBCity.Items.Add(city.Trim());
                        lstHistory.Items.Add(city.Trim());
                    }
                }
            }

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
            try
            {
                using (WebClient web = new WebClient())
                {
                    string city = TBCity.Text.Trim();
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

                    if(!TBCity.Items.Contains(city))
                    {
                        TBCity.Items.Add(city);
                        File.AppendAllText("history.txt", city + Environment.NewLine);
                    }
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("Khong the ket noi toi may chu thoi tiet.\nVui long kiem tra ket noi Internet hoac nhap dung ten thanh pho.", "Loi mang", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (JsonException ex)
            {
                MessageBox.Show("Du lieu phan hoi tu API khong hop le.\nCo the ten thanh pho khong ton tai hoac API thay doi.", "Loi du lieu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Da xay ra loi khong xac dinh:\n" + ex.Message, "Loi", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        DateTime convertDataTime(long sec)
        {
            DateTime day = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime();
            day = day.AddSeconds(sec).ToLocalTime();

            return day;
        }

        private void btnDeleteCity_Click(object sender, EventArgs e)
        {
            if (lstHistory.SelectedItems.Count == 0)
            {
                MessageBox.Show("Chon thanh pho muon xoa.");
                return;
            }

            List<string> allCities = File.ReadAllLines("history.txt").ToList();

            foreach (var selected in lstHistory.SelectedItems)
            {
                string cityToRemove = selected.ToString().Trim();
                allCities.RemoveAll(c => c.Equals(cityToRemove, StringComparison.OrdinalIgnoreCase));
            }

            File.WriteAllLines("history.txt", allCities);

            lstHistory.Items.Clear();
            foreach (var city in allCities.Distinct())
                lstHistory.Items.Add(city);

            TBCity.Items.Clear();
            foreach (var city in allCities.Distinct())
                TBCity.Items.Add(city);

            MessageBox.Show("Da xoa thanh cong");
        }
    }
}
