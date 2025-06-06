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
    /// Логика взаимодействия для MotherboardFormFactorPage.xaml
    /// </summary>
    public partial class MotherboardFormFactorPage : Page
    {
        private MotherboardFormFactor selectedMBFF = null;
        private bool isNewRecord = false;
        public MotherboardFormFactorPage()
        {
            InitializeComponent();
            LoadMotherboardFormFactors();
        }
        private void LoadMotherboardFormFactors()
        {
            DGMotherboardFormFactors.ItemsSource = DatabaseEntities.GetContext().MotherboardFormFactor.ToList();
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            selectedMBFF = null;
            isNewRecord = true;
            tbName.Text = "";
            EditPanel.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            selectedMBFF = (sender as Button).DataContext as MotherboardFormFactor;
            if (selectedMBFF != null)
            {
                isNewRecord = false;
                tbName.Text = selectedMBFF.MotherboardFFName;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = DatabaseEntities.GetContext();
            if (isNewRecord)
            {
                MotherboardFormFactor newMBFF = new MotherboardFormFactor
                {
                    MotherboardFFName = tbName.Text
                };
                context.MotherboardFormFactor.Add(newMBFF);
            }
            else if (selectedMBFF != null)
            {
                selectedMBFF.MotherboardFFName = tbName.Text;
            }
            context.SaveChanges();
            LoadMotherboardFormFactors();
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var mbff = (sender as Button).DataContext as MotherboardFormFactor;
            if (mbff != null && MessageBox.Show("Удалить этот форм-фактор материнской платы?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DatabaseEntities.GetContext().MotherboardFormFactor.Remove(mbff);
                DatabaseEntities.GetContext().SaveChanges();
                LoadMotherboardFormFactors();
            }
        }
    }
}
