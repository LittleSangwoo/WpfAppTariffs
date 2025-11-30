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
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userObj = AppConnect.model0db.clients.FirstOrDefault(x => x.login == textBlockLogin.Text && x.password == textBlockPassword.Text);
                if( userObj == null)
                {
                    MessageBox.Show("Такого пользователя не существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Здравстуйте " + userObj.name + "!", "Успешная авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                    Contract();
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Ошибка " + ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void Contract()
        {
            txtBlockAu.Visibility = Visibility.Hidden;
            txtBlockPas.Visibility = Visibility.Hidden;
            txtBlockLog.Visibility = Visibility.Hidden;
            btnLogIn.Visibility = Visibility.Hidden;
            btnReg.Visibility = Visibility.Hidden;
            textBlockLogin.Visibility = Visibility.Hidden;
            textBlockPassword.Visibility = Visibility.Hidden;

            txtBlockPhone.Visibility = Visibility.Visible;
            txtBoxPhone.Visibility = Visibility.Visible;
            btnContract.Visibility = Visibility.Visible;


        }

        private void btnContract_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var phoneObj = AppConnect.model0db.clientPhones.FirstOrDefault(x => x.phone == txtBoxPhone.Text);
                if (phoneObj == null)
                {
                    MessageBox.Show("Такого номера телефона не существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    string contractPath = ContractGenerator.GenerateContract(txtBoxPhone.Text);

                    MessageBox.Show($"Договор успешно создан!\nПуть: {contractPath}",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка " + ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
