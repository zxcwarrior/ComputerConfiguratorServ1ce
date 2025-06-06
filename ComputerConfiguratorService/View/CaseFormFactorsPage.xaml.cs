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
    /// Логика взаимодействия для CaseFormFactorsPage.xaml
    /// </summary>
    public partial class CaseFormFactorsPage : Page
    {
        private CaseFormFactors selectedCaseFF = null;
        private bool isNewRecord = false;
        public CaseFormFactorsPage()
        {
            InitializeComponent();
            LoadCaseFormFactors();
        }
        private void LoadCaseFormFactors()
        {
            DGCaseFormFactors.ItemsSource = DatabaseEntities.GetContext().CaseFormFactors.ToList();
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            selectedCaseFF = null;
            isNewRecord = true;
            tbName.Text = "";
            EditPanel.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            selectedCaseFF = (sender as Button).DataContext as CaseFormFactors;
            if (selectedCaseFF != null)
            {
                isNewRecord = false;
                tbName.Text = selectedCaseFF.CaseFFName;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = DatabaseEntities.GetContext();
            if (isNewRecord)
            {
                CaseFormFactors newCaseFF = new CaseFormFactors
                {
                    CaseFFName = tbName.Text
                };
                context.CaseFormFactors.Add(newCaseFF);
            }
            else if (selectedCaseFF != null)
            {
                selectedCaseFF.CaseFFName = tbName.Text;
            }
            context.SaveChanges();
            LoadCaseFormFactors();
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var caseFF = (sender as Button).DataContext as CaseFormFactors;
            if (caseFF != null && MessageBox.Show("Удалить этот форм-фактор корпуса?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DatabaseEntities.GetContext().CaseFormFactors.Remove(caseFF);
                DatabaseEntities.GetContext().SaveChanges();
                LoadCaseFormFactors();
            }
        }
    }
}
