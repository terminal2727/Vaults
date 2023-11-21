using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using VaultsII.MediaStorage;
using WpfAnimatedGif;

namespace VaultsII.Controls {
    /// <summary>
    /// Interaction logic for AlbumOverlay.xaml
    /// </summary>
    public partial class AlbumOverlay : UserControl {
        public int ColumnIndex { get; set; }
        public UIElement InteriorElement { get; private set; }
        public List<UIElement> Elements { get; private set; }

        public event RoutedEventHandler OnHorizontalDivide;
        public event RoutedEventHandler OnVerticalDivide;
        public event RoutedEventHandler OnRemove;

        private int index = 0;

        public AlbumOverlay(int startingIndex, List<UIElement> elements) {
            InitializeComponent();

            index = startingIndex;

            Elements = elements;
            InteriorElement = elements[index];

            AssignInterior();

            Information.Text = InteriorElement.GetInformation();
        }

        private void LeftArrow_Click(object sender, RoutedEventArgs e) {
            index = index - 1 < 0 ? Elements.Count - 1 : index - 1;

            Interior.Children.Clear();
            InteriorElement = Elements[index];

            AssignInterior();

            Information.Text = InteriorElement.GetInformation();
        }

        private void RightArrow_Click(object sender, RoutedEventArgs e) {
            index = index + 1 >= Elements.Count ? 0 : index + 1;
            InteriorElement = Elements[index];

            Interior.Children.Clear();
            InteriorElement = Elements[index];

            AssignInterior();

            Information.Text = InteriorElement.GetInformation();
        }

        private void AssignInterior() {
            if (InteriorElement is Image image) {
                Image _image = image.Source.GetType() != typeof(BitmapImage) ?
                    new() :
                    new() { Source = image.Source };

                if (_image.Source == null) {
                    ImageBehavior.SetAnimatedSource(_image, ImageBehavior.GetAnimatedSource(image));
                    ImageBehavior.SetRepeatBehavior(_image, RepeatBehavior.Forever);
                }

                Interior.Children.Add(_image);
            } else if (InteriorElement is MediaElement element) {
                Interior.Children.Add(new VideoPlayer(new MediaElement() { Source = element.Source }));
            }
        }

        private void HorizontalDivide_Click(object sender, RoutedEventArgs e) {
            OnHorizontalDivide?.Invoke(this, e);
        }

        private void VerticalDivide_Click(object sender, RoutedEventArgs e) {
            OnVerticalDivide?.Invoke(this, e);
        }

        private void RemoveSegment_Click(object sender, RoutedEventArgs e) {
            OnRemove?.Invoke(this, e);
        }
    }

    public static class AlbumOverlaySystem {
        public static string GetInformation(this UIElement element) {
            Container container = element.RetrieveContainer();
            string fileName = container.GetFileName();
            string tags = container.GetTags();
            string creationDate = container.GetReadableCreationDate();
            return $"{fileName} ({creationDate}): {tags}";
        }

        public static Container RetrieveContainer(this UIElement element) {
            if (element is Image image) {
                string source = image.Source.GetType() != typeof(BitmapImage) ?
                ImageBehavior.GetAnimatedSource(image).ToString() :
                ((BitmapImage)image.Source).UriSource.ToString();

                if (AlbumStorage.Instance.Everything.TryGetContainer(source, out Container container)) { return container; ; }
            } else if (element is MediaElement video) {
                string source = video.Source.ToString();

                if (AlbumStorage.Instance.Everything.TryGetContainer(source, out Container container)) { return container; ; }
            }

            return null;
        }

        public static string GetFileName(this Container container) {
            return Path.GetFileName(container.FilePath);
        }

        public static string GetTags(this Container container) {
            string tags = "";
            foreach (string tag in container.Tags) { tags += $"{tag}, "; }
            return tags;
        }

        public static string GetReadableCreationDate(this Container container) {
            return container.Created.ToString("MMMM d, yyyy");
        }
    }
}
