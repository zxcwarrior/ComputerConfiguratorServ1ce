using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ComputerConfiguratorService.Model;

namespace ComputerConfiguratorService.View
{
    public partial class VendorsPage : Page
    {
        private Vendors _selected = null;
        private bool _isNew = false;

        public VendorsPage()
        {
            InitializeComponent();
            LoadReference();
        }

        private void LoadReference()
        {
            var ctx = DatabaseEntities.GetContext();
            LVReference.ItemsSource = ctx.Vendors.OrderBy(v => v.VendorName).ToList();
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = false;
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            _isNew = true; _selected = null; tbName.Clear();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _selected = LVReference.SelectedItem as Vendors;
            if (_selected != null)
            {
                _isNew = false; tbName.Text = _selected.VendorName;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = LVReference.SelectedItem as Vendors;
            if (item != null &&
                MessageBox.Show("Удалить этого вендора?", "Подтверждение",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var ctx = DatabaseEntities.GetContext();
                ctx.Vendors.Remove(item);
                ctx.SaveChanges();
                LoadReference();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var ctx = DatabaseEntities.GetContext();
            if (_isNew)
                ctx.Vendors.Add(new Vendors { VendorName = tbName.Text.Trim() });
            else if (_selected != null)
                _selected.VendorName = tbName.Text.Trim();
            ctx.SaveChanges();
            EditPanel.Visibility = Visibility.Collapsed;
            LoadReference();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) =>
            EditPanel.Visibility = Visibility.Collapsed;

        private void LVReference_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool has = LVReference.SelectedItem != null;
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = has;
        }
    }
}
