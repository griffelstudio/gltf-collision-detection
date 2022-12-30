using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GS.Gltf.Collisions.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new DemoLogger();

            if (!args.Any())
            {
                Console.WriteLine("Available args:" +
                    "\n -i input files;" +
                    "\n -o output file name;" +
                    "\n -inmodel whether to check collisions in the model itself;" +
                    "\n -merge whether to merge all models into one file;");
                return;
            }

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

            var output = args.GetValue("-o") ?? Path.Combine(inputDirectory, "result.glb");
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
            _ = detector.Detect();
        }
    }
}
