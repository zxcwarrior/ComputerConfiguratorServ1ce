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
    /// Логика взаимодействия для AccessoriesPage.xaml
    /// </summary>
    public partial class AccessoriesPage : Page
    {
        public AccessoriesPage()
        {
            InitializeComponent();
        }
        private void TypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeListBox.SelectedItem is ListBoxItem item)
            {
                switch (item.Content.ToString())
                {
                    case "Клавиатуры":
                        Manager.DetailFrame.Navigate(new KeyboardsPage());
                        break;
                    case "Мышки":
                        Manager.DetailFrame.Navigate(new MousesPage());
                        break;
                    case "Наушники":
                        Manager.DetailFrame.Navigate(new HeadphonesPage());
                        break;
                    case "Микрофоны":
                        Manager.DetailFrame.Navigate(new MicrophonesPage());
                        break;
                    case "Мониторы":
                        Manager.DetailFrame.Navigate(new MonitorsPage());
                        break;
                }
            }
        }
    }
}
