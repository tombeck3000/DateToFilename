using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace DateToFilename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _selectedPath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog{SelectedPath = @"T:\photo"})
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    lblPath.Content = _selectedPath = dialog.SelectedPath;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (lstLog.SelectedItems.Count > 0)
                lstLog.Items.Clear();

            DirectoryInfo folder = new DirectoryInfo(_selectedPath);
            if (folder.Exists)
            {
                foreach (FileInfo fileInfo in folder.GetFiles())
                {
                    DateTime datetime = GetDateTakenFromImage(fileInfo.FullName);

                    string oldPathName = fileInfo.FullName;
                    string newPathName = $"{fileInfo.DirectoryName}\\{datetime:yyyyMMddhhmmss}.jpg";

                    if (!File.Exists(newPathName))
                    {
                        File.Move(oldPathName, newPathName);
                    }
                    

                    ListBoxItem item = new ListBoxItem();
                    item.Content = $"{oldPathName} {newPathName}";
                    lstLog.Items.Add(item);
                }
            }
        }

        private static Regex r = new Regex(":");

        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (System.Drawing.Image myImage = System.Drawing.Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }
    }
}
