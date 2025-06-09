using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ComputerConfiguratorService.Model;

namespace ComputerConfiguratorService.View
{
    public partial class MonitorsPage : Page
    {
        private readonly DatabaseEntities _context;
        private Monitors _current;

        public MonitorsPage()
        {
            InitializeComponent();
            _context = DatabaseEntities.GetContext();
            LoadAll();
            LoadManufacturers();
        }

        private void LoadAll()
        {
            LVMonitors.ItemsSource = _context.Monitors
                .Include("Manufacturers")
                .OrderBy(m => m.Model)
                .ToList();
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = false;
        }

        private void LoadManufacturers()
        {
            CbManufacturer.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                CbManufacturer.Items.Add(m.ManufacturerName);
        }

        private void LV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool sel = LVMonitors.SelectedItem != null;
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = sel;
        }

        private void BtnAdd_Click(object s, RoutedEventArgs e)
        {
            _current = null;
            ClearForm();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object s, RoutedEventArgs e)
        {
            if (LVMonitors.SelectedItem is Monitors m)
            {
                _current = _context.Monitors.Find(m.MonitorID);
                CbManufacturer.SelectedItem = _current.Manufacturers.ManufacturerName;
                TbModel.Text = _current.Model;
                TbPrice.Text = _current.Price.ToString("N0");
                TbImagePath.Text = _current.ImagePath;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void BtnDelete_Click(object s, RoutedEventArgs e)
        {
            if (LVMonitors.SelectedItem is Monitors m)
            {
                var toDel = _context.Monitors.Find(m.MonitorID);
                if (toDel != null &&
                    MessageBox.Show($"Удалить «{toDel.Model}»?", "Подтвердите",
                                    MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    == MessageBoxResult.Yes)
                {
                    _context.Monitors.Remove(toDel);
                    _context.SaveChanges();
                    LoadAll();
                }
            }
        }

        private void BtnSave_Click(object s, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            if (CbManufacturer.SelectedItem == null) sb.AppendLine("• Выберите производителя.");
            if (string.IsNullOrWhiteSpace(TbModel.Text)) sb.AppendLine("• Введите модель.");
            if (!decimal.TryParse(TbPrice.Text, out var pr) || pr <= 0) sb.AppendLine("• Укажите цену.");
            if (string.IsNullOrWhiteSpace(TbImagePath.Text)) sb.AppendLine("• Укажите URL картинки.");
            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var man = _context.Manufacturers.First(m => m.ManufacturerName == (string)CbManufacturer.SelectedItem);

            if (_current == null)
            {
                _current = new Monitors
                {
                    Model = TbModel.Text.Trim(),
                    ManufacturerID = man.ManufacturerID,
                    Price = pr,
                    ImagePath = TbImagePath.Text.Trim()
                };
                _context.Monitors.Add(_current);
            }
            else
            {
                _current.Model = TbModel.Text.Trim();
                _current.ManufacturerID = man.ManufacturerID;
                _current.Price = pr;
                _current.ImagePath = TbImagePath.Text.Trim();
            }

            _context.SaveChanges();
            LoadAll();
            EditPanel.Visibility = Visibility.Collapsed;
            ClearForm();
        }

        private void BtnCancel_Click(object s, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
            ClearForm();
        }

        private void ClearForm()
        {
            CbManufacturer.SelectedIndex = -1;
            TbModel.Clear();
            TbPrice.Clear();
            TbImagePath.Clear();
            _current = null;
        }

        private void NumericOnly(object sender, TextCompositionEventArgs e)
            => e.Handled = !e.Text.All(char.IsDigit);
    }
}
