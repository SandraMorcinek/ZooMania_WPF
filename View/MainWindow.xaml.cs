using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Microsoft.Data.Sqlite;
using ZooMania.View;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace ZooMania
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";
        public MainWindow()
        {
            InitializeComponent();
        }

        // Importuje funkcję SendMessage z biblioteki systemowej user32.dll
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        // Obsługuje zdarzenie kliknięcia lewym przyciskiem myszy na panelu pnlControlBar
        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Tworzy obiekt WindowInteropHelper, który umożliwia dostęp do uchwytu okna aplikacji
            WindowInteropHelper helper = new WindowInteropHelper(this);

            // Wysyła wiadomość do okna aplikacji za pomocą funkcji SendMessage
            // z parametrami hWnd (uchwyt okna aplikacji), wMsg (typ wiadomości), wParam (parametr wiadomości) i lParam (parametr wiadomości)
            // W tym przypadku, wiadomość o typie 161 (czyli WM_SYSCOMMAND) jest wysyłana z parametrami wParam=2 i lParam=0, co wskazuje na akcję "przesunięcia"
            SendMessage(helper.Handle, 161, 2, 0);
        }
 
        // Obsługuje zdarzenie najechania myszą na panel pnlControlBar
        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            // Ustawia maksymalną wysokość okna aplikacji (MaxHeight) na wysokość głównego ekranu (SystemParameters.MaximizedPrimaryScreenHeight)
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }
        // Zamknięcie aplikacji
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        // Zminimalizowanie okna
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        // Maksymalizuj okno
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }
        // Wyświetlenie aktualnej daty i godziny
        private void btnClock_Click(object sender, RoutedEventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            string message = "Bieżąca data i czas to: " + currentTime.ToString("dd-MM-yyyy HH:mm:ss");
            MessageBox.Show(message);
        }
        // Wyświetlenie MessageBox
        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Brak nowych wiadomości");
        }
        // Wyświetlenie MessageBox
        private void btnBell_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Brak nowych powiadomień");
        }
        // Wyświetlenie zapytania z możliwością wybory tak/nie
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz zamknąć program?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }


        
    }
}
