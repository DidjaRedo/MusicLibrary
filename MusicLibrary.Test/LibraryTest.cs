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
            var library = new Library("./data/mp3/Music");
            Assert.Equal(2, library.Tracks.Count);
        }

        [Fact]
        public void ShouldExportJson() {
            string expected = File.ReadAllText("./data/json/test.json");
            var library = new Library("./data/mp3/Music");
            library.Root = ".\\data\\Music\\";
            var json = library.ExportToJson(true);
            Assert.Equal(expected, json);
        }

        [Fact]
        public void ShouldImportJsonFiles() {
            var target = new DirectoryInfo(".").FullName;
            Utils.EnsureTestData(target);
            var expected = Library.FromJsonFile("./data/json/library.json");
            var library = new Library("./data/json/Music");
            Assert.Equal(expected.Tracks.Count, library.Tracks.Count);
        }

        [Fact]
        public void ShouldRoundTripImportedJson() {
            var expected = File.ReadAllText("./data/json/library.json");
            var library = Library.FromJsonText(expected);
            var got = library.ExportToJson(true);
            Assert.Equal(expected, got);
        }
    }
}
