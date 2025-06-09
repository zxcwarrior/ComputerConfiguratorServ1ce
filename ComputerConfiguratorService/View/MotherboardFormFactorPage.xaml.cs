using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ComputerConfiguratorService.Model;

namespace ComputerConfiguratorService.View
{
    public partial class MotherboardFormFactorPage : Page
    {
        private MotherboardFormFactor _selected = null;
        private bool _isNew = false;

        public MotherboardFormFactorPage()
        {
            InitializeComponent();
            LoadReference();
        }

        private void LoadReference()
        {
            var ctx = DatabaseEntities.GetContext();
            LVReference.ItemsSource = ctx.MotherboardFormFactor
                                         .OrderBy(f => f.MotherboardFFName)
                                         .ToList();
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = false;
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            _isNew = true; _selected = null; tbName.Clear();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _selected = LVReference.SelectedItem as MotherboardFormFactor;
            if (_selected != null)
            {
                _isNew = false; tbName.Text = _selected.MotherboardFFName;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = LVReference.SelectedItem as MotherboardFormFactor;
            if (item != null &&
                MessageBox.Show("Удалить этот форм-фактор?", "Подтверждение",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var ctx = DatabaseEntities.GetContext();
                ctx.MotherboardFormFactor.Remove(item);
                ctx.SaveChanges();
                LoadReference();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var ctx = DatabaseEntities.GetContext();
            if (_isNew)
                ctx.MotherboardFormFactor.Add(new MotherboardFormFactor { MotherboardFFName = tbName.Text.Trim() });
            else if (_selected != null)
                _selected.MotherboardFFName = tbName.Text.Trim();
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
