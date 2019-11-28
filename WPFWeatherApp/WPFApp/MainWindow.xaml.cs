using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFApp.Models;
using WPFApp.Services.OpenWeather;
using System.Diagnostics;
using WPFApp.Entities;
using System.IO;
using System.Reflection;

namespace WPFApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private List<WeatherForecast> forecast { get; set; }
		private List<Location> cityList { get; set; }
		private WeatherForecast currentWeather;
		private OpenWeatherMapService service;
		private Task _API_CALL;
		public string iconFolderPath = @"C:\Users\Isaac Baculima\source\repos\WPFNetFrameworkDemos\WPFWeatherApp\WPFApp\WeatherIcons\";
		public MainWindow()
		{
			InitializeComponent();
			cityList = new List<Location>();
			forecast = new List<WeatherForecast>();
			service = new OpenWeatherMapService();
			tbx_Input.Focus();

			using (var db = new WeatherEntities())
			{
				cityList = db.Locations.ToList();
			}
			// Set the ListView source, i.e. collection
			lv_cities.ItemsSource = cityList;
			lv_cities.DisplayMemberPath = "City";
			ForecastPanel.Visibility = Visibility.Collapsed;
			CityListPanel.Visibility = Visibility.Visible;
			#region DIR TEST
				// C:\Users\Isaac Baculima\source\repos\WPFNetFrameworkDemos\WPFWeatherApp\WPFApp\bin\Debug
				// WPFApp folder dir
				//string directory = Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString();
				// The path of all weather icon files
				//var icons = Directory.GetFiles(directory + "\\WeatherIcons\\");
				//Trace.WriteLine("Directory: "+ directory);
				//Trace.WriteLine("Icons dir: " + icons[0] + " | "+icons[1]);
				//WeatherIcon.Source = new BitmapImage(new Uri(@"C:\Users\Isaac Baculima\source\repos\WPFNetFrameworkDemos\WPFWeatherApp\WPFApp\WeatherIcons\01d@2x.png"));
			#endregion
		}

		public async Task GetWeather(string location, OpenWeatherMapService.QueryType type)
		{
			try
			{
				switch (type)
				{
					case OpenWeatherMapService.QueryType.SINGLE_DAY:
						currentWeather = await service.GetSingleForecastAsync(location);
						forecast.Add(currentWeather);
						break;
					case OpenWeatherMapService.QueryType.FIVE_DAYS:
						var weather = await service.GetForecastAsync(location);
						currentWeather = weather.First();
						forecast = weather.Skip(1).Take(2).ToList();
						break;
				}
				Trace.WriteLine("FIRST");
				//Update UI element now
				CityName.Text = currentWeather.City;
				WeatherIcon.Source = new BitmapImage(new Uri($"{iconFolderPath}{currentWeather.ImageId}@2x.png"));
				Description.Text = currentWeather.Description.First().ToString().ToUpper() + currentWeather.Description.Substring(1);
				Date.Text = currentWeather.Date.ToString("MM/dd/yyyy");
				Temp.Text = $"{Math.Round(currentWeather.CurrentTemperature).ToString()}";
				MaxTemp.Text = $"{Math.Round(currentWeather.MaxTemperature).ToString()}{"\u00B0"}";
				MinTemp.Text = $"{Math.Round(currentWeather.MinTemperature).ToString()}{"\u00B0"}";
				WindSpeed.Text = $"{currentWeather.WindSpeed.ToString()} m/s";
				Humidity.Text = $"{currentWeather.Humidity} %";
				Pressure.Text = $"{currentWeather.Pressure} hPa";

				var inputCity = new Location { City = location };
				// Check if a city already exists in the list before we add it 
				bool check = cityList.Any(i => i.City == inputCity.City);

				if (!check)
				{
					using (var db = new WeatherEntities())
					{
						db.Locations.Add(inputCity);
						db.SaveChanges();
						lv_cities.ItemsSource = null;
						cityList.Add(inputCity);
						lv_cities.ItemsSource = cityList;
						lv_cities.DisplayMemberPath = "City";
					}
				} 

				Trace.WriteLine("Success.");

				// Swap Panels
				CityListPanel.Visibility = Visibility.Collapsed;
				ForecastPanel.Visibility = Visibility.Visible;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		private void btn_CityListClick(object sender, RoutedEventArgs e)
		{
			ForecastPanel.Visibility = Visibility.Collapsed;
			CityListPanel.Visibility = Visibility.Visible;

			tbx_Input.Text = "";	// clear textbox content
			tbx_Input.Focus();		// set cursor in textbox
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (CityListPanel.Visibility == Visibility.Visible)
				{
					if (tbx_Input.Text != string.Empty)
					{
						// Get Data
						_API_CALL = GetWeather(tbx_Input.Text, OpenWeatherMapService.QueryType.SINGLE_DAY);
					}
				}
			}
		}

		private void lv_cities_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Location city = (Location)lv_cities.SelectedItem;

			if (city != null)
			{
				if (city.City != string.Empty)
				{
					_API_CALL = GetWeather(city.City, OpenWeatherMapService.QueryType.SINGLE_DAY);
				}
			}

		}
	}
}
