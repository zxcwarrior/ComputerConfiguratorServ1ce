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
    public partial class CPUsPage : Page
    {
        private readonly DatabaseEntities _context;
        private CPUs _currentCPU;

        public CPUsPage()
        {
            InitializeComponent();
            CheckUser();
            _context = DatabaseEntities.GetContext();
            LoadAllCPUs();
            LoadSocketsIntoEdit();
        }

        private void CheckUser()
        {
            if (Manager.AuthUser == null || Manager.AuthUser.Roles.RoleID != 1)
                BtnAdminPanel.Visibility = Visibility.Collapsed;
            else
                BtnAdminPanel.Visibility = Visibility.Visible;
        }

        private void LoadAllCPUs()
        {
            LVCPUs.ItemsSource = _context.CPUs.ToList();
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LoadSocketsIntoEdit()
        {
            CbEditSocket.Items.Clear();
            foreach (var s in _context.Sockets.OrderBy(s => s.SocketName))
                CbEditSocket.Items.Add(s.SocketName);
        }

        private void BtnToggleFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterPanel.Visibility =
                FilterPanel.Visibility == Visibility.Collapsed
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void BtnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            var selectedManufacturers = LbManufacturers.SelectedItems
                .Cast<ListBoxItem>()
                .Select(li => li.Content.ToString())
                .ToList();

            decimal priceMin = 0m, priceMax = decimal.MaxValue;
            if (decimal.TryParse(TbPriceMin.Text, out var pm))
                priceMin = pm;
            if (decimal.TryParse(TbPriceMax.Text, out var pM))
                priceMax = pM;

            int coresExact = -1;
            if (int.TryParse(TbCores.Text, out var c))
                coresExact = c;

            string modelFilter = TbModelFilter.Text?.Trim().ToLower();

            var query = _context.CPUs
                .Include(cpu => cpu.Manufacturers)
                .AsQueryable();

            if (selectedManufacturers.Count > 0)
                query = query.Where(cpu =>
                    selectedManufacturers.Contains(cpu.Manufacturers.ManufacturerName));

            query = query.Where(cpu => cpu.Price >= priceMin && cpu.Price <= priceMax);

            if (coresExact > 0)
                query = query.Where(cpu => cpu.Cores == coresExact);

            if (!string.IsNullOrEmpty(modelFilter))
                query = query.Where(cpu =>
                    cpu.Model.ToLower().Contains(modelFilter));

            LVCPUs.ItemsSource = query.ToList();
            FilterPanel.Visibility = Visibility.Collapsed;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LVCPUs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isSelected = LVCPUs.SelectedItem != null;
            BtnEdit.IsEnabled = isSelected;
            BtnDelete.IsEnabled = isSelected;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _currentCPU = null;
            ClearEditFields();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LVCPUs.SelectedItem is CPUs selected)
            {
                _currentCPU = _context.CPUs.Find(selected.CPUID);
                if (_currentCPU != null)
                {
                    CbEditSocket.SelectedItem = _currentCPU.Sockets.SocketName;
                    CbEditManufacturer.SelectedItem = CbEditManufacturer.Items
                        .OfType<ComboBoxItem>()
                        .FirstOrDefault(i => i.Content?.ToString() == _currentCPU.Manufacturers.ManufacturerName);
                    TbEditModel.Text = _currentCPU.Model;
                    TbEditCores.Text = _currentCPU.Cores.ToString();
                    TbEditThreads.Text = _currentCPU.Threads.ToString();
                    TbEditBaseClock.Text = _currentCPU.BaseClock.ToString("N2");
                    TbEditBoostClock.Text = _currentCPU.BoostClock.ToString("N2");
                    TbEditTDP.Text = _currentCPU.TDP.ToString();
                    TbEditPowerConsuption.Text = _currentCPU.PowerConsumption.ToString();
                    TbEditPrice.Text = _currentCPU.Price.ToString("N2");
                    TbEditImagePath.Text = _currentCPU.ImagePath;
                    EditPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVCPUs.SelectedItem is CPUs selected)
            {
                var cpuToDelete = _context.CPUs.Find(selected.CPUID);
                if (cpuToDelete != null)
                {
                    var result = MessageBox.Show(
                        $"Удалить процессор \"{cpuToDelete.Model}\"?",
                        "Подтвердите удаление",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.CPUs.Remove(cpuToDelete);
                        _context.SaveChanges();
                        LoadAllCPUs();
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();

            if (CbEditSocket.SelectedItem == null)
                sb.AppendLine("• Выберите сокет.");
            if (CbEditManufacturer.SelectedItem == null)
                sb.AppendLine("• Выберите производителя.");
            if (string.IsNullOrWhiteSpace(TbEditModel.Text))
                sb.AppendLine("• Введите модель.");
            if (!int.TryParse(TbEditCores.Text, out var cores))
                sb.AppendLine("• Неверное число ядер.");
            if (!int.TryParse(TbEditThreads.Text, out var threads))
                sb.AppendLine("• Неверное число потоков.");
            if (!decimal.TryParse(TbEditBaseClock.Text, out var baseClock))
                sb.AppendLine("• Неверная базовая частота.");
            if (!decimal.TryParse(TbEditBoostClock.Text, out var boostClock))
                sb.AppendLine("• Неверная турбо-частота.");
            if (!int.TryParse(TbEditTDP.Text, out var tdp))
                sb.AppendLine("• Неверно указан TDP.");
            if (!int.TryParse(TbEditPowerConsuption.Text, out var powerConsuption))
                sb.AppendLine("• Неверно указан TDP.");
            if (!decimal.TryParse(TbEditPrice.Text, out var price))
                sb.AppendLine("• Неверно указана цена.");

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

            var selectedManufacturerItem = CbEditManufacturer.SelectedItem as ComboBoxItem;
            var selectedManufacturerName = selectedManufacturerItem.Content.ToString();
            var manufacturerEntity = _context.Manufacturers.First(m => m.ManufacturerName == selectedManufacturerName);

            if (_currentCPU == null)
            {
                var newCPU = new CPUs
                {
                    Sockets = socketEntity,
                    Manufacturers = manufacturerEntity,
                    Model = TbEditModel.Text.Trim(),
                    Cores = cores,
                    Threads = threads,
                    BaseClock = baseClock,
                    BoostClock = boostClock,
                    TDP = tdp,
                    PowerConsumption = powerConsuption,
                    Price = price,
                    ImagePath = TbEditImagePath.Text.Trim()
                };
                _context.CPUs.Add(newCPU);
            }
            else
            {
                _currentCPU.Sockets = socketEntity;
                _currentCPU.Manufacturers = manufacturerEntity;
                _currentCPU.Model = TbEditModel.Text.Trim();
                _currentCPU.Cores = cores;
                _currentCPU.Threads = threads;
                _currentCPU.BaseClock = baseClock;
                _currentCPU.BoostClock = boostClock;
                _currentCPU.TDP = tdp;
                _currentCPU.PowerConsumption = powerConsuption;
                _currentCPU.Price = price;
                _currentCPU.ImagePath = TbEditImagePath.Text.Trim();
            }

            _context.SaveChanges();
            LoadAllCPUs();
            EditPanel.Visibility = Visibility.Collapsed;
            ClearEditFields();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
            ClearEditFields();
        }

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void ClearEditFields()
        {
            CbEditSocket.SelectedIndex = -1;
            CbEditManufacturer.SelectedIndex = -1;
            TbEditModel.Clear();
            TbEditCores.Clear();
            TbEditThreads.Clear();
            TbEditBaseClock.Clear();
            TbEditBoostClock.Clear();
            TbEditTDP.Clear();
            TbEditPowerConsuption.Clear();
            TbEditPrice.Clear();
            TbEditImagePath.Clear();
            _currentCPU = null;
        }
    }
}
