﻿using System;
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
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace TakenokoMusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<MediaFile> _FileList = new ObservableCollection<MediaFile>();
        private MediaFile _CurrentMediaFile = null;

        public MainWindow()
        {
            InitializeComponent();

            this.Player.LoadedBehavior = MediaState.Manual;

            this.FileListBox.MouseDoubleClick += FileListBox_MouseDoubleClick;
            this.ReloadButton.Click += ReloadButton_Click;
            this.PreviousButton.Click += PreviousButton_Click;
            this.PlayButton.Click += PlayButton_Click;
            this.NextButton.Click += NextButton_Click;
            this.Player.MediaEnded += Player_MediaEnded;
            this.Closing += MainWindow_Closing;

            this.FileListBox.ItemsSource = _FileList;

            this.FolderPathTextbox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            this.LoadConfigFile();
            this.LoadFileList();
        }

        private void LoadConfigFile()
        {
            try
            {
                var filePath = System.IO.Path.Combine(Environment.CurrentDirectory, "Config.txt");
                var json = File.ReadAllText(filePath);
                var config = JsonConvert.DeserializeObject<ConfigData>(json);
                this.Width = config.Width;
                this.Height = config.Height;
                this.FolderPathTextbox.Text = config.FolderPath;
            }
            catch
            {

            }
        }

        private void FileListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mediaFile = this.FileListBox.SelectedItem as MediaFile;
            this.Play(mediaFile);
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            this.LoadFileList();
        }
        private void LoadFileList()
        {
            _FileList.Clear();
            foreach (var filePath in Directory.EnumerateFiles(this.FolderPathTextbox.Text, "*.mp3"
                , SearchOption.AllDirectories))
            {
                _FileList.Add(new MediaFile(filePath));
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            var mediaFile = this.GetPreviousMediaFile();
            this.Play(mediaFile);
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var mediaFile = this.FileListBox.SelectedItem as MediaFile;
            if (mediaFile == null)
            {
                MessageBox.Show("曲を選択してください。");
                return;
            }

            if (_CurrentMediaFile == null)
            {
                this.Play(mediaFile);
            }
            else
            {
                this.Player.Pause();
                _CurrentMediaFile = null;
                this.PlayButtonIcon.Source = new BitmapImage(new Uri("./Icon/PlayIcon.png", UriKind.Relative));
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var mediaFile = this.GetNextMediaFile();
            this.Play(mediaFile);
        }
        private void Play(MediaFile mediaFile)
        {
            var filePath = mediaFile.FilePath;
            if (this.Player.Source == null ||
                this.Player.Source.LocalPath != filePath)
            {
                this.Player.Source = new Uri(filePath);
            }
            this.Player.Play();
            this.FileListBox.SelectedItem = mediaFile;
            _CurrentMediaFile = mediaFile;
            this.PlayButtonIcon.Source = new BitmapImage(new Uri("./Icon/PauseIcon.png", UriKind.Relative));
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.Player.Stop();

            if (this.IsContinueCheckBox.IsChecked == true)
            {
                var mediaFile = this.GetNextMediaFile();
                this.Play(mediaFile);
            }
            else
            {
                _CurrentMediaFile = null;
            }
        }
        private MediaFile GetPreviousMediaFile()
        {
            var index = _FileList.IndexOf(_CurrentMediaFile);
            if (index == 0)
            {
                index = _FileList.Count;
            }
            var mediaFile = _FileList[index - 1];
            return mediaFile;
        }
        private MediaFile GetNextMediaFile()
        {
            var index = _FileList.IndexOf(_CurrentMediaFile);
            if (_FileList.Count == index + 1)
            {
                index = -1;
            }
            var mediaFile = _FileList[index + 1];
            return mediaFile;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var w = this.Width;
            var h = this.Height;

            var config = new ConfigData();
            config.Width = this.Width;
            config.Height = this.Height;
            config.FolderPath = this.FolderPathTextbox.Text;
            var json = JsonConvert.SerializeObject(config);

            var filePath = System.IO.Path.Combine(Environment.CurrentDirectory, "Config.txt");
            File.WriteAllText(filePath, json);
        }
    }
}