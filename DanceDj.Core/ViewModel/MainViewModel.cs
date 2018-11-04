using GalaSoft.MvvmLight;
using DanceDj.Model;

namespace DanceDj.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// The <see cref="Source" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _source = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Source {
            get {
                return _source;
            }
            set {
                Set(ref _source, value);
            }
        }

        private bool _found = false;
        public bool Found {
            get { return _found; }
            set { Set(ref _found, value); }
        }

        private LibraryViewModel _library = null;
        public LibraryViewModel Library {
            get { return _library; }
            set { Set(ref _library, value); }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService) {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) => {
                    if (error != null) {
                        // Report error here
                        return;
                    }

                    Source = item.Source;
                    Found = (item.Library != null);
                    if (Found) {
                        Library = new LibraryViewModel(item.Library);
                    }
                });
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}