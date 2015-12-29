using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SqlBundler.Models
{
    public class BundlerModel
    {
        public string OriginalFileName { get; set; }
        public Collection<KeyValuePair<string, string>> Dictionaries { get; set; }
        public Collection<string> Files { get; set; }
        public string OutputDirectory { get; set; }
    }
}