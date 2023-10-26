using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/*
 * To consider: Creating all 3 possible mosaics and then caching all 3 at the same time
 */

namespace VaultsII.MediaStorage {
    public class AlbumData {
        public string Name { get; set; }
        
        public List<Container> Media;
        public List<List<Container>> Mosaic; 

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public static readonly string[] ImageExtensions = new string[] { ".png", ".jpg", ".jpeg", ".webp" };
        public static readonly string[] VideoExtensions = new string[] { ".mp4", ".mov", ".avi" };
        public static readonly string GifExtension = ".gif";

        public void AddMedia(string path) {
            string extension = Path.GetExtension(path);

            if (extension == GifExtension && !ContainsPath(path)) { 
                Media.Add(new GifContainer(path, Array.Empty<string>(), ContainerType.Gif));
                return;
            }

            foreach (string vidExtension in VideoExtensions) {
                if (extension != vidExtension || ContainsPath(path)) { continue; }
                Media.Add(new VideoContainer(path, Array.Empty<string>(), ContainerType.Video));
                return;
            }

            foreach (string imageExtension in ImageExtensions) {
                if (extension != imageExtension || ContainsPath(path)) { continue; }
                Media.Add(new ImageContainer(path, Array.Empty<string>(), ContainerType.Image));
                return;
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

        public void SetMosaic(List<List<Container>> Mosaic) => this.Mosaic = Mosaic;

        public void SortMedia() => Media = Media.OrderByDescending(item => item.Created).ToList(); 

        public AlbumDataPackage GetAlbumDataPackage() {
            return new AlbumDataPackage(Name, Media, Created, Updated, Mosaic);
        }

        private bool ContainsPath(string path) {
            foreach (Container container in Media) {
                if (container.FilePath == path) { return true; }
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
                } else if(package.Gifs[i] != null) {
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

        public List<List<Container>> Mosaic;
        public ImageContainer[] Images { get; set; }
        public VideoContainer[] Videos { get; set; }
        public GifContainer[] Gifs { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public bool IsEmpty { get; set; }

        public AlbumDataPackage(string Name, List<Container> Media, DateTime Created, DateTime Updated, List<List<Container>> Mosaic) {
            this.Name = Name;
            this.Created = Created;
            this.Updated = Updated;
            this.Mosaic = Mosaic;

            ImageContainer[] imageContainers = new ImageContainer[Media.Count];
            VideoContainer[] videoContainers = new VideoContainer[Media.Count];
            GifContainer[] gifContainers = new GifContainer[Media.Count];

            for (int i = 0; i < Media.Count; i++) {
                if (Media[i]  is ImageContainer image) {
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

            Mosaic = new();
            Images = Array.Empty<ImageContainer>();
            Videos = Array.Empty<VideoContainer>();
            Gifs = Array.Empty<GifContainer>();

            Created = default;
            Updated = default;

            this.IsEmpty = true;
        }
    }
}
