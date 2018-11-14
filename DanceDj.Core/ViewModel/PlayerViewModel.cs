using System;
using System.Collections.Generic;
using System.Text;

using DanceDj.Core;

using GalaSoft.MvvmLight;

namespace DanceDj.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        public PlayerViewModel(MockPlayer player) {
            InnerPlayer = player;
            InnerPlayer.PropertyChanged += InnerPlayerPropertyChanged;
        }

        private void InnerPlayerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            var player = (MockPlayer)sender;
            if (e.PropertyName == "NowPlaying") {
                _nowPlaying = TrackViewModel.GetOrAdd(player.NowPlaying);
            }
            RaisePropertyChanged(e.PropertyName);
        }

        private MockPlayer InnerPlayer { get; }

        public PlayerState PlayerState => InnerPlayer.PlayerState;
        public int FadeDuration => InnerPlayer.FadeDuration;
        public double ConfiguredVolume {
            get => InnerPlayer.ConfiguredVolume;
            set => InnerPlayer.ConfiguredVolume = value;
        }

        public double EffectiveVolume => InnerPlayer.EffectiveVolume;

        private TrackViewModel _nowPlaying;
        public TrackViewModel NowPlaying => _nowPlaying;
    }
}
