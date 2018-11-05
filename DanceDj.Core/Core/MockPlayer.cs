using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

using MusicLibrary.Lib;
using GalaSoft.MvvmLight;

namespace DanceDj.Core
{
    public class PlayerStoppingEventArgs : EventArgs
    {
        public PlayerStoppingEventArgs() : base() {

        }

        public bool Cancel { get; set; } = false;
        public ITrack NextTrack { get; set; } = null;
    }

    public delegate void PlayerStoppingHandler(object player, PlayerStoppingEventArgs e);

    public class MockPlayer : ViewModelBase
    {
        protected PlayerTimes _playerTimes;
        protected PlayerState _playerState = PlayerState.Stopped;
        protected double _configuredVolume = 0.5;
        protected int _fadeDuration = 10;
        protected ITrack _nowPlaying;
        protected object _timerLock = new object();
        protected Utils.ITimer _timer = new Utils.SystemTimer(1000) { AutoReset = true, Enabled = false };

        public PlayerState PlayerState { get => _playerState; protected set => Set("PlayerState", ref _playerState, value); }
        public event PlayerStoppingHandler PlayerStopping;

        public int FadeDuration { get => _fadeDuration; protected set => Set("FadeDuration", ref _fadeDuration, value); }
        public double ConfiguredVolume {
            get => _configuredVolume;
            set {
                _configuredVolume = ConfiguredVolume;
                RaisePropertyChanged("ConfiguredVolume");
                RaisePropertyChanged("EffectiveVolume");
            }
        }
        public double EffectiveVolume {
            get => _configuredVolume * (_playerTimes?.FaderVolume ?? 1.0);
        }
        public ITrack NowPlaying { get => _nowPlaying; protected set => Set("NowPlaying", ref _nowPlaying, value); }
        public Utils.ITimer Timer {
            get => _timer;
            set {
                lock (_timerLock) {
                    if (!object.ReferenceEquals(_timer, value)) {
                        if (_timer != null) {
                            if (_timer.Enabled) {
                                throw new ApplicationException("Player must be stopped before timer change.");
                            }
                            _timer.Elapsed -= Timer_Elapsed;
                        }
                        value.Elapsed += Timer_Elapsed;
                        _timer = value;
                        RaisePropertyChanged("Timer");
                    }
                }
            }
        }

        public ITrack Play(ITrack track) {
            NowPlaying = track;
            UpdatePlayerTimes((p) => new PlayerTimes(track, FadeDuration));
            StartTimer();
            return NowPlaying;
        }

        public ITrack Play() {
            switch (PlayerState) {
                case PlayerState.Playing:
                    break;
                case PlayerState.Error:
                case PlayerState.Paused:
                case PlayerState.Stopped:
                    StartTimer();
                    break;
            }
            return NowPlaying;
        }

        public void Seek(int position) {
            if ((NowPlaying != null) && (position >= 0) && (position < NowPlaying.DurationInSeconds)) {
                UpdatePlayerTimes((t) => t?.Seek(position));
            }
        }

        public void Stop() {
            Timer.Stop();
            UpdatePlayerTimes((t) => t?.Reset());
            PlayerState = PlayerState.Stopped;
        }

        public void Pause() {
            Timer.Stop();
            PlayerState = ((NowPlaying != null) && IsAtEnd ? PlayerState.Stopped : PlayerState.Paused);
        }

        public void StartFade() {
            if (PlayerTimes != null) {
                UpdatePlayerTimes((t) => t?.StartFade());
            }
        }

        public void GoToStart() {
            if (PlayerTimes != null) {
                UpdatePlayerTimes((t) => t?.Reset());
            }
        }

        protected bool RaisePlayerStopping(out ITrack nextTrack) {
            var e = new PlayerStoppingEventArgs();
            PlayerStopping?.Invoke(this, e);
            nextTrack = e.NextTrack;
            return !e.Cancel;
        }

        public PlayerTimes PlayerTimes { get => _playerTimes; }
        public bool IsAtStart { get => (PlayerTimes != null) && (PlayerTimes.ElapsedTimeInSeconds == 0);  }
        public bool IsAtEnd { get => (PlayerTimes != null) && (PlayerTimes.ElapsedTimeInSeconds == PlayerTimes.TotalTimeInSeconds);  }

        private object PlayerTimesLock = new object();
        private delegate PlayerTimes PlayerTimesUpdateDelegate(PlayerTimes current);
        private PlayerTimes UpdatePlayerTimes(PlayerTimesUpdateDelegate update) {
            lock (PlayerTimesLock) {
                bool start = IsAtStart;
                bool end = IsAtEnd;
                Set("PlayerTimes", ref _playerTimes, update(_playerTimes));
                if (start != IsAtStart) {
                    RaisePropertyChanged("IsAtStart");
                }
                if (end != IsAtEnd) {
                    RaisePropertyChanged("IsAtEnd");
                }
                return _playerTimes;
            }
        }

        protected void StartTimer() {
            if (NowPlaying != null) {
                PlayerState = PlayerState.Playing;
                Timer.Start();
            }
        }

        private void Timer_Elapsed(object sender, EventArgs e) {
            if (_playerTimes != null) {
                double previousFader = PlayerTimes?.FaderVolume ?? 1.0;
                var newTimes = UpdatePlayerTimes((t) => t?.Tick());
                if (newTimes != null) {
                    if (newTimes.FaderVolume != previousFader) {
                        RaisePropertyChanged("EffectiveVolume");
                    }
                    if (newTimes.RemainingTimeInSeconds < 1) {
                        ITrack nextTrack;
                        if (RaisePlayerStopping(out nextTrack)) {
                            Pause();
                        }
                        else if (nextTrack == null) {
                            nextTrack = NowPlaying;
                        }

                        if (nextTrack != null) {
                            NowPlaying = nextTrack;
                            UpdatePlayerTimes((t) => new PlayerTimes(nextTrack, FadeDuration));
                        }
                    }
                }
            }
        }
    }
}
