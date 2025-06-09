using System.Windows.Controls;
using ComputerConfiguratorService.Utilities;

namespace ComputerConfiguratorService.View
{
    /// <summary>
    /// Логика взаимодействия для ConfiguratorMenu.xaml
    /// </summary>
    public partial class ConfiguratorMenu : Page
    {
        public ConfiguratorMenu()
        {
            InitializeComponent();
        }
        private void TypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeListBox.SelectedItem is ListBoxItem item)
            {
                switch (item.Content.ToString())
                {
                    case "процессор":
                        Manager.DetailFrame.Navigate(new CPUsPage());
                        break;
                    case "материнскую плату":
                        Manager.DetailFrame.Navigate(new MotherboardsPage());
                        break;
                    case "оперативную память":
                        Manager.DetailFrame.Navigate(new RAMsPage());
                        break;
                    case "охлаждение процессора":
                        Manager.DetailFrame.Navigate(new CPUCoolingPage());
                        break;
                    case "видеокарту":
                        Manager.DetailFrame.Navigate(new GPUsPage());
                        break;
                    case "блок питания":
                        Manager.DetailFrame.Navigate(new PowerSuppliesPage());
                        break;
                    case "накопитель":
                        Manager.DetailFrame.Navigate(new StoragesPage());
                        break;
                    case "корпус":
                        Manager.DetailFrame.Navigate(new CasesPage());
                        break;
                    case "охлаждение корпуса":
                        Manager.DetailFrame.Navigate(new CaseCoolingPage());
                        break;
                }
            }
        }
    }
}
