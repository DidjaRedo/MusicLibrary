using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Timers;
using System.Linq;

using MusicLibrary.Lib;
using GalaSoft.MvvmLight;

namespace DanceDj.Core
{
    public enum PlayerState
    {
        Playing,
        Paused,
        Stopped,
        Error
    };

    public class QueuePlayer : ViewModelBase {
        public QueuePlayer(LibraryFilter library) {
            Library = library;
            Queue = new ReadOnlyObservableCollection<ITrack>(_queue);
            LastPlayed = new ReadOnlyObservableCollection<ITrack>(_lastPlayed);
            InnerPlayer = new MockPlayer();
            InnerPlayer.PropertyChanged += InnerPlayerPropertyChanged;
        }

        protected PlayerTimes _playerTimes;
        protected ObservableCollection<ITrack> _lastPlayed = new ObservableCollection<ITrack>();
        protected ObservableCollection<ITrack> _queue = new ObservableCollection<ITrack>();

        public LibraryFilter Library { get; }

        #region Player

        protected MockPlayer InnerPlayer { get; }
        protected bool _autoCue = true;

        private void InnerPlayerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case "NowPlaying":
                case "PlayerTimes":
                case "ConfiguredVolume":
                case "EffectiveVolume":
                    RaisePropertyChanged(e.PropertyName);
                    break;
                case "PlayerState":
                    RaisePropertyChanged(e.PropertyName);
                    break;
            }
        }

        public ITrack NowPlaying => InnerPlayer.NowPlaying;
        public PlayerState PlayerState => InnerPlayer.PlayerState;
        public PlayerTimes PlayerTimes => InnerPlayer.PlayerTimes;
        public double ConfiguredVolume { get => InnerPlayer.ConfiguredVolume; set => InnerPlayer.ConfiguredVolume = value; }
        public double EffectiveVolume => InnerPlayer.EffectiveVolume;
        public bool AutoCue { get => _autoCue; set => Set("AutoCue", ref _autoCue, value); }

        #endregion

        #region Queue
        public ReadOnlyObservableCollection<ITrack> Queue { get; }
        public ReadOnlyObservableCollection<ITrack> LastPlayed { get; }

        public void Add(ITrack track) {
            _queue.Add(track);
        }

        public bool Remove(ITrack track, int? index) {
            if (index.HasValue) {
                if (_queue[index.Value] == track) {
                    _queue.RemoveAt(index.Value);
                    return true;
                }
                return false;
            }
            return _queue.Remove(track);
        }

        public bool Remove(ITrack track) => Remove(track, null);

        public void MoveUp(ITrack track, int? wantIndex) {
            var index = (wantIndex.HasValue ? wantIndex.Value : _queue.IndexOf(track));
            if (index > 0) {
                if (_queue[index] == track) {
                    _queue.Move(index, index - 1);
                }
                else {
                    throw new ApplicationException($"MoveUp expected track {track} at position {index} but found {_queue[index]}.");
                }
            }
        }

        public void MoveUp(ITrack track) => MoveUp(track, null);

        public void MoveDown(ITrack track, int? wantIndex) {
            var index = (wantIndex.HasValue ? wantIndex.Value : _queue.IndexOf(track));
            if ((index > 0) && (index < (_queue.Count - 1))) {
                if (_queue[index] == track) {
                    _queue.Move(index, index + 1);
                }
                else {
                    throw new ApplicationException($"MoveDown expected track {track} at position {index} but found {_queue[index]}.");
                }
            }
        }

        public void MoveDown(ITrack track) => MoveDown(track, null);
#endregion

    }
}
