using FFMpegCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VaultsII.MediaStorage {
    public static class MetadataManager {
        public static bool IsLoadingMetadata { get; private set; }

        public delegate void MetadataLoaded();
        public static event MetadataLoaded OnMetadataLoaded;

        public static async Task UpdateContainerMetadata(this List<Container> containers) {
            await Task.Run(() => {
                IsLoadingMetadata = true;

                foreach (Container container in containers) {
                    if (container.IsMetaDataLoaded) { continue; }

                    Uri path = new(container.FilePath, UriKind.RelativeOrAbsolute);
                    bool isVideo = container is VideoContainer;
                    object source = isVideo ? path : new BitmapImage(path);

                    if (isVideo) {
                        var info = FFProbe.Analyse(container.FilePath);
                        var stream = info.PrimaryVideoStream;

                        container.Width = stream != null ? stream.Width : -1;
                        container.Height = stream != null ? stream.Height : -1;
                    } else {
                        ImageSource imageSource = (ImageSource)source;
                        container.Width = imageSource != null ? imageSource.Width : -1;
                        container.Height = imageSource != null ? imageSource.Height : -1;
                    }

                    container.IsMetaDataLoaded = true;
                }

                IsLoadingMetadata = false;
                OnMetadataLoaded?.Invoke();
            });
        }
    }
}
