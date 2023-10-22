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

/*
 * TODO:
 * Parallelize Container => Image/AnimatedImage/MediaElement creation
 * Add drag and drop feature
 * Add clickable media containers
 * Add clean up function where events are removed when album is changed
 */

namespace VaultsII.Views.HomePanelViews {
    /// <summary>
    /// Interaction logic for Album.xaml
    /// </summary>
    public partial class Album : UserControl {
        private bool isPlaying = false;
        private int numberOfColumns = 0;
        private double totalItemSpace = 0;
        private double maximumAspectLength = 325;

        private TimeSpan lastPosition = TimeSpan.Zero;

        private LayoutDirection layoutDirection = LayoutDirection.Horizontal;

        private readonly List<StackPanel> segments;
        private readonly AlbumStorage storage;
        private readonly ViewControl control;
        
        private const int MIN_ITEMS_PER_LINE = 3;

        public Album() {
            storage = AlbumStorage.Instance;
            control = ViewControl.Instance;

            segments = new();

            storage.OnCurrentAlbumChange += (s, e) => UpdateTitle(storage.Current.Name);
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
            UpdateTitle("Loading...");

            await Body.GetLoadedAwaitable();

            UpdateDisplay();

            PopulateMosaic();

            UpdateTitle(storage.Current.Name);
        }

        private void UpdateDisplay() {
            totalItemSpace = Body.ActualWidth;
            int itemsPerLine = (int)MathF.Floor((float)(totalItemSpace / maximumAspectLength));

            // Only really needed if the window is scaled down. 325 * 3 + 15 should be less that 1080
            if (itemsPerLine < MIN_ITEMS_PER_LINE) {
                maximumAspectLength = totalItemSpace / MIN_ITEMS_PER_LINE;
                numberOfColumns = MIN_ITEMS_PER_LINE;
            } 

            numberOfColumns = itemsPerLine;
        }

        public void PopulateMosaic() {
            List<StackPanel> segments = new();

            if (layoutDirection == LayoutDirection.Horizontal) {
                segments = MosaicConstructor.ConstructHorizontalMosaic(storage.Current, maximumAspectLength, totalItemSpace);
            } else { 
                //constructed = MosaicConstructor.ConstructVerticalMosaic(); 
            }

            Body.ItemsSource = segments;

            foreach (var segment in segments) {
                foreach (var item in segment.Children) {
                    if (item is Image image) {
                        image.MouseLeftButtonDown += (s, e) => ShowOverlay(s, e);

                    } else if (item is MediaElement video) {
                        video.MouseLeftButtonDown += (s, e) => ShowOverlay(s, e);

                        video.MouseEnter += (o, e) => PeekVideo(o, e, true);
                        video.MouseLeave += (o, e) => PeekVideo(o, e, false);
                    }
                }
            }
        }

        private async void PeekVideo(object o, MouseEventArgs e, bool isEntering) {
            MediaElement video = (MediaElement)o;
            video.LoadedBehavior = MediaState.Manual;

            if (!isEntering) {
                video.Stop();
                return;
            }

            await Task.Delay(500); // Wait a second in case the overlay should fire instead

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
                    ImageSource sauce = ImageBehavior.GetAnimatedSource(imageSource);

                    ImageBehavior.SetAnimatedSource(image, sauce);
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

        private void UpdateTitle(string name) => AlbumNameDisplay.Text = name;

        private void SwitchLayout(bool horizontal) => layoutDirection = horizontal ? LayoutDirection.Horizontal : LayoutDirection.Vertical;

        enum LayoutDirection {
            Horizontal = 0, Vertical = 1
        }

        readonly struct EmptySpace {
            public readonly int Index { get; }
            public readonly double Space { get; }

            public EmptySpace(int index, double space) {
                Index = index;
                Space = space;
            }
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

        public static Task GetLoadedAwaitable(this Image item) {
            var tcs = new TaskCompletionSource<object>();

            void Completed() {
                item.Loaded -= (s, e) => Completed();
                tcs.SetResult(null);
            }

            item.Loaded += (s, e) => Completed();
            return tcs.Task;
        }

        public static Task GetLoadedAwaitable(this MediaElement item) {
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
