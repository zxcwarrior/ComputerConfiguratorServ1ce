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
    public partial class PowerSuppliesPage : Page
    {
        private readonly DatabaseEntities _context;
        private PowerSupplies _currentPSU;

        public PowerSuppliesPage()
        {
            InitializeComponent();
            CheckUser();
            _context = DatabaseEntities.GetContext();
            LoadAllPowerSupplies();
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

        private void LoadAllPowerSupplies()
        {
            LVPowerSupplies.ItemsSource = _context.PowerSupplies
                .Include(p => p.Manufacturers)
                .Include(p => p.EfficiencyRatings)
                .ToList();
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LoadFilterLists()
        {
            LbManufacturers.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                LbManufacturers.Items.Add(new ListBoxItem { Content = m.ManufacturerName });

            LbEfficiencyRatings.Items.Clear();
            foreach (var er in _context.EfficiencyRatings.OrderBy(er => er.Rating))
                LbEfficiencyRatings.Items.Add(new ListBoxItem { Content = er.Rating });
        }

        private void LoadEditCombos()
        {
            CbEditManufacturer.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                CbEditManufacturer.Items.Add(new ComboBoxItem { Content = m.ManufacturerName });

            CbEditEfficiencyRating.Items.Clear();
            foreach (var er in _context.EfficiencyRatings.OrderBy(er => er.Rating))
                CbEditEfficiencyRating.Items.Add(er.Rating);
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

            var selectedRatings = LbEfficiencyRatings.SelectedItems
                .Cast<ListBoxItem>()
                .Select(li => li.Content.ToString())
                .ToList();

            int wattageExact = -1;
            if (int.TryParse(TbWattage.Text, out var w))
                wattageExact = w;

            decimal priceMin = 0m, priceMax = decimal.MaxValue;
            if (decimal.TryParse(TbPriceMin.Text, out var pMin))
                priceMin = pMin;
            if (decimal.TryParse(TbPriceMax.Text, out var pMax))
                priceMax = pMax;

            string modelFilter = TbModelFilter.Text?.Trim().ToLower();

            var query = _context.PowerSupplies
                .Include(p => p.Manufacturers)
                .Include(p => p.EfficiencyRatings)
                .AsQueryable();

            if (selectedManufacturers.Count > 0)
                query = query.Where(p => selectedManufacturers.Contains(p.Manufacturers.ManufacturerName));

            if (selectedRatings.Count > 0)
                query = query.Where(p => selectedRatings.Contains(p.EfficiencyRatings.Rating));

            if (wattageExact > 0)
                query = query.Where(p => p.Wattage == wattageExact);

            query = query.Where(p => p.Price >= priceMin && p.Price <= priceMax);

            if (!string.IsNullOrEmpty(modelFilter))
                query = query.Where(p => p.Model.ToLower().Contains(modelFilter));

            LVPowerSupplies.ItemsSource = query.ToList();
            FilterPanel.Visibility = Visibility.Collapsed;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LVPowerSupplies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isSelected = LVPowerSupplies.SelectedItem != null;
            BtnEdit.IsEnabled = isSelected;
            BtnDelete.IsEnabled = isSelected;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _currentPSU = null;
            ClearEditFields();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LVPowerSupplies.SelectedItem is PowerSupplies selected)
            {
                _currentPSU = _context.PowerSupplies.Find(selected.PowerSupplyID);
                if (_currentPSU != null)
                {
                    CbEditManufacturer.SelectedItem = CbEditManufacturer.Items
                        .OfType<ComboBoxItem>()
                        .FirstOrDefault(i => i.Content?.ToString() == _currentPSU.Manufacturers.ManufacturerName);

                    CbEditEfficiencyRating.SelectedItem = _currentPSU.EfficiencyRatings.Rating;
                    TbEditWattage.Text = _currentPSU.Wattage.ToString();
                    TbEditModel.Text = _currentPSU.Model;
                    TbEditPrice.Text = _currentPSU.Price.ToString("N2");
                    TbEditImagePath.Text = _currentPSU.ImagePath;
                    EditPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVPowerSupplies.SelectedItem is PowerSupplies selected)
            {
                var psuToDelete = _context.PowerSupplies.Find(selected.PowerSupplyID);
                if (psuToDelete != null)
                {
                    var result = MessageBox.Show(
                        $"Удалить блок питания \"{psuToDelete.Model}\"?",
                        "Подтвердите удаление",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.PowerSupplies.Remove(psuToDelete);
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
                        LoadAllPowerSupplies();
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            if (CbEditManufacturer.SelectedItem == null)
                sb.AppendLine("• Выберите производителя.");
            if (CbEditEfficiencyRating.SelectedItem == null)
                sb.AppendLine("• Выберите рейтинг.");
            if (!int.TryParse(TbEditWattage.Text, out var watt))
                sb.AppendLine("• Неверное значение мощности.");
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

            var selectedRating = CbEditEfficiencyRating.SelectedItem as string;
            var ratingEntity = _context.EfficiencyRatings.First(er => er.Rating == selectedRating);

            if (_currentPSU == null)
            {
                var newPSU = new PowerSupplies
                {
                    Manufacturers = manufacturerEntity,
                    EfficiencyRatings = ratingEntity,
                    Wattage = watt,
                    Model = TbEditModel.Text.Trim(),
                    Price = price,
                    ImagePath = TbEditImagePath.Text.Trim()
                };
                _context.PowerSupplies.Add(newPSU);
            }
            else
            {
                _currentPSU.Manufacturers = manufacturerEntity;
                _currentPSU.EfficiencyRatings = ratingEntity;
                _currentPSU.Wattage = watt;
                _currentPSU.Model = TbEditModel.Text.Trim();
                _currentPSU.Price = price;
                _currentPSU.ImagePath = TbEditImagePath.Text.Trim();
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

            LoadAllPowerSupplies();
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
            CbEditEfficiencyRating.SelectedIndex = -1;
            TbEditWattage.Clear();
            TbEditModel.Clear();
            TbEditPrice.Clear();
            TbEditImagePath.Clear();
            _currentPSU = null;
        }
    }
}
