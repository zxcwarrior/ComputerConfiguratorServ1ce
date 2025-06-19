using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ComputerConfiguratorService.Model;

namespace ComputerConfiguratorService.View
{
    public partial class GPUMemoryTypesPage : Page
    {
        private GPUMemoryTypes _selected = null;
        private bool _isNew = false;

        public GPUMemoryTypesPage()
        {
            InitializeComponent();
            LoadReference();
        }

        private void LoadReference()
        {
            var ctx = DatabaseEntities.GetContext();
            LVReference.ItemsSource = ctx.GPUMemoryTypes
                                         .OrderBy(mt => mt.MemoryType)
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
            _selected = LVReference.SelectedItem as GPUMemoryTypes;
            if (_selected != null)
            {
                _isNew = false;
                tbName.Text = _selected.MemoryType;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = LVReference.SelectedItem as GPUMemoryTypes;
            if (item != null &&
                MessageBox.Show("Удалить этот тип памяти GPU?", "Подтверждение",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var ctx = DatabaseEntities.GetContext();
                ctx.GPUMemoryTypes.Remove(item);
                ctx.SaveChanges();
                LoadReference();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var ctx = DatabaseEntities.GetContext();
            if (_isNew)
            {
                ctx.GPUMemoryTypes.Add(new GPUMemoryTypes { MemoryType = tbName.Text.Trim() });
            }
            else if (_selected != null)
            {
                _selected.MemoryType = tbName.Text.Trim();
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
            BtnEdit.Background = System.Windows.Media.Brushes.Green;
            BtnDelete.Background = System.Windows.Media.Brushes.Green;
            BtnDelete.IsEnabled = has;
        }
    }
}
