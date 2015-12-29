using System.Collections.ObjectModel;
using System.Linq;
using SqlBundler.Helpers;

namespace SqlBundler
{
    public static class Bundler
    {
        public static void Bundle(string root, Collection<string> files, bool sampleDataOnly)
        {
            foreach (var model in files.Select(file => Parser.Parse(root, file, sampleDataOnly)))
            {
                if (model == null)
                {
                    return;
                }

                var bundles = Processor.Process(root, model);

                if (bundles == null)
                {
                    return;
                }

                BundleWriter.WriteBundles(root, bundles);
            }
        }
    }
}