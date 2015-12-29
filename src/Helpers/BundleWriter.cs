using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SqlBundler.Models;

namespace SqlBundler.Helpers
{
    public static class BundleWriter
    {
        private static string Normalize(string contents)
        {
            return Regex.Replace(contents, @"\r\n|\n\r|\n|\r", "\r\n");
        }

        public static void WriteBundles(string root, IEnumerable<SQLBundle> bundles)
        {
            foreach (var bundle in bundles)
            {
                string filePath = Path.Combine(root, bundle.FileName);

                Console.WriteLine(@"Writing bundle {0}", filePath);
                File.WriteAllText(filePath, Normalize(bundle.Script), Encoding.UTF8);
            }
        }
    }
}