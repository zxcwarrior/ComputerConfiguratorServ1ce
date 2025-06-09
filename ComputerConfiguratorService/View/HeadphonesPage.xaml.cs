using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ComputerConfiguratorService.Model;

namespace ComputerConfiguratorService.View
{
    public partial class HeadphonesPage : Page
    {
        private readonly DatabaseEntities _context;
        private Headphones _current;

        public HeadphonesPage()
        {
            InitializeComponent();
            _context = DatabaseEntities.GetContext();
            LoadAll();
            LoadManufacturers();
        }

        private void LoadAll()
        {
            LVHeadphones.ItemsSource = _context.Headphones
                .OrderBy(h => h.Model)
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
            bool sel = LVHeadphones.SelectedItem != null;
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = sel;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _current = null;
            ClearEdit();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LVHeadphones.SelectedItem is Headphones h)
            {
                _current = _context.Headphones.Find(h.HeadphonesID);
                if (_current != null)
                {
                    CbManufacturer.SelectedItem = _current.Manufacturers.ManufacturerName;
                    TbModel.Text = _current.Model;
                    TbPrice.Text = _current.Price.ToString("N0");
                    TbImagePath.Text = _current.ImagePath;
                    EditPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVHeadphones.SelectedItem is Headphones h)
            {
                var toDel = _context.Headphones.Find(h.HeadphonesID);
                if (toDel != null &&
                    MessageBox.Show($"Удалить наушники «{toDel.Model}»?", "Подтвердите",
                                    MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    == MessageBoxResult.Yes)
                {
                    _context.Headphones.Remove(toDel);
                    _context.SaveChanges();
                    LoadAll();
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            if (CbManufacturer.SelectedItem == null) sb.AppendLine("• Выберите производителя.");
            if (string.IsNullOrWhiteSpace(TbModel.Text)) sb.AppendLine("• Введите модель.");
            if (!decimal.TryParse(TbPrice.Text, out var pr) || pr <= 0) sb.AppendLine("• Укажите корректную цену.");
            if (string.IsNullOrWhiteSpace(TbImagePath.Text)) sb.AppendLine("• Укажите URL картинки.");
            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var manName = CbManufacturer.SelectedItem.ToString();
            var man = _context.Manufacturers.First(m => m.ManufacturerName == manName);

            if (_current == null)
            {
                var nd = new Headphones
                {
                    Model = TbModel.Text.Trim(),
                    ManufacturerID = man.ManufacturerID,
                    Price = pr,
                    ImagePath = TbImagePath.Text.Trim()
                };
                _context.Headphones.Add(nd);
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
            ClearEdit();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
            ClearEdit();
        }

        private void ClearEdit()
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
