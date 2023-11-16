namespace VaultsII.MediaStorage {
    internal static class AlbumDataExtensions {

        public static AlbumDataPackage GetAlbumDataPackage(this AlbumData data) {
            return new AlbumDataPackage(data.Name, data.Media, data.Created, data.Updated);
        }
    }
}