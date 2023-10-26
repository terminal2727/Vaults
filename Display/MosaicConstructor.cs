using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using VaultsII.MediaStorage;
using WpfAnimatedGif;
using FFMpegCore;

/* TODO:
 * Modify construction function to resemble Google Photos more.
 * Best I can tell, there's variable height values per line, but only to a point - it seems like the
 * heights are always between 350 and 370 (px). It also seems to aim for ~7 items per line, which
 * seems like a good number to me.
 * 
 * The process is probably something like:
 *     1. Get 7 items
 *     2. Set their cumulative width to the line's length (+ spacers, which will have to be... transparent rectangles?)
 *     3. Check if their heights are within the predefined limits
 *     4. If they're not, remove one photo, and check again.
 *     5. Ad nauseumb
 */

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
                AspectRatio ratio;

                double scaledHeight = 0;
                double scaledWidth = 0;

                bool isVideo = container is VideoContainer;

                Uri path = new(container.FilePath, UriKind.RelativeOrAbsolute);

                object source = isVideo ? path : new BitmapImage(path);

                if (isVideo) {
                    var (width, height) = GetVideoDimensions(container.FilePath);

                    scaledHeight = maximumAspectLength;
                    scaledWidth = maximumAspectLength * ((double)width / (double)height);

                    if (scaledWidth > totalItemSpace) {
                        scaledWidth = totalItemSpace;
                        scaledHeight = scaledWidth * (width / height);
                    }

                    ratio = new(scaledWidth, scaledHeight);
                } else {
                    ratio = GetAspectRatio((ImageSource)source, maximumAspectLength, totalItemSpace);
                }

                Image image = default;
                MediaElement video = default;

                if (isVideo) {
                    video = new() {
                        Source = path,
                        Height = scaledHeight,
                        Width = scaledWidth,
                        LoadedBehavior = MediaState.Pause
                    };
                } else {
                    ImageSource imageSource = (ImageSource)source;
                    image = container is ImageContainer i_Container ?
                                new() { Source = imageSource, Height = ratio.Height, Width = ratio.Width } :
                                new() { Height = ratio.Height, Width = ratio.Width };

                    if (image.Source == null) {
                        ImageBehavior.SetAnimatedSource(image, imageSource);
                        ImageBehavior.SetRepeatBehavior(image, RepeatBehavior.Forever);
                    }
                }

                bool skip = false;

                for (int i = 0; i < spaces.Count; i++) {
                    if (ratio.Width > spaces[i].Space) { continue; }

                    segments[spaces[i].Index].Children.Add(isVideo ? video : image);
                    spaces[i] = new(spaces[i].Index, spaces[i].Space - ratio.Width);

                    skip = true;
                    break;
                }

                if (skip) { continue; }

                // Determines if the line's gotten too long
                if (currentSegmentsLength + ratio.Width > totalItemSpace) {
                    spaces.Add(new(currentSegmentIndex, totalItemSpace - currentSegmentsLength));

                    StackPanel panel = new() { Orientation = Orientation.Horizontal };

                    panel.Children.Add(isVideo ? video : image);
                    segments.Add(panel);

                    currentSegmentsLength = ratio.Width;

                    currentSegmentIndex++;
                    continue;
                }

                // Else, adds the line to the current segment
                currentSegmentsLength += ratio.Width;

                segments[currentSegmentIndex].Children.Add(isVideo ? video : image);
            }

            return segments;
        }

        public static List<StackPanel> ConstructVerticalMosaic() {
            return new();
        }

        public static List<StackPanel> ConstructMosaicFromAlbumData(AlbumData data, double maximumAspectLength, double totalItemSpace) { // We trust that the data we're given is true
            List<StackPanel> segments = new();

            int index = 0;
            foreach (List<Container> segment in data.Mosaic) {
                segments.Add(new StackPanel() { Orientation = Orientation.Horizontal });

                foreach (Container container in segment) {
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

                        segments[index].Children.Add(image);
                    } else if (container is VideoContainer v_container) {
                        var (width, height) = GetVideoDimensions(v_container.FilePath);

                        double scaledHeight = maximumAspectLength;
                        double scaledWidth = maximumAspectLength * ((double)width / (double)height);

                        if (scaledWidth > totalItemSpace) {
                            scaledWidth = totalItemSpace;
                            scaledHeight = scaledWidth * (width / height);
                        }

                        AspectRatio ratio = new(scaledWidth, scaledHeight);

                        Uri path = new(v_container.FilePath, UriKind.RelativeOrAbsolute);

                        MediaElement video = new() {
                            Source = path,
                            Height = scaledHeight,
                            Width = scaledWidth,
                            LoadedBehavior = MediaState.Pause
                        };

                        segments[index].Children.Add(video);
                    }
                }

                index++;
            }

            return segments;
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

    public enum SortDirection {
        Ascending, Descending
    }

    public enum SortType {
        Chronological, Name, Custom
    }

    public struct Mosaic {
        public bool isConstructed { get; set; }
        public List<StackPanel> segments { get; set; }
        public SortDirection sortDirection { get; set; }
        public SortType sortType { get; set; }

        public Mosaic(List<StackPanel> segments) {
            this.segments = segments;
            sortDirection = SortDirection.Ascending;
            sortType = SortType.Chronological;

            isConstructed = segments.Count > 0;
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
