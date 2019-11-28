using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFApp.Models
{
	public class WeatherForecast
	{
		public string City { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public double CurrentTemperature { get; set; }
		public double MinTemperature { get; set; }
		public double MaxTemperature { get; set; }
		public double WindSpeed { get; set; }
		public int Humidity { get; set; }
		public int Pressure { get; set; }
	}
}
