using System;
using System.Collections.Generic;
using System.IO;
using CS = VaultsII.CachingSystem;

namespace VaultsII.MediaStorage {
    public class MonitoredFolders {
        public static MonitoredFolders Instance {
            get { return instance ??= new MonitoredFolders(); }
            set { instance = value; }
        }
        private static MonitoredFolders instance;

        public AlbumStorage storage;
        private readonly List<string> paths = new();
        private readonly List<FileSystemWatcher> watchers = new();

        public EventHandler OnMonitoredFoldersUpdated;

        public void AddNewMonitoredFolderPath(string path) {
            if (paths.Contains(path)) { return; }

            paths.Add(path);

            string[] subdirectories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            foreach (string directory in subdirectories) { paths.Add(directory); }

            string[] filePaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (string filePath in filePaths) {
                storage.Everything.AddMedia(filePath);
            }

            AlbumStorage.SaveAlbumChanges(storage.Everything);
            storage.Everything.Media.UpdateContainerMetadata();
            CS.CachingSystem.SaveMonitoredFolders(paths.ToArray());

            OnMonitoredFoldersUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveMonitoredFolderPath(string path) {
            storage = AlbumStorage.Instance;

            if (!paths.Contains(path)) { return; }

            int index = paths.IndexOf(path);
            paths.Remove(path);

            FileSystemWatcher watcher = watchers[index];
            watcher.Dispose();
            watchers.RemoveAt(index);

            string[] subdirectories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            foreach (string directory in subdirectories) { paths.Remove(directory); }


            string[] filePaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

            foreach (string filePath in filePaths) {
                foreach (AlbumData album in storage.Albums) {
                    album.RemoveMedia(filePath);
                    AlbumStorage.SaveAlbumChanges(album);
                }
            }

            CS.CachingSystem.SaveMonitoredFolders(paths.ToArray());

            OnMonitoredFoldersUpdated?.Invoke(this, EventArgs.Empty);
        }
        
        public void UpdateMonitoredFolders() {
            
        }

        public string GetFormattedList() {
            string formatted = String.Empty;

            foreach (string path in paths) {
                string name = Path.GetFileNameWithoutExtension(path);
                formatted += $"{name} \n";
            }

            return formatted;
        }

        public string[] GetPathsArray() {
            return paths.ToArray();
        }

        public bool Contains(string filePath) {
            foreach (string path in paths) {
                if (String.Equals(path, filePath, StringComparison.OrdinalIgnoreCase)) { return true; }
            }

            return false;
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e) {
            
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e) {

        }

        private void OnFileRenamed(object sender, FileSystemEventArgs e) {

        }

        public MonitoredFolders() {
            // We have to set the instance first, or else the album storage could cause an infinite loop
            // where the monitored folders instance asks for the storage instance which causes the storage
            // instance to request the monitored folders instance. But, since it'd still be null, it'd create
            // a new one, where inside its constructor, it'd attempt to call the storage instance, which is still
            // null, and so on until the program crashes.
            Instance = this;

            paths = new List<string>(CS.CachingSystem.LoadMonitoredFolders());
            watchers = new();

            storage = AlbumStorage.Instance;

            for (int i = 0; i < paths.Count; i++) {
                if (!Directory.Exists(paths[i])) {
                    paths.RemoveAt(i);
                    continue;
                }

                #region Creating File Watchers
                FileSystemWatcher @new = new(paths[i]);
                watchers.Add(@new);

                int index = watchers.IndexOf(@new);

                @new.Created += OnFileCreated;
                @new.Deleted += OnFileDeleted;
                @new.Renamed += OnFileRenamed;
            
                @new.EnableRaisingEvents = true;
                #endregion

                #region Updating Albums
                string[] subdirectories = Directory.GetDirectories(paths[i], "*", SearchOption.AllDirectories);
                foreach (string directory in subdirectories) { if (!paths.Contains(directory)) { paths.Add(directory); } }

                string[] filePaths = Directory.GetFiles(paths[i], "*", SearchOption.AllDirectories);
                foreach (string filePath in filePaths) { 
                    storage.Everything.AddMedia(filePath); }
                #endregion
            }
        }
    }
}
