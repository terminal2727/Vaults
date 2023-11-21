using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace VaultsII.Controls {
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl {
        public MediaElement Video { get; private set; }

        private bool isPlaying = true;
        private bool isMuted = true;
        private bool isUserChanging = false;

        private readonly DispatcherTimer timer = new();

        public VideoPlayer(MediaElement video) {
            InitializeComponent();

            Video = video;
            Video.LoadedBehavior = MediaState.Manual;

            VideoBody.Children.Add(Video);

            timer.Tick += UpdateTimelinePosition;
            timer.Interval = TimeSpan.FromMilliseconds(100);

            Video.Play();
            timer.Start();

        }

        private void UpdateTimelinePosition(object? sender, EventArgs e) {
            if (isUserChanging) { return; }
            if (Video.NaturalDuration.HasTimeSpan) {
                VideoTimeline.Value = (Video.Position.TotalSeconds / Video.NaturalDuration.TimeSpan.TotalSeconds) * 100;

                // For automatically resetting a video's length
                if (Video.NaturalDuration.TimeSpan.TotalSeconds / Video.Position.TotalSeconds == 1) { 
                    Video.Position = TimeSpan.FromSeconds(0); 
                    VideoTimeline.Value = 0;
                }
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e) {
            if (isPlaying) {
                Video.Pause();
            } else {
                Video.Play();
            }

            isPlaying = !isPlaying;

            PlayButton.Content = new Image() { 
                Source = new BitmapImage(new Uri(
                    isPlaying ? 
                    "pack://application:,,,/VaultsII;component/Icons/PlayIcon.png" :
                    "pack://application:,,,/VaultsII;component/Icons/PauseIcon.png")
                ) 
            };
        }

        private void MuteButton_Click(object sender, RoutedEventArgs e) {
            Video.Volume = isMuted ? 1 : 0;
            isMuted = !isMuted;
            MuteButton.Content = new Image() {
                Source = new BitmapImage(new Uri(
                    isMuted ?
                    "pack://application:,,,/VaultsII;component/Icons/AudioIcon.png" :
                    "pack://application:,,,/VaultsII;component/Icons/MuteIcon.png")
                )
            };
        }

        private void VideoTimeline_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e) {
            isUserChanging = true;
            if (isPlaying) { Video.Pause(); }
        }

        private void VideoTimeline_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e) {
            Video.Position = TimeSpan.FromSeconds((VideoTimeline.Value / 100) * Video.NaturalDuration.TimeSpan.TotalSeconds);
            if (isPlaying) { Video.Play(); }
            isUserChanging = false;
        }

    }
}
