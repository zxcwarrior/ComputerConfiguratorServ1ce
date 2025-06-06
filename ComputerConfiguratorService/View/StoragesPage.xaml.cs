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
    public partial class StoragesPage : Page
    {
        private readonly DatabaseEntities _context;
        private Storages _currentStorage;

        public StoragesPage()
        {
            InitializeComponent();
            CheckUser();
            _context = DatabaseEntities.GetContext();
            LoadAllStorages();
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

        private void LoadAllStorages()
        {
            LVStorages.ItemsSource = _context.Storages
                .Include(s => s.Manufacturers)
                .Include(s => s.StorageTypes)
                .ToList();
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LoadFilterLists()
        {
            LbManufacturers.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                LbManufacturers.Items.Add(new ListBoxItem { Content = m.ManufacturerName });

            LbStorageTypes.Items.Clear();
            foreach (var st in _context.StorageTypes.OrderBy(st => st.StorageType))
                LbStorageTypes.Items.Add(new ListBoxItem { Content = st.StorageType });
        }

        private void LoadEditCombos()
        {
            CbEditManufacturer.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                CbEditManufacturer.Items.Add(new ComboBoxItem { Content = m.ManufacturerName });

            CbEditStorageType.Items.Clear();
            foreach (var st in _context.StorageTypes.OrderBy(st => st.StorageType))
                CbEditStorageType.Items.Add(st.StorageType);
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

            var selectedStorageTypes = LbStorageTypes.SelectedItems
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

            var query = _context.Storages
                .Include(s => s.Manufacturers)
                .Include(s => s.StorageTypes)
                .AsQueryable();

            if (selectedManufacturers.Count > 0)
                query = query.Where(s => selectedManufacturers.Contains(s.Manufacturers.ManufacturerName));

            if (selectedStorageTypes.Count > 0)
                query = query.Where(s => selectedStorageTypes.Contains(s.StorageTypes.StorageType));

            if (capacityExact > 0)
                query = query.Where(s => s.CapacityGB == capacityExact);

            query = query.Where(s => s.Price >= priceMin && s.Price <= priceMax);

            if (!string.IsNullOrEmpty(modelFilter))
                query = query.Where(s => s.Model.ToLower().Contains(modelFilter));

            LVStorages.ItemsSource = query.ToList();
            FilterPanel.Visibility = Visibility.Collapsed;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LVStorages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isSelected = LVStorages.SelectedItem != null;
            BtnEdit.IsEnabled = isSelected;
            BtnDelete.IsEnabled = isSelected;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _currentStorage = null;
            ClearEditFields();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LVStorages.SelectedItem is Storages selected)
            {
                _currentStorage = _context.Storages.Find(selected.StorageID);
                if (_currentStorage != null)
                {
                    CbEditStorageType.SelectedItem = _currentStorage.StorageTypes.StorageType;
                    CbEditManufacturer.SelectedItem = CbEditManufacturer.Items
                        .OfType<ComboBoxItem>()
                        .FirstOrDefault(i => i.Content?.ToString() == _currentStorage.Manufacturers.ManufacturerName);
                    TbEditModel.Text = _currentStorage.Model;
                    TbEditCapacityGB.Text = _currentStorage.CapacityGB.ToString();
                    TbEditPrice.Text = _currentStorage.Price.ToString("N2");
                    TbEditImagePath.Text = _currentStorage.ImagePath;
                    EditPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVStorages.SelectedItem is Storages selected)
            {
                var storageToDelete = _context.Storages.Find(selected.StorageID);
                if (storageToDelete != null)
                {
                    var result = MessageBox.Show(
                        $"Удалить накопитель \"{storageToDelete.Model}\"?",
                        "Подтвердите удаление",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.Storages.Remove(storageToDelete);
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
                        LoadAllStorages();
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            if (CbEditManufacturer.SelectedItem == null)
                sb.AppendLine("• Выберите производителя.");
            if (CbEditStorageType.SelectedItem == null)
                sb.AppendLine("• Выберите тип.");
            if (string.IsNullOrWhiteSpace(TbEditModel.Text))
                sb.AppendLine("• Введите модель.");
            if (!int.TryParse(TbEditCapacityGB.Text, out var capacity))
                sb.AppendLine("• Неверное значение объёма.");
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

            var selectedStorageType = CbEditStorageType.SelectedItem as string;
            var storageTypeEntity = _context.StorageTypes.First(st => st.StorageType == selectedStorageType);

            if (_currentStorage == null)
            {
                var newStorage = new Storages
                {
                    Manufacturers = manufacturerEntity,
                    StorageTypes = storageTypeEntity,
                    Model = TbEditModel.Text.Trim(),
                    CapacityGB = capacity,
                    Price = price,
                    ImagePath = TbEditImagePath.Text.Trim()
                };
                _context.Storages.Add(newStorage);
            }
            else
            {
                _currentStorage.Manufacturers = manufacturerEntity;
                _currentStorage.StorageTypes = storageTypeEntity;
                _currentStorage.Model = TbEditModel.Text.Trim();
                _currentStorage.CapacityGB = capacity;
                _currentStorage.Price = price;
                _currentStorage.ImagePath = TbEditImagePath.Text.Trim();
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

            LoadAllStorages();
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
            CbEditStorageType.SelectedIndex = -1;
            TbEditModel.Clear();
            TbEditCapacityGB.Clear();
            TbEditPrice.Clear();
            TbEditImagePath.Clear();
            _currentStorage = null;
        }
    }
}
