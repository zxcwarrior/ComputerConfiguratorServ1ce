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
    public partial class CPUCoolingPage : Page
    {
        private readonly DatabaseEntities _context;
        private CPUCooling _currentCooling;

        public CPUCoolingPage()
        {
            InitializeComponent();
            CheckUser();
            _context = DatabaseEntities.GetContext();
            LoadAllCooling();
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

        private void LoadAllCooling()
        {
            LVCpuCooling.ItemsSource = _context.CPUCooling
                .Include(c => c.Manufacturers)
                .Include(c => c.CoolingTypes)
                .ToList();
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LoadFilterLists()
        {
            LbManufacturers.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                LbManufacturers.Items.Add(new ListBoxItem { Content = m.ManufacturerName });

            LbCoolingTypes.Items.Clear();
            foreach (var ct in _context.CoolingTypes.OrderBy(ct => ct.CoolingType))
                LbCoolingTypes.Items.Add(new ListBoxItem { Content = ct.CoolingType });
        }

        private void LoadEditCombos()
        {
            CbEditManufacturer.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                CbEditManufacturer.Items.Add(new ComboBoxItem { Content = m.ManufacturerName });

            CbEditCoolingType.Items.Clear();
            foreach (var ct in _context.CoolingTypes.OrderBy(ct => ct.CoolingType))
                CbEditCoolingType.Items.Add(ct.CoolingType);
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

            var selectedCoolingTypes = LbCoolingTypes.SelectedItems
                .Cast<ListBoxItem>()
                .Select(li => li.Content.ToString())
                .ToList();

            int tdpMax = -1;
            if (int.TryParse(TbTDPMax.Text, out var tdp))
                tdpMax = tdp;

            decimal priceMin = 0m, priceMax = decimal.MaxValue;
            if (decimal.TryParse(TbPriceMin.Text, out var pMin))
                priceMin = pMin;
            if (decimal.TryParse(TbPriceMax.Text, out var pMax))
                priceMax = pMax;

            string modelFilter = TbModelFilter.Text?.Trim().ToLower();

            var query = _context.CPUCooling
                .Include(c => c.Manufacturers)
                .Include(c => c.CoolingTypes)
                .AsQueryable();

            if (selectedManufacturers.Count > 0)
                query = query.Where(c => selectedManufacturers.Contains(c.Manufacturers.ManufacturerName));

            if (selectedCoolingTypes.Count > 0)
                query = query.Where(c => selectedCoolingTypes.Contains(c.CoolingTypes.CoolingType));

            if (tdpMax > 0)
                query = query.Where(c => c.MaxSupportedTDP <= tdpMax);

            query = query.Where(c => c.Price >= priceMin && c.Price <= priceMax);

            if (!string.IsNullOrEmpty(modelFilter))
                query = query.Where(c => c.Model.ToLower().Contains(modelFilter));

            LVCpuCooling.ItemsSource = query.ToList();
            FilterPanel.Visibility = Visibility.Collapsed;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LVCpuCooling_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isSelected = LVCpuCooling.SelectedItem != null;
            BtnEdit.IsEnabled = isSelected;
            BtnDelete.IsEnabled = isSelected;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _currentCooling = null;
            ClearEditFields();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LVCpuCooling.SelectedItem is CPUCooling selected)
            {
                _currentCooling = _context.CPUCooling.Find(selected.CPUCoolingID);
                if (_currentCooling != null)
                {
                    CbEditManufacturer.SelectedItem = CbEditManufacturer.Items
                        .OfType<ComboBoxItem>()
                        .FirstOrDefault(i => i.Content?.ToString() == _currentCooling.Manufacturers.ManufacturerName);

                    CbEditCoolingType.SelectedItem = _currentCooling.CoolingTypes.CoolingType;
                    TbEditModel.Text = _currentCooling.Model;
                    TbEditMaxTDP.Text = _currentCooling.MaxSupportedTDP.ToString();
                    TbEditPrice.Text = _currentCooling.Price.ToString("N2");
                    TbEditImagePath.Text = _currentCooling.ImagePath;
                    EditPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVCpuCooling.SelectedItem is CPUCooling selected)
            {
                var coolingToDelete = _context.CPUCooling.Find(selected.CPUCoolingID);
                if (coolingToDelete != null)
                {
                    var result = MessageBox.Show(
                        $"Удалить охлаждение \"{coolingToDelete.Model}\"?",
                        "Подтвердите удаление",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.CPUCooling.Remove(coolingToDelete);
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
                        LoadAllCooling();
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            if (CbEditManufacturer.SelectedItem == null)
                sb.AppendLine("• Выберите производителя.");
            if (CbEditCoolingType.SelectedItem == null)
                sb.AppendLine("• Выберите тип охлаждения.");
            if (string.IsNullOrWhiteSpace(TbEditModel.Text))
                sb.AppendLine("• Введите модель.");
            if (!int.TryParse(TbEditMaxTDP.Text, out var maxTdp))
                sb.AppendLine("• Неверное значение TDP.");
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

            var selectedCoolingType = CbEditCoolingType.SelectedItem as string;
            var coolingTypeEntity = _context.CoolingTypes.First(ct => ct.CoolingType == selectedCoolingType);

            if (_currentCooling == null)
            {
                var newCooling = new CPUCooling
                {
                    Manufacturers = manufacturerEntity,
                    CoolingTypes = coolingTypeEntity,
                    Model = TbEditModel.Text.Trim(),
                    MaxSupportedTDP = maxTdp,
                    Price = price,
                    ImagePath = TbEditImagePath.Text.Trim()
                };
                _context.CPUCooling.Add(newCooling);
            }
            else
            {
                _currentCooling.Manufacturers = manufacturerEntity;
                _currentCooling.CoolingTypes = coolingTypeEntity;
                _currentCooling.Model = TbEditModel.Text.Trim();
                _currentCooling.MaxSupportedTDP = maxTdp;
                _currentCooling.Price = price;
                _currentCooling.ImagePath = TbEditImagePath.Text.Trim();
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

            LoadAllCooling();
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
            CbEditCoolingType.SelectedIndex = -1;
            TbEditModel.Clear();
            TbEditMaxTDP.Clear();
            TbEditPrice.Clear();
            TbEditImagePath.Clear();
            _currentCooling = null;
        }
    }
}
