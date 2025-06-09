using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ComputerConfiguratorService.Model;

namespace ComputerConfiguratorService.View
{
    public partial class ManufacturersPage : Page
    {
        private Manufacturers _selected = null;
        private bool _isNew = false;

        public ManufacturersPage()
        {
            InitializeComponent();
            LoadReference();
        }

        private void LoadReference()
        {
            var ctx = DatabaseEntities.GetContext();
            LVReference.ItemsSource = ctx.Manufacturers
                                         .OrderBy(m => m.ManufacturerName)
                                         .ToList();
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = false;
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            _isNew = true;
            _selected = null;
            tbName.Clear();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _selected = LVReference.SelectedItem as Manufacturers;
            if (_selected != null)
            {
                _isNew = false;
                tbName.Text = _selected.ManufacturerName;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = LVReference.SelectedItem as Manufacturers;
            if (item != null &&
                MessageBox.Show("Удалить этого производителя?", "Подтверждение",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var ctx = DatabaseEntities.GetContext();
                ctx.Manufacturers.Remove(item);
                ctx.SaveChanges();
                LoadReference();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var ctx = DatabaseEntities.GetContext();
            if (_isNew)
            {
                ctx.Manufacturers.Add(new Manufacturers { ManufacturerName = tbName.Text.Trim() });
            }
            else if (_selected != null)
            {
                _selected.ManufacturerName = tbName.Text.Trim();
            }
            ctx.SaveChanges();
            EditPanel.Visibility = Visibility.Collapsed;
            LoadReference();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void LVReference_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool has = LVReference.SelectedItem != null;
            BtnEdit.IsEnabled = has;
            BtnDelete.IsEnabled = has;
        }
    }
}
