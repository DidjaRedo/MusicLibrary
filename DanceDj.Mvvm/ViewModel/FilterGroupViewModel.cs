using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

using MusicLibrary.Lib;
using GalaSoft.MvvmLight;

namespace DanceDj.Mvvm.ViewModel
{
    public class FilterGroupViewModel : ViewModelBase
    {
        public FilterGroupViewModel(FilterGroup group) {
            Group = group;

            var filterVMs = group.All.Select<FilteredTracks, FilteredTracksViewModel>((fi) => new FilteredTracksViewModel(fi));
            Filters = new ObservableCollection<FilteredTracksViewModel>(filterVMs);
        }

        protected FilterGroup Group { get; }

        public string Name => Group.Name;
        public ObservableCollection<FilteredTracksViewModel> Filters { get; }
    }
}
