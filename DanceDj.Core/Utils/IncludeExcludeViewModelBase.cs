using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DanceDj.Mvvm.Utils
{
    public delegate void ItemAddedDelegate<T>(T item);
    public delegate void ItemRemovedDelegate<T>(T item);

    public interface IIncludeExcludeViewModelBase<T> : System.ComponentModel.INotifyPropertyChanged
    {
        ReadOnlyObservableCollection<T> Included { get; }
        T SelectedIncluded { get; set; }

        RelayCommand<T> RemoveItemCommand { get; }
        bool RemoveEnabled { get; }

        ReadOnlyObservableCollection<T> Excluded { get; }
        T SelectedExcluded { get; set; }

        RelayCommand<T> AddItemCommand { get; }
        bool AddEnabled { get; }
    }

    public abstract class IncludeExcludeViewModelBase<T> : ViewModelBase, IIncludeExcludeViewModelBase<T> {
        protected IncludeExcludeViewModelBase(IEnumerable<T> allowedItems, IEnumerable<T> included = null) {
            AllowedItems = allowedItems;
            UpdateIncluded(included);
        }

        protected virtual bool IsValid(T value) {
            return !ReferenceEquals(value, null);
        }

        protected virtual T DefaultValue {
            get => default(T);
        }

        protected void UpdateAllowed(IEnumerable<T> allowed) {
            if ((allowed != null) && allowed.SequenceEqual(AllowedItems)) {
                // no change
                return;
            }
            AllowedItems = allowed;
            RaisePropertyChanged("AllowedItems");
            UpdateIncluded(Included);
        }

        protected void UpdateIncluded(IEnumerable<T> update) {
            var included = new ObservableCollection<T>();
            var excluded = new ObservableCollection<T>();

            foreach (var item in AllowedItems) {
                if ((update != null) && update.Contains(item)) {
                    included.Add(item);
                }
                else {
                    excluded.Add(item);
                }
            }

            if ((_included == null) || (!_included.SequenceEqual(included))) {
                var couldAdd = AddEnabled;
                var couldRemove = RemoveEnabled;

                _included = included;
                _excluded = excluded;
                Included = new ReadOnlyObservableCollection<T>(_included);
                Excluded = new ReadOnlyObservableCollection<T>(_excluded);
                RaisePropertyChanged("Included");
                RaisePropertyChanged("Excluded");

                if ((!IsValid(SelectedIncluded)) || (!_included.Contains(SelectedIncluded))) {
                    SelectedIncluded = (_included.Count > 0 ? _included[0] : DefaultValue);
                    RemoveItemCommand.RaiseCanExecuteChanged();
                }

                if ((!IsValid(SelectedExcluded)) || (!_excluded.Contains(SelectedExcluded))) {
                    SelectedExcluded = (_excluded.Count > 0 ? _excluded[0] : DefaultValue);
                    AddItemCommand.RaiseCanExecuteChanged();
                }

                if (AddEnabled != couldAdd) {
                    RaisePropertyChanged("AddEnabled");
                }
                if (RemoveEnabled != couldRemove) {
                    RaisePropertyChanged("RemoveEnabled");
                }
            }
        }

        public IEnumerable<T> AllowedItems { get; protected set; }
 
        #region Included Elements

        protected ObservableCollection<T> _included;
        protected T _selectedIncluded;

        public ReadOnlyObservableCollection<T> Included { get; protected set; }
        public T SelectedIncluded { get => _selectedIncluded; set => Set("SelectedIncluded", ref _selectedIncluded, value); }

        protected RelayCommand<T> _removeItemCommand;
        public RelayCommand<T> RemoveItemCommand {
            get => _removeItemCommand ?? (_removeItemCommand = new RelayCommand<T>(_RemoveItem, _CanRemoveItem));
        }

        protected abstract void InnerRemove(T item);

        protected void _RemoveItem(T item) {
            item = (IsValid(item) ? item : SelectedIncluded);
            if (!_CanRemoveItem(item)) {
                throw new ApplicationException($"Cannot remove {item}.");
            }
            InnerRemove(item);
        }

        protected bool _CanRemoveItem(T item) {
            item = (IsValid(item) ? item : SelectedIncluded);
            // disabled because can execute is not being requeried for some reason:w
            //return (Included.Count > 0) && IsValid(item) && Included.Contains(item);
            return true;
        }

        public bool RemoveEnabled => (Included != null) && (Included.Count > 0) && IsValid(SelectedIncluded) && Included.Contains(SelectedIncluded);

        #endregion

        #region Excluded Items

        protected ObservableCollection<T> _excluded;
        protected T _selectedExcluded;
        public ReadOnlyObservableCollection<T> Excluded { get; protected set; }
        public T SelectedExcluded { get => _selectedExcluded; set => Set("SelectedExcluded", ref _selectedExcluded, value); }

        protected RelayCommand<T> _addItemCommand;
        public RelayCommand<T> AddItemCommand {
            get => _addItemCommand ?? (_addItemCommand = new RelayCommand<T>(_AddItem, _CanAddItem));
        }

        protected abstract void InnerAdd(T item);

        protected void _AddItem(T item) {
            item = (IsValid(item) ? item : SelectedExcluded);
            if (!_CanAddItem(item)) {
                throw new ApplicationException($"Cannot add {item}.");
            }
            InnerAdd(item);
        }

        protected bool _CanAddItem(T item) {
            item = (IsValid(item) ? item : SelectedExcluded);
            // disabled because CanExecute is not being requeried for some reason
            // return (Excluded.Count > 0) && IsValid(item) && Excluded.Contains(item);
            return true;
        }

        public bool AddEnabled => (Excluded != null) && (Excluded.Count > 0) && IsValid(SelectedExcluded) && Excluded.Contains(SelectedExcluded);

        #endregion
    }
}
