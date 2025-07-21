using FontAwesome.Sharp;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZooMania.Model;
using ZooMania.Model.Repositories;

namespace ZooMania.ViewModel
{
    public class MainViewModel: ViewModelBase
    {
        // pola
        private UserAccountModel _currentUserAccount;
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;

        private IUserRepository userRepository;

        // właściwości
        public UserAccountModel CurrentUserAccount
        {
            get
            {
                return _currentUserAccount;
            }
            set
            {
                _currentUserAccount = value;
                OnPropertyChanged(nameof(CurrentUserAccount));
            }
        }

        public ViewModelBase CurrentChildView
        {
            get
            {
                 return _currentChildView;
            }
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));

                OnPropertyChanged(nameof(ProfileImage));
            }
        }

        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public IconChar Icon 
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        // polecenia

        public ICommand ShowHomeViewCommand { get; }
        public ICommand ShowCustomerViewCommand { get; }
        public ICommand ShowOrderViewCommand { get; }
        public ICommand ShowProductViewCommand { get; }
        public ICommand ShowCategoryViewCommand { get; }
        public ICommand ShowStorageViewCommand { get; }
        public ICommand ShowExecutionoftheorderViewCommand { get; }
        public ICommand ShowFinanceViewCommand { get; }
        public ICommand ShowRaportViewCommand { get; }
        public ICommand ShowSettingViewCommand { get; }

        public MainViewModel()
        {
            userRepository = new UserRepository();
            CurrentUserAccount = new UserAccountModel();

            // Zainicjalizowanie polecen

            ShowHomeViewCommand = new ViewModelCommand(ExecuteShowHomeViewCommand);
            ShowCustomerViewCommand = new ViewModelCommand(ExecuteShowCustomerViewCommand);
            ShowOrderViewCommand = new ViewModelCommand(ExecuteShowOrderViewCommand);
            ShowProductViewCommand = new ViewModelCommand(ExecuteShowProductViewCommand);
            ShowCategoryViewCommand = new ViewModelCommand(ExecuteShowCategoryViewCommand);
            ShowStorageViewCommand = new ViewModelCommand(ExecuteShowStorageViewCommand);
            ShowExecutionoftheorderViewCommand = new ViewModelCommand(ExecuteShowExecutionoftheorderViewCommand);
            ShowFinanceViewCommand = new ViewModelCommand(ExecuteShowFinanceViewCommand);
            ShowRaportViewCommand = new ViewModelCommand(ExecuteShowRaportViewCommand);
            ShowSettingViewCommand = new ViewModelCommand(ExecuteShowSettingViewCommand);

            // Widok standardowy
            ExecuteShowHomeViewCommand(null);

            LoadCurrentUserData();
        }

        private void ExecuteShowHomeViewCommand(object obj)
        {
            CurrentChildView = new HomeViewModel();
            Caption = "Strona główna";
            Icon = IconChar.Home;
        }

        private void ExecuteShowCustomerViewCommand(object obj)
        {
            CurrentChildView = new CustomerViewModel();
            Caption = "Klienci";
            Icon = IconChar.UserGroup;
        }

        private void ExecuteShowOrderViewCommand(object obj)
        {
            CurrentChildView = new OrderViewModel();
            Caption = "Zamówienia";
            Icon = IconChar.Truck;
        }

        private void ExecuteShowProductViewCommand(object obj)
        {
            CurrentChildView = new ProductViewModel();
            Caption = "Produkty";
            Icon = IconChar.ShoppingBag;
        }
        private void ExecuteShowCategoryViewCommand(object obj)
        {
            CurrentChildView = new CategoryViewModel();
            Caption = "Kategorie";
            Icon = IconChar.Paw;
        }
        private void ExecuteShowStorageViewCommand(object obj)
        {
            CurrentChildView = new StorageViewModel();
            Caption = "Magazyn";
            Icon = IconChar.Box;
        }
        private void ExecuteShowExecutionoftheorderViewCommand(object obj)
        {
            CurrentChildView = new ExecutionOfTheOrderViewModel();
            Caption = "Realizacja";
            Icon = IconChar.ClipboardCheck;
        }
        private void ExecuteShowFinanceViewCommand(object obj)
        {
            CurrentChildView = new FinanceViewModel();
            Caption = "Finanse";
            Icon = IconChar.Wallet;
        }
        private void ExecuteShowRaportViewCommand(object obj)
        {
            CurrentChildView = new RaportViewModel();
            Caption = "Raport";
            Icon = IconChar.PieChart;
        }
        private void ExecuteShowSettingViewCommand(object obj)
        {
            CurrentChildView = new SettingViewModel();
            Caption = "Ustawienia";
            Icon = IconChar.Tools;
        }

        private void LoadCurrentUserData()
        {
            var user = userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            if (user != null)
            {
                CurrentUserAccount.Username = user.Username;
                CurrentUserAccount.DisplayName = $"{user.Name} {user.LastName}";
                CurrentUserAccount.ProfilePicture = null;
            }
            else
            {
                CurrentUserAccount.DisplayName = "Użytkownik niezalogowany";
                // ukrycie child views.
            }
        }

        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";

        private ImageSource _profileImage;

        public ImageSource ProfileImage
        {
            get
            {
                if (CurrentUserAccount != null)
                {
                    // Pobieranie nazwy użytkownika aktualnie zalogowanego użytkownika
                    string currentUsername = CurrentUserAccount.Username;

                    // Sprawdzenie, czy nazwa użytkownika nie jest pusta
                    if (!string.IsNullOrEmpty(currentUsername))
                    {
                        // Pobieranie obrazka z bazy i zwracanie go jako ImageSource
                        using (var connection = new SqliteConnection(LokalizacjaBazy))
                        {
                            connection.Open();

                            using (var cmd = new SqliteCommand("SELECT ProfilePicture FROM [User] WHERE Username=@username", connection))
                            {
                                cmd.Parameters.AddWithValue("@username", currentUsername);
                                using (var reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        if (!reader.IsDBNull(0))
                                        {
                                            byte[] imageData = (byte[])reader["ProfilePicture"];
                                            using (var stream = new MemoryStream(imageData))
                                            {
                                                BitmapImage image = new BitmapImage();
                                                image.BeginInit();
                                                image.CacheOption = BitmapCacheOption.OnLoad;
                                                image.StreamSource = stream;
                                                image.EndInit();
                                                return image; // Zwracanie obrazka jako ImageSource
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // Zwracanie domyślnego obrazka, jeśli nazwa użytkownika jest pusta lub nie ma obrazka w bazie
                return new BitmapImage(new Uri("user.png", UriKind.Relative));
            }
            set
            {
                _profileImage = value;
                OnPropertyChanged(nameof(ProfileImage)); // Powiadomienie o zmianie wartości właściwości
            }
        }



   }
}
