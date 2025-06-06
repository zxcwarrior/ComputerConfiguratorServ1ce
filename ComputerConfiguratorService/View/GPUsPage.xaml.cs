using ComputerConfiguratorService.Model;
using ComputerConfiguratorService.Utilities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ComputerConfiguratorService.View
{
    public partial class GPUsPage : Page
    {
        private readonly DatabaseEntities _context;
        private GPUs _currentGPU;

        public GPUsPage()
        {
            InitializeComponent();
            CheckUser();
            _context = DatabaseEntities.GetContext();
            LoadAllGPUs();
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

        private void LoadAllGPUs()
        {
            LVGPUs.ItemsSource = _context.GPUs
                .Include(g => g.Manufacturers)
                .Include(g => g.Vendors)
                .Include(g => g.GPUMemoryTypes)
                .ToList();
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LoadFilterLists()
        {
            LbVendors.Items.Clear();
            foreach (var v in _context.Vendors.OrderBy(v => v.VendorName))
                LbVendors.Items.Add(new ListBoxItem { Content = v.VendorName });

            LbMemoryTypes.Items.Clear();
            foreach (var mt in _context.GPUMemoryTypes.OrderBy(mt => mt.MemoryType))
                LbMemoryTypes.Items.Add(new ListBoxItem { Content = mt.MemoryType });
        }

        private void LoadEditCombos()
        {
            CbEditVendor.Items.Clear();
            foreach (var v in _context.Vendors.OrderBy(v => v.VendorName))
                CbEditVendor.Items.Add(new ComboBoxItem { Content = v.VendorName });

            CbEditMemoryType.Items.Clear();
            foreach (var mt in _context.GPUMemoryTypes.OrderBy(mt => mt.MemoryType))
                CbEditMemoryType.Items.Add(mt.MemoryType);
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
            var selectedVendors = LbVendors.SelectedItems
                .Cast<ListBoxItem>()
                .Select(li => li.Content.ToString())
                .ToList();

            var selectedMemoryTypes = LbMemoryTypes.SelectedItems
                .Cast<ListBoxItem>()
                .Select(li => li.Content.ToString())
                .ToList();

            decimal priceMin = 0m, priceMax = decimal.MaxValue;
            if (decimal.TryParse(TbPriceMin.Text, out var pMin))
                priceMin = pMin;
            if (decimal.TryParse(TbPriceMax.Text, out var pMax))
                priceMax = pMax;

            int memoryExact = -1;
            if (int.TryParse(TbMemoryGB.Text, out var mGb))
                memoryExact = mGb;

            string modelFilter = TbModelFilter.Text?.Trim().ToLower();

            var query = _context.GPUs
                .Include(g => g.Manufacturers)
                .Include(g => g.Vendors)
                .Include(g => g.GPUMemoryTypes)
                .AsQueryable();

            if (selectedVendors.Count > 0)
                query = query.Where(g => selectedVendors.Contains(g.Vendors.VendorName));

            if (selectedMemoryTypes.Count > 0)
                query = query.Where(g => selectedMemoryTypes.Contains(g.GPUMemoryTypes.MemoryType));

            query = query.Where(g => g.Price >= priceMin && g.Price <= priceMax);

            if (memoryExact > 0)
                query = query.Where(g => g.MemoryGB == memoryExact);

            if (!string.IsNullOrEmpty(modelFilter))
                query = query.Where(g => g.Model.ToLower().Contains(modelFilter));

            LVGPUs.ItemsSource = query.ToList();
            FilterPanel.Visibility = Visibility.Collapsed;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LVGPUs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isSelected = LVGPUs.SelectedItem != null;
            BtnEdit.IsEnabled = isSelected;
            BtnDelete.IsEnabled = isSelected;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _currentGPU = null;
            ClearEditFields();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LVGPUs.SelectedItem is GPUs selected)
            {
                _currentGPU = _context.GPUs.Find(selected.GPUID);
                if (_currentGPU != null)
                {
                    CbEditVendor.SelectedItem = CbEditVendor.Items
                        .OfType<ComboBoxItem>()
                        .FirstOrDefault(i => i.Content?.ToString() == _currentGPU.Vendors.VendorName);

                    CbEditMemoryType.SelectedItem = _currentGPU.GPUMemoryTypes.MemoryType;
                    TbEditModel.Text = _currentGPU.Model;
                    TbEditMemoryGB.Text = _currentGPU.MemoryGB.ToString();
                    TbEditPrice.Text = _currentGPU.Price.ToString("N2");
                    TbEditImagePath.Text = _currentGPU.ImagePath;
                    EditPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVGPUs.SelectedItem is GPUs selected)
            {
                var gpuToDelete = _context.GPUs.Find(selected.GPUID);
                if (gpuToDelete != null)
                {
                    var result = MessageBox.Show(
                        $"Удалить видеокарту \"{gpuToDelete.Model}\"?",
                        "Подтвердите удаление",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.GPUs.Remove(gpuToDelete);
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
                        LoadAllGPUs();
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();

            if (CbEditVendor.SelectedItem == null)
                sb.AppendLine("• Выберите вендора.");

            if (CbEditMemoryType.SelectedItem == null)
                sb.AppendLine("• Выберите тип видеопамяти.");

            if (string.IsNullOrWhiteSpace(TbEditModel.Text))
                sb.AppendLine("• Введите модель.");

            if (!int.TryParse(TbEditMemoryGB.Text, out var memoryGB) || memoryGB <= 0)
                sb.AppendLine("• Неверный объем видеопамяти.");

            if (!int.TryParse(TbEditCoreBaseClock.Text, out var coreBaseClock) || coreBaseClock <= 0)
                sb.AppendLine("• Неверная базовая частота ядра.");

            if (!int.TryParse(TbEditGPULength.Text, out var gpuLength) || gpuLength <= 0)
                sb.AppendLine("• Неверная длина видеокарты.");

            if (!int.TryParse(TbEditPowerConsumption.Text, out var powerConsumption) || powerConsumption <= 0)
                sb.AppendLine("• Неверное потребление энергии.");

            if (!decimal.TryParse(TbEditPrice.Text, out var price) || price <= 0)
                sb.AppendLine("• Неверная цена.");

            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString().TrimEnd(), "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Сохранение данных
            if (_currentGPU == null)
            {
                var newGPU = new GPUs
                {
                    Vendors = (Vendors)CbEditVendor.SelectedItem,
                    GPUMemoryTypes = (GPUMemoryTypes)CbEditMemoryType.SelectedItem,
                    Model = TbEditModel.Text.Trim(),
                    MemoryGB = memoryGB,
                    CoreBaseClock = coreBaseClock,
                    CoreBoostClock = string.IsNullOrEmpty(TbEditCoreBoostClock.Text) ? (int?)null : int.Parse(TbEditCoreBoostClock.Text),
                    MemoryClock = string.IsNullOrEmpty(TbEditMemoryClock.Text) ? (int?)null : int.Parse(TbEditMemoryClock.Text),
                    GPULength = gpuLength,
                    PowerConsumption = powerConsumption,
                    Price = price,
                    ImagePath = TbEditImagePath.Text.Trim()
                };
                _context.GPUs.Add(newGPU);
            }
            else
            {
                _currentGPU.Vendors = (Vendors)CbEditVendor.SelectedItem;
                _currentGPU.GPUMemoryTypes = (GPUMemoryTypes)CbEditMemoryType.SelectedItem;
                _currentGPU.Model = TbEditModel.Text.Trim();
                _currentGPU.MemoryGB = memoryGB;
                _currentGPU.CoreBaseClock = coreBaseClock;
                _currentGPU.CoreBoostClock = string.IsNullOrEmpty(TbEditCoreBoostClock.Text) ? (int?)null : int.Parse(TbEditCoreBoostClock.Text);
                _currentGPU.MemoryClock = string.IsNullOrEmpty(TbEditMemoryClock.Text) ? (int?)null : int.Parse(TbEditMemoryClock.Text);
                _currentGPU.GPULength = gpuLength;
                _currentGPU.PowerConsumption = powerConsumption;
                _currentGPU.Price = price;
                _currentGPU.ImagePath = TbEditImagePath.Text.Trim();
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadAllGPUs();
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
            CbEditVendor.SelectedIndex = -1;
            CbEditMemoryType.SelectedIndex = -1;
            TbEditModel.Clear();
            TbEditMemoryGB.Clear();
            TbEditPrice.Clear();
            TbEditImagePath.Clear();
            _currentGPU = null;
        }
    }
}
