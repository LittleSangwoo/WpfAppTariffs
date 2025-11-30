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

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для TariffsPage.xaml
    /// </summary>
    public partial class TariffsPage : Page
    {
        public List<View_1> view { get; set; } = new List<View_1>();
        public TariffsPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadView();
        }
        private void OpenCalculator(object sender, RoutedEventArgs e)
        {
            WindowHeight = 450;
            WindowWidth = 350;
            AppConnect.frame.Navigate(new Calculator());
        }
        public void LoadView()
        {
            view.Clear();
            try
            {
                using (var db = new cellServiceEntities())
                {
                    foreach (var ads in db.View_1.OrderBy(t => t.idTariffs).ToList())
                    {
                        view.Add(ads);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить представление: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
