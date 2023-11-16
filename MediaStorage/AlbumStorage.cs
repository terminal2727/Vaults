using System;
using System.Collections.Generic;
using CS = VaultsII.CachingSystem;

namespace VaultsII.MediaStorage {
    public class AlbumStorage {
        public static AlbumStorage Instance {
            get { return instance ??= new AlbumStorage(); }
            set { instance = value; }
        }
        private static AlbumStorage instance;
        public List<AlbumData> Albums { get; private set; }

        public AlbumData Current { get; private set; }
        public AlbumData Everything { get; private set; }

        public event EventHandler OnAlbumsListChange;
        public event EventHandler OnCurrentAlbumChange;

        public static void SaveAlbumChanges(AlbumData albumData) {
            CS.CachingSystem.SaveAlbumDataPackage(AlbumDataExtensions.GetAlbumDataPackage(albumData));
        }

        public void SetCurrentAlbum(string name) {
            bool a = TryFindAlbum(name, out AlbumData _);
            if (TryFindAlbum(name, out AlbumData album)) { 
                Current = album;
                Current.RemoveInvalidFiles(MonitoredFolders.Instance);
            }

            OnCurrentAlbumChange?.Invoke(this, EventArgs.Empty);
        }

        public void SetCurrentToEverything() {
            Current = Everything;
            OnCurrentAlbumChange?.Invoke(this, EventArgs.Empty );
        }

        public void AddNewAlbum(string name) {
            string newName = name;
            int @try = 0;

            while (TryFindAlbum(newName, out AlbumData _)) {
                newName = name + @try;

                if (@try >= 100) { return; } // Shouldn't ever really be reached, but just so it doesn't continue on for forever.
            }

            AlbumData newAlbum = new(name);
            Albums.Add(newAlbum);

            CS.CachingSystem.SaveAlbumDataPackage(AlbumDataExtensions.GetAlbumDataPackage(newAlbum));

            OnAlbumsListChange?.Invoke(this, EventArgs.Empty);
        }

        public void DeleteAlbum(string name) {
            if (TryFindAlbum(name, out AlbumData album)) { Albums.Remove(album); }

            CS.CachingSystem.DeleteCacheFile(name);

            OnAlbumsListChange?.Invoke(this, EventArgs.Empty);
        }

        public bool AlbumExists(string name) {
            return TryFindAlbum(name, out _);
        }

        private bool TryFindAlbum(string name, out AlbumData album) {
            album = null;

            if (name == Everything.Name) {
                album = Everything;
                return true; 
            }

            foreach (AlbumData data in Albums) {
                if (data.Name != name) { continue; }
                album = data;
                return true;
            }

            return false;
        }

        public AlbumStorage() {
            Instance = this;

            Albums = new();
            Everything = new(nameof(Everything));

            foreach (AlbumDataPackage package in CS.CachingSystem.LoadAllAlbumDataPackages()) {
                if (package.IsEmpty) { continue; }

                if (package.Name == Everything.Name) { 
                    Everything = new AlbumData(package);
                    continue;
                }

                Albums.Add(new AlbumData(package));
            }

            Current = Everything;

            Current.RemoveInvalidFiles(MonitoredFolders.Instance);
        }
    }
}
