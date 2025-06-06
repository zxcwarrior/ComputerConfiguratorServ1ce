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
    public partial class MotherboardsPage : Page
    {
        private readonly DatabaseEntities _context;
        private Motherboards _currentMB;

        public MotherboardsPage()
        {
            InitializeComponent();
            CheckUser();
            _context = DatabaseEntities.GetContext();
            LoadAllMotherboards();
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

        private void LoadAllMotherboards()
        {
            LVMotherboards.ItemsSource = _context.Motherboards
                .Include(mb => mb.Manufacturers)
                .Include(mb => mb.Sockets)
                .Include(mb => mb.RAMTypes)
                .Include(mb => mb.MotherboardFormFactor)
                .ToList();
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LoadFilterLists()
        {
            LbManufacturers.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                LbManufacturers.Items.Add(new ListBoxItem { Content = m.ManufacturerName });

            LbSockets.Items.Clear();
            foreach (var s in _context.Sockets.OrderBy(s => s.SocketName))
                LbSockets.Items.Add(new ListBoxItem { Content = s.SocketName });
        }

        private void LoadEditCombos()
        {
            CbEditSocket.Items.Clear();
            foreach (var s in _context.Sockets.OrderBy(s => s.SocketName))
                CbEditSocket.Items.Add(s.SocketName);

            CbEditRAMType.Items.Clear();
            foreach (var rt in _context.RAMTypes.OrderBy(rt => rt.RAMType))
                CbEditRAMType.Items.Add(rt.RAMType);

            CbEditFormFactor.Items.Clear();
            foreach (var ff in _context.MotherboardFormFactor.OrderBy(ff => ff.MotherboardFFName))
                CbEditFormFactor.Items.Add(ff.MotherboardFFName);

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

            var selectedSockets = LbSockets.SelectedItems
                .Cast<ListBoxItem>()
                .Select(li => li.Content.ToString())
                .ToList();

            decimal priceMin = 0m, priceMax = decimal.MaxValue;
            if (decimal.TryParse(TbPriceMin.Text, out var pMin))
                priceMin = pMin;
            if (decimal.TryParse(TbPriceMax.Text, out var pMax))
                priceMax = pMax;

            string modelFilter = TbModelFilter.Text?.Trim().ToLower();

            var query = _context.Motherboards
                .Include(mb => mb.Manufacturers)
                .Include(mb => mb.Sockets)
                .Include(mb => mb.RAMTypes)
                .Include(mb => mb.MotherboardFormFactor)
                .AsQueryable();

            if (selectedManufacturers.Count > 0)
                query = query.Where(mb => selectedManufacturers.Contains(mb.Manufacturers.ManufacturerName));

            if (selectedSockets.Count > 0)
                query = query.Where(mb => selectedSockets.Contains(mb.Sockets.SocketName));

            query = query.Where(mb => mb.Price >= priceMin && mb.Price <= priceMax);

            if (!string.IsNullOrEmpty(modelFilter))
                query = query.Where(mb => mb.Model.ToLower().Contains(modelFilter));

            LVMotherboards.ItemsSource = query.ToList();
            FilterPanel.Visibility = Visibility.Collapsed;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LVMotherboards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isSelected = LVMotherboards.SelectedItem != null;
            BtnEdit.IsEnabled = isSelected;
            BtnDelete.IsEnabled = isSelected;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _currentMB = null;
            ClearEditFields();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LVMotherboards.SelectedItem is Motherboards selected)
            {
                _currentMB = _context.Motherboards.Find(selected.MotherboardID);
                if (_currentMB != null)
                {
                    CbEditSocket.SelectedItem = _currentMB.Sockets.SocketName;
                    CbEditRAMType.SelectedItem = _currentMB.RAMTypes.RAMType;
                    CbEditFormFactor.SelectedItem = _currentMB.MotherboardFormFactor.MotherboardFFName;
                    CbEditManufacturer.SelectedItem = CbEditManufacturer.Items
                        .OfType<ComboBoxItem>()
                        .FirstOrDefault(i => i.Content?.ToString() == _currentMB.Manufacturers.ManufacturerName);
                    TbEditModel.Text = _currentMB.Model;
                    TbEditPrice.Text = _currentMB.Price.ToString("N2");
                    TbEditImagePath.Text = _currentMB.ImagePath;
                    EditPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVMotherboards.SelectedItem is Motherboards selected)
            {
                var mbToDelete = _context.Motherboards.Find(selected.MotherboardID);
                if (mbToDelete != null)
                {
                    var result = MessageBox.Show(
                        $"Удалить материнскую плату \"{mbToDelete.Model}\"?",
                        "Подтвердите удаление",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.Motherboards.Remove(mbToDelete);
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
                        LoadAllMotherboards();
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            if (CbEditSocket.SelectedItem == null)
                sb.AppendLine("• Выберите сокет.");
            if (CbEditRAMType.SelectedItem == null)
                sb.AppendLine("• Выберите тип RAM.");
            if (CbEditFormFactor.SelectedItem == null)
                sb.AppendLine("• Выберите форм-фактор.");
            if (CbEditManufacturer.SelectedItem == null)
                sb.AppendLine("• Выберите производителя.");
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

            var selectedSocketName = CbEditSocket.SelectedItem as string;
            var socketEntity = _context.Sockets.First(s => s.SocketName == selectedSocketName);

            var selectedRAMType = CbEditRAMType.SelectedItem as string;
            var ramTypeEntity = _context.RAMTypes.First(rt => rt.RAMType == selectedRAMType);

            var selectedFormFactor = CbEditFormFactor.SelectedItem as string;
            var formFactorEntity = _context.MotherboardFormFactor.First(ff => ff.MotherboardFFName == selectedFormFactor);

            var selectedManufacturerItem = CbEditManufacturer.SelectedItem as ComboBoxItem;
            var selectedManufacturerName = selectedManufacturerItem.Content.ToString();
            var manufacturerEntity = _context.Manufacturers.First(m => m.ManufacturerName == selectedManufacturerName);

            if (_currentMB == null)
            {
                var newMB = new Motherboards
                {
                    SocketID = socketEntity.SocketID,
                    RAMTypeID = ramTypeEntity.RAMTypeID,
                    MotherboardFFID = formFactorEntity.MotherboardFFID,
                    ManufacturerID = manufacturerEntity.ManufacturerID,
                    Model = TbEditModel.Text.Trim(),
                    Price = price,
                    ImagePath = TbEditImagePath.Text.Trim()
                };
                _context.Motherboards.Add(newMB);
            }
            else
            {
                _currentMB.SocketID = socketEntity.SocketID;
                _currentMB.RAMTypeID = ramTypeEntity.RAMTypeID;
                _currentMB.MotherboardFFID = formFactorEntity.MotherboardFFID;
                _currentMB.ManufacturerID = manufacturerEntity.ManufacturerID;
                _currentMB.Model = TbEditModel.Text.Trim();
                _currentMB.Price = price;
                _currentMB.ImagePath = TbEditImagePath.Text.Trim();
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

            LoadAllMotherboards();
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
            CbEditSocket.SelectedIndex = -1;
            CbEditRAMType.SelectedIndex = -1;
            CbEditFormFactor.SelectedIndex = -1;
            CbEditManufacturer.SelectedIndex = -1;
            TbEditModel.Clear();
            TbEditPrice.Clear();
            TbEditImagePath.Clear();
            _currentMB = null;
        }
    }
}
