using System.Windows;
using System.Windows.Controls;
using ComputerConfiguratorService.Utilities;

namespace ComputerConfiguratorService.View
{
    /// <summary>
    /// Логика взаимодействия для CategoryMenuPage.xaml
    /// </summary>
    public partial class CategoryMenuPage : Page
    {
        public CategoryMenuPage()
        {
            InitializeComponent();
            CheckUser();
        }
        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuListBox.SelectedItem is ListBoxItem item)
            {
                switch (item.Content.ToString())
                {
                    case "Комплектующие":
                        Manager.MenuFrame.Navigate(new ComponentsPage());
                        break;
                    case "Готовые сборки":
                        Manager.DetailFrame.Navigate(new CompleteBuildsPage());//not ready
                        break;
                    case "Конфигуратор":
                        Manager.DetailFrame.Navigate(new ConfiguratorPage());//half ready 
                        break;
                    case "Справочные данные":
                        Manager.MenuFrame.Navigate(new BackDataPage());
                        break;
                    case "Аксессуары":
                        Manager.MenuFrame.Navigate(new AccessoriesPage());
                        break;
                    case "Услуги":
                        Manager.DetailFrame.Navigate(new ServicesPage());
                        break;
                    case "Заказы":
                        Manager.DetailFrame.Navigate(new OrdersPage());//not ready
                        break;
                }
            }
        }
        private void CheckUser()
        {
            if (Manager.AuthUser == null || Manager.AuthUser.Roles.RoleID != 1)
            {
                BackData.Visibility = Visibility.Collapsed;
                Components.Visibility = Visibility.Collapsed;
                Accessories.Visibility = Visibility.Collapsed;
                Services.Visibility = Visibility.Collapsed;
            }
        }
    }
}
