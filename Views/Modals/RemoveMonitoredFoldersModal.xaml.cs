using System.Linq;
using System.Windows;
using VaultsII.MediaStorage;

namespace VaultsII.Views.Modals {
    /// <summary>
    /// Interaction logic for RemoveMonitoredFoldersModal.xaml
    /// </summary>
    public partial class RemoveMonitoredFoldersModal : Window {
        private readonly MonitoredFolders monitoredFolders;

        public RemoveMonitoredFoldersModal() {
            InitializeComponent();

            monitoredFolders = MonitoredFolders.Instance;
            PopulateList();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void Remove_Click(object sender, RoutedEventArgs e) {
            string[] paths = MonitoredFoldersList.SelectedItems.Cast<string>().ToArray();
            
            foreach (string path in paths) {
                monitoredFolders.RemoveMonitoredFolderPath(path);
            }

            Close();
        }

        private void PopulateList() {
            MonitoredFoldersList.Items.Clear();

            foreach (string path in monitoredFolders.GetPathsArray()) {
                MonitoredFoldersList.Items.Add(path);
            }
        }
    }
}
