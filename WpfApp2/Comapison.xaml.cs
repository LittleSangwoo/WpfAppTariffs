using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для Comapison.xaml
    /// </summary>
    public partial class Comapison : Page
    {
        public List<View_1> view { get; set; } = new List<View_1>();
        public Comapison()
        {
            InitializeComponent();
            DataContext = this;
            LoadView();
            // Используем глобальное хранилище тарифов для сравнения
            // Это ObservableCollection, поэтому UI автоматически обновится при изменении
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            AppConnect.frame.Navigate(new Calculator());
        }

        private void OrderTariff_Click(object sender, RoutedEventArgs e)
        {
            // Получаем тариф из DataContext элемента
            if (sender is Button button)
            {
                // DataContext кнопки устанавливается автоматически через DataTemplate
                if (button.DataContext is View_1 tariff)
                {
                    MessageBox.Show($"Оформление тарифа \"{tariff.name}\".", "Заказ", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        public void LoadView()
        {
            if (App.selectedTariffs == null || App.selectedTariffs.Count == 0) { 
                MessageBox.Show($"Не удалось загрузить лист с тарифами", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return;
            }
            foreach(var item in App.selectedTariffs)
            {
                view.Add(item);
            }
        }
    }
}
