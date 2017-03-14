﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace File_manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>



    public partial class MainWindow : Window
    {
        ObservableCollection<Drive> _drives;
        public ObservableCollection<Drive> Drives
        {
            get; set;
        }

        public ObservableCollection<FileModel> FilesL
        {
            get; set;
        }

        public ObservableCollection<FileModel> FilesR
        {
            get; set;
        }

        public MainWindow()
        {
            InitializeComponent();
            InitDrives();
            
        }
        private void InitDrives()
        {
            _drives = new ObservableCollection<Drive>();
            DriveInfo[] drive = DriveInfo.GetDrives();
            foreach(var d in drive)
            {
                _drives.Add(new Drive() {
                    Name = d.Name,
                    Label = d.VolumeLabel,
                    TotalFreeSpace = d.TotalFreeSpace / 1024,
                    TotalSize = d.TotalSize / 1024,
                    AvailableFreeSpace = d.AvailableFreeSpace / 1024
                });
            }
            Drives = _drives;
            Drive_left.ItemsSource = Drives;
            Drive_left.SelectedItem = _drives[0];
            Drive_right.ItemsSource = Drives;
            Drive_right.SelectedItem = _drives[0];
            
        }
        
       

        private ObservableCollection<FileModel> InitFiles(ObservableCollection<FileModel> _files , string directory)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            if (dir.Exists == true)
            {
                _files = new ObservableCollection<FileModel>();
                AddDir(_files, dir);
                AddFiles(_files, dir);
                return _files;
            }
            else return _files;
        }

        private void AddDir(ObservableCollection<FileModel> filesCollection, DirectoryInfo dir)
        {
            if (dir.Exists == true)
            {
                if (dir.Parent!=null)
                {
                    filesCollection.Add(new FileModel() { FileName = "[ . . ]", FullPath = dir.Parent.FullName});
                }
                DirectoryInfo[] folders = dir.GetDirectories();
                    foreach (var f in folders)
                    {
                    //не добавляем системные каталоги
                        if (!f.Attributes.HasFlag(FileAttributes.Hidden|FileAttributes.Directory))
                            filesCollection.Add(new FileModel()
                            {
                                FileName = f.Name,
                                IsDirectory = true,
                                FullPath = f.FullName,
                                Attributes = f.Attributes.ToString()
                            });
                    }
               
            }
        }

        private void AddFiles(ObservableCollection<FileModel> filesCollection, DirectoryInfo dir)
        {
            if (dir.Exists == true)
            {
                FileInfo[] files = dir.GetFiles();
                foreach (var file in files)
                {
                    filesCollection.Add(new FileModel()
                    {
                        FileName = file.Name,
                        Date = file.LastAccessTime,
                        Size = file.Length,
                        FullPath = file.FullName,
                        Extension = file.Extension,
                        Attributes = file.Attributes.ToString(),
                    });
                }
            }
        }

        private void Drive_left_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tmp = new ObservableCollection<FileModel>();
            Drive dr = (sender as ComboBox).SelectedItem as Drive;
            CurrentLeft.Text = dr.Name;
            FilesL = InitFiles(tmp, dr.Name);
            LeftField.ItemsSource = FilesL;
        }

        private void Drive_right_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tmp = new ObservableCollection<FileModel>();
            Drive dr = (sender as ComboBox).SelectedItem as Drive;
            CurrentRight.Text = dr.Name;
            FilesR = InitFiles(tmp, dr.Name);
            RightField.ItemsSource = FilesR;
        }

        private void LeftField_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                FileModel i = ((ListView)sender).SelectedItem as FileModel;
                var dirExist = System.IO.Directory.Exists(i.FullPath);
                if (dirExist != false)
                {
                    if (i != null)
                    {
                        CurrentLeft.Text = i.FullPath;
                        ChangeView(FilesL, i.FullPath, LeftField);
                    }
                }
            }
        }

        private void RightField_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileModel i = ((ListView)sender).SelectedItem as FileModel;
            var dirExist = System.IO.Directory.Exists(i.FullPath);
            if (dirExist != false)
            {
                if (i != null)
                {
                    CurrentRight.Text = i.FullPath;
                    ChangeView(FilesR, i.FullPath, RightField);
                }
            }
        }

        private void ChangeView(ObservableCollection<FileModel> FilesList, string dir, ListView list)
        {
            FilesList = InitFiles(FilesList, dir);
            list.ItemsSource = FilesList;
        }
    }
}
