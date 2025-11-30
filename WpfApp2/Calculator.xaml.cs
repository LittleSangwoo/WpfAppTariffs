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
    /// Логика взаимодействия для Calculator.xaml
    /// </summary>
    public partial class Calculator : Page
    {
        private List<View_1> view = new List<View_1>();
        private View_1 selectedTariff;

        public Calculator()
        {
            InitializeComponent();
            LoadTariffs();
            SetupComboBox();
        }

        public void BackPage(object sender, RoutedEventArgs e)
        {
            AppConnect.frame.Navigate(new TariffsPage());
        }

        private void LoadTariffs()
        {
            try
            {
                using (var db = new cellServiceEntities())
                {
                    view = db.View_1
                        .OrderBy(t => t.idTariffs)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить представление: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupComboBox()
        {
            comboBoxTariffs.Text = "Выберите тариф";
            comboBoxTariffs.ItemsSource = view;
            comboBoxTariffs.DisplayMemberPath = nameof(View_1.name);
            comboBoxTariffs.SelectedIndex = -1;
        }

        private void comboBoxTariffs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedTariff = comboBoxTariffs.SelectedItem as View_1;

            if (selectedTariff == null)
            {
                ClearFields();
                return;
            }

            textBoxMinutes.Text = selectedTariff.minutes.ToString();
            textBoxSms.Text = selectedTariff.sms.ToString();
            textBoxGb.Text = selectedTariff.gb.ToString();

            textBoxAddMinutes.Text = "0";
            textBoxAddSms.Text = "0";
            textBoxAddGb.Text = "0";

            UpdateTotalPrice();
        }

        private void textBoxAddMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            if (selectedTariff == null)
            {
                finalPrice.Text = string.Empty;
                return;
            }

            decimal total = selectedTariff.price;
            total += CalculateAdditionalCost(textBoxAddMinutes.Text, selectedTariff.minutesPrice, 50);
            total += CalculateAdditionalCost(textBoxAddSms.Text, selectedTariff.smsPrice, 10);
            total += CalculateAdditionalCost(textBoxAddGb.Text, selectedTariff.gbPrice, 5);

            finalPrice.Text = total.ToString("F2");
        }

        private static decimal CalculateAdditionalCost(string input, int? packPrice, int packSize)
        {
            if (!packPrice.HasValue)
            {
                return 0;
            }

            if (!int.TryParse(input, out int packs) || packs <= 0)
            {
                return 0;
            }

            decimal singlePackPrice = packPrice.Value / (decimal)packSize;
            return packs * singlePackPrice;
        }

        private void ClearFields()
        {
            textBoxMinutes.Text = string.Empty;
            textBoxSms.Text = string.Empty;
            textBoxGb.Text = string.Empty;
            textBoxAddMinutes.Text = string.Empty;
            textBoxAddSms.Text = string.Empty;
            textBoxAddGb.Text = string.Empty;
            finalPrice.Text = string.Empty;
            comboBoxTariffs.Text = "Выберите тариф";
            comboBoxTariffs.SelectedValue = "Выберите тариф";
        }

        private void AddToComparison_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTariff == null)
            {
                MessageBox.Show("Сначала выберите тариф.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            selectedTariff.minutesPrice = int.Parse(textBoxAddMinutes.Text);
            selectedTariff.smsPrice = int.Parse(textBoxAddSms.Text);
            selectedTariff.gbPrice = int.Parse(textBoxAddGb.Text);
            selectedTariff.price = decimal.Parse(finalPrice.Text);

            App.selectedTariffs.Add(selectedTariff);
            // Добавляем тариф в глобальное хранилище для сравнения
            MessageBox.Show($"Тариф \"{selectedTariff.name}\" добавлен к сравнению.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            selectedTariff = null;
            ClearFields();
        }

        private void OpenComparison_Click(object sender, RoutedEventArgs e)
        {
            AppConnect.frame.Navigate(new Comapison());
        }
    }
}
