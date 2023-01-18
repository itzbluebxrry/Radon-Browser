using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Radon.ViewModels
{
    public class BrowserTabViewModel : INotifyPropertyChanged
    {
        public BrowserTabViewModel()
        {

        }

        bool isLoadingBar;
        public bool IsLoadingBar
        {
            get => isLoadingBar;
            set
            {
                isLoadingBar = value;
                OnPropertyChanged(nameof(IsLoadingBar));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string isLoadingBar)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(isLoadingBar));
    }
}
