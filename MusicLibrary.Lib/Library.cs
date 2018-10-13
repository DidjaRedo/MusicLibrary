using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

namespace MusicLibrary.Lib
{
    public class Library
    {
        public delegate void TrackAddedHandler(ITrack track, string path);
 
        public Library(IEnumerable<ITrack> tracks, string root = null) {
            Root = root;
            _tracks = tracks.ToList();
        }

        public Library() : this(new Track[0]) {
        }

        public Library(string path) : this() {
            var file = new FileInfo(path);
            if ((file.Attributes & FileAttributes.Directory) != 0) {
                Root = file.FullName;
            }
            Import(path);
        }

        [JsonConstructor]
        public Library(string root, Track[] tracks) : this(tracks, root) {

        }

        public void Import(string path) {
            if (Directory.Exists(path)) {
                ImportDirectory(new DirectoryInfo(path));
            }
            else if (File.Exists(path)) {
                TryImportFile(new FileInfo(path));
            }
            else {
                throw new ApplicationException($"Cannot import '{path}' - no such file or directory.");
            }
        }

        public void ImportDirectory(DirectoryInfo dir) {
            if (!dir.Exists) {
                throw new ApplicationException($"Cannot import '{dir.FullName}' - directory not found.");
            }

            foreach (var subdir in dir.GetDirectories()) {
                ImportDirectory(subdir);
            }

            foreach (var file in dir.GetFiles()) {
                TryImportFile(file);
            }
        }

        public bool TryImportFile(FileInfo file) {
            bool imported = false;
            if (!file.Exists) {
                throw new ApplicationException($"Cannot import '{file.FullName}' - file not found.");
            }

            switch (file.Extension) {
                case ".json":
                    throw new NotImplementedException($"Cannot import {file.FullName} - JSON import not implemented yet.");
                    break;
                case ".mp3":
                case ".wma":
                case ".aiff":
                case ".flac":
                case ".wav":
                    var track = TrackFile.TryCreate(file.FullName, Root);
                    if (track != null) {
                        _tracks.Add(track);
                        RaiseTrackAdded(track, file.FullName);
                        imported = true;
                    }
                    break;
            }
            return imported;
        }

        public string ExportToJson(bool pretty = false) {
            var formatting = (pretty ? Formatting.Indented : Formatting.None);
            return JsonConvert.SerializeObject(this, formatting);
        }

        public string Root { get; set; }
        public IReadOnlyList<ITrack> Tracks => _tracks.AsReadOnly();
        protected List<ITrack> _tracks;

        public event TrackAddedHandler OnTrackAdded;

        protected void RaiseTrackAdded(ITrack track, string path) {
            OnTrackAdded?.Invoke(track, path);
        }
    }
}
