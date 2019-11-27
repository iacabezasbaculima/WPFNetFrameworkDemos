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

namespace WPFApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<WeatherForecast> forecast;
		WeatherForecast currentWeather;
		OpenWeatherMapService service;

		Task API_CALL;

		public MainWindow()
		{
			InitializeComponent();
			
			service = new OpenWeatherMapService();

			API_CALL = GetWeather();
		}
	
		public async Task GetWeather()
		{
			try
			{
				var weather = await service.GetForecastAsync("London", 3);
				currentWeather = weather.First();
				forecast = weather.Skip(1).Take(2).ToList();

				//Update UI element now
				CityName.Text = "London";
				Date.Text = currentWeather.Date.ToString("MM/dd/yyyy");
				Temp.Text = Math.Round(currentWeather.CurrentTemperature - 273.15).ToString();
				MaxTemp.Text = Math.Round(currentWeather.MaxTemperature - 273.15).ToString();
				MinTemp.Text = Math.Round(currentWeather.MinTemperature - 273.15).ToString();
				WindSpeed.Text = $"{currentWeather.WindSpeed.ToString()} mps";
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		private void ButtonGetWeather_Click(object sender, RoutedEventArgs e)
		{
			API_CALL = GetWeather();
		}

		private void btn_CityListClick(object sender, RoutedEventArgs e)
		{
			CityListPanel.Visibility = Visibility.Visible;
			ForecastPanel.Visibility = Visibility.Collapsed;
		}
	}
}
