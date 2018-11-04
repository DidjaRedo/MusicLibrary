using System;
using System.IO;

using MusicLibrary.Lib;
using Newtonsoft.Json;

namespace MusicLibrary.Cli
{
    class Program
    {
        static void Main(string[] args) {
            var cmd = (args.Length > 0) ? args[0].ToLowerInvariant() : null;
            switch (cmd) {
                case "import":
                    if ((args.Length < 2) || (args.Length > 3)) {
                        Help("Invalid arguments for import.");
                    }
                    else {
                        var from = args[1];
                        var to = (args.Length == 3) ? args[2] : "MusicLibrary.json";
                        Import(from, to);
                    }
                    break;
                default:
                    Help(String.IsNullOrEmpty(cmd) ? String.Empty : $"Unknown command '{cmd}'.");
                    break;
            }
        }

        static void Help(string message = null) {
            if (String.IsNullOrEmpty(message)) {
                Console.WriteLine($"{message}");
            }
            Console.WriteLine("Usage: ml <command> [<args>]");
            Console.WriteLine("Where commands are:");
            Console.WriteLine("    import <path> [<output>]");
        }

        static void Import(string fromPath, string toPath) {
            var from = new DirectoryInfo(fromPath);
            var to = new DirectoryInfo(toPath);
            if (!from.Exists) {
                throw new ApplicationException($"Source directory {from.FullName} does not exist.");
            }
            if (!to.Exists) {
                to.Create();
            }
            else if ((to.Attributes & FileAttributes.Directory) == 0) {
                throw new ApplicationException($"Destination {to.FullName} must be a directory.");
            }

            var dest = Path.Combine(to.FullName, "library.json");
            var library = new Library();
            library.OnTrackAdded += (t, p) => Console.WriteLine($"Added {t} (\"{p}\")");
            library.Root = from.FullName;
            library.Import(from.FullName);
            File.WriteAllText(dest, library.ExportToJson(true));

            foreach (var track in library.Tracks) {
                dest = Path.Combine(to.FullName, $"{track.Path}.json");
                var dir = new DirectoryInfo(Path.GetDirectoryName(dest));
                dir.Create();
                File.WriteAllText(dest, JsonConvert.SerializeObject(track, Formatting.Indented));
            }
        }
    }
}
