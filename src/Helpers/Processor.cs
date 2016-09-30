using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using SqlBundler.Models;

namespace SqlBundler.Helpers
{
    public static class Processor
    {
        const string SOURCE_CANDIDATE = "src";

        public static Collection<SQLBundle> Process(string root, BundlerModel model)
        {
            if (model == null)
            {
                return null;
            }

            var script = new StringBuilder();
            var bundles = new Collection<SQLBundle>();

            foreach (string fileName in model.Files)
            {
                if (!string.IsNullOrWhiteSpace(script.ToString()))
                {
                    script.Append(Environment.NewLine);
                }

                var directories = Path.GetFullPath(root).Split(Path.DirectorySeparatorChar);
                var pos = directories.TakeWhile(x => x.ToLower() != SOURCE_CANDIDATE).Count();
                var start = string.Join(Path.DirectorySeparatorChar.ToString(), directories.Skip(pos));
                                
                Console.WriteLine(@"Adding {0} to bundle", fileName);
                script.Append("-->-->-- ");
                script.Append((start + Path.GetFullPath(fileName).Replace(Path.GetFullPath(root), "")).Replace("\\", "/"));
                script.Append(" --<--<--");

                script.Append(Environment.NewLine);

                script.Append(File.ReadAllText(fileName, Encoding.UTF8));
                script.Append(Environment.NewLine);
            }

            if (string.IsNullOrWhiteSpace(script.ToString()))
            {
                return null;
            }

            var defaultBundle = new SQLBundle
            {
                FileName = GetBundleFileName(model.OutputDirectory, model.OriginalFileName),
                Script = script.ToString()
            };

            bundles.Add(defaultBundle);

            foreach (var dictionary in model.Dictionaries)
            {
                Console.WriteLine(@"SQL localization dictionary: {0}", dictionary.Value);

                var bundle = new SQLBundle();
                string filePath = Path.Combine(root, dictionary.Value);

                bundle.FileName = GetBundleFileName(model.OutputDirectory, model.OriginalFileName);
                bundle.Script = script.ToString();

                var lines = File.ReadAllText(filePath, Encoding.UTF8)
                    .Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                foreach (string line in lines)
                {
                    string find = line.Split('=')[0].Trim();
                    string replace = line.Split('=')[1].Trim();

                    bundle.Script = bundle.Script.Replace(find, replace);
                }

                bundles.Add(bundle);
            }

            return bundles;
        }

        private static string GetBundleFileName(string outputDirectory, string fileName)
        {
            return outputDirectory + "/" + fileName + ".sql";
        }
    }
}