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

    public class Player : ViewModelBase
    {
        public Player(LibraryFilter library) {
            Library = library;
            Queue = new ReadOnlyObservableCollection<ITrack>(_queue);
            LastPlayed = new ReadOnlyObservableCollection<ITrack>(_lastPlayed);
        }

        protected PlayerTimes _playerTimes;
        protected ObservableCollection<ITrack> _lastPlayed = new ObservableCollection<ITrack>();
        protected ObservableCollection<ITrack> _queue = new ObservableCollection<ITrack>();

        public LibraryFilter Library { get; }

#if notyet
#region Player

        protected PlayerState _playerState = PlayerState.Stopped;;
        protected int _fadeDuration = 10;
        protected bool _autoCue;
        protected ITrack _nowPlaying;

        public PlayerState PlayerState { get => _playerState; set => Set("PlayerState", ref _playerState, value); }
        public int FadeDuration { get => _fadeDuration; protected set => Set("FadeDuration", ref _fadeDuration, value); }
        public bool AutoCue { get => _autoCue; protected set => Set("AutoCue", ref _autoCue, value); }
        public ITrack NowPlaying { get => _nowPlaying; set => Set("NowPlaying", ref _nowPlaying, value); }

        public void Play() {
            if (NowPlaying == null) {
                if (_queue.Count > 0) {
                    NowPlaying = _queue[0];
                    _queue.RemoveAt(0);
                    PlayerTimes = new PlayerTimes(NowPlaying, FadeDuration);
                }
            }

            switch (PlayerState) {
                case PlayerState.Playing:
                    break;
                case PlayerState.Error:
                case PlayerState.Paused:
                case PlayerState.Stopped:
                    StartMockPlayer();
                    break;
            }
        }

        public void Stop() {
            StopMockPlayer();
        }

        public void Pause() {
            PauseMockPlayer();
        }

        public void StartFade() {
            if (PlayerTimes != null) {
                PlayerTimes = PlayerTimes.StartFade();
            }
        }

        public ITrack SkipForward() {
            if (NowPlaying != null) {
                _lastPlayed.Insert(0, NowPlaying);
            }
            if (_queue.Count > 0) {
                _nowPlaying = _queue[0];
                _queue.RemoveAt(0);
                PlayerTimes = new PlayerTimes(NowPlaying.DurationInSeconds, FadeDuration);
            }
            else {
                NowPlaying = null;
            }
            return NowPlaying;
        }

        public void SkipBack() {
            if (NowPlaying != null) {
                if (PlayerTimes.ElapsedTimeInSeconds > 0) {
                    PlayerTimes = PlayerTimes.Reset();
                }
                else if (_lastPlayed.Count > 0) {
                    _queue.Insert(0, NowPlaying);
                    _nowPlaying = _lastPlayed[0];
                    _lastPlayed.RemoveAt(0);
                    PlayerTimes = new PlayerTimes(NowPlaying.DurationInSeconds, FadeDuration);
                }
            }
        }

        private object PlayerTimesLock = new object();

        public PlayerTimes PlayerTimes { get => _playerTimes; protected set => Set("PlayerTimes", ref _playerTimes, value); }

        protected Timer Timer { get; set; }

        protected void StartMockPlayer() {
            if (Timer == null) {
                Timer = new Timer(1000) {
                    AutoReset = true,
                };
                Timer.Elapsed += Timer_Elapsed;
            }
            if (NowPlaying != null) {
                PlayerState = PlayerState.Playing;
                Timer.Start();
            }
        }

        protected void PauseMockPlayer() {
            if (Timer != null) {
                Timer.Stop();
                PlayerState = (NowPlaying != null) ? PlayerState.Paused : PlayerState.Stopped;
            }
        }

        protected void StopMockPlayer() {
            if (Timer != null) {
                Timer.Stop();
                PlayerTimes = PlayerTimes?.Reset();
                PlayerState = (NowPlaying != null) ? PlayerState.Paused : PlayerState.Stopped;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            if (_playerTimes != null) {
                PlayerTimes = _playerTimes.Tick();
                if (PlayerTimes.RemainingTimeInSeconds < 1) {
                    OnEndOfTrack();
                }
            }
        }

        private void OnEndOfTrack() {
            if (AutoCue) {
                Stop();
            }
            SkipForward();
        }

#endregion
#endif

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
