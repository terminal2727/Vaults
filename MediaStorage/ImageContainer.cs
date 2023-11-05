﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultsII.MediaStorage {
    public class ImageContainer : Container {
        public override string FilePath { get; set; }
        public override string[] Tags { get; set; }
        public override DateTime Created { get; set; }
        public override ContainerType Type { get; set; }
        public override double Width { get; set; }
        public override double Height { get; set; }
        public override bool IsMetaDataLoaded { get; set; }

        public ImageContainer(string FilePath, string[] Tags, ContainerType Type) { 
            this.FilePath = FilePath;
            this.Tags = Tags;
            this.Type = Type;

            Created = Directory.GetCreationTime(FilePath);
        }
    }
}
