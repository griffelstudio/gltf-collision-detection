using GS.Gltf.Collision;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new DemoLogger();

            Console.WriteLine("Available args:" +
                "\n -i input files;" +
                "\n -o output file name;" +
                "\n -inmodel whether to check collisions in the model itself;" +
                "\n -merge whether to merge all models into one file;");

            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var input = args.GetManyValues("-i");
            var paths = new List<string>();
            for (int i = 0; i < input.Length; i++)
            {
                if (!Path.IsPathRooted(input[i]))
                {
                    input[i] = Path.Combine(currentDirectory, input[i]);
                }

                if (File.Exists(input[i]))
                {
                    paths.Add(input[i]);
                }
            }

            if (!paths.Any())
            {
                return;
            }

            var inputDirectory = Path.GetDirectoryName(paths[0]);

            var output = args.GetValue("-o") ?? Path.Combine(currentDirectory, "result.glb");
            if (!Path.IsPathRooted(output))
            {
                output = Path.Combine(inputDirectory, output);
            }

            var settings = new CollisionSettings(paths)
            {
                Delta = 0.01f,
                OutputMode = args.HasParameter("-merge")
                    ? OutputMode.MergeAll
                    : OutputMode.SeparateFile,
                OutputSavePath = Path.GetDirectoryName(output),
                OutputFilename = Path.GetFileName(output),
                InModelDetection = args.HasParameter("-inmodel"),
            };

            var detector = new CollisionDetector(settings, logger);
            var detectResult = detector.Detect();
        }

    }

    internal static class ArgsExtensions
    {
        public static bool HasParameter(this string[] args, string key)
        {
            var index = args.ToList().IndexOf(key);
            return index >= 0;
        }

        public static string GetValue(this string[] args, string key)
        {
            var index = args.ToList().IndexOf(key);
            if (index < 0)
            {
                // Parameter wasn't specified.
                return default;
            }

            if (index + 1 > args.Length
                || args[index + 1].StartsWith('-'))
            {
                // Parameter has no values.
                return default;
            }

            return args[index + 1];
        }

        public static string[] GetManyValues(this string[] args, string key)
        {
            var index = args.ToList().IndexOf(key);
            if (index < 0)
            {
                // Parameter wasn't specified.
                return default;
            }

            if (index + 1 > args.Length
                || args[index + 1].StartsWith('-'))
            {
                // Parameter has no values.
                return default;
            }

            var res = new List<string>();
            for(int i = index + 1; i < args.Length; i++)
            {
                if (args[i].StartsWith('-'))
                {
                    break;
                }

                res.Add(args[i]);
            }

            return res.ToArray();
        }
    }
}
