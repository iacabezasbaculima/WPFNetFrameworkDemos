using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Xml.Linq;
using System.Net;
using System.Diagnostics;
using WPFApp.Models;

namespace WPFApp.Services.OpenWeather
{
	public class OpenWeatherMapService
	{
		private const string _APP_KEY = "cf57eb65fef30ff16cf331c040d18b32";
		private const int _MAX_FORECAST_DAYS = 5;
		private HttpClient _client;

		public OpenWeatherMapService()
		{
			_client = new HttpClient();
			_client.BaseAddress = new Uri("http://samples.openweathermap.org/data/2.5/");
		}
		public async Task<IEnumerable<WeatherForecast>> GetForecastAsync(string location, int days)
		{
			if (location == null) throw new ArgumentNullException("Location can't be null.");
			if (location == string.Empty) throw new ArgumentException("Location can't be an empty string.");
			if (days <= 0) throw new ArgumentOutOfRangeException("Days should be greather than zero.");
            if (days > _MAX_FORECAST_DAYS) throw new ArgumentOutOfRangeException($"Days can't be greater than {_MAX_FORECAST_DAYS}");

			//var query = $"forecast/daily?q={location}&type=accurate&mode=xml&units=metric&cnt={days}&appid={_APP_KEY}";
			var query = $"forecast?q=London,us&mode=xml&appid=b6907d289e10d714a6e88b30761fae22";
			var response = await _client.GetAsync(query);

			switch (response.StatusCode)
			{
				case HttpStatusCode.Unauthorized:
					throw new Exception("Invalid API key.");
				case HttpStatusCode.NotFound:
					throw new Exception("Location not found.");
				case HttpStatusCode.OK:
					var s = await response.Content.ReadAsStringAsync();
					var x = XElement.Load(new StringReader(s));
					
					var data = x.Descendants("time").Select(w => new WeatherForecast
					{
						Date = DateTime.Parse(w.Attribute("from").Value.Substring(0, 10)),
						Description = w.Element("symbol").Attribute("name").Value,
						WindSpeed = double.Parse(w.Element("windSpeed").Attribute("mps").Value),
						CurrentTemperature = double.Parse(w.Element("temperature").Attribute("value").Value),
						MaxTemperature = double.Parse(w.Element("temperature").Attribute("max").Value),
						MinTemperature = double.Parse(w.Element("temperature").Attribute("min").Value),
					});
					
					return data;
				default:
					throw new NotImplementedException(response.StatusCode.ToString());
			}
		}
	}
}
