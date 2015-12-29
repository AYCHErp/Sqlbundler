using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using SqlBundler.Models;
using static System.String;

namespace SqlBundler.Helpers
{
    public static class Parser
    {
        public static BundlerModel Parse(string root, string path, bool sampleDataOnly)
        {
            if (IsNullOrWhiteSpace(path))
            {
                return null;
            }

            var model = new BundlerModel();
            var dictionaries = new Collection<KeyValuePair<string, string>>();

            string content = File.ReadAllText(path, Encoding.UTF8);
            string scriptDirectory = Empty;

            var lines = content.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                var items = line.Split(':');
                string key = items[0].TrimStart('-').Trim();
                string value = items[1].Trim();

                if (!IsNullOrWhiteSpace(GetScriptDirectory(key, value)))
                {
                    scriptDirectory = GetScriptDirectory(key, value);
                    scriptDirectory = Path.Combine(root, scriptDirectory);

                    if (!Directory.Exists(scriptDirectory))
                    {
                        scriptDirectory = Empty;
                    }
                }

                var dictionary = GetDictionary(key, value);
                string outputDirectory = GetOutputDirectory(key, value);

                if (!IsNullOrWhiteSpace(dictionary.Key) && !IsNullOrWhiteSpace(dictionary.Value))
                {
                    dictionaries.Add(dictionary);
                }

                if (!IsNullOrWhiteSpace(outputDirectory))
                {
                    model.OutputDirectory = outputDirectory;
                }
            }

            model.OriginalFileName = Path.GetFileNameWithoutExtension(path).Replace(".sqlbundle", "");
            model.Files = GetScripts(scriptDirectory, sampleDataOnly);
            model.Dictionaries = dictionaries;

            return model;
        }

        private static KeyValuePair<string, string> GetDictionary(string key, string value)
        {
            if (!key.Equals("dictionary"))
            {
                return new KeyValuePair<string, string>();
            }

            value = value.TrimStart('[').TrimEnd(']');
            return new KeyValuePair<string, string>(value.Split(',')[0].Trim(), value.Split(',')[1].Trim());
        }

        private static IEnumerable<string> GetFiles(string path, bool sampleDataOnly)
        {
            var queue = new Queue<string>();
            queue.Enqueue(path);

            while (queue.Count > 0)
            {
                path = queue.Dequeue();

                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                // ReSharper disable once CatchAllClause
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path, "*.sql");
                }
                // ReSharper disable once CatchAllClause
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

                if (files == null) continue;

                foreach (string file in files)
                {
                    if (sampleDataOnly)
                    {
                        if (file.ToLower().EndsWith(".sample.sql"))
                        {
                            yield return file;
                        }
                    }
                    else
                    {
                        if (!file.ToLower().EndsWith(".sample.sql"))
                        {
                            yield return file;
                        }
                    }
                }
            }
        }

        private static string GetOutputDirectory(string key, string value)
        {
            return key.Equals("output-directory") ? value : Empty;
        }

        private static string GetScriptDirectory(string key, string value)
        {
            return key.Equals("script-directory") ? value : Empty;
        }

        private static Collection<string> GetScripts(string directory, bool includeSample)
        {
            return new Collection<string>(GetFiles(directory, includeSample).OrderBy(s => s).ToList());
        }
    }
}