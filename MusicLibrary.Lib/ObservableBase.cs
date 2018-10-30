using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MusicLibrary.Lib
{
    public class ObservableBase : System.ComponentModel.INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Set<T>(ref T property, T value, params string[] propertyNames) {
            if (!EqualityComparer<T>.Default.Equals(property, value)) {
                property = value;
                RaisePropertyChanged(propertyNames);
            }
        }

        protected void RaisePropertyChanged(params string[] propertyNames) {
            if ((propertyNames != null) && (propertyNames.Length > 0)) {
                if (PropertyChanged != null) {
                    foreach (var propertyName in propertyNames) {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                    }
                }
            }
            else {
                throw new ApplicationException("RaisePropertyChanged requires at least one property name");
            }
        }
    }
}
