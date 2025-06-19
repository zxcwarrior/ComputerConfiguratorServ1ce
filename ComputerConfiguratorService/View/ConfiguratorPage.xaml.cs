using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ComputerConfiguratorService.Model;

namespace ComputerConfiguratorService.View
{
    public partial class ConfiguratorPage : Page
    {
        private readonly DatabaseEntities _ctx = DatabaseEntities.GetContext();

        // Одиночный выбор
        private CPUs selCpu;
        private Motherboards selMb;
        private GPUs selGpu;
        private Cases selCase;
        private PowerSupplies selPsu;
        private CPUCooling selCpuCooling;

        // Множественный выбор
        private readonly List<BuildCaseCooling> _caseFans = new List<BuildCaseCooling>();
        private readonly List<BuildRAMs> _ramList = new List<BuildRAMs>();
        private readonly List<BuildStorages> _storList = new List<BuildStorages>();

        // Аксессуары и услуги
        private readonly List<BuildHeadphones> _headList = new List<BuildHeadphones>();
        private readonly List<BuildKeyboards> _keyList = new List<BuildKeyboards>();
        private readonly List<BuildMouses> _mouseList = new List<BuildMouses>();
        private readonly List<BuildMonitors> _monList = new List<BuildMonitors>();
        private readonly List<BuildMicrophones> _micList = new List<BuildMicrophones>();
        private readonly List<BuildServices> _servList = new List<BuildServices>();

        public ConfiguratorPage()
        {
            InitializeComponent();
            LoadAll();
            UpdateSummary();
        }

        private void LoadAll()
        {
            LvCPU.ItemsSource = _ctx.CPUs.ToList();
            LvMB.ItemsSource = _ctx.Motherboards.ToList();
            LvGPU.ItemsSource = _ctx.GPUs.ToList();
            LvCase.ItemsSource = _ctx.Cases.ToList();
            LvPSU.ItemsSource = _ctx.PowerSupplies.ToList();
            LvCPUCooling.ItemsSource = _ctx.CPUCooling.ToList();
            LvCaseCooling.ItemsSource = _ctx.CaseCooling.ToList();
            LvRAM.ItemsSource = _ctx.RAMs.ToList();
            LvStorage.ItemsSource = _ctx.Storages.ToList();
            LvHeadphones.ItemsSource = _ctx.Headphones.ToList();
            LvKeyboards.ItemsSource = _ctx.Keyboards.ToList();
            LvMouses.ItemsSource = _ctx.Mouses.ToList();
            LvMonitors.ItemsSource = _ctx.Monitors.ToList();
            LvMicrophones.ItemsSource = _ctx.Microphones.ToList();
            LvServices.ItemsSource = _ctx.Services.ToList();
        }

        private void CollapseAllBorders()
        {
            BorderCPU.Visibility = BorderMB.Visibility = BorderGPU.Visibility =
            BorderCase.Visibility = BorderPSU.Visibility = BorderCPUCooling.Visibility =
            BorderCaseCooling.Visibility = BorderRAM.Visibility = BorderStorage.Visibility =
            BorderHeadphones.Visibility = BorderKeyboards.Visibility = BorderMouses.Visibility =
            BorderMonitors.Visibility = BorderMicrophones.Visibility = BorderServices.Visibility =
                Visibility.Collapsed;
        }

        // Панели выбора компонентов
        private void BtnToggleCPU_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderCPU.Visibility = Visibility.Visible;
            LvCPU.ItemsSource = selMb != null
                ? _ctx.CPUs.Where(c => c.SocketID == selMb.SocketID).ToList()
                : _ctx.CPUs.ToList();
        }

        private void BtnToggleMB_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderMB.Visibility = Visibility.Visible;
            if (selCpu != null)
                LvMB.ItemsSource = _ctx.Motherboards.Where(m => m.SocketID == selCpu.SocketID).ToList();
            else if (_ramList.Any())
            {
                int ramTypeId = _ramList.First().RAMs.RAMTypeID;
                LvMB.ItemsSource = _ctx.Motherboards.Where(m => m.RAMTypeID == ramTypeId).ToList();
            }
            else
                LvMB.ItemsSource = _ctx.Motherboards.ToList();
        }

        private void BtnToggleGPU_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderGPU.Visibility = Visibility.Visible;
        }

        private void BtnToggleCase_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderCase.Visibility = Visibility.Visible;
        }

        private void BtnTogglePSU_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderPSU.Visibility = Visibility.Visible;
        }

        private void BtnToggleCPUCooling_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderCPUCooling.Visibility = Visibility.Visible;
        }

        private void BtnToggleCaseCooling_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderCaseCooling.Visibility = Visibility.Visible;
        }

        private void BtnToggleRAM_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderRAM.Visibility = Visibility.Visible;
            LvRAM.ItemsSource = selMb != null
                ? _ctx.RAMs.Where(r => r.RAMTypeID == selMb.RAMTypeID).ToList()
                : _ctx.RAMs.ToList();
        }

        private void BtnToggleStorage_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderStorage.Visibility = Visibility.Visible;
        }

        private void BtnToggleHeadphones_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderHeadphones.Visibility = Visibility.Visible;
        }

        private void BtnToggleKeyboards_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderKeyboards.Visibility = Visibility.Visible;
        }

        private void BtnToggleMouses_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderMouses.Visibility = Visibility.Visible;
        }

        private void BtnToggleMonitors_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderMonitors.Visibility = Visibility.Visible;
        }

        private void BtnToggleMicrophones_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderMicrophones.Visibility = Visibility.Visible;
        }

        private void BtnToggleServices_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderServices.Visibility = Visibility.Visible;
        }

        // Обработка выбора компонента
        private void Component_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == LvCPU)
            {
                selCpu = LvCPU.SelectedItem as CPUs;
                BorderCPU.Visibility = Visibility.Collapsed;
                CheckCoolingCompatibility();
                CheckPowerCompatibility();
            }
            else if (sender == LvMB)
            {
                selMb = LvMB.SelectedItem as Motherboards;
                BorderMB.Visibility = Visibility.Collapsed;
            }
            else if (sender == LvGPU)
            {
                selGpu = LvGPU.SelectedItem as GPUs;
                BorderGPU.Visibility = Visibility.Collapsed;
                CheckPowerCompatibility();
            }
            else if (sender == LvCase)
            {
                selCase = LvCase.SelectedItem as Cases;
                BorderCase.Visibility = Visibility.Collapsed;
            }
            else if (sender == LvPSU)
            {
                selPsu = LvPSU.SelectedItem as PowerSupplies;
                BorderPSU.Visibility = Visibility.Collapsed;
                CheckPowerCompatibility();
            }
            else if (sender == LvCPUCooling)
            {
                selCpuCooling = LvCPUCooling.SelectedItem as CPUCooling;
                BorderCPUCooling.Visibility = Visibility.Collapsed;
                CheckCoolingCompatibility();
            }
            UpdateSummary();
        }

        private void CheckCoolingCompatibility()
        {
            if (selCpu != null && selCpuCooling != null &&
                selCpu.TDP > selCpuCooling.MaxSupportedTDP)
            {
                MessageBox.Show(
                    $"Несовместимость: охлаждение '{selCpuCooling.Model}' поддерживает до {selCpuCooling.MaxSupportedTDP}W, " +
                    $"а процессор '{selCpu.Model}' имеет TDP {selCpu.TDP}W.",
                    "Ошибка совместимости", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CheckPowerCompatibility()
        {
            if (selPsu != null)
            {
                decimal required = 100m;
                if (selCpu != null) required += selCpu.TDP;
                if (selGpu != null) required += selGpu.PowerConsumption;

                if (required > selPsu.Wattage)
                {
                    MessageBox.Show(
                        $"Несовместимость: блок питания '{selPsu.Model}' ({selPsu.Wattage}W) не обеспечивает {required}W.",
                        "Ошибка совместимости", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // Добавление корпусных вентиляторов (с ограничением MaxCoolers)
        private void BtnAddCaseCooling_Click(object sender, RoutedEventArgs e)
        {
            if (selCase == null)
            {
                MessageBox.Show("Сначала выберите корпус.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (LvCaseCooling.SelectedItem is CaseCooling fan &&
                int.TryParse(TbCaseCoolingQty.Text, out int qty) && qty > 0)
            {
                int maxFans = selCase.MaxCoolers;
                if (_caseFans.Sum(f => f.Quantity) + qty > maxFans)
                {
                    MessageBox.Show($"Максимум вентиляторов в корпусе: {maxFans}.", "Превышено ограничение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _caseFans.Add(new BuildCaseCooling
                {
                    BuildID = 0,
                    CaseCoolingID = fan.CaseCoolingID,
                    CaseCooling = fan,
                    Quantity = qty
                });
                BorderCaseCooling.Visibility = Visibility.Collapsed;
                UpdateSummary();
            }
        }

        // Добавление RAM (до 4 планок)
        private void BtnAddRAM_Click(object sender, RoutedEventArgs e)
        {
            if (LvRAM.SelectedItem is RAMs ram &&
                int.TryParse(TbRAMQty.Text, out int qty) && qty > 0)
            {
                if (_ramList.Sum(r => r.Quantity) + qty > 4)
                {
                    MessageBox.Show("Общее количество планок RAM не может превышать 4.", "Превышено ограничение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _ramList.Add(new BuildRAMs
                {
                    BuildID = 0,
                    RAMID = ram.RAMID,
                    RAMs = ram,
                    Quantity = qty
                });
                BorderRAM.Visibility = Visibility.Collapsed;
                UpdateSummary();
            }
        }

        // Добавление хранилищ (до 6)
        private void BtnAddStorage_Click(object sender, RoutedEventArgs e)
        {
            if (LvStorage.SelectedItem is Storages st &&
                int.TryParse(TbStorageQty.Text, out int qty) && qty > 0)
            {
                if (_storList.Sum(s => s.Quantity) + qty > 6)
                {
                    MessageBox.Show("Общее количество накопителей не может превышать 6.", "Превышено ограничение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _storList.Add(new BuildStorages
                {
                    BuildID = 0,
                    StorageID = st.StorageID,
                    Storages = st,
                    Quantity = qty
                });
                BorderStorage.Visibility = Visibility.Collapsed;
                UpdateSummary();
            }
        }

        // Остальные Add-методы (аксессуары и услуги) без ограничений
        private void BtnAddHeadphones_Click(object sender, RoutedEventArgs e)
        {
            if (LvHeadphones.SelectedItem is Headphones hp)
            {
                _headList.Add(new BuildHeadphones
                {
                    BuildID = 0,
                    HeadphoneID = hp.HeadphonesID,
                    Headphones = hp,
                });
                BorderHeadphones.Visibility = Visibility.Collapsed;
                UpdateSummary();
            }
        }

        private void BtnAddKeyboards_Click(object sender, RoutedEventArgs e)
        {
            if (LvKeyboards.SelectedItem is Keyboards kb)
            {
                _keyList.Add(new BuildKeyboards
                {
                    BuildID = 0,
                    KeyboardID = kb.KeyboardID,
                    Keyboards = kb,
                });
                BorderKeyboards.Visibility = Visibility.Collapsed;
                UpdateSummary();
            }
        }

        private void BtnAddMouses_Click(object sender, RoutedEventArgs e)
        {
            if (LvMouses.SelectedItem is Mouses mo)
            {
                _mouseList.Add(new BuildMouses
                {
                    BuildID = 0,
                    MouseID = mo.MouseID,
                    Mouses = mo,
                });
                BorderMouses.Visibility = Visibility.Collapsed;
                UpdateSummary();
            }
        }

        private void BtnAddMonitors_Click(object sender, RoutedEventArgs e)
        {
            if (LvMonitors.SelectedItem is Monitors mon)
            {
                _monList.Add(new BuildMonitors
                {
                    BuildID = 0,
                    MonitorID = mon.MonitorID,
                    Monitors = mon,
                });
                BorderMonitors.Visibility = Visibility.Collapsed;
                UpdateSummary();
            }
        }

        private void BtnAddMicrophones_Click(object sender, RoutedEventArgs e)
        {
            if (LvMicrophones.SelectedItem is Microphones mic)
            {
                _micList.Add(new BuildMicrophones
                {
                    BuildID = 0,
                    MicrophoneID = mic.MicrophoneID,
                    Microphones = mic,
                });
                BorderMicrophones.Visibility = Visibility.Collapsed;
                UpdateSummary();
            }
        }

        private void BtnAddServices_Click(object sender, RoutedEventArgs e)
        {
            if (LvServices.SelectedItem is Services sv)
            {
                _servList.Add(new BuildServices
                {
                    BuildID = 0,
                    ServiceID = sv.ServiceID,
                    Services = sv,
                });
                BorderServices.Visibility = Visibility.Collapsed;
                UpdateSummary();
            }
        }

        private void UpdateSummary()
        {
            SpSummary.Children.Clear();
            decimal total = 0m;

            void AddLine(string label, decimal price)
            {
                SpSummary.Children.Add(new TextBlock
                {
                    Text = $"{label}: ₽{price:N2}",
                    Margin = new Thickness(0, 2, 0, 2)
                });
                total += price;
            }

            if (selCpu != null) AddLine(selCpu.Model, selCpu.Price);
            if (selMb != null) AddLine(selMb.Model, selMb.Price);
            if (selGpu != null) AddLine(selGpu.Model, selGpu.Price);
            if (selCase != null) AddLine(selCase.Model, selCase.Price);
            if (selPsu != null) AddLine(selPsu.Model, selPsu.Price);
            if (selCpuCooling != null) AddLine(selCpuCooling.Model, selCpuCooling.Price);

            foreach (var cf in _caseFans)
                AddLine($"{cf.CaseCooling.Model}×{cf.Quantity}", cf.CaseCooling.Price * cf.Quantity);

            foreach (var r in _ramList)
                AddLine($"{r.RAMs.Model}×{r.Quantity}", r.RAMs.Price * r.Quantity);

            foreach (var s in _storList)
                AddLine($"{s.Storages.Model}×{s.Quantity}", s.Storages.Price * s.Quantity);

            foreach (var h in _headList)
                AddLine(h.Headphones.Model, h.Headphones.Price);

            foreach (var k in _keyList)
                AddLine(k.Keyboards.Model, k.Keyboards.Price);

            foreach (var m in _mouseList)
                AddLine(m.Mouses.Model, m.Mouses.Price);

            foreach (var m in _monList)
                AddLine(m.Monitors.Model, m.Monitors.Price);

            foreach (var m in _micList)
                AddLine(m.Microphones.Model, m.Microphones.Price);

            foreach (var s in _servList)
                AddLine(s.Services.ServiceName, s.Services.Price);

            TbTotal.Text = $"Итого: ₽{total:N2}";
        }

        private bool ValidateBuild()
        {
            if (selCpu == null || selMb == null || selGpu == null || selCase == null ||
                selPsu == null || selCpuCooling == null || !_ramList.Any() || !_storList.Any())
            {
                MessageBox.Show(
                    "Пожалуйста, выберите обязательные компоненты:\n" +
                    "процессор, материнская плата, видеокарта, корпус, блок питания,\n" +
                    "охлаждение процессора, RAM и накопители.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateBuild()) return;

            try
            {
                var build = new Builds
                {
                    BuildName = $"Сборка_{DateTime.Now:yyyyMMddHHmm}",
                    CPUID = selCpu.CPUID,
                    MotherboardID = selMb.MotherboardID,
                    GPUID = selGpu.GPUID,
                    CaseID = selCase.CaseID,
                    PowerSupplyID = selPsu.PowerSupplyID,
                    CPUCoolingID = selCpuCooling.CPUCoolingID
                };
                _ctx.Builds.Add(build);
                _ctx.SaveChanges();

                // Этап 1: RAM, Storage, CaseCooling
                foreach (var r in _ramList) { r.BuildID = build.BuildID; _ctx.BuildRAMs.Add(r); }
                foreach (var s in _storList) { s.BuildID = build.BuildID; _ctx.BuildStorages.Add(s); }
                foreach (var cf in _caseFans) { cf.BuildID = build.BuildID; _ctx.BuildCaseCooling.Add(cf); }
                _ctx.SaveChanges();

                // Этап 2: аксессуары и услуги
                foreach (var h in _headList) { h.BuildID = build.BuildID; _ctx.BuildHeadphones.Add(h); }
                foreach (var k in _keyList) { k.BuildID = build.BuildID; _ctx.BuildKeyboards.Add(k); }
                foreach (var m in _mouseList) { m.BuildID = build.BuildID; _ctx.BuildMouses.Add(m); }
                foreach (var m in _monList) { m.BuildID = build.BuildID; _ctx.BuildMonitors.Add(m); }
                foreach (var m in _micList) { m.BuildID = build.BuildID; _ctx.BuildMicrophones.Add(m); }
                foreach (var s in _servList) { s.BuildID = build.BuildID; _ctx.BuildServices.Add(s); }
                _ctx.SaveChanges();

                MessageBox.Show("Сборка и опции успешно сохранены.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении сборки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NumericOnly(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}
