using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using VaultsII.MediaStorage;
using WpfAnimatedGif;
using System.Windows.Input;
using FFMpegCore;

namespace VaultsII.Display {
    public static class MosaicConstructor {

        public static List<StackPanel> ConstructHorizontalMosaic(AlbumData data, double maximumAspectLength, double totalItemSpace) {
            data.SortMedia();

            List<StackPanel> segments = new();
            List<EmptySpace> spaces = new();

            segments.Add(new StackPanel() { Orientation = Orientation.Horizontal });

            int currentSegmentIndex = 0;
            double currentSegmentsLength = 0;

            foreach (Container container in data.Media) {
                if (container is ImageContainer || container is GifContainer) {
                    ImageContainer i_container = container as ImageContainer;
                    GifContainer g_container = container as GifContainer;

                    AspectRatio ratio;
                    Image image;

                    ImageSource source = new BitmapImage(new Uri(i_container != null ? 
                                                                 i_container.FilePath : 
                                                                 g_container.FilePath));

                    ratio = GetAspectRatio(source, maximumAspectLength, totalItemSpace);

                    // Differentiate different behavior for images/gifs
                    if (i_container != null) {
                        // Only want to assign the source if it's actually an image
                        image = new() {
                            Source = source,
                            Height = ratio.Height,
                            Width = ratio.Width,
                        };
                    } else {
                        image = new() {
                            Height = ratio.Height,
                            Width = ratio.Width
                        };

                        // Gif specific settings
                        ImageBehavior.SetAnimatedSource(image, source);
                        ImageBehavior.SetRepeatBehavior(image, RepeatBehavior.Forever);
                    }

                    // Determines if the image can fit into any spaces
                    bool skip = false;

                    for (int i = 0; i < spaces.Count; i++) {
                        if (ratio.Width > spaces[i].Space) { continue; }

                        segments[spaces[i].Index].Children.Add(image);
                        spaces[i] = new(spaces[i].Index, spaces[i].Space - ratio.Width);

                        skip = true;
                        break;
                    }

                    if (skip) { continue; }

                    // Determines if the line's gotten too long
                    if (currentSegmentsLength + ratio.Width > totalItemSpace) {
                        spaces.Add(new(currentSegmentIndex, totalItemSpace - currentSegmentsLength));

                        StackPanel panel = new() { Orientation = Orientation.Horizontal };

                        panel.Children.Add(image);
                        segments.Add(panel);

                        currentSegmentsLength = ratio.Width;

                        currentSegmentIndex++;
                        continue;
                    }

                    // Else, adds the line to the current segment
                    currentSegmentsLength += ratio.Width;

                    segments[currentSegmentIndex].Children.Add(image);
                } else if (container is VideoContainer v_container) {
                    var dimensions = GetVideoDimensions(v_container.FilePath);

                    double scaledHeight = maximumAspectLength;
                    double scaledWidth = maximumAspectLength * ((double)dimensions.width / (double)dimensions.height);

                    if (scaledWidth > totalItemSpace) {
                        scaledWidth = totalItemSpace;
                        scaledHeight = scaledWidth * (dimensions.width / dimensions.height);
                    }

                    AspectRatio ratio = new(scaledWidth, scaledHeight);
                    Uri path = new(v_container.FilePath, UriKind.RelativeOrAbsolute);

                    MediaElement video = new() { 
                        Source = path,
                        Height = scaledHeight,
                        Width = scaledWidth,
                        LoadedBehavior = MediaState.Stop
                    };

                    // Determines if the image can fit into any spaces
                    bool skip = false;

                    for (int i = 0; i < spaces.Count; i++) {
                        if (ratio.Width > spaces[i].Space) { continue; }

                        segments[spaces[i].Index].Children.Add(video);
                        spaces[i] = new(spaces[i].Index, spaces[i].Space - ratio.Width);

                        skip = true;
                        break;
                    }

                    if (skip) { continue; }

                    // Determines if the line's gotten too long
                    if (currentSegmentsLength + ratio.Width > totalItemSpace) {
                        spaces.Add(new(currentSegmentIndex, totalItemSpace - currentSegmentsLength));

                        StackPanel panel = new() { Orientation = Orientation.Horizontal };

                        panel.Children.Add(video);
                        segments.Add(panel);

                        currentSegmentsLength = ratio.Width;

                        currentSegmentIndex++;
                        continue;
                    }

                    // Else, adds the line to the current segment
                    currentSegmentsLength += ratio.Width;

                    segments[currentSegmentIndex].Children.Add(video);
                }
            }

            return segments;
        }

        public static Mosaic ConstructVerticalMosaic() {
            return default;
        }

        private static (int width, int height) GetVideoDimensions(string path) {
            var info = FFProbe.Analyse(path);
            var stream = info.PrimaryVideoStream;

            if (stream != null) {
                int width = stream.Width;
                int height = stream.Height;

                return (width, height);
            }

            return (-1, -1);
        }

        private static AspectRatio GetAspectRatio(ImageSource source, double maximumAspectLength, double totalItemSpace) {
            double scaledHeight = maximumAspectLength;
            double scaledWidth = maximumAspectLength * (source.Width / source.Height);

            if (scaledWidth > totalItemSpace) {
                scaledWidth = totalItemSpace;
                scaledHeight = scaledWidth * (source.Width / source.Height);
            }

            return new AspectRatio(scaledWidth, scaledHeight);
        }

        static MosaicConstructor() {

        }
    }

    public struct Mosaic {
        public List<StackPanel> segments { get; set; }

        public Mosaic(List<StackPanel> segments) {
            this.segments = segments;
        }
    }

    public struct AspectRatio {
        public double Width { get; }
        public double Height { get; }

        public AspectRatio(double Width, double Height) {
            this.Width = Width;
            this.Height = Height;
        }
    }

    public readonly struct EmptySpace {
        public readonly int Index { get; }
        public readonly double Space { get; }

        public EmptySpace(int index, double space) {
            Index = index;
            Space = space;
        }
    }
}
