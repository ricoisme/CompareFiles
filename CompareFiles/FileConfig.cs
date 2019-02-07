using System;
using System.Collections.Generic;
using System.Text;

namespace CompareFiles
{
    public sealed class FileConfig
    {     
        public IEnumerable<FileProperties> Files { get; set; }
        public sealed class FileProperties
        {
            public string SourceFileName { get; set; }
            public string TargetFileName { get; set; }
        }
    }
}
