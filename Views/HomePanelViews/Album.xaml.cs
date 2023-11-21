using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using VaultsII.Controls;
using VaultsII.Core;
using VaultsII.Display;
using VaultsII.MediaStorage;
using VaultsII.Views.Modals.AlbumModals;
using WpfAnimatedGif;

#pragma warning disable CS8604 // Possible null reference argument.

/*
 * TODO:
 * Add drag and drop feature (Maybe)
 * Add overlay division screen (& change overlay removal to fit). probably will have to change to a grid splitter
 * Consider adding an object pool for rectangles
 * Move Rectangle Scrolller creation over to Mosaic Constructor
 */

namespace VaultsII.Views.HomePanelViews {
    /// <summary>
    /// Interaction logic for Album.xaml
    /// </summary>
    public partial class Album : UserControl {
        private bool isPlaying = false;
        private int rectangleCount = 0;

        private List<FrameworkElement> segments;
        private List<UIElement> visualElements;

        private TimeSpan lastPosition = TimeSpan.Zero;
        private static Color color { 
            get { 
                return (Color)ColorConverter.ConvertFromString("#586f99"); 
            } 
        }

        private readonly AlbumStorage storage;
        private readonly List<Container> selected = new();
        private readonly ViewControl control;

        private const double MAX_HEIGHT = 1000;
        private const double MIN_HEIGHT = 950;

        public Album() {
            storage = AlbumStorage.Instance;
            control = ViewControl.Instance;

            storage.OnCurrentAlbumChange += (s, e) => CreateMosaic();
            control.OnViewChanged += (s, e) => AlbumNameDisplay.Text = storage.Current.Name;

            WindowSizeChange.OnWindowSizeChanged += (s, e) => CreateMosaic();

            InitializeComponent();

            CreateMosaic();
        }

        private async void CreateMosaic() {
            if (!AlbumNameDisplay.IsLoaded) { await AlbumNameDisplay.GetLoadedAwaitable(); }

            AlbumNameDisplay.Text = storage.Current.Media.Count == 0 ? storage.Current.Name : $"Loading {storage.Current.Media.Count} photos";

            if (!Body.IsLoaded) { await Body.GetLoadedAwaitable(); }

            if (storage.Current.Media.Count != 0) {
                ClearAlbumContents();

                Configs.SetMaxHeight(MAX_HEIGHT);
                Configs.SetMinHeight(MIN_HEIGHT);
                Configs.SetTotalWidth(Body.ActualWidth);

                PopulateMosaic();
            } else {
                ClearAlbumContents();
            }

            AlbumNameDisplay.Text = storage.Current.Name;

            Add.Visibility = storage.Current == storage.Everything ? Visibility.Hidden : Visibility.Visible;
        }

        public async void PopulateMosaic() {
            segments = new();
            visualElements = new();

            segments = await storage.Current.ConstructHorizontalMosaic(); 

            Body.ItemsSource = segments;

            foreach (FrameworkElement segment in segments) {
                if (segment is not StackPanel panel) { continue; }

                panel.Background = Brushes.Transparent;

                foreach (var item in panel.Children) {
                    if (item is Border border) {
                        visualElements.Add(border.Child);

                        border.MouseLeftButtonDown += (s, e) => LeftClick(s, e);
                        border.MouseEnter += (o, e) => PeekVideo(o, e, true); // Only applicable if video
                        border.MouseLeave += (o, e) => PeekVideo(o, e, false); // Only applicable if video
                    }
                }
            }


            AlbumStorage.SaveAlbumChanges(storage.Current);
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

            await Task.Delay(500);

            if (!video.IsMouseOver) { return; }

            video.Volume = 0;
            video.Play();
        }

        private void LeftClick(object sender, MouseButtonEventArgs e) {
            // May have to change to a grid splitter. Standby
            if (e.ClickCount == 2) {
                ClearSelected();

                Overlay.Visibility = Visibility.Visible;

                if (sender is not Border border) { return; }

                if (border.Child is Image imageSource) {
                    int index = 0;
                    foreach (UIElement element in visualElements) {
                        if (element as Image == imageSource) {
                            Overlay.Children.Add(new AlbumOverlay(index, visualElements));
                            break;
                        }
                        index++;
                    }
                } else if (border.Child is MediaElement elementSource) {
                    elementSource.LoadedBehavior = MediaState.Manual;
                    elementSource.Pause();

                    int index = 0;
                    foreach (UIElement element in visualElements) {
                        if (element as MediaElement == elementSource) {
                            Overlay.Children.Add(new AlbumOverlay(index, visualElements));
                            break;
                        }
                        index++;
                    }

                    isPlaying = false;
                }

                return;
            }

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
                        AlbumNameDisplay.Text = storage.Current.Name;
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

                AlbumNameDisplay.Text = selected.Count.ToString();

                return;
            }

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

                AlbumNameDisplay.Text = storage.Current.Name;
            }
        }

        private void OverlayBack_Click(object sender, RoutedEventArgs e) {
            Overlay.Visibility = Visibility.Hidden;
            Overlay.Children.RemoveAt(2); // Should always be the media container

            lastPosition = TimeSpan.Zero;
        }

        private void Settings_Click(object sender, RoutedEventArgs e) {
            AlbumSettings modal = new();
            modal.Show();
        }

        private void Add_Click(object sender, RoutedEventArgs e) {
            AddPhotosModal modal = new();

            modal.Closed += (_, _) => {
                AlbumStorage.SaveAlbumChanges(storage.Current);
                CreateMosaic();
            };

            modal.Show();
        }

        private void AlbumNameDisplay_LostFocus(object sender, RoutedEventArgs e) {
            if (storage.Current.Name != AlbumNameDisplay.Text) {
                storage.Current.Name = AlbumNameDisplay.Text;
                AlbumNameDisplay.Text = storage.Current.Name;
            }
        }

        private void AlbumNameDisplay_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter && storage.Current.Name != AlbumNameDisplay.Text) {
                storage.Current.Name = AlbumNameDisplay.Text;
                AlbumNameDisplay.Text = storage.Current.Name;
            }
        }

        private void ClearAlbumContents() {
            Body.ItemsSource = new List<FrameworkElement>() { 
                new TextBlock() { 
                    Text = storage.Current.Media.Count == 0 ? "Nothing here!" : $"Loading {storage.Current.Media.Count} files", 
                    Style = Application.Current.FindResource("DetailText") as Style, 
                    FontSize = 32 
                } 
            };
        }
    }

    public enum LayoutDirection {
        Horizontal = 0, Vertical = 1
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

        public static Task GetLoadedAwaitable(this TextBox item) {
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
