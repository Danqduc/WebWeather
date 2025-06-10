using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Weather_App
{
    internal class WeatherInfo
    {
        public class coord
        {
            double lot { get; set; }
            double lat { get; set; }
        }
        public class weather
        {
            string main { get; set; }
            string description { get; set; }
            string icon { get; set; }
        }
        public class main
        {
            double temp { get; set; }
            double presure { get; set; }
            double humidity { get; set; }
        }
        public class wind
        {
            double speed { get; set; }
        }
        public class sys
        {
            long sunrise { get; set; }
            long sunset { get; set; }
        }
        public class root
        {
            public coord coord { get; set; }
            public List <weather> weathern { get; set; }
            public main main { get; set; }
            public wind wind { get; set; }
            public sys sys { get; set; }
        }
    }
}
