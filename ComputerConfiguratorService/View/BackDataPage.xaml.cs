using ComputerConfiguratorService.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace ComputerConfiguratorService.View
{
    /// <summary>
    /// Логика взаимодействия для BackDataPage.xaml
    /// </summary>
    public partial class BackDataPage : Page
    {
        public BackDataPage()
        {
            InitializeComponent();
        }
        private void TypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeListBox.SelectedItem is ListBoxItem item)
            {
                switch (item.Content.ToString())
                {
                    case "Производители":
                        Manager.DetailFrame.Navigate(new ManufacturersPage());
                        break;
                    case "Сокеты":
                        Manager.DetailFrame.Navigate(new SocketsPage());
                        break;
                    case "Типы RAM":
                        Manager.DetailFrame.Navigate(new RAMTypesPage());
                        break;
                    case "Типы накопителей":
                        Manager.DetailFrame.Navigate(new StorageTypesPage());
                        break;
                    case "Типы охлаждения":
                        Manager.DetailFrame.Navigate(new CoolingTypesPage());
                        break;
                    case "Форм-факторы корпусов":
                        Manager.DetailFrame.Navigate(new CaseFormFactorsPage());
                        break;
                    case "Форм-факторы мат.плат":
                        Manager.DetailFrame.Navigate(new MotherboardFormFactorPage());
                        break;
                    case "Рейтинги эффективности":
                        Manager.DetailFrame.Navigate(new EfficiencyRatingsPage());
                        break;
                    case "Вендоры":
                        Manager.DetailFrame.Navigate(new VendorsPage());
                        break;
                    case "Типы памяти GPU":
                        Manager.DetailFrame.Navigate(new GPUMemoryTypesPage());
                        break;
                }
            }
        }
    }
}
