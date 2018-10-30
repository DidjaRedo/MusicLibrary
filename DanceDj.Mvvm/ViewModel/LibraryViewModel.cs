using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using GalaSoft.MvvmLight;
using MusicLibrary.Lib;

namespace DanceDj.Mvvm.ViewModel
{
    public class LibraryViewModel : ViewModelBase
    {
        private LibraryFilter Library { get; }
        public LibraryViewModel(LibraryFilter library) {
            Library = library;
            var filterVMs = library.Filters.Values.Select<LibraryFilter.FilterInfo, FilteredTracksViewModel>((fi) => new FilteredTracksViewModel(fi));
            Default = new FilteredTracksViewModel(library.Default);
            Filters = new ObservableCollection<FilteredTracksViewModel>(filterVMs);
        }

        public FilteredTracksViewModel Default { get; }
        public ObservableCollection<FilteredTracksViewModel> Filters { get; }
    }
}
