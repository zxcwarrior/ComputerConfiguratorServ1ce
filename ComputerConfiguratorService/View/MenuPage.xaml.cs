using ComputerConfiguratorService.Utilities;
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

namespace ComputerConfiguratorService.View
{
    /// <summary>
    /// Логика взаимодействия для MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
            Manager.MenuFrame = MenuFrame;         
            Manager.DetailFrame = DetailFrame;
            Manager.MenuFrame.Navigate(new CategoryMenuPage());
        }

        private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MenuFrame.Navigate(new CategoryMenuPage());
            Manager.DetailFrame.Navigate((object)null);
        }
    }
}
