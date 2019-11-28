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
		private HttpClient _client;

		public enum QueryType { SINGLE_DAY, FIVE_DAYS }
		public OpenWeatherMapService()
		{
			_client = new HttpClient();
			_client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
		}
		public async Task<IEnumerable<WeatherForecast>> GetForecastAsync(string location)
		{
			if (location == null) throw new ArgumentNullException("Location can't be null.");
			if (location == string.Empty) throw new ArgumentException("Location can't be an empty string.");

			var query = $"forecast?q={location}&type=accurate&units=metric&mode=xml&appid={_APP_KEY}";
			
			//var query = $"forecast/daily?q={location}&type=accurate&mode=xml&units=metric&cnt={days}&appid={_APP_KEY}";
			//var query = $"forecast?q={location},us&mode=xml&appid=b6907d289e10d714a6e88b30761fae22";
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

		public async Task<WeatherForecast> GetSingleForecastAsync(string location)
		{
			if (location == null) throw new ArgumentNullException("Location can't be null.");
			if (location == string.Empty) throw new ArgumentException("Location can't be an empty string.");

			var query = $"weather?q={location}&type=accurate&units=metric&mode=xml&appid={_APP_KEY}";
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

					WeatherForecast forecast = new WeatherForecast
					{
						City = x.Element("city").Attribute("name").Value,
						Date = DateTime.Parse(x.Element("lastupdate").Attribute("value").Value),
						Description = x.Element("clouds").Attribute("name").Value,
						CurrentTemperature = double.Parse(x.Element("temperature").Attribute("value").Value),
						MaxTemperature = double.Parse(x.Element("temperature").Attribute("max").Value),
						MinTemperature = double.Parse(x.Element("temperature").Attribute("min").Value),
						WindSpeed = double.Parse(x.Element("wind").Element("speed").Attribute("value").Value),
						Humidity = int.Parse(x.Element("humidity").Attribute("value").Value),
						Pressure = int.Parse(x.Element("pressure").Attribute("value").Value),
					};
					return forecast;
				default:
					throw new NotImplementedException(response.StatusCode.ToString());
			}
		}
	}
}
