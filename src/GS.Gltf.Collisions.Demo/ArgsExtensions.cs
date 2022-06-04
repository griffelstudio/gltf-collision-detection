using System.Collections.Generic;
using System.Linq;

namespace GS.Gltf.Collisions.Demo
{
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
            for (int i = index + 1; i < args.Length; i++)
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
