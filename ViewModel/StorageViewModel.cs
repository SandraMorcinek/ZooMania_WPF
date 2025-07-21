using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZooMania.ViewModel
{
    public class StorageViewModel: ViewModelBase, INotifyPropertyChanged
    {
        private int _quantity;

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged("Quantity");
                }
            }
        }

        public ICommand IncrementCommand { get; private set; }
        public ICommand DecrementCommand { get; private set; }

        public StorageViewModel()
        {
            IncrementCommand = new RelayCommand(IncrementQuantity);
            DecrementCommand = new RelayCommand(DecrementQuantity);
            Quantity = 0;
        }

        private void IncrementQuantity()
        {
            Quantity++;
        }

        private void DecrementQuantity()
        {
            if (Quantity > 0)
            {
                Quantity--;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
