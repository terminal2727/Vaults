using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using VaultsII.Display;

/*
 * To consider: Creating all 3 possible mosaics and then caching all 3 at the same time
 */

namespace VaultsII.MediaStorage {
    public class AlbumData {
        public string Name { get; set; }

        public List<Container> Media { get; private set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public static readonly string[] ImageExtensions = new string[] { ".png", ".jpg", ".jpeg", ".webp" };
        public static readonly string[] VideoExtensions = new string[] { ".mp4", ".mov", ".avi" };
        public static readonly string GifExtension = ".gif";

        public void AddMedia(string path) {
            if (ContainsPath(path)) { return; }

            string extension = Path.GetExtension(path);

            if (String.Equals(extension, GifExtension, StringComparison.OrdinalIgnoreCase)) {
                Media.Add(new GifContainer(path, Array.Empty<string>(), ContainerType.Gif));
                return;
            }

            foreach (string vidExtension in VideoExtensions) {
                if (String.Equals(extension, vidExtension, StringComparison.OrdinalIgnoreCase)) {
                    Media.Add(new VideoContainer(path, Array.Empty<string>(), ContainerType.Video));
                    return;
                }
            }

            foreach (string imageExtension in ImageExtensions) {
                if (String.Equals(extension, imageExtension, StringComparison.OrdinalIgnoreCase)) {
                    Media.Add(new ImageContainer(path, Array.Empty<string>(), ContainerType.Image));
                    return;
                }
            }
        }

        public void RemoveMedia(string path) {
            for (int i = 0; i < Media.Count; i++) {
                if (Media[i].FilePath == path) {
                    Media.RemoveAt(i);
                    return;
                }
            }
        }

        public void SortMedia() => Media = Media.OrderByDescending(item => item.Created).ToList();

        private bool ContainsPath(string path) {
            foreach (Container container in Media) {
                if (String.Equals(path, container.FilePath, StringComparison.OrdinalIgnoreCase)) { 
                    return true; 
                } else {
                    Console.WriteLine("ffs");
                }
            }

            return false;
        }

        public bool TryGetContainer(string path, out Container container) {
            container = null;

            foreach (Container c in Media) {
                Uri c_path = new(path);
                Uri p_path = new(c.FilePath, UriKind.Absolute);

                if (Uri.Compare(c_path, p_path, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) != 0) { continue; }

                container = c;
                return true;
            }

            return false;
        }

        public AlbumData(AlbumDataPackage package) {
            this.Name = package.Name;
            this.Created = package.Created;
            this.Updated = package.Updated;

            Media = new();

            for (int i = 0; i < package.Images.Length; i++) {
                if (package.Images[i] != null) {
                    Media.Add(package.Images[i]);
                } else if (package.Videos[i] != null) {
                    Media.Add(package.Videos[i]);
                } else if (package.Gifs[i] != null) {
                    Media.Add(package.Gifs[i]);
                }
            }
        }

        public AlbumData(string Name) {
            this.Name = Name;
            Media = new();

            Created = DateTime.Now;
        }
    }

    public struct AlbumDataPackage {
        public string Name { get; set; }

        public ImageContainer[] Images { get; set; }
        public VideoContainer[] Videos { get; set; }
        public GifContainer[] Gifs { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public bool IsEmpty { get; set; }

        public AlbumDataPackage(string Name, List<Container> Media, DateTime Created, DateTime Updated) {
            this.Name = Name;
            this.Created = Created;
            this.Updated = Updated;

            ImageContainer[] imageContainers = new ImageContainer[Media.Count];
            VideoContainer[] videoContainers = new VideoContainer[Media.Count];
            GifContainer[] gifContainers = new GifContainer[Media.Count];

            for (int i = 0; i < Media.Count; i++) {
                if (Media[i] is ImageContainer image) {
                    imageContainers[i] = image;
                    continue;
                }

                if (Media[i] is VideoContainer video) {
                    videoContainers[i] = video;
                    continue;
                }

                if (Media[i] is GifContainer gif) {
                    gifContainers[i] = gif;
                    continue;
                }
            }

            this.Images = imageContainers;
            this.Videos = videoContainers;
            this.Gifs = gifContainers;

            IsEmpty = false;
        }

        public AlbumDataPackage() {
            Name = String.Empty;

            Images = Array.Empty<ImageContainer>();
            Videos = Array.Empty<VideoContainer>();
            Gifs = Array.Empty<GifContainer>();

            Created = default;
            Updated = default;

            this.IsEmpty = true;
        }
    }

    public static class AlbumDataSystem {
        public static void RemoveInvalidFiles(this AlbumData data, MonitoredFolders monitored) {
            for (int i = 0; i < data.Media.Count; i++) {
                string parentPath = Path.GetDirectoryName(data.Media[i].FilePath) ?? "";
                if (File.Exists(data.Media[i].FilePath) && monitored.Contains(parentPath)) { continue; }
                data.Media.RemoveAt(i);
            }
        }
    }
}
