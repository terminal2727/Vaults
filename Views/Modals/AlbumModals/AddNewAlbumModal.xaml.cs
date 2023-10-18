using System.IO;
using System.Windows;
using System.Windows.Input;
using VaultsII.MediaStorage;

namespace VaultsII.Views.Modals.AlbumModals {
    /// <summary>
    /// Interaction logic for AddNewAlbumModal.xaml
    /// </summary>
    public partial class AddNewAlbumModal : Window {
        private readonly AlbumStorage storage;
        private readonly char[] invalid;
        public AddNewAlbumModal() {
            storage = AlbumStorage.Instance;
            invalid = Path.GetInvalidFileNameChars();

            InitializeComponent();
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e) {
            string name = AlbumNameTextBox.Text;

            foreach (char @char in invalid) {
                if (name.Contains(@char)) {
                    AlbumNameTextBox.Text = $"Album name cannot contain character {@char}. Please create a name without it.";
                    return;
                }
            }

            if (storage.AlbumExists(name)) {
                AlbumNameTextBox.Text = "Album name taken. Please input a new one.";
                return;
            }

            storage.AddNewAlbum(name);

            Close();
        }
    }
}
