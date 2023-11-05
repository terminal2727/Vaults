using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using VaultsII.MediaStorage;
using WpfAnimatedGif;
using FFMpegCore;
using System.Threading.Tasks;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Shapes;
using System.Data;
using System.Windows;

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
 *     5. Ad nauseum
 */

namespace VaultsII.Display {
    public static class MosaicConstructor {
        public static async Task<List<FrameworkElement>> ConstructHorizontalMosaic(this AlbumData data) {
            await data.Media.UpdateContainerMetadata();
            data.SortMedia();

            List<FrameworkElement> segments = new();
            List<Container> contestants = new();

            #region Calculating items-per-line
            for (int i = 0; i < data.Media.Count;) {
                contestants.Clear();

                for (int j = 0; j < Configs.PreferredItemsPerLine; j++) {
                    if (i + j < data.Media.Count) { contestants.Add(data.Media[i + j]); };
                }

                double tallest = contestants.GetTallestHeight();
                double cumulativeWidth = contestants.ScaleWidthsToHeight(tallest).GetCumulativeWidth();
                double newHeight = (cumulativeWidth / Configs.TotalWidth) * tallest;

                if (newHeight > Configs.MaxHeight) {
                    int loop = 1;
                    while (newHeight > Configs.MaxHeight) {
                        if (contestants.Count >= loop) {
                            contestants.RemoveAt(contestants.Count - loop);
                        } else {
                            // If we try to remove one more image when there's only one image left
                            // there's nothing else to do, and we'll just have to be satisfied with
                            // having a taller line
                            break; 
                        }

                        tallest = contestants.GetTallestHeight();
                        cumulativeWidth = contestants.ScaleWidthsToHeight(tallest).GetCumulativeWidth();
                        newHeight = (Configs.TotalWidth / cumulativeWidth) * tallest;

                        loop++;
                    }
                } else if (newHeight < Configs.MinHeight) {
                    int loop = 1;
                    while (newHeight < Configs.MinHeight) {
                        if (data.Media.Count > i + (Configs.PreferredItemsPerLine - 1) + loop) { 
                            contestants.Add(data.Media[i + (Configs.PreferredItemsPerLine - 1) + loop]); 
                        } else {
                            // If we try grabbing a new image when there's no images left, there's
                            // nothing else to do, and we'll just have to be satisfied with having
                            // a shorter line.
                            break;
                        }

                        tallest = contestants.GetTallestHeight();
                        cumulativeWidth = contestants.ScaleWidthsToHeight(tallest).GetCumulativeWidth();
                        newHeight = (Configs.TotalWidth / cumulativeWidth) * tallest;

                        loop++;
                    }
                }
                #endregion

                #region Constructing the StackPanel
                StackPanel panel = new() { Orientation = Orientation.Horizontal };

                foreach (Container container in contestants) {
                    Uri path = new(container.FilePath, UriKind.RelativeOrAbsolute);

                    bool isVideo = container is VideoContainer;
                    object source = isVideo ? path : new BitmapImage(path);

                    if (isVideo) {
                        MediaElement video = new() {
                            Source = (Uri)source,
                            Height = newHeight,
                            LoadedBehavior = MediaState.Pause
                        };

                        panel.Children.Add(video);
                    } else {
                        ImageSource imageSource = (ImageSource)source;
                        Image image = container is ImageContainer ?
                                      new() { Source = imageSource, Height = newHeight } :
                                      new() { Height = newHeight };

                        if (image.Source == null) {
                            ImageBehavior.SetAnimatedSource(image, imageSource);
                            ImageBehavior.SetRepeatBehavior(image, RepeatBehavior.Forever);
                        }

                        panel.Children.Add(image);
                    }

                    Rectangle spacer = new() { Height = newHeight, 
                        Width = Configs.SpacerWidth, 
                        Fill = Brushes.Transparent
                    };

                    panel.Children.Add(spacer);
                }

                segments.Add(panel);
                segments.Add(new Rectangle() { Height = Configs.SpacerWidth, 
                    Width = Configs.TotalWidth, 
                    Fill = Brushes.Transparent 
                });
                #endregion

                i += contestants.Count;
            }

            return segments;
        }

        private static double GetCumulativeWidth(this double[] widths) {
            double total = widths.Length * Configs.SpacerWidth; // So there's room for the spacers
            foreach (double width in widths) { total += width; }
            return total;
        }

        private static double[] ScaleWidthsToHeight(this List<Container> items, double newHeight) {
            double[] scaled = new double[items.Count];
            for (int i = 0; i < items.Count; i++) {
                scaled[i] = (newHeight / items[i].Height) * items[i].Width;
            }
            return scaled;
        }

        private static double GetTallestHeight(this List<Container> items) {
            double tallest = 0;
            foreach (Container container in items) {
                if (container.Height > tallest) { tallest = container.Height; }
            }
            return tallest;
        }
    }
    public static class Configs {
        public readonly static double MaxHeight = 1000;
        public readonly static double MinHeight = 975;
        public readonly static int PreferredItemsPerLine = 7;
        public readonly static int SpacerWidth = 3;

        public static double TotalWidth { get; private set; }
        public static SortingStyle Style { get; private set; } = SortingStyle.Chronological;
        public static SortingDirections Direction { get; private set; } = SortingDirections.Ascending;

        public static void SetTotalWidth(double width) => TotalWidth = width;
        public static void SetStyle(SortingStyle style) => Style = style;
        public static void SetDirection(SortingDirections direction) => Direction = direction;

        public enum SortingStyle {
            Chronological, Name, Date, Custom
        }

        public enum SortingDirections {
            Ascending, Descending, Custom
        }
    }
}