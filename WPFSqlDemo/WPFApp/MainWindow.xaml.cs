using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using DB = DataLibrary.Models;
using UI = WPFApp.Models;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;

namespace WPFApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{ 
		List<DB.Task> tasks = new List<DB.Task>();
		string currentSelectedItemValue = null;

		public MainWindow()
		{
			InitializeComponent();

			TaskListBox.ItemsSource = LoadTaskList();
		}

		private ArrayList LoadTaskList()
		{
			tasks.Clear();

			using (var db = new DB.TasksDBEntities())
			{
				tasks = db.Tasks.ToList();
			}

			ArrayList temp = new ArrayList();
			foreach (var t in tasks)
			{
				temp.Add(t.Description);
			}

			return temp;
		}

		private void btn_Save(object sender, RoutedEventArgs e)
		{
			// Hide edit panel
			EditPanel.Visibility = Visibility.Collapsed;

			currentSelectedItemValue = (string)TaskListBox.SelectedItem;

			string input = tbx_Input.Text;

			using (var db = new DB.TasksDBEntities())
			{
				var task  = db.Tasks.First(i => i.Description == currentSelectedItemValue);
				if (task != null)
				{
					task.Description = input;
					db.SaveChanges();
				}
			}

			// Update list box with new data
			TaskListBox.ItemsSource = LoadTaskList();
		}

		private void btn_EditTask(object sender, RoutedEventArgs e)
		{
			// Enable the edit panel
			EditPanel.Visibility = Visibility.Visible;
		}

		private void btn_AddTask(object sender, RoutedEventArgs e)
		{

		}
	}
}
