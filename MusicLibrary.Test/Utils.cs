using System;
using System.Collections.Generic;
using System.Text;


using System.IO;
using Ionic.Zip;

namespace MusicLibrary.Test
{
    internal static class Utils
    {
        private static bool Initialized { get; set; } = false;
        private static void Initialize() {
            if (!Initialized) {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Initialized = true;
            }
        }

        public static void EnsureTestData(string target) {
            var looseDestination = Path.Join(target, "data\\json");
            var expandedFolder = Path.Join(looseDestination, "Music");
            if (!Directory.Exists(expandedFolder)) {
                var zipSource = Path.Join(target, "data\\json\\Music.zip");
                if (!File.Exists(zipSource)) {
                    throw new ApplicationException($"Test data source {zipSource} not found");
                }

                Initialize();

                using (var zip = new ZipFile(zipSource)) {
                    zip.ExtractAll(looseDestination, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }
    }
}
