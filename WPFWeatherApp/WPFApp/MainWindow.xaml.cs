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
using System.Collections.ObjectModel;

namespace WPFApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<WeatherForecast> forecast { get; set; }
		List<string> cityList { get; set; }
		WeatherForecast currentWeather;
		OpenWeatherMapService service;
		Task _API_CALL;

		//ObservableCollection<string> mySource { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			cityList = new List<string>();
			forecast = new List<WeatherForecast>();
			service = new OpenWeatherMapService();

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
				
				//Update UI element now
				CityName.Text = currentWeather.City;
				Date.Text = currentWeather.Date.ToString("MM/dd/yyyy");
				Temp.Text = $"{Math.Round(currentWeather.CurrentTemperature).ToString()}{"\u00B0"}{"C"}";
				MaxTemp.Text = Math.Round(currentWeather.MaxTemperature).ToString();
				MinTemp.Text = Math.Round(currentWeather.MinTemperature).ToString();
				WindSpeed.Text = $"{currentWeather.WindSpeed.ToString()} m/s";

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
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (CityListPanel.Visibility == Visibility.Visible)
				{
					if (tbx_Input.Text != string.Empty)
					{
						var city = tbx_Input.Text;
						cityList.Add(city);

						//lv_cities.ItemsSource = cityList;
						lv_cities.Items.Add(city);

						// Get Data
						_API_CALL = GetWeather(city, OpenWeatherMapService.QueryType.SINGLE_DAY);
					}
				}
			}
		}

		private void tbx_Input_QueryCursor(object sender, QueryCursorEventArgs e)
		{
			MessageBox.Show("hello");
		}

		private void tbx_Input_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			MessageBox.Show("hello");

		}
	}
}
