using System;
using System.Collections.Generic;
using System.Text;

using GalaSoft.MvvmLight;
using MusicLibrary.Lib;

namespace DanceDj.Mvvm.ViewModel
{
    public class FilterViewModel : ViewModelBase
    {
        public FilterViewModel(TrackDanceFilter filter) {
            Filter = filter;
        }

        internal TrackDanceFilter Filter { get; }

        public string Name => Filter.Name;
        public DanceReviewStatus? ReviewStatus => Filter.ReviewStatus;
        public DanceDifficulty? Difficulty => Filter.Difficulty;
    }
}
