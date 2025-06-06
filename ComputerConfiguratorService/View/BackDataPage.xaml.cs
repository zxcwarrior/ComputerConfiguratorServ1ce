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
        private void ManufacturersButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ManufacturersPage());
        }

        private void SocketsButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new SocketsPage());
        }

        private void RAMTypesButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new RAMTypesPage());
        }

        private void StorageTypesButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new StorageTypesPage());
        }

        private void CoolingTypesButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new CoolingTypesPage());
        }

        private void CaseFormFactorsButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new CaseFormFactorsPage());
        }
        private void MotherboardFormFactorsButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new MotherboardFormFactorPage());
        }
        private void EfficiencyRatingsButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new EfficiencyRatingsPage());
        }

        private void VendorsButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new VendorsPage());
        }

        private void GPUMemoryTypesButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new GPUMemoryTypesPage());
        }
    }
}
