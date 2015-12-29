using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SqlBundler
{
    internal class Program
    {
        private static string _bundlePath = string.Empty;
        private static string _root = string.Empty;

        public static void SetBundleDirectory(string path)
        {
            if (Directory.Exists(Path.Combine(_root, path)))
            {
                _bundlePath = Path.Combine(_root, path);
            }
        }

        public static void SetRootDirectory(string dir)
        {
            if (Directory.Exists(dir))
            {
                _root = dir;
            }
        }

        private static void Main(string[] args)
        {
            bool sample = false;

            if (args[0] == null || args[1] == null)
            {
                return;
            }

            SetRootDirectory(args[0]);

            if (string.IsNullOrWhiteSpace(_root))
            {
                return;
            }

            SetBundleDirectory(args[1]);

            if (string.IsNullOrWhiteSpace(_bundlePath))
            {
                return;
            }

            if (args.Length > 2)
            {
                if (args[2] != null)
                {
                    sample = args[2].ToUpperInvariant().Equals("TRUE");
                }
            }


            Console.WriteLine(@"---------MixERP SqlBundler---------");

            var files = new Collection<string>();

            foreach (var file in Directory.GetFiles(_bundlePath).Where(file => file != null).Where(file => Path.GetExtension(file).Equals(".sqlbundle")))
            {
                files.Add(file);
            }

            if (files.Count > 0)
            {
                Bundler.Bundle(_root, files, sample);
            }

            Console.WriteLine(@"---------MixERP SqlBundler---------");
        }
    }
}