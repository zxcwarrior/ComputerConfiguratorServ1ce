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
    public partial class RAMsPage : Page
    {
        private readonly DatabaseEntities _context;
        private RAMs _currentRAM;

        public RAMsPage()
        {
            InitializeComponent();
            CheckUser();
            _context = DatabaseEntities.GetContext();
            LoadAllRAMs();
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

        private void LoadAllRAMs()
        {
            LVRAMs.ItemsSource = _context.RAMs
                .Include(r => r.Manufacturers)
                .Include(r => r.RAMTypes)
                .ToList();
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LoadFilterLists()
        {
            LbManufacturers.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                LbManufacturers.Items.Add(new ListBoxItem { Content = m.ManufacturerName });

            LbRAMTypes.Items.Clear();
            foreach (var rt in _context.RAMTypes.OrderBy(rt => rt.RAMType))
                LbRAMTypes.Items.Add(new ListBoxItem { Content = rt.RAMType });
        }

        private void LoadEditCombos()
        {
            CbEditManufacturer.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                CbEditManufacturer.Items.Add(new ComboBoxItem { Content = m.ManufacturerName });

            CbEditRAMType.Items.Clear();
            foreach (var rt in _context.RAMTypes.OrderBy(rt => rt.RAMType))
                CbEditRAMType.Items.Add(rt.RAMType);
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

            var selectedRAMTypes = LbRAMTypes.SelectedItems
                .Cast<ListBoxItem>()
                .Select(li => li.Content.ToString())
                .ToList();

            int capacityExact = -1;
            if (int.TryParse(TbCapacityGB.Text, out var cap))
                capacityExact = cap;

            decimal priceMin = 0m, priceMax = decimal.MaxValue;
            if (decimal.TryParse(TbPriceMin.Text, out var pMin))
                priceMin = pMin;
            if (decimal.TryParse(TbPriceMax.Text, out var pMax))
                priceMax = pMax;

            string modelFilter = TbModelFilter.Text?.Trim().ToLower();

            var query = _context.RAMs
                .Include(r => r.Manufacturers)
                .Include(r => r.RAMTypes)
                .AsQueryable();

            if (selectedManufacturers.Count > 0)
                query = query.Where(r => selectedManufacturers.Contains(r.Manufacturers.ManufacturerName));

            if (selectedRAMTypes.Count > 0)
                query = query.Where(r => selectedRAMTypes.Contains(r.RAMTypes.RAMType));

            if (capacityExact > 0)
                query = query.Where(r => r.CapacityGB == capacityExact);

            query = query.Where(r => r.Price >= priceMin && r.Price <= priceMax);

            if (!string.IsNullOrEmpty(modelFilter))
                query = query.Where(r => r.Model.ToLower().Contains(modelFilter));

            LVRAMs.ItemsSource = query.ToList();
            FilterPanel.Visibility = Visibility.Collapsed;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LVRAMs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isSelected = LVRAMs.SelectedItem != null;
            BtnEdit.IsEnabled = isSelected;
            BtnDelete.IsEnabled = isSelected;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _currentRAM = null;
            ClearEditFields();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LVRAMs.SelectedItem is RAMs selected)
            {
                _currentRAM = _context.RAMs.Find(selected.RAMID);
                if (_currentRAM != null)
                {
                    CbEditManufacturer.SelectedItem = CbEditManufacturer.Items
                        .OfType<ComboBoxItem>()
                        .FirstOrDefault(i => i.Content?.ToString() == _currentRAM.Manufacturers.ManufacturerName);

                    CbEditRAMType.SelectedItem = _currentRAM.RAMTypes.RAMType;
                    TbEditModel.Text = _currentRAM.Model;
                    TbEditCapacityGB.Text = _currentRAM.CapacityGB.ToString();
                    TbEditSpeedMHz.Text = _currentRAM.SpeedMHz.ToString();
                    TbEditPrice.Text = _currentRAM.Price.ToString("N2");
                    TbEditImagePath.Text = _currentRAM.ImagePath;
                    EditPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVRAMs.SelectedItem is RAMs selected)
            {
                var ramToDelete = _context.RAMs.Find(selected.RAMID);
                if (ramToDelete != null)
                {
                    var result = MessageBox.Show(
                        $"Удалить память \"{ramToDelete.Model}\"?",
                        "Подтвердите удаление",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.RAMs.Remove(ramToDelete);
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
                        LoadAllRAMs();
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            if (CbEditManufacturer.SelectedItem == null)
                sb.AppendLine("• Выберите производителя.");
            if (CbEditRAMType.SelectedItem == null)
                sb.AppendLine("• Выберите тип памяти.");
            if (string.IsNullOrWhiteSpace(TbEditModel.Text))
                sb.AppendLine("• Введите модель.");
            if (!int.TryParse(TbEditCapacityGB.Text, out var capacity))
                sb.AppendLine("• Неверное значение объёма.");
            if (!int.TryParse(TbEditSpeedMHz.Text, out var speed))
                sb.AppendLine("• Неверная частота.");
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

            var selectedRAMType = CbEditRAMType.SelectedItem as string;
            var ramTypeEntity = _context.RAMTypes.First(rt => rt.RAMType == selectedRAMType);

            if (_currentRAM == null)
            {
                var newRAM = new RAMs
                {
                    Manufacturers = manufacturerEntity,
                    RAMTypes = ramTypeEntity,
                    Model = TbEditModel.Text.Trim(),
                    CapacityGB = capacity,
                    SpeedMHz = speed,
                    Price = price,
                    ImagePath = TbEditImagePath.Text.Trim()
                };
                _context.RAMs.Add(newRAM);
            }
            else
            {
                _currentRAM.Manufacturers = manufacturerEntity;
                _currentRAM.RAMTypes = ramTypeEntity;
                _currentRAM.Model = TbEditModel.Text.Trim();
                _currentRAM.CapacityGB = capacity;
                _currentRAM.SpeedMHz = speed;
                _currentRAM.Price = price;
                _currentRAM.ImagePath = TbEditImagePath.Text.Trim();
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

            LoadAllRAMs();
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
            CbEditRAMType.SelectedIndex = -1;
            TbEditModel.Clear();
            TbEditCapacityGB.Clear();
            TbEditSpeedMHz.Clear();
            TbEditPrice.Clear();
            TbEditImagePath.Clear();
            _currentRAM = null;
        }
    }
}
