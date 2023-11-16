using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using VaultsII.MediaStorage;

namespace VaultsII.CachingSystem {
    public static class CachingSystem {
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string CachePath = Path.Combine(BaseDirectory, "cache");
        private static readonly string moniFoldsPath = Path.Combine(CachePath, "monitored_folders");

        public static void SaveMonitoredFolders (string[] files) { 
            if (!Directory.Exists(CachePath)) { Directory.CreateDirectory(CachePath); }

            try {
                string JSONData = JsonSerializer.Serialize(files);
                File.WriteAllText(moniFoldsPath, JSONData);
            } catch (Exception e) {
                // Will throw error to error modal
                Console.WriteLine(e.ToString());
            }
        }

        public static string[] LoadMonitoredFolders() {
            if (!Directory.Exists(CachePath)) {  return Array.Empty<string>(); } 

            try {
                string JSONData = File.ReadAllText(moniFoldsPath);
                return JsonSerializer.Deserialize<string[]>(JSONData) ?? Array.Empty<string>();
            } catch (Exception e) {
                // Will throw error to error modal
                Console.WriteLine(e.ToString());
                return Array.Empty<string>();
            }
        }

        public static void SaveAlbumDataPackage(AlbumDataPackage package) {
            if (!Directory.Exists(CachePath)) { Directory.CreateDirectory(CachePath); }

            string path = Path.Combine(CachePath, package.Name);

            try {
                string JSONData = JsonSerializer.Serialize(package);
                File.WriteAllText(path, JSONData);
            } catch (Exception e) {
                // Will throw error to error modal
                Console.WriteLine(e.ToString()); 
            }
        }

        public static AlbumDataPackage LoadAlbumDataPackage(string name) {
            if (!Directory.Exists(CachePath)) { return new AlbumDataPackage(); }

            string path = Path.Combine(CachePath, name);

            try {
                string JSONData = File.ReadAllText(path);
                return JsonSerializer.Deserialize<AlbumDataPackage>(JSONData);
            } catch (Exception e) {
                // Will throw error to error modal
                Console.WriteLine(e.ToString());
                return new AlbumDataPackage();
            }
        }

        public static AlbumDataPackage[] LoadAllAlbumDataPackages() {
            if (!Directory.Exists(CachePath)) { return Array.Empty<AlbumDataPackage>(); }

            string[] paths = Directory.GetFiles(CachePath);

            try {
                List<AlbumDataPackage> packages = new();

                foreach (string path in paths) { 
                    if (path == moniFoldsPath || !File.Exists(path)) { continue; }

                    string JSONData = File.ReadAllText(path);
                    packages.Add(JsonSerializer.Deserialize<AlbumDataPackage>(JSONData));
                }

                return packages.ToArray();
            } catch (Exception e) {
                // Will throw error to error modal
                Console.WriteLine(e.ToString());
                return Array.Empty<AlbumDataPackage>();
            }
        }

        public static void DeleteCacheFile(string name) {
            try {
                File.Delete(Path.Combine(CachePath, name));
            } catch (Exception e) {
                // Will throw error to error modal
                Console.WriteLine(e.ToString());
            }
        }
    }
}
