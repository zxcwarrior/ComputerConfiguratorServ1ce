using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ComputerConfiguratorService.Utilities;
using ComputerConfiguratorService.Model;

namespace ComputerConfiguratorService.View
{
    public partial class CaseCoolingPage : Page
    {
        private readonly DatabaseEntities _context;
        private CaseCooling _currentCaseCooling;

        public CaseCoolingPage()
        {
            InitializeComponent();
            CheckUser();
            _context = DatabaseEntities.GetContext();
            LoadAllCaseCooling();
            LoadFilterLists();
            LoadEditCombos();
        }

        private void CheckUser()
        {
            if (Manager.AuthUser == null || Manager.AuthUser.Roles.RoleID != 1)
                BtnAdminPanel.Visibility = Visibility.Collapsed;
            else
                BtnAdminPanel.Visibility = Visibility.Visible;
        }

        private void LoadAllCaseCooling()
        {
            LVCaseCooling.ItemsSource = _context.CaseCooling
                .Include(c => c.Manufacturers)
                .ToList();
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LoadFilterLists()
        {
            LbManufacturers.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                LbManufacturers.Items.Add(new ListBoxItem { Content = m.ManufacturerName });
        }

        private void LoadEditCombos()
        {
            CbEditManufacturer.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                CbEditManufacturer.Items.Add(new ComboBoxItem { Content = m.ManufacturerName });
        }

        private void BtnToggleFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterPanel.Visibility =
                FilterPanel.Visibility == Visibility.Collapsed
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void BtnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            var selectedManufacturers = LbManufacturers.SelectedItems
                .Cast<ListBoxItem>()
                .Select(li => li.Content.ToString())
                .ToList();

            int fanSizeExact = -1;
            if (int.TryParse(TbFanSize.Text, out var fs))
                fanSizeExact = fs;

            decimal priceMin = 0m, priceMax = decimal.MaxValue;
            if (decimal.TryParse(TbPriceMin.Text, out var pMin))
                priceMin = pMin;
            if (decimal.TryParse(TbPriceMax.Text, out var pMax))
                priceMax = pMax;

            string modelFilter = TbModelFilter.Text?.Trim().ToLower();

            var query = _context.CaseCooling
                .Include(c => c.Manufacturers)
                .AsQueryable();

            if (selectedManufacturers.Count > 0)
                query = query.Where(c => selectedManufacturers.Contains(c.Manufacturers.ManufacturerName));

            if (fanSizeExact > 0)
                query = query.Where(c => c.FanSize == fanSizeExact);

            query = query.Where(c => c.Price >= priceMin && c.Price <= priceMax);

            if (!string.IsNullOrEmpty(modelFilter))
                query = query.Where(c => c.Model.ToLower().Contains(modelFilter));

            LVCaseCooling.ItemsSource = query.ToList();
            FilterPanel.Visibility = Visibility.Collapsed;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LVCaseCooling_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isSelected = LVCaseCooling.SelectedItem != null;
            BtnEdit.IsEnabled = isSelected;
            BtnDelete.IsEnabled = isSelected;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _currentCaseCooling = null;
            ClearEditFields();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LVCaseCooling.SelectedItem is CaseCooling selected)
            {
                _currentCaseCooling = _context.CaseCooling.Find(selected.CaseCoolingID);
                if (_currentCaseCooling != null)
                {
                    CbEditManufacturer.SelectedItem = CbEditManufacturer.Items
                        .OfType<ComboBoxItem>()
                        .FirstOrDefault(i => i.Content?.ToString() == _currentCaseCooling.Manufacturers.ManufacturerName);

                    TbEditFanSize.Text = _currentCaseCooling.FanSize.ToString();
                    TbEditModel.Text = _currentCaseCooling.Model;
                    TbEditPrice.Text = _currentCaseCooling.Price.ToString("N2");
                    TbEditImagePath.Text = _currentCaseCooling.ImagePath;
                    EditPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVCaseCooling.SelectedItem is CaseCooling selected)
            {
                var ccToDelete = _context.CaseCooling.Find(selected.CaseCoolingID);
                if (ccToDelete != null)
                {
                    var result = MessageBox.Show(
                        $"Удалить охлаждение \"{ccToDelete.Model}\"?",
                        "Подтвердите удаление",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.CaseCooling.Remove(ccToDelete);
                        try
                        {
                            _context.SaveChanges();
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("Ошибка при удалении: " + ex.Message,
                                            "Ошибка",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                        }
                        LoadAllCaseCooling();
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            if (CbEditManufacturer.SelectedItem == null)
                sb.AppendLine("• Выберите производителя.");
            if (!int.TryParse(TbEditFanSize.Text, out var fanSize))
                sb.AppendLine("• Неверное значение размера.");
            if (string.IsNullOrWhiteSpace(TbEditModel.Text))
                sb.AppendLine("• Введите модель.");
            if (!decimal.TryParse(TbEditPrice.Text, out var price))
                sb.AppendLine("• Неверная цена.");

            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString().TrimEnd(),
                                "Ошибка ввода",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            var selectedManufacturerItem = CbEditManufacturer.SelectedItem as ComboBoxItem;
            var selectedManufacturerName = selectedManufacturerItem.Content.ToString();
            var manufacturerEntity = _context.Manufacturers.First(m => m.ManufacturerName == selectedManufacturerName);

            if (_currentCaseCooling == null)
            {
                var newCC = new CaseCooling
                {
                    Manufacturers = manufacturerEntity,
                    FanSize = fanSize,
                    Model = TbEditModel.Text.Trim(),
                    Price = price,
                    ImagePath = TbEditImagePath.Text.Trim()
                };
                _context.CaseCooling.Add(newCC);
            }
            else
            {
                _currentCaseCooling.Manufacturers = manufacturerEntity;
                _currentCaseCooling.FanSize = fanSize;
                _currentCaseCooling.Model = TbEditModel.Text.Trim();
                _currentCaseCooling.Price = price;
                _currentCaseCooling.ImagePath = TbEditImagePath.Text.Trim();
            }

            try
            {
                _context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message,
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            LoadAllCaseCooling();
            EditPanel.Visibility = Visibility.Collapsed;
            ClearEditFields();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
            ClearEditFields();
        }

        private void ClearEditFields()
        {
            CbEditManufacturer.SelectedIndex = -1;
            TbEditFanSize.Clear();
            TbEditModel.Clear();
            TbEditPrice.Clear();
            TbEditImagePath.Clear();
            _currentCaseCooling = null;
        }
    }
}
