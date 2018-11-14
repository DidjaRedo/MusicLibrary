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
        public QueuePlayer(MockPlayer player) {
            Queue = new ReadOnlyObservableCollection<ITrack>(_queue);
            LastPlayed = new ReadOnlyObservableCollection<ITrack>(_lastPlayed);
            InnerPlayer = player;
            InnerPlayer.PropertyChanged += InnerPlayerPropertyChanged;
            InnerPlayer.PlaybackStopping += InnerPlayerPlaybackStopping;
        }

        public QueuePlayer() : this(new MockPlayer()) {
        }

        protected PlayerTimes _playerTimes;
        protected ObservableCollection<ITrack> _lastPlayed = new ObservableCollection<ITrack>();
        protected ObservableCollection<ITrack> _queue = new ObservableCollection<ITrack>();

        #region Player

        protected MockPlayer InnerPlayer { get; }
        protected bool _autoCue = true;

        private void InnerPlayerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case "NowPlaying":
                case "PlayerTimes":
                case "ConfiguredVolume":
                case "EffectiveVolume":
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

        public ITrack Play(ITrack track) {
            if (NowPlaying != null) {
                InnerPlayer.Pause();
                NowPlayingDone();
            }
            return InnerPlayer.Play(track);
        }

        public ITrack Play() {
            if (NowPlaying == null) {
                var next = PopNextTrack();
                return InnerPlayer.Play(next);
            }
            return InnerPlayer.Play();
        }

        public void Pause() {
            InnerPlayer.Pause();
        }

        public void Stop() {
            InnerPlayer.Stop();
        }

        public void StartFade() {
            InnerPlayer.StartFade();
        }

        public ITrack GoForward() {
            if (NowPlaying != null) {
                InnerPlayer.Pause();
                NowPlayingDone();
            }
            return InnerPlayer.Play(PopNextTrack());
        }

        public ITrack GoBack() {
            if (PlayerTimes.ElapsedTimeInSeconds != 0) {
                if (AutoCue) {
                    InnerPlayer.Pause();
                }
                InnerPlayer.Seek(0);
                return NowPlaying;
            }

            var last = PopLastTrack();
            _queue.Insert(0, NowPlaying);
            return InnerPlayer.Play(last);
        }

        private void InnerPlayerPlaybackStopping(object player, PlaybackStoppingEventArgs e) {
            NowPlayingDone();
            var next = PopNextTrack();
            if (next != null) {
                e.Cancel = !AutoCue;
                e.NextTrack = next;
            }
        }

        private void NowPlayingDone() {
            if (NowPlaying != null) {
                _lastPlayed.Insert(0, NowPlaying);
            }
        }

        private ITrack PopNextTrack() {
            if (_queue.Count > 0) {
                var next = _queue[0];
                _queue.RemoveAt(0);
                return next;
            }
            return null;
        }

        private ITrack PopLastTrack() {
            if (_lastPlayed.Count > 0) {
                var next = _lastPlayed[0];
                _lastPlayed.RemoveAt(0);
                return next;
            }
            return null;
        }

        #endregion

        #region Queue
        public ReadOnlyObservableCollection<ITrack> Queue { get; }
        public ReadOnlyObservableCollection<ITrack> LastPlayed { get; }

        public void Add(params ITrack[] tracks) {
            foreach (var track in tracks) {
                _queue.Add(track);
            }
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
