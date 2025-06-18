using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ComputerConfiguratorService.Model;
using ComputerConfiguratorService.Utilities;

namespace ComputerConfiguratorService.View
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private string login;
        private string password;
        public AuthPage()
        {
            InitializeComponent();
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            login = LoginTextBox.Text;
            password = PasswordPassBx.Password;

            if (string.IsNullOrWhiteSpace(login) && string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Пожалуйста, введите логин.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите пароль.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var userObj = DatabaseEntities.GetContext().Users
                    .FirstOrDefault(x => x.UserLogin == login && x.UserPassword == password);

                if (userObj != null)
                {
                    Manager.AuthUser = userObj;
                    MessageBox.Show("Авторизация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    Manager.MainFrame.Navigate(new MenuPage());
                    Manager.BackButton.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при авторизации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
