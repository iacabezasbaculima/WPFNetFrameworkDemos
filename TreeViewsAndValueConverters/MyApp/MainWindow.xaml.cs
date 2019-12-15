using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace MyApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Constructor

		public MainWindow()
		{
			InitializeComponent();
		}
		#endregion

		#region OnLoaded
		/// <summary>
		/// Run when app is loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			foreach (var drive in Directory.GetLogicalDrives())
			{
				if (drive.Contains("D:"))
				{
					break;
				}
				// Create a new item
				var item = new TreeViewItem()
				{
					// Add header and full path
					Header = drive,
					Tag = drive
				};

				item.Items.Add(null);
				// OnItemExpanded
				item.Expanded += Folder_Expanded;
				FolderView.Items.Add(item);
			}		
		}

		private void Folder_Expanded(object sender, RoutedEventArgs e)
		{
			var item = (TreeViewItem)sender;

			// if the item only contains the dummy data
			if (item.Items.Count != 1 || item.Items[0] != null)
				return;

			// clera dummy data
			item.Items.Clear();

			// Get full path
			var fullPath = (string)item.Tag;

			var directories = new List<string>();

			// try to get directories from the folder 
			// NEVER DO THIS: ignore any issues, catch does nothing
			try
			{
				var dirs = Directory.GetDirectories(fullPath);

				if (dirs.Length > 0)
					directories.AddRange(dirs);
			}
			catch (Exception ex){ Console.WriteLine(ex.Message); }

			directories.ForEach(dirPath => {

				// Create directory item
				var subItem = new TreeViewItem()
				{
					// Set header as folder name
					Header = GetFileFolderName(dirPath),
					// Tag as full path
					Tag = dirPath
				};

				// Add dummy item so we can expand the folder
				subItem.Items.Add(null);

				// Handle expanding
				subItem.Expanded += Folder_Expanded;

				// Add this folder to the parent
				item.Items.Add(subItem);
			});
		}
		/// <summary>
		/// Get folder or file name from full path
		/// </summary>
		/// <param name="path">The full path</param>
		/// <returns></returns>
		private static string GetFileFolderName(string path)
		{
			if (string.IsNullOrEmpty(path)) return string.Empty;

			// To handle \ and / slashes
			var normalisedPath = path.Replace('/', '\\');
			
			// Find the index of the last backslash 
			var lastIndex = normalisedPath.LastIndexOf('\\');
			
			// Check that path has a backslash
			if (lastIndex <= 0) return path;
			
			// return the file or folder name
			return path.Substring(lastIndex + 1);
		}
		#endregion
	}
}
