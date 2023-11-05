using System;
using System.Collections.Generic;
using System.IO;
using CS = VaultsII.CachingSystem;
using static VaultsII.MediaStorage.MetadataManager;

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
            storage = AlbumStorage.Instance;

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

        private void OnFileCreated(object sender, FileSystemEventArgs e) {
            
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e) {

        }

        private void OnFileRenamed(object sender, FileSystemEventArgs e) {

        }

        public MonitoredFolders() {
            paths = new List<string>(CS.CachingSystem.LoadMonitoredFolders());
            watchers = new();

            for (int i = 0; i < paths.Count; i++) {
                FileSystemWatcher @new = new(paths[i]);
                watchers.Add(@new);

                int index = watchers.IndexOf(@new);

                @new.Created += OnFileCreated;
                @new.Deleted += OnFileDeleted;
                @new.Renamed += OnFileRenamed;
            
                @new.EnableRaisingEvents = true;
            }

            storage = AlbumStorage.Instance;
            Instance = this;
        }
    }
}
