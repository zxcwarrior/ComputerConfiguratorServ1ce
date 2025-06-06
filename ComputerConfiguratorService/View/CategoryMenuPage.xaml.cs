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
                        //Manager.MenuFrame.Navigate(new ComponentsPage());
                        break;
                    case "Конфигуратор":
                        //Manager.MenuFrame.Navigate(new ComponentsPage());
                        break;
                }
            }
        }
    }
}
