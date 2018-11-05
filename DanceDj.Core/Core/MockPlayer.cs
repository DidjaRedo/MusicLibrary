using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

using MusicLibrary.Lib;
using GalaSoft.MvvmLight;

namespace DanceDj.Core
{
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

        public void Play(ITrack track) {
            NowPlaying = track;
            UpdatePlayerTimes((p) => new PlayerTimes(track, FadeDuration));
            StartMockPlayer();
        }

        public void Play() {
            switch (PlayerState) {
                case PlayerState.Playing:
                    break;
                case PlayerState.Error:
                case PlayerState.Paused:
                case PlayerState.Stopped:
                    if (NowPlaying != null) {
                        StartMockPlayer();
                    }
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
                UpdatePlayerTimes((t) => t?.StartFade());
            }
        }

        public void GoToStart() {
            if (PlayerTimes != null) {
                UpdatePlayerTimes((t) => t?.Reset());
            }
        }

        public PlayerTimes PlayerTimes { get => _playerTimes; }

        private object PlayerTimesLock = new object();
        private delegate PlayerTimes PlayerTimesUpdateDelegate(PlayerTimes current);
        private PlayerTimes UpdatePlayerTimes(PlayerTimesUpdateDelegate update) {
            lock (PlayerTimesLock) {
                Set("PlayerTimes", ref _playerTimes, update(_playerTimes));
                return _playerTimes;
            }
        }

        protected void StartMockPlayer() {
            if (NowPlaying != null) {
                PlayerState = PlayerState.Playing;
                Timer.Start();
            }
        }

        protected void PauseMockPlayer() {
            Timer.Stop();
            PlayerState = ((NowPlaying != null) && (PlayerTimes.ElapsedTimeInSeconds < PlayerTimes.TotalTimeInSeconds) ? PlayerState.Paused : PlayerState.Stopped);
        }

        protected void StopMockPlayer() {
            Timer.Stop();
            UpdatePlayerTimes((t) =>t?.Reset());
            PlayerState = (NowPlaying != null) ? PlayerState.Paused : PlayerState.Stopped;
        }

        private void Timer_Elapsed(object sender, EventArgs e) {
            if (_playerTimes != null) {
                double previousFader = PlayerTimes?.FaderVolume ?? 1.0;
                var newTimes = UpdatePlayerTimes((t) => t?.Tick());
                if (newTimes != null) {
                    if (newTimes.RemainingTimeInSeconds < 1) {
                        PauseMockPlayer();
                    }
                    if (newTimes.FaderVolume != previousFader) {
                        RaisePropertyChanged("EffectiveVolume");
                    }
                }
            }
        }
    }
}
