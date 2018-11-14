using System;
using System.Collections.Generic;
using System.Text;

using DanceDj.Core;
using GalaSoft.MvvmLight;

namespace DanceDj.ViewModel
{
    public class QueuePlayerViewModel : ViewModelBase
    {
        public QueuePlayerViewModel(QueuePlayer player) {
            InnerPlayer = player;
        }

        private QueuePlayer InnerPlayer { get; }
        public QueuePlayerViewModel Player { get; }
    }
}
