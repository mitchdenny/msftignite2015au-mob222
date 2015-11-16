using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            this.Readings = new ObservableCollection<Reading>();
        }

        public ObservableCollection<Reading> Readings { get; private set; }
    }
}
