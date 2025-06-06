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
    /// Логика взаимодействия для EfficiencyRatingsPage.xaml
    /// </summary>
    public partial class EfficiencyRatingsPage : Page
    {
        private EfficiencyRatings selectedRating = null;
        private bool isNewRecord = false;
        public EfficiencyRatingsPage()
        {
            InitializeComponent();
            LoadEfficiencyRatings();
        }
        private void LoadEfficiencyRatings()
        {
            DGEfficiencyRatings.ItemsSource = DatabaseEntities.GetContext().EfficiencyRatings.ToList();
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            selectedRating = null;
            isNewRecord = true;
            tbName.Text = "";
            EditPanel.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            selectedRating = (sender as Button).DataContext as EfficiencyRatings;
            if (selectedRating != null)
            {
                isNewRecord = false;
                tbName.Text = selectedRating.Rating;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = DatabaseEntities.GetContext();
            if (isNewRecord)
            {
                EfficiencyRatings newRating = new EfficiencyRatings
                {
                    Rating = tbName.Text
                };
                context.EfficiencyRatings.Add(newRating);
            }
            else if (selectedRating != null)
            {
                selectedRating.Rating = tbName.Text;
            }
            context.SaveChanges();
            LoadEfficiencyRatings();
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var rating = (sender as Button).DataContext as EfficiencyRatings;
            if (rating != null && MessageBox.Show("Удалить этот рейтинг эффективности?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DatabaseEntities.GetContext().EfficiencyRatings.Remove(rating);
                DatabaseEntities.GetContext().SaveChanges();
                LoadEfficiencyRatings();
            }
        }
    }
}
