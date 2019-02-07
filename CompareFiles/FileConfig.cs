using System;
using System.Collections.Generic;
using System.Text;

namespace CompareFiles
{
    public sealed class FileConfig
    {     
        public IEnumerable<FileProbs> SourceFiles { get; set; }
        public IEnumerable<FileProbs> TargetFiles { get; set; }
        public sealed class FileProbs
        {
            public string FileName { get; set; }
        }
    }
}
