using System;
using System.Collections.Generic;
using System.IO;
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

namespace WPFTreeView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Get every logical drive on the machine
            foreach (var drive in Directory.GetLogicalDrives())
            {
                // Create a new item for it 
                var item = new TreeViewItem()
                {
                    // Set the header 
                    Header = drive,

                    // And full path 
                    Tag = drive
                };

                // Add dummy item
                item.Items.Add(null);

                // Listen out for item being expanded 
                item.Expanded += Folder_Expanded;

                // Add it to the main tree view
                FolderView.Items.Add(item);
            }
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            #region Checks

            var item = (TreeViewItem)sender;

            // If the item only contains the dummy data
            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            // Clear dummy data
            item.Items.Clear();

            // Get fullpath 
            var fullpath = (string)item.Tag;

            #endregion

            #region Directories

            //Create a blank list for directories
            var directories = new List<string>();

            // Try and get directories from the folder
            // ignoring any issues doing so

            try
            {
                var dirs = Directory.GetDirectories(fullpath);

                if (dirs.Length > 0)
                    directories.AddRange(dirs);
            }
            catch { }
            
            // For each directory
            directories.ForEach(directoryPath =>
            {
                // Create a directory item 
                var subItem = new TreeViewItem()
                {
                    // Set header as folder name
                    Header = GetFileFolderName(directoryPath),
                    // And tag as full path
                    Tag = directoryPath
                };

                // Add dummy item so we can expand folder
                subItem.Items.Add(null);

                // Handle expanding
                subItem.Expanded += Folder_Expanded;

                //Add this item to the parent
                item.Items.Add(subItem);
            });
            #endregion

            #region Get files

            //Create a blank list for directories
            var files = new List<string>();

            // Try and get files from the folder
            // ignoring any issues doing so

            try
            {
                var fs = Directory.GetFiles(fullpath);

                if (fs.Length > 0)
                    files.AddRange(fs);
            }
            catch {}

            // For each file...
            files.ForEach(filePath =>
            {
                // Create a file item 
                var subItem = new TreeViewItem()
                {
                    // Set header as folder name
                    Header = GetFileFolderName(filePath),
                    // And tag as full path
                    Tag = filePath
                };

                //Add this item to the parent
                item.Items.Add(subItem);
            });           
        }
        #endregion

        public static string GetFileFolderName(string path)
        {
            // C : \Something\a file
            // C : \Something\a file.png

            // If we have no path return empty
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            // Make all slashes back slashes
            var normalizedPath = path.Replace('/', '\\');

            // Find the last backslash in the path
            var lastIndex = normalizedPath.LastIndexOf('\\');

            // If we don't find a back slash return the path itself
            return path.Substring(lastIndex + 1);
        }

        
    }
}
