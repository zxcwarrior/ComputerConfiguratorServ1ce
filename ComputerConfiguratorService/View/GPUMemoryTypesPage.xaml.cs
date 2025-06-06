using ComputerConfiguratorService.Model;
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
    /// Логика взаимодействия для GPUMemoryTypesPage.xaml
    /// </summary>
    public partial class GPUMemoryTypesPage : Page
    {
        private GPUMemoryTypes selectedMemoryType = null;
        private bool isNewRecord = false;
        public GPUMemoryTypesPage()
        {
            InitializeComponent();
            LoadGPUMemoryTypes();
        }
        private void LoadGPUMemoryTypes()
        {
            DGGPUMemoryTypes.ItemsSource = DatabaseEntities.GetContext().GPUMemoryTypes.ToList();
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            selectedMemoryType = null;
            isNewRecord = true;
            tbName.Text = "";
            EditPanel.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            selectedMemoryType = (sender as Button).DataContext as GPUMemoryTypes;
            if (selectedMemoryType != null)
            {
                isNewRecord = false;
                tbName.Text = selectedMemoryType.MemoryType;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = DatabaseEntities.GetContext();
            if (isNewRecord)
            {
                GPUMemoryTypes newMemoryType = new GPUMemoryTypes
                {
                    MemoryType = tbName.Text
                };
                context.GPUMemoryTypes.Add(newMemoryType);
            }
            else if (selectedMemoryType != null)
            {
                selectedMemoryType.MemoryType = tbName.Text;
            }
            context.SaveChanges();
            LoadGPUMemoryTypes();
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var memoryType = (sender as Button).DataContext as GPUMemoryTypes;
            if (memoryType != null && MessageBox.Show("Удалить этот тип памяти GPU?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DatabaseEntities.GetContext().GPUMemoryTypes.Remove(memoryType);
                DatabaseEntities.GetContext().SaveChanges();
                LoadGPUMemoryTypes();
            }
        }
    }
}
