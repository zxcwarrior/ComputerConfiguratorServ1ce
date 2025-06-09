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
            BtnAdminPanel.Visibility = (Manager.AuthUser?.Roles.RoleID == 1)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void LoadAllGPUs()
        {
            LVGPUs.ItemsSource = _context.GPUs
                .Include(g => g.Manufacturers)
                .Include(g => g.Vendors)
                .Include(g => g.GPUMemoryTypes)
                .ToList();
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = false;
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
            // Производители GPU
            CbEditManufacturer.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                CbEditManufacturer.Items.Add(new ComboBoxItem { Content = m.ManufacturerName, Tag = m.ManufacturerID });

            // Вендоры
            CbEditVendor.Items.Clear();
            foreach (var v in _context.Vendors.OrderBy(v => v.VendorName))
                CbEditVendor.Items.Add(new ComboBoxItem { Content = v.VendorName, Tag = v.VendorID });

            // Типы памяти
            CbEditMemoryType.Items.Clear();
            foreach (var mt in _context.GPUMemoryTypes.OrderBy(mt => mt.MemoryType))
                CbEditMemoryType.Items.Add(mt.MemoryType);
        }

        private void BtnToggleFilter_Click(object sender, RoutedEventArgs e)
            => FilterPanel.Visibility =
                FilterPanel.Visibility == Visibility.Collapsed
                    ? Visibility.Visible
                    : Visibility.Collapsed;

        private void NumericOnly(object sender, TextCompositionEventArgs e)
            => e.Handled = !e.Text.All(char.IsDigit);

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
            if (decimal.TryParse(TbPriceMin.Text, out var pMin)) priceMin = pMin;
            if (decimal.TryParse(TbPriceMax.Text, out var pMax)) priceMax = pMax;

            int memoryExact = -1;
            if (int.TryParse(TbMemoryGB.Text, out var mGb)) memoryExact = mGb;

            string modelFilter = TbModelFilter.Text?.Trim().ToLower();

            var query = _context.GPUs
                .Include(g => g.Manufacturers)
                .Include(g => g.Vendors)
                .Include(g => g.GPUMemoryTypes)
                .AsQueryable();

            if (selectedVendors.Any())
                query = query.Where(g => selectedVendors.Contains(g.Vendors.VendorName));

            if (selectedMemoryTypes.Any())
                query = query.Where(g => selectedMemoryTypes.Contains(g.GPUMemoryTypes.MemoryType));

            query = query.Where(g => g.Price >= priceMin && g.Price <= priceMax);

            if (memoryExact > 0)
                query = query.Where(g => g.MemoryGB == memoryExact);

            if (!string.IsNullOrEmpty(modelFilter))
                query = query.Where(g => g.Model.ToLower().Contains(modelFilter));

            LVGPUs.ItemsSource = query.ToList();
            FilterPanel.Visibility = Visibility.Collapsed;
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = false;
        }

        private void LVGPUs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool sel = LVGPUs.SelectedItem != null;
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = sel;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _currentGPU = null;
            ClearEditFields();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LVGPUs.SelectedItem is GPUs sel)
            {
                _currentGPU = _context.GPUs.Find(sel.GPUID);
                if (_currentGPU != null)
                {
                    // Manufacturer
                    CbEditManufacturer.SelectedItem = CbEditManufacturer.Items
                        .OfType<ComboBoxItem>()
                        .FirstOrDefault(i => (int)i.Tag == _currentGPU.ManufacturerID);
                    // Vendor
                    CbEditVendor.SelectedItem = CbEditVendor.Items
                        .OfType<ComboBoxItem>()
                        .FirstOrDefault(i => (int)i.Tag == _currentGPU.VendorID);
                    // Memory Type
                    CbEditMemoryType.SelectedItem = _currentGPU.GPUMemoryTypes.MemoryType;

                    TbEditModel.Text = _currentGPU.Model;
                    TbEditMemoryGB.Text = _currentGPU.MemoryGB.ToString();
                    TbEditCoreBaseClock.Text = _currentGPU.CoreBaseClock.ToString();
                    TbEditCoreBoostClock.Text = _currentGPU.CoreBoostClock?.ToString() ?? "";
                    TbEditMemoryClock.Text = _currentGPU.MemoryClock?.ToString() ?? "";
                    TbEditGPULength.Text = _currentGPU.GPULength.ToString();
                    TbEditPowerConsumption.Text = _currentGPU.PowerConsumption.ToString();
                    TbEditPrice.Text = _currentGPU.Price.ToString("N2");
                    TbEditImagePath.Text = _currentGPU.ImagePath;

                    EditPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVGPUs.SelectedItem is GPUs sel)
            {
                var gpu = _context.GPUs.Find(sel.GPUID);
                if (gpu != null &&
                    MessageBox.Show($"Удалить «{gpu.Model}»?", "Подтвердите",
                                    MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    == MessageBoxResult.Yes)
                {
                    _context.GPUs.Remove(gpu);
                    _context.SaveChanges();
                    LoadAllGPUs();
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            if (CbEditManufacturer.SelectedItem == null) sb.AppendLine("• Выберите производителя.");
            if (CbEditVendor.SelectedItem == null) sb.AppendLine("• Выберите вендора.");
            if (CbEditMemoryType.SelectedItem == null) sb.AppendLine("• Выберите тип памяти.");
            if (string.IsNullOrWhiteSpace(TbEditModel.Text)) sb.AppendLine("• Введите модель.");
            if (!int.TryParse(TbEditMemoryGB.Text, out var mem) || mem <= 0) sb.AppendLine("• Неверный объём.");
            if (!int.TryParse(TbEditCoreBaseClock.Text, out var baseClk) || baseClk <= 0) sb.AppendLine("• Неверная базовая частота.");
            if (!int.TryParse(TbEditGPULength.Text, out var length) || length <= 0) sb.AppendLine("• Неверная длина.");
            if (!int.TryParse(TbEditPowerConsumption.Text, out var power) || power <= 0) sb.AppendLine("• Неверное энергопотребление.");
            if (!decimal.TryParse(TbEditPrice.Text, out var pr) || pr <= 0) sb.AppendLine("• Неверная цена.");

            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // manufacturer
            var manItem = CbEditManufacturer.SelectedItem as ComboBoxItem;
            var manId = (int)manItem.Tag;
            var manEnt = _context.Manufacturers.Find(manId);

            // vendor
            var venItem = CbEditVendor.SelectedItem as ComboBoxItem;
            var venId = (int)venItem.Tag;
            var venEnt = _context.Vendors.Find(venId);

            // memory type
            var memType = CbEditMemoryType.SelectedItem as string;
            var memEnt = _context.GPUMemoryTypes.First(mt => mt.MemoryType == memType);

            if (_currentGPU == null)
            {
                var g = new GPUs
                {
                    ManufacturerID = manId,
                    Vendors = venEnt,
                    VendorID = venId,
                    GPUMemoryTypeID = memEnt.GPUMemoryTypeID,
                    Model = TbEditModel.Text.Trim(),
                    MemoryGB = mem,
                    CoreBaseClock = baseClk,
                    CoreBoostClock = int.TryParse(TbEditCoreBoostClock.Text, out var cb) ? cb : (int?)null,
                    MemoryClock = int.TryParse(TbEditMemoryClock.Text, out var mc) ? mc : (int?)null,
                    GPULength = length,
                    PowerConsumption = power,
                    Price = pr,
                    ImagePath = TbEditImagePath.Text.Trim()
                };
                _context.GPUs.Add(g);
            }
            else
            {
                _currentGPU.ManufacturerID = manId;
                _currentGPU.VendorID = venId;
                _currentGPU.GPUMemoryTypeID = memEnt.GPUMemoryTypeID;
                _currentGPU.Model = TbEditModel.Text.Trim();
                _currentGPU.MemoryGB = mem;
                _currentGPU.CoreBaseClock = baseClk;
                _currentGPU.CoreBoostClock = int.TryParse(TbEditCoreBoostClock.Text, out var cb2) ? cb2 : (int?)null;
                _currentGPU.MemoryClock = int.TryParse(TbEditMemoryClock.Text, out var mc2) ? mc2 : (int?)null;
                _currentGPU.GPULength = length;
                _currentGPU.PowerConsumption = power;
                _currentGPU.Price = pr;
                _currentGPU.ImagePath = TbEditImagePath.Text.Trim();
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                var msg = new StringBuilder();
                msg.AppendLine("Ошибка при сохранении:");
                msg.AppendLine(ex.Message);
                if (ex.InnerException != null)
                {
                    msg.AppendLine("Внутренняя ошибка:");
                    msg.AppendLine(ex.InnerException.Message);
                }
                MessageBox.Show(msg.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            CbEditManufacturer.SelectedIndex = -1;
            CbEditVendor.SelectedIndex = -1;
            CbEditMemoryType.SelectedIndex = -1;
            TbEditModel.Clear();
            TbEditMemoryGB.Clear();
            TbEditCoreBaseClock.Clear();
            TbEditCoreBoostClock.Clear();
            TbEditMemoryClock.Clear();
            TbEditGPULength.Clear();
            TbEditPowerConsumption.Clear();
            TbEditPrice.Clear();
            TbEditImagePath.Clear();
            _currentGPU = null;
        }
    }
}
