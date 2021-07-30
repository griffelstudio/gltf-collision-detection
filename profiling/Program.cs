using GS.Gltf.Collision;
using GS.Gltf.Collision.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging.Console;
using NLog;
using NDesk.Options;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            
            using ILoggerFactory loggerFactory =
                LoggerFactory.Create(builder =>
                    builder.AddConsole(options =>
                    {
                        options.IncludeScopes = false;
                    }));


            DemoLogger logger = new DemoLogger();
            var paths = new List<string>();

            int filesEndPointer = 0;
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] == "-m")
                {
                    filesEndPointer = i;
                    break;
                }
                paths.Add(args[i]);
            }
            string mode = args[filesEndPointer + 1];
            string outDir = args[filesEndPointer + 3];
            string outFile = args[filesEndPointer + 5];
            string inModelCheck = args[filesEndPointer + 7];

            if (args.Length == 0)
            {
                while (true)
                {
                    Console.WriteLine("add model path");
                    var path = Console.ReadLine();
                    if (path == "")
                    {
                        break;
                    }
                    paths.Add(path);
                }

                mode = Console.ReadLine();
                
                Console.WriteLine("Check collision into model?");
                Console.WriteLine("0 - NO");
                Console.WriteLine("1 - Yes");
                inModelCheck = "false";
                if (Console.ReadLine() == "1")
                {
                    inModelCheck = "true";
                }
                Console.WriteLine("Enter Output directory");
                outDir = Console.ReadLine();
                Console.WriteLine("Enter Output filename");
                outFile = Console.ReadLine();
            }

            CollisionHighlighting highlightning = CollisionHighlighting.None;
            switch (mode)
            {
                case "2":
                    highlightning = CollisionHighlighting.SeparateFile;
                    break;
                case "3":
                    highlightning = CollisionHighlighting.MergeAll;
                    break;
                case "mergeall":
                    highlightning = CollisionHighlighting.MergeAll;
                    break;
                case "separate":
                    highlightning = CollisionHighlighting.SeparateFile;
                    break;
            }



            var settings = new CollisionSettings(paths)
            {
                Delta = 0.01f,
                HiglightCollisions = highlightning,
                OutputSavePath = outDir,
                OutputFilename = outFile,
                InModelDetection = bool.Parse(inModelCheck),
            };



            
            var detector = new CollisionDetector(settings, logger);
            var detectResult = detector.Detect();


        }
    }
}
