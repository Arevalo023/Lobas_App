using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobasAppOrdersNew.Helpers
{
    public class BaseViewModel : ObservableObject
    {
        private bool _isBusy;
        private string _title = string.Empty;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}