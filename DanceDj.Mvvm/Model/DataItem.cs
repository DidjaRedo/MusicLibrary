﻿using System;
using MusicLibrary.Lib;

namespace DanceDj.Mvvm.Model
{
    public class DataItem
    {
        private string _source = string.Empty;
        public string Source {
            get { return _source; }
            set {
                if (!string.IsNullOrEmpty(value)) {
                    _source = value;
                    var fi = new System.IO.FileInfo(value);
                    if (fi.Exists) {
                        var library = new LibraryFilter(MusicLibrary.Lib.Library.FromJsonFile(fi.FullName));
                        library?.AddFilter(new TrackDanceFilter(Dances.Waltz) { Name = "Waltz" });
                        library?.AddFilter(new TrackDanceFilter(Dances.Tango) { Name = "Tango", });
                        library?.AddFilter(new TrackDanceFilter(Dances.Foxtrot) { Name = "Foxtrot", });
                        library?.AddFilter(new TrackDanceFilter(Dances.Quickstep) { Name = "Quickstep", });
                        library?.AddFilter(new TrackDanceFilter(Dances.VienneseWaltz) { Name = "Viennese", });
                        Library = library;
                        _source = fi.FullName;
                    }
                    else {
                        throw new ApplicationException($"Library source \"{value}\" not found.");
                    }
                }
                else {
                    _source = string.Empty;
                    Library = null;
                }
            }
        }

        public LibraryFilter Library { get; private set; } = null;

        public DataItem(string source) {
            Source = source;
        }
    }
}