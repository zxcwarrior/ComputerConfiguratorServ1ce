using System.Windows.Controls;
using ComputerConfiguratorService.Utilities;

namespace ComputerConfiguratorService.View
{
    /// <summary>
    /// Логика взаимодействия для ComponentsPage.xaml
    /// </summary>
    public partial class ComponentsPage : Page
    {
        public ComponentsPage()
        {
            InitializeComponent();
        }
        private void TypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeListBox.SelectedItem is ListBoxItem item)
            {
                switch (item.Content.ToString())
                {
                    case "Процессоры":
                        Manager.DetailFrame.Navigate(new CPUsPage());
                        break;
                    case "Материнские платы":
                        Manager.DetailFrame.Navigate(new MotherboardsPage());
                        break;
                    case "Оперативная память":
                        Manager.DetailFrame.Navigate(new RAMsPage());
                        break;
                    case "Охлаждение процессора":
                        Manager.DetailFrame.Navigate(new CPUCoolingPage());
                        break;
                    case "Видеокарты":
                        Manager.DetailFrame.Navigate(new GPUsPage());
                        break;
                    case "Блоки питания":
                        Manager.DetailFrame.Navigate(new PowerSuppliesPage());
                        break;
                    case "Накопители":
                        Manager.DetailFrame.Navigate(new StoragesPage());
                        break;
                    case "Корпусы":
                        Manager.DetailFrame.Navigate(new CasesPage());
                        break;
                    case "Корпусные вентиляторы":
                        Manager.DetailFrame.Navigate(new CaseCoolingPage());
                        break;
                }
            }
        }
    }
}
