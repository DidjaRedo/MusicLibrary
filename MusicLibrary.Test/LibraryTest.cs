using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MusicLibrary.Lib;
using Xunit;

namespace MusicLibrary.Test
{
    public class LibraryTest
    {
        [Fact]
        public void ShouldRecurseDirectories() {
            var library = new Library("./data/Music");
            Assert.Equal(2, library.Tracks.Count);
        }

        [Fact]
        public void ShouldExportJson() {
            string expected = File.ReadAllText("./data/json/test.json");
            var library = new Library("./data/Music");
            library.Root = ".\\data\\Music\\";
            var json = library.ExportToJson(true);
            Assert.Equal(expected, json);
        }

        [Fact]
        public void ShouldImportJson() {
            // var library = new Library("./data/json/library.json");
            var expected = File.ReadAllText("./data/json/library.json");
            var library = Newtonsoft.Json.JsonConvert.DeserializeObject<Library>(expected);
            var got = library.ExportToJson(true);
            Assert.Equal(expected, got);
        }
    }
}
