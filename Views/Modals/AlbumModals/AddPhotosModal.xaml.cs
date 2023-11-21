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

        private const double MAX_HEIGHT = 475;
        private const double MIN_HEIGHT = 425;

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

            foreach (FrameworkElement segment in segments) {
                if (segment is not StackPanel panel) { continue; }

                panel.Background = Brushes.Transparent;

                foreach (var item in panel.Children) {
                    if (item is Border border) {
                        border.MouseLeftButtonDown += (s, e) => UpdateSelected(s, e);
                        border.MouseEnter += (o, e) => PeekVideo(o, e, true); // Only applicable if video
                        border.MouseLeave += (o, e) => PeekVideo(o, e, false); // Only applicable if video
                    }
                }
            }
        }

        private void UpdateSelected(object sender, MouseButtonEventArgs e) {
            if (Keyboard.IsKeyDown(Key.Escape)) {
                ClearSelected();
                return;
            }

            if (e.ClickCount == 1) {
                bool isVideo;
                string source;

                Container container;

                if (sender is not Border border) { return; } // Shouldn't really be necessary, but just in case.

                // Get the element's source/information
                if (border.Child is Image image) {
                    source = image.Source.GetType() != typeof(BitmapImage) ?
                        ImageBehavior.GetAnimatedSource(image).ToString() :
                        ((BitmapImage)image.Source).UriSource.ToString();

                    if (!storage.Everything.TryGetContainer(source, out container)) { return; }

                    isVideo = false;
                } else if (border.Child is MediaElement video) {
                    source = video.Source.ToString();

                    if (!storage.Everything.TryGetContainer(source, out container)) { return; }

                    isVideo = true;
                } else {
                    return;
                }

                if (selected.Contains(container)) {
                    // Remove element from selected & destroy outline
                    FrameworkElement element = isVideo ? (MediaElement)border.Child : (Image)border.Child;

                    selected.Remove(container);

                    border.BorderBrush = Brushes.Transparent;

                    element.Width += Configs.OutlineWidth;
                    element.Height += Configs.OutlineWidth;

                    border.BorderThickness = new Thickness(0);

                    if (selected.Count == 0) {
                        ClearSelected();
                        SelectedPhotos.Text = $"Photos Selected: {selected.Count}";
                        return;
                    }
                } else {
                    // Add element to selected & create outline
                    FrameworkElement element = isVideo ? (MediaElement)border.Child : (Image)border.Child;

                    selected.Add(container);

                    border.BorderBrush = new SolidColorBrush(Configs.OutlineColor);

                    element.Width -= Configs.OutlineWidth;
                    element.Height -= Configs.OutlineWidth;

                    border.BorderThickness = new Thickness(Configs.OutlineWidth);
                }

                SelectedPhotos.Text = $"Photos Selected: {selected.Count}";

                return;
            }

            SelectedPhotos.Text = $"Photos Selected: {selected.Count}";

            void ClearSelected() {
                foreach (Container container in selected) {
                    foreach (FrameworkElement element in segments) {
                        if (element is not StackPanel panel) { continue; }

                        foreach (var item in panel.Children) {
                            if (item is not Border border) { continue; }

                            border.BorderBrush = Brushes.Transparent;
                            border.BorderThickness = new Thickness(4);

                            if (border.Child is Image image) {
                                image.RenderTransform = new ScaleTransform() { ScaleX = 1, ScaleY = 1 };
                            } else if (border.Child is MediaElement video) {
                                video.RenderTransform = new ScaleTransform() { ScaleX = 1, ScaleY = 1 };
                            }
                        }
                    }
                }

                selected.Clear();
            }
        }

        private static async void PeekVideo(object o, MouseEventArgs e, bool isEntering) {
            if (e.LeftButton == MouseButtonState.Pressed) { return; }

            Border border = (Border)o;

            if (border.Child is not MediaElement video) { return; }

            video.LoadedBehavior = MediaState.Manual;

            if (!isEntering) {
                video.Pause();
                return;
            }

            await Task.Delay(250);

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
