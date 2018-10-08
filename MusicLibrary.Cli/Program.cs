using System;

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

        static void Import(string from, string to) {
            Console.WriteLine($"Should import from {from} to {to}");
        }
    }
}
