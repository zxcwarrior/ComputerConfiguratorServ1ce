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
            BorderCPU.Visibility = Visibility.Collapsed;
            BorderMB.Visibility = Visibility.Collapsed;
            BorderGPU.Visibility = Visibility.Collapsed;
            BorderCase.Visibility = Visibility.Collapsed;
            BorderPSU.Visibility = Visibility.Collapsed;
            BorderCPUCooling.Visibility = Visibility.Collapsed;
            BorderCaseCooling.Visibility = Visibility.Collapsed;
            BorderRAM.Visibility = Visibility.Collapsed;
            BorderStorage.Visibility = Visibility.Collapsed;
            BorderHeadphones.Visibility = Visibility.Collapsed;
            BorderKeyboards.Visibility = Visibility.Collapsed;
            BorderMouses.Visibility = Visibility.Collapsed;
            BorderMonitors.Visibility = Visibility.Collapsed;
            BorderMicrophones.Visibility = Visibility.Collapsed;
            BorderServices.Visibility = Visibility.Collapsed;
        }

        private void BtnToggleCPU_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderCPU.Visibility = Visibility.Visible;
            if (selMb != null)
            {
                LvCPU.ItemsSource = _ctx.CPUs.Where(c => c.Sockets.SocketID == selMb.Sockets.SocketID).ToList();
            }
            else
            {
                LvCPU.ItemsSource = _ctx.CPUs.ToList();
            }
        }

        private void BtnToggleMB_Click(object sender, RoutedEventArgs e)
        {
            CollapseAllBorders();
            BorderMB.Visibility = Visibility.Visible;
            if (selCpu != null)
            {
                LvMB.ItemsSource = _ctx.Motherboards.Where(m => m.Sockets.SocketID == selCpu.Sockets.SocketID).ToList();
            }
            else if (_ramList.Any())
            {
                var ramType = _ramList.First().RAMs.RAMTypes.RAMTypeID;
                LvMB.ItemsSource = _ctx.Motherboards.Where(m => m.RAMTypes.RAMTypeID == ramType).ToList();
            }
            else
            {
                LvMB.ItemsSource = _ctx.Motherboards.ToList();
            }
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
            if (selMb != null)
            {
                LvRAM.ItemsSource = _ctx.RAMs.Where(r => r.RAMTypes.RAMTypeID == selMb.RAMTypes.RAMTypeID).ToList();
            }
            else
            {
                LvRAM.ItemsSource = _ctx.RAMs.ToList();
            }
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
            if (selCpu != null && selCpuCooling != null)
            {
                if (selCpu.TDP > selCpuCooling.MaxSupportedTDP)
                {
                    MessageBox.Show($"Несовместимость: Охлаждение '{selCpuCooling.Model}' (макс. {selCpuCooling.MaxSupportedTDP} Вт) " +
                                    $"не справится с процессором '{selCpu.Model}' (TDP {selCpu.TDP} Вт).",
                                    "Ошибка совместимости", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void CheckPowerCompatibility()
        {
            if (selPsu != null && (selCpu != null || selGpu != null))
            {
                decimal totalWattage = 100m; // Фиксированная мощность для остальных компонентов
                if (selCpu != null) totalWattage += (decimal)selCpu.PowerConsumption;
                if (selGpu != null) totalWattage += selGpu.PowerConsumption;

                if (totalWattage > selPsu.Wattage)
                {
                    MessageBox.Show($"Несовместимость: Блок питания '{selPsu.Model}' ({selPsu.Wattage} Вт) " +
                                    $"не обеспечивает достаточную мощность ({totalWattage} Вт требуется).",
                                    "Ошибка совместимости", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnAddCaseCooling_Click(object sender, RoutedEventArgs e)
        {
            var fan = LvCaseCooling.SelectedItem as CaseCooling;
            if (fan == null) return;
            if (!int.TryParse(TbCaseCoolingQty.Text, out int qty) || qty < 1) return;
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

        private void BtnAddRAM_Click(object sender, RoutedEventArgs e)
        {
            var ram = LvRAM.SelectedItem as RAMs;
            if (ram == null) return;
            if (!int.TryParse(TbRAMQty.Text, out int qty) || qty < 1) return;
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

        private void BtnAddStorage_Click(object sender, RoutedEventArgs e)
        {
            var st = LvStorage.SelectedItem as Storages;
            if (st == null) return;
            if (!int.TryParse(TbStorageQty.Text, out int qty) || qty < 1) return;
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

        private void BtnAddHeadphones_Click(object sender, RoutedEventArgs e)
        {
            var hp = LvHeadphones.SelectedItem as Headphones;
            if (hp == null) return;
            _headList.Add(new BuildHeadphones
            {
                BuildID = 0,
                HeadphoneID = hp.HeadphonesID,
                Headphones = hp
            });
            BorderHeadphones.Visibility = Visibility.Collapsed;
            UpdateSummary();
        }

        private void BtnAddKeyboards_Click(object sender, RoutedEventArgs e)
        {
            var kb = LvKeyboards.SelectedItem as Keyboards;
            if (kb == null) return;
            _keyList.Add(new BuildKeyboards
            {
                BuildID = 0,
                KeyboardID = kb.KeyboardID,
                Keyboards = kb
            });
            BorderKeyboards.Visibility = Visibility.Collapsed;
            UpdateSummary();
        }

        private void BtnAddMouses_Click(object sender, RoutedEventArgs e)
        {
            var mo = LvMouses.SelectedItem as Mouses;
            if (mo == null) return;
            _mouseList.Add(new BuildMouses
            {
                BuildID = 0,
                MouseID = mo.MouseID,
                Mouses = mo
            });
            BorderMouses.Visibility = Visibility.Collapsed;
            UpdateSummary();
        }

        private void BtnAddMonitors_Click(object sender, RoutedEventArgs e)
        {
            var mn = LvMonitors.SelectedItem as Monitors;
            if (mn == null) return;
            _monList.Add(new BuildMonitors
            {
                BuildID = 0,
                MonitorID = mn.MonitorID,
                Monitors = mn
            });
            BorderMonitors.Visibility = Visibility.Collapsed;
            UpdateSummary();
        }

        private void BtnAddMicrophones_Click(object sender, RoutedEventArgs e)
        {
            var mc = LvMicrophones.SelectedItem as Microphones;
            if (mc == null) return;
            _micList.Add(new BuildMicrophones
            {
                BuildID = 0,
                MicrophoneID = mc.MicrophoneID,
                Microphones = mc
            });
            BorderMicrophones.Visibility = Visibility.Collapsed;
            UpdateSummary();
        }

        private void BtnAddServices_Click(object sender, RoutedEventArgs e)
        {
            var sv = LvServices.SelectedItem as Services;
            if (sv == null) return;
            _servList.Add(new BuildServices
            {
                BuildID = 0,
                ServiceID = sv.ServiceID,
                Services = sv
            });
            BorderServices.Visibility = Visibility.Collapsed;
            UpdateSummary();
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
                selPsu == null || selCpuCooling == null || _ramList.Count == 0 || _storList.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите все обязательные компоненты (процессор, материнская плата, видеокарта, корпус, блок питания, охлаждение процессора, оперативная память и накопители).",
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
                    BuildName = $"Сборка {DateTime.Now:yyyyMMddHHmm}",
                    CPUID = selCpu.CPUID,
                    MotherboardID = selMb.MotherboardID,
                    GPUID = selGpu.GPUID,
                    CaseID = selCase.CaseID,
                    PowerSupplyID = selPsu.PowerSupplyID,
                    CPUCoolingID = selCpuCooling.CPUCoolingID
                };
                _ctx.Builds.Add(build);
                _ctx.SaveChanges();

                foreach (var r in _ramList)
                {
                    r.BuildID = build.BuildID;
                    _ctx.BuildRAMs.Add(r);
                }

                foreach (var s in _storList)
                {
                    s.BuildID = build.BuildID;
                    _ctx.BuildStorages.Add(s);
                }

                foreach (var cf in _caseFans)
                {
                    cf.BuildID = build.BuildID;
                    _ctx.BuildCaseCooling.Add(cf);
                }

                foreach (var h in _headList) { h.BuildID = build.BuildID; _ctx.BuildHeadphones.Add(h); }
                foreach (var k in _keyList) { k.BuildID = build.BuildID; _ctx.BuildKeyboards.Add(k); }
                foreach (var m in _mouseList) { m.BuildID = build.BuildID; _ctx.BuildMouses.Add(m); }
                foreach (var m in _monList) { m.BuildID = build.BuildID; _ctx.BuildMonitors.Add(m); }
                foreach (var m in _micList) { m.BuildID = build.BuildID; _ctx.BuildMicrophones.Add(m); }
                foreach (var s in _servList) { s.BuildID = build.BuildID; _ctx.BuildServices.Add(s); }

                _ctx.SaveChanges();
                MessageBox.Show("Сборка и все опции успешно сохранены.",
                    "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении сборки: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NumericOnly(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}