using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MusicLibrary.Lib;
using Xunit;

namespace MusicLibrary.Test
{
    public class TestLibrary
    {
        [Fact]
        public void ShouldRecurseDirectories() {
            var library = new Library("./data/Music");
            Assert.Equal(2, library.Tracks.Count);
        }

        [Fact]
        public void ShouldExportJson() {
            string expected = "{\"Root\":\".\\\\data\\\\Music\\\\\",\"Tracks\":[{\"TrackNumber\":3,\"Title\":\"Business Of Love\",\"ArtistNames\":[\"Domino\"],\"AlbumTitle\":\"PAPC Foxtrot 1\"," +
                              "\"AlbumArtistNames\":[\"Various Artists\"],\"BeatsPerMinute\":119," +
                              "\"Comments\":{},\"Genres\":[\"Ballroom\",\"Foxtrot\"],\"Grouping\":\"Foxtrot\"," +
                              "\"Path\":\"Various Artists\\\\PAPC Foxtrot 1\\\\03 - Domino - Business Of Love.mp3\"," +
                              "\"Rating\":{\"Rater\":\"no@email\",\"rating\":196,\"PlayCount\":0},\"Ratings\":[{\"Rater\":\"no@email\",\"rating\":196,\"PlayCount\":0}]," +
                              "\"MediaMonkey\":{\"Occasion\":null,\"Custom1\":null,\"Custom2\":null,\"Custom3\":null,\"Custom4\":null,\"Custom5\":null}}," +
                              "{\"TrackNumber\":13,\"Title\":\"A Wink & A Smile\",\"ArtistNames\":[\"Premium Standard\"],\"AlbumTitle\":\"Quest - Foxtrot Z\"," +
                              "\"AlbumArtistNames\":[\"Various Artists\"],\"BeatsPerMinute\":0," +
                              "\"Comments\":{\"Songs-DB_Occasion\":\"Seasonal\",\"MusicMatch_Situation\":\"Seasonal\",\"Songs-DB_Custom1\":\"NoAuto\",\"Songs-DB_Custom2\":\"FOX\",\"Songs-DB_Custom3\":\"Quest\"," +
                              "\"Songs-DB_Custom4\":\"Social\",\"Songs-DB_Custom5\":\"Any\"},\"Genres\":[\"Ballroom\",\"Standard\",\"Foxtrot\"],\"Grouping\":\"Foxtrot\"," +
                              "\"Path\":\"Various Artists\\\\Quest - Foxtrot Z\\\\13 - Premium Standard - A Wink & A Smile.mp3\"," +
                              "\"Rating\":{\"Rater\":\"no@email\",\"rating\":186,\"PlayCount\":0}," +
                              "\"Ratings\":[{\"Rater\":\"no@email\",\"rating\":186,\"PlayCount\":0}]," +
                              "\"MediaMonkey\":{\"Occasion\":\"Seasonal\",\"Custom1\":\"NoAuto\",\"Custom2\":\"FOX\",\"Custom3\":\"Quest\",\"Custom4\":\"Social\",\"Custom5\":\"Any\"}}]}";
            var library = new Library("./data/Music");
            library.Root = ".\\data\\Music\\";
            var json = library.ExportToJson();
            Assert.Equal(expected, json);
        }
    }
}
