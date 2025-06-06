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
    public partial class CasesPage : Page
    {
        private readonly DatabaseEntities _context;
        private Cases _currentCase;

        public CasesPage()
        {
            InitializeComponent();
            CheckUser();
            _context = DatabaseEntities.GetContext();
            LoadAllCases();
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

        private void LoadAllCases()
        {
            LVCases.ItemsSource = _context.Cases
                .Include(c => c.Manufacturers)
                .Include(c => c.CaseFormFactors)
                .ToList();
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LoadFilterLists()
        {
            LbManufacturers.Items.Clear();
            foreach (var m in _context.Manufacturers.OrderBy(m => m.ManufacturerName))
                LbManufacturers.Items.Add(new ListBoxItem { Content = m.ManufacturerName });

            LbCaseFFs.Items.Clear();
            foreach (var ff in _context.CaseFormFactors.OrderBy(ff => ff.CaseFFName))
                LbCaseFFs.Items.Add(new ListBoxItem { Content = ff.CaseFFName });
        }

        private void LoadEditCombos()
        {
            CbEditCaseFF.Items.Clear();
            foreach (var ff in _context.CaseFormFactors.OrderBy(ff => ff.CaseFFName))
                CbEditCaseFF.Items.Add(ff.CaseFFName);

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

            var selectedCaseFFs = LbCaseFFs.SelectedItems
                .Cast<ListBoxItem>()
                .Select(li => li.Content.ToString())
                .ToList();

            decimal priceMin = 0m, priceMax = decimal.MaxValue;
            if (decimal.TryParse(TbPriceMin.Text, out var pMin))
                priceMin = pMin;
            if (decimal.TryParse(TbPriceMax.Text, out var pMax))
                priceMax = pMax;

            string modelFilter = TbModelFilter.Text?.Trim().ToLower();

            var query = _context.Cases
                .Include(c => c.Manufacturers)
                .Include(c => c.CaseFormFactors)
                .AsQueryable();

            if (selectedManufacturers.Count > 0)
                query = query.Where(c => selectedManufacturers.Contains(c.Manufacturers.ManufacturerName));

            if (selectedCaseFFs.Count > 0)
                query = query.Where(c => selectedCaseFFs.Contains(c.CaseFormFactors.CaseFFName));

            query = query.Where(c => c.Price >= priceMin && c.Price <= priceMax);

            if (!string.IsNullOrEmpty(modelFilter))
                query = query.Where(c => c.Model.ToLower().Contains(modelFilter));

            LVCases.ItemsSource = query.ToList();
            FilterPanel.Visibility = Visibility.Collapsed;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void LVCases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isSelected = LVCases.SelectedItem != null;
            BtnEdit.IsEnabled = isSelected;
            BtnDelete.IsEnabled = isSelected;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _currentCase = null;
            ClearEditFields();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LVCases.SelectedItem is Cases selected)
            {
                _currentCase = _context.Cases.Find(selected.CaseID);
                if (_currentCase != null)
                {
                    CbEditCaseFF.SelectedItem = _currentCase.CaseFormFactors.CaseFFName;
                    CbEditManufacturer.SelectedItem = CbEditManufacturer.Items
                        .OfType<ComboBoxItem>()
                        .FirstOrDefault(i => i.Content?.ToString() == _currentCase.Manufacturers.ManufacturerName);
                    TbEditModel.Text = _currentCase.Model;
                    TbEditMaxGPULength.Text = _currentCase.MaxGPULength.ToString();
                    TbEditMaxCoolers.Text = _currentCase.MaxCoolers.ToString();
                    TbEditPrice.Text = _currentCase.Price.ToString("N2");
                    TbEditImagePath.Text = _currentCase.ImagePath;
                    EditPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVCases.SelectedItem is Cases selected)
            {
                var caseToDelete = _context.Cases.Find(selected.CaseID);
                if (caseToDelete != null)
                {
                    var result = MessageBox.Show(
                        $"Удалить корпус \"{caseToDelete.Model}\"?",
                        "Подтвердите удаление",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.Cases.Remove(caseToDelete);
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
                        LoadAllCases();
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            if (CbEditCaseFF.SelectedItem == null)
                sb.AppendLine("• Выберите форм-фактор.");
            if (CbEditManufacturer.SelectedItem == null)
                sb.AppendLine("• Выберите производителя.");
            if (string.IsNullOrWhiteSpace(TbEditModel.Text))
                sb.AppendLine("• Введите модель.");
            if (!int.TryParse(TbEditMaxGPULength.Text, out var gpuLen))
                sb.AppendLine("• Неверная макс. длина GPU.");
            if (!int.TryParse(TbEditMaxCoolers.Text, out var coolers))
                sb.AppendLine("• Неверное число кулеров.");
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

            var selectedCaseFF = CbEditCaseFF.SelectedItem as string;
            var caseFFEntity = _context.CaseFormFactors.First(ff => ff.CaseFFName == selectedCaseFF);

            var selectedManufacturerItem = CbEditManufacturer.SelectedItem as ComboBoxItem;
            var selectedManufacturerName = selectedManufacturerItem.Content.ToString();
            var manufacturerEntity = _context.Manufacturers.First(m => m.ManufacturerName == selectedManufacturerName);

            if (_currentCase == null)
            {
                var newCase = new Cases
                {
                    CaseFFID = caseFFEntity.CaseFFID,
                    ManufacturerID = manufacturerEntity.ManufacturerID,
                    Model = TbEditModel.Text.Trim(),
                    MaxGPULength = gpuLen,
                    MaxCoolers = coolers,
                    Price = price,
                    ImagePath = TbEditImagePath.Text.Trim()
                };
                _context.Cases.Add(newCase);
            }
            else
            {
                _currentCase.CaseFFID = caseFFEntity.CaseFFID;
                _currentCase.ManufacturerID = manufacturerEntity.ManufacturerID;
                _currentCase.Model = TbEditModel.Text.Trim();
                _currentCase.MaxGPULength = gpuLen;
                _currentCase.MaxCoolers = coolers;
                _currentCase.Price = price;
                _currentCase.ImagePath = TbEditImagePath.Text.Trim();
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

            LoadAllCases();
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
            CbEditCaseFF.SelectedIndex = -1;
            CbEditManufacturer.SelectedIndex = -1;
            TbEditModel.Clear();
            TbEditMaxGPULength.Clear();
            TbEditMaxCoolers.Clear();
            TbEditPrice.Clear();
            TbEditImagePath.Clear();
            _currentCase = null;
        }
    }
}
