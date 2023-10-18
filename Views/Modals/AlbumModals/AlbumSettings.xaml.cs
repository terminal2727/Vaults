using System.Windows;
using System.Windows.Input;
using VaultsII.MediaStorage;

namespace VaultsII.Views.Modals.AlbumModals {
    /// <summary>
    /// Interaction logic for AlbumSettings.xaml
    /// </summary>
    public partial class AlbumSettings : Window {
        private readonly AlbumStorage storage;
        private readonly ViewControl control;

        public AlbumSettings() {
            storage = AlbumStorage.Instance;
            control = ViewControl.Instance;

            InitializeComponent();

            if (storage.Current == storage.Everything) {
                TitleTB.Text = "Cannot Delete";
                DetailTB.Text = "Cannot delete Everything album. This contains every photo, and deleting it would remove all of those photos." +
                    " If you wish to remove photos, remove their folders from the Monitored Folders list.";
                DeleteAlbum.IsEnabled = false;
            }
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void Exit_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void DeleteAlbum_Click(object sender, RoutedEventArgs e) {
            if (storage.Current == storage.Everything) { return; }

            storage.DeleteAlbum(storage.Current.Name);
            storage.SetCurrentToEverything();

            control.SetStartView();

            Close();
        }
    }
}
