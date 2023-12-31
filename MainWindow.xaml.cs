﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VaultsII.Core;
using VaultsII.MediaStorage;
using VaultsII.Views;
using VaultsII.Views.Modals;
using WinForms = System.Windows.Forms;

namespace VaultsII {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly ViewControl control;
        private readonly MonitoredFolders monitoredFolders;
        private readonly AlbumStorage storage;

        public MainWindow() {
            InitializeComponent();

            control = ViewControl.Instance;
            monitoredFolders = MonitoredFolders.Instance;
            storage = AlbumStorage.Instance;

            storage.OnAlbumsListChange += (s, e) => PopulateAlbumViewsList();
            control.OnViewChanged += (s, e) => UpdateView();

            PopulateAlbumViewsList();
        }

        private void PopulateAlbumViewsList() {
            ViewButtonsColumn.Children.Clear();

            var everythingButton = new RadioButton() {
                Content = "Everything",
                Style = (Style)App.Current.Resources["AlbumButtons"],
                DataContext = control
            };

            everythingButton.Checked += (s, e) => CheckButton(s, e);

            ViewButtonsColumn.Children.Add(everythingButton);

            for (int i = 0; i < storage.Albums.Count; i++) {
                var button = new RadioButton() {
                    Content = $"{storage.Albums[i].Name}",
                    Style = (Style)App.Current.Resources["AlbumButtons"],
                    DataContext = control
                };

                button.Checked += (s, e) => CheckButton(s, e);

                ViewButtonsColumn.Children.Add(button);
            }

            void CheckButton(object sender, RoutedEventArgs e) {
                View.DataContext = control;

                string a = ((RadioButton)sender).Content.ToString() ?? "name";

                storage.SetCurrentAlbum(
                    ((RadioButton)sender).Content.ToString() ?? "name" // Shouldn't ever be null, just to get that annoying warning to go away
                );

                control.SetAlbumView();
            }

            control.UpdateUI();
        }

        private void UpdateView() {
            switch (control.CurrentView) {
                case Views.HomePanelViews.StartView:
                    UncheckButtons();
                    break;
                case Views.HomePanelViews.AlbumView:
                    // I don't think anything needs to be placed here... May something contextual in the future
                    break;
                default:
                    Console.WriteLine(control.CurrentView.ToString());
                    break;
            }
        }

        private void UncheckButtons() {
            foreach (var button in ViewButtonsColumn.Children) {
                RadioButton rButton = (RadioButton)button;
                rButton.IsChecked = false;
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e) {
            if (WindowState != WindowState.Maximized) {
                WindowState = WindowState.Maximized;
            } else {
                WindowState = WindowState.Normal;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void GrabBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void Back_Click(object sender, RoutedEventArgs e) {
            control.RevertView();
        }

        private void Home_Click(object sender, RoutedEventArgs e) {
            control.SetStartView();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            WindowSizeChange.ChangedWindowSize();
        }

        private void AddMonitoredFolder_Click(object sender, RoutedEventArgs e) {
            WinForms.FolderBrowserDialog dialog = new();
            WinForms.DialogResult result = dialog.ShowDialog();

            if (result == WinForms.DialogResult.OK) {
                string path = dialog.SelectedPath;
                monitoredFolders.AddNewMonitoredFolderPath(path);
            }
        }

        private void RemoveMonitoredFolder_Click(object sender, RoutedEventArgs e) {
            RemoveMonitoredFoldersModal removeMonitoredFoldersModal = new();
            removeMonitoredFoldersModal.ShowDialog();
        }

        private void SortNewContent_Click(object sender, RoutedEventArgs e) {

        }
    }
}
