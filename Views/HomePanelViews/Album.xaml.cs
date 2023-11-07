using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using VaultsII.Core;
using VaultsII.Display;
using VaultsII.MediaStorage;
using VaultsII.Views.Modals.AlbumModals;
using WpfAnimatedGif;

#pragma warning disable CS8604 // Possible null reference argument.

/*
 * TODO:
 * Add drag and drop feature (Maybe)
 * Add overlay division screen
 */

namespace VaultsII.Views.HomePanelViews {
    /// <summary>
    /// Interaction logic for Album.xaml
    /// </summary>
    public partial class Album : UserControl {
        private bool isPlaying = false;
        private int numberOfColumns = 5;

        private List<FrameworkElement> segments;

        private TimeSpan lastPosition = TimeSpan.Zero;

        private LayoutDirection layoutDirection = LayoutDirection.Horizontal;

        private readonly AlbumStorage storage;
        private readonly ViewControl control;

        private const double MAX_HEIGHT = 1000;
        private const double MIN_HEIGHT = 950;

        public Album() {
            storage = AlbumStorage.Instance;
            control = ViewControl.Instance;

            storage.OnCurrentAlbumChange += (s, e) => CreateMosaic();
            control.OnViewChanged += (s, e) => UpdateTitle(storage.Current.Name);

            WindowSizeChange.OnWindowSizeChanged += (s, e) => CreateMosaic();

            InitializeComponent();

            CreateMosaic();

            RectangleOverlay.MouseDown += (s, e) => { 
                Overlay.Visibility = Visibility.Hidden;
                Overlay.Children.RemoveAt(2); // Should always be the image
            };
        }

        private async void CreateMosaic() {
            if (!Body.IsLoaded) { await Body.GetLoadedAwaitable(); }

            if (storage.Current.Media.Count != 0) {
                Configs.SetMaxHeight(MAX_HEIGHT);
                Configs.SetMinHeight(MIN_HEIGHT);
                Configs.SetTotalWidth(Body.ActualWidth);

                PopulateMosaic();
            } else {
                ClearAlbumContents();
            }

            Add.Visibility = storage.Current == storage.Everything ? Visibility.Hidden : Visibility.Visible;
        }

        public async void PopulateMosaic() {
            segments = new();

            if (layoutDirection == LayoutDirection.Horizontal) {
                segments = await storage.Current.ConstructHorizontalMosaic();
            } else {
                /*segments = storage.Current.ConstructVerticalMosaic();*/
            }

            Body.ItemsSource = segments;

            foreach (FrameworkElement segment in segments) {
                if (segment is not StackPanel panel) { continue; }

                panel.Background = Brushes.Transparent;

                foreach (var item in panel.Children) {
                    if (item is Image image) {
                        image.MouseLeftButtonDown += (s, e) => ShowOverlay(s, e);;
                    } else if (item is MediaElement video) {
                        video.MouseLeftButtonDown += (s, e) => ShowOverlay(s, e);

                        video.MouseEnter += (o, e) => PeekVideo(o, e, true);
                        video.MouseLeave += (o, e) => PeekVideo(o, e, false);
                    }
                }
            }
        }

        private static async void PeekVideo(object o, MouseEventArgs e, bool isEntering) {
            if (e.LeftButton == MouseButtonState.Pressed) { return; }

            MediaElement video = (MediaElement)o;
            video.LoadedBehavior = MediaState.Manual;

            if (!isEntering) {
                video.Stop();
                return;
            }

            await Task.Delay(500);

            if (!video.IsMouseOver) { return; }

            video.Volume = 0;
            video.Play();
        }

        private void ShowOverlay(object sender, MouseButtonEventArgs e) {
            Overlay.Visibility = Visibility.Visible;

            if (sender is Image imageSource) {
                Image image = new() {
                    Margin = new Thickness(53),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                if (imageSource.Source is not BitmapImage _) {
                    ImageSource source = ImageBehavior.GetAnimatedSource(imageSource);

                    ImageBehavior.SetAnimatedSource(image, source);
                    ImageBehavior.SetRepeatBehavior(image, RepeatBehavior.Forever);
                } else {
                    image = new() {
                        Source = imageSource.Source, 
                        Margin = image.Margin,
                        HorizontalAlignment = image.HorizontalAlignment,
                        VerticalAlignment = image.VerticalAlignment
                    };
                }

                Overlay.Children.Add(image); 
            } else if (sender is MediaElement elementSource) {
                MediaElement video = new() {
                    Source = elementSource.Source,
                    Margin = new Thickness(53),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    LoadedBehavior = MediaState.Play
                };

                video.MouseLeftButtonDown += OverlayTogglePlay;
                video.Position = lastPosition; // Probably unneeded

                Overlay.Children.Add(video);

                isPlaying = true;
            }
        }

        private void OverlayTogglePlay(object sender, MouseButtonEventArgs e) {
            MediaElement video = (MediaElement)sender;

            video.LoadedBehavior = MediaState.Manual;

            if (!isPlaying) {
                isPlaying = true;
                video.Position = lastPosition;
                video.Play();
            } else {
                isPlaying = false;
                lastPosition = video.Position;
                video.Stop();
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
            modal.Show();
        }

        private void AlbumNameDisplay_LostFocus(object sender, RoutedEventArgs e) {
            if (storage.Current.Name != AlbumNameDisplay.Text) {
                storage.Current.Name = AlbumNameDisplay.Text;
                UpdateTitle(storage.Current.Name);
            }
        }

        private void AlbumNameDisplay_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter && storage.Current.Name != AlbumNameDisplay.Text) {
                storage.Current.Name = AlbumNameDisplay.Text;
                UpdateTitle(storage.Current.Name);
            }
        }

        private void SwitchLayout(bool horizontal) => layoutDirection = horizontal ? LayoutDirection.Horizontal : LayoutDirection.Vertical;
        private void UpdateTitle(string name) => AlbumNameDisplay.Text = name;

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
    }
}
