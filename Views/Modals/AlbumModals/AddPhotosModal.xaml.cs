using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VaultsII.Display;
using VaultsII.MediaStorage;
using WpfAnimatedGif;
using static VaultsII.Display.MosaicConstructor;

namespace VaultsII.Views.Modals.AlbumModals
{
    /// <summary>
    /// Interaction logic for AddPhotosModal.xaml
    /// </summary>
    public partial class AddPhotosModal : Window {
        private readonly AlbumStorage storage;
        private List<FrameworkElement> segments;
        private readonly List<Container> selected = new();

        private const double MAX_HEIGHT = 500;
        private const double MIN_HEIGHT = 450;

        public AddPhotosModal() {
            storage = AlbumStorage.Instance;

            InitializeComponent();

            CreateMosaic();
        }

        private async void CreateMosaic() {
            SelectedPhotos.Text = $"Loading {storage.Everything.Media.Count}";

            if (!Body.IsLoaded) { await Body.GetLoadedAwaitable(); }

            Configs.SetMaxHeight(MAX_HEIGHT);
            Configs.SetMinHeight(MIN_HEIGHT);
            Configs.SetTotalWidth(Body.ActualWidth);

            PopulateMosaic();

            SelectedPhotos.Text = $"Photos Selected: {0}";
        }

        private async void PopulateMosaic() {
            segments = new();

            segments = await storage.Everything.ConstructHorizontalMosaic();

            Body.ItemsSource = segments;

            foreach (FrameworkElement element in segments) {
                if (element is not StackPanel panel) { continue; }

                panel.Background = Brushes.Transparent;

                foreach (var item in panel.Children) {
                    if (item is Image image) {
                        image.MouseLeftButtonDown += (o, e) => UpdateSelected(o, e);
                    } else if (item is MediaElement video) {
                        video.MouseLeftButtonDown += (o, e) => UpdateSelected(o, e);

                        video.MouseEnter += (o, e) => PeekVideo(o, e, true);
                        video.MouseLeave += (o, e) => PeekVideo(o, e, false);
                    }
                }
            }
        }

        private void UpdateSelected(object sender, MouseButtonEventArgs e) {
            string source;

            if (sender is Image image) {
                source = image.Source.GetType() != typeof(BitmapImage) ? 
                    ImageBehavior.GetAnimatedSource(image).ToString() : 
                    ((BitmapImage)image.Source).UriSource.ToString();

                if (!storage.Everything.TryGetContainer(source, out Container container)) { return; }

                image.RenderTransformOrigin = new Point(0.5, 0.5);

                if (selected.Contains(container)) {
                    selected.Remove(container);
                    image.RenderTransform = new ScaleTransform() { ScaleX = 1, ScaleY = 1 };
                } else {
                    selected.Add(container);
                    image.RenderTransform = new ScaleTransform() { ScaleX = 0.95, ScaleY = 0.95 };
                }
            } else if (sender is MediaElement video) {
                source = video.Source.ToString();

                if (!storage.Everything.TryGetContainer(source, out Container container)) { return; }

                video.RenderTransformOrigin = new Point(0.5, 0.5);

                if (selected.Contains(container)) {
                    selected.Remove(container);
                    video.RenderTransform = new ScaleTransform() { ScaleX = 1, ScaleY = 1 };
                } else {
                    selected.Add(container);
                    video.RenderTransform = new ScaleTransform() { ScaleX = 0.95, ScaleY = 0.95 };
                }
            }

            SelectedPhotos.Text = $"Photos Selected: {selected.Count}";
        }

        private static async void PeekVideo(object o, MouseEventArgs e, bool isEntering) {
            MediaElement video = (MediaElement)o;
            video.LoadedBehavior = MediaState.Manual;

            if (!isEntering) {
                video.Stop();
                return;
            }

            await Task.Delay(500);

            if (e.LeftButton == MouseButtonState.Pressed) { return; }
            if (!video.IsMouseOver) { return; }

            video.Volume = 0;
            video.Play();
        }

        private void AddPhotos_Click(object sender, RoutedEventArgs e) {
            foreach (Container container in selected) {
                storage.Current.AddMedia(container.FilePath);
            }

            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }

    public static class AlbumExtensions {
        public static Task GetLoadedAwaitable(this ItemsControl item) {
            var tcs = new TaskCompletionSource<object>();

            void Completed() {
                item.Loaded -= (s, e) => Completed();
                tcs.SetResult(null);
            }

            item.Loaded += (s, e) => Completed();
            return tcs.Task;
        }
    }
}
