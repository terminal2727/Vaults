using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultsII.MediaStorage {
    public abstract class Container {
        public abstract string FilePath { get; set; } // Has to be FilePath to not obscure the Path class
        public abstract string[] Tags { get; set; }
        public abstract DateTime Created { get; set; }
        public abstract ContainerType Type { get; set; }
    }
    
    public enum ContainerType {
        Image, Video, Gif
    }
}
