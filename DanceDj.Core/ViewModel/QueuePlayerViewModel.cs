using System;
using System.Collections.Generic;
using System.Text;

using MusicLibrary.Lib;
using DanceDj.Core;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DanceDj.ViewModel
{
    public class QueuePlayerViewModel : ViewModelBase
    {
        public QueuePlayerViewModel(QueuePlayer player) {
            InnerPlayer = player;
        }

        private QueuePlayer InnerPlayer { get; }

        public ITrack NowPlaying => InnerPlayer.NowPlaying;
        public PlayerState PlayerState => InnerPlayer.PlayerState;
        public PlayerTimes PlayerTimes => InnerPlayer.PlayerTimes;
        public double ConfiguredVolume { get => InnerPlayer.ConfiguredVolume; set => InnerPlayer.ConfiguredVolume = value; }
        public double EffectiveVolume => InnerPlayer.EffectiveVolume;
        public bool AutoCue { get => InnerPlayer.AutoCue; set => InnerPlayer.AutoCue = value; }

        protected RelayCommand _playCommand;
        protected RelayCommand<TrackViewModel> _playTrackCommand;
        protected RelayCommand _pauseCommand;
        protected RelayCommand _stopCommand;
        protected RelayCommand _fadeCommand;
        protected RelayCommand _goForwardCommand;
        protected RelayCommand _goBackCommand;

        public RelayCommand PlayCommand {
            get => _playCommand ??
                    (_playCommand = new RelayCommand(
                        () => InnerPlayer.Play(),
                        _CanPlay));
        }

        protected bool _CanPlay() {
            if (InnerPlayer.PlayerState == PlayerState.Playing) {
                return false;
            }
            return (InnerPlayer.NowPlaying != null) || (InnerPlayer.Queue.Count > 0);
        }

        public RelayCommand<TrackViewModel> PlayTrackCommand {
            get =>  _playTrackCommand ??
                    (_playTrackCommand = new RelayCommand<TrackViewModel>(
                        (t) => InnerPlayer.Play(t.Track),
                        (t) => (t != null)));
        }

        public RelayCommand PauseCommand {
            get => _pauseCommand ??
                    (_pauseCommand = new RelayCommand(
                        () => InnerPlayer.Pause(),
                        () => InnerPlayer.PlayerState == PlayerState.Playing));
        }

        public RelayCommand StopCommand {
            get => _stopCommand ??
                    (_stopCommand = new RelayCommand(
                        () => InnerPlayer.Stop(),
                        () => InnerPlayer.PlayerState != PlayerState.Stopped));
        }

        public RelayCommand FadeCommand {
            get => _fadeCommand ??
                    (_fadeCommand = new RelayCommand(
                        () => InnerPlayer.StartFade(),
                        () => ((InnerPlayer.PlayerState == PlayerState.Playing) || (InnerPlayer.PlayerState == PlayerState.Paused))));
        }

        public RelayCommand GoForwardCommand {
            get => _goForwardCommand ??
                    (_goForwardCommand = new RelayCommand(
                        () => InnerPlayer.GoForward(),
                        _CanGoForward
                        ));
        }
        protected bool _CanGoForward() {
            return (InnerPlayer.Queue.Count > 0) || (InnerPlayer.NowPlaying != null);
        }

        public RelayCommand GoBackCommand {
            get => _goBackCommand ??
                    (_goBackCommand = new RelayCommand(
                        () => InnerPlayer.GoBack(),
                        _CanGoBack
                        ));
        }
        protected bool _CanGoBack() {
            return (InnerPlayer.LastPlayed.Count > 0) || ((InnerPlayer.NowPlaying != null) && (InnerPlayer.PlayerTimes.ElapsedTimeInSeconds != 0));
        }
    }
}
