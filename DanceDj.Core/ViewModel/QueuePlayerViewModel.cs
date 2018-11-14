using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;

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
            UpdateQueue();
            UpdateLastPlayed();
            InnerPlayer.PropertyChanged += InnerPlayerPropertyChanged;
        }

        private void InnerPlayerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case "Queue":
                    UpdateQueue();
                    break;
                case "LastPlayed":
                    UpdateLastPlayed();
                    break;
                default:
                    RaisePropertyChanged(e.PropertyName);
                    break;
            }
            
        }

        private void UpdateQueue() {
            _queue = new ObservableCollection<TrackViewModel>(((IEnumerable<ITrack>)InnerPlayer.Queue).ToViewModel());
            Queue = new ReadOnlyObservableCollection<TrackViewModel>(_queue);
            RaisePropertyChanged("Queue");

            if ((_selectedTrack == null) || (!_queue.Contains(_selectedTrack))) {
                SelectedTrack = ((_queue.Count > 0) ? _queue[0] : null);
            }
        }

        private void UpdateLastPlayed() {
            _lastPlayed = new ObservableCollection<TrackViewModel>(((IEnumerable<ITrack>)InnerPlayer.LastPlayed).ToViewModel());
            LastPlayed = new ReadOnlyObservableCollection<TrackViewModel>(_lastPlayed);
            RaisePropertyChanged("LastPlayed");
        }

        private QueuePlayer InnerPlayer { get; }
        private ObservableCollection<TrackViewModel> _queue;
        private ObservableCollection<TrackViewModel> _lastPlayed;

        public ITrack NowPlaying => InnerPlayer.NowPlaying;
        public ReadOnlyObservableCollection<TrackViewModel> Queue { get; protected set; }
        public ReadOnlyObservableCollection<TrackViewModel> LastPlayed { get; protected set; }

        private TrackViewModel _selectedTrack;
        public TrackViewModel SelectedTrack { get => _selectedTrack; set => Set("SelectedTrack", ref _selectedTrack, value); }

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
                        () => PlayEnabled));
        }

        public bool PlayEnabled {
            get {
                if (InnerPlayer.PlayerState == PlayerState.Playing) {
                    return false;
                }
                return (InnerPlayer.NowPlaying != null) || (InnerPlayer.Queue.Count > 0);
            }
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
                        () => PauseEnabled));
        }
        public bool PauseEnabled => (InnerPlayer.PlayerState == PlayerState.Playing);

        public RelayCommand StopCommand {
            get => _stopCommand ??
                    (_stopCommand = new RelayCommand(
                        () => InnerPlayer.Stop(),
                        () => StopEnabled));
        }
        public bool StopEnabled => (InnerPlayer.PlayerState != PlayerState.Stopped);

        public RelayCommand FadeCommand {
            get => _fadeCommand ??
                    (_fadeCommand = new RelayCommand(
                        () => InnerPlayer.StartFade(),
                        () => FadeEnabled));
        }
        public bool FadeEnabled => ((InnerPlayer.PlayerState == PlayerState.Playing) || (InnerPlayer.PlayerState == PlayerState.Paused));

        public RelayCommand GoForwardCommand {
            get => _goForwardCommand ??
                    (_goForwardCommand = new RelayCommand(
                        () => InnerPlayer.GoForward(),
                        () => GoForwardEnabled
                        ));
        }
        public bool GoForwardEnabled => (InnerPlayer.Queue.Count > 0) || (InnerPlayer.NowPlaying != null);

        public RelayCommand GoBackCommand {
            get => _goBackCommand ??
                    (_goBackCommand = new RelayCommand(
                        () => InnerPlayer.GoBack(),
                        () => GoBackEnabled
                        ));
        }
        public bool GoBackEnabled => (InnerPlayer.LastPlayed.Count > 0) || ((InnerPlayer.NowPlaying != null) && (InnerPlayer.PlayerTimes.ElapsedTimeInSeconds != 0));
    }
}
