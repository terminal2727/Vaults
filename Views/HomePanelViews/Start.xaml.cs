using System;
using System.Windows;
using System.Windows.Controls;
using VaultsII.MediaStorage;
using VaultsII.Views.Modals;
using VaultsII.Views.Modals.AlbumModals;
using WinForms = System.Windows.Forms;

#pragma warning disable CS8604 // Possible null reference argument.

namespace VaultsII.Views.HomePanelViews {
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : UserControl {
        private readonly MonitoredFolders monitoredFolders;
        private readonly AlbumStorage storage;

        public Start() {
            InitializeComponent();

            monitoredFolders = MonitoredFolders.Instance;
            storage = AlbumStorage.Instance;

            monitoredFolders.OnMonitoredFoldersUpdated += (s, e) => UpdateMonitoredFoldersUI(s, e);

            MonitoredFoldersListOne.Text = monitoredFolders.GetFormattedList();
        }

        private void UpdateMonitoredFoldersUI(object sender, EventArgs e) {
            MonitoredFoldersListOne.Text = monitoredFolders.GetFormattedList();
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

        private void CreateNewAlbum_Click(object sender, RoutedEventArgs e) {
            AddNewAlbumModal modal = new AddNewAlbumModal();
            modal.ShowDialog();
        }

        private void SortNewContent_Click(object sender, RoutedEventArgs e) {

        }
    }
}
