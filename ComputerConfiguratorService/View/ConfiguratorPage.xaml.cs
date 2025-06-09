using ComputerConfiguratorService.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ComputerConfiguratorService.View
{
    public partial class ConfiguratorPage : Page
    {
        private readonly DatabaseEntities _context;

        private CPUs _selCpu;
        private Motherboards _selMb;
        private GPUs _selGpu;
        private PowerSupplies _selPsu;
        private Cases _selCase;

        private List<BuildRAMs> _selRams;
        private List<BuildStorages> _selStorages;
        private List<BuildCaseCooling> _selCaseFans;

        public ConfiguratorPage()
        {
            InitializeComponent();
            _context = DatabaseEntities.GetContext();

            _selRams = new List<BuildRAMs>();
            _selStorages = new List<BuildStorages>();
            _selCaseFans = new List<BuildCaseCooling>();

            LoadAll();
            UpdateSummary();
        }

        private void LoadAll()
        {
            LvCPU.ItemsSource = _context.CPUs.ToList();
            LvMB.ItemsSource = _context.Motherboards.Include(m => m.RAMTypes).ToList();
            LvGPU.ItemsSource = _context.GPUs.ToList();
            LvRAM.ItemsSource = _context.RAMs.ToList();
            LvStorage.ItemsSource = _context.Storages.ToList();
            LvCase.ItemsSource = _context.Cases.ToList();
            LvCaseCooling.ItemsSource = _context.CaseCooling.ToList();
            LvPowerSupply.ItemsSource = _context.PowerSupplies.ToList();
        }

        private void LvComponent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv == null) return;

            switch (lv.Name)
            {
                case "LvCPU":
                    _selCpu = lv.SelectedItem as CPUs;
                    break;
                case "LvMB":
                    _selMb = lv.SelectedItem as Motherboards;
                    break;
                case "LvGPU":
                    _selGpu = lv.SelectedItem as GPUs;
                    break;
                case "LvPowerSupply":
                    _selPsu = lv.SelectedItem as PowerSupplies;
                    break;
                case "LvCase":
                    _selCase = lv.SelectedItem as Cases;
                    break;
            }

            UpdateSummary();
        }

        private void BtnAddRAM_Click(object sender, RoutedEventArgs e)
        {
            RAMs ram = LvRAM.SelectedItem as RAMs;
            if (ram == null) return;
            int qty;
            if (!int.TryParse(TbRAMQty.Text, out qty) || qty < 1) return;

            BuildRAMs br = new BuildRAMs();
            br.RAMID = ram.RAMID;
            br.Quantity = qty;
            br.RAMs = ram;
            _selRams.Add(br);
            LbRAM.Items.Add(ram.Model + " ×" + qty);
            UpdateSummary();
        }

        private void BtnAddStorage_Click(object sender, RoutedEventArgs e)
        {
            Storages st = LvStorage.SelectedItem as Storages;
            if (st == null) return;
            int qty;
            if (!int.TryParse(TbStorageQty.Text, out qty) || qty < 1) return;

            BuildStorages bs = new BuildStorages();
            bs.StorageID = st.StorageID;
            bs.Quantity = qty;
            bs.Storages = st;
            _selStorages.Add(bs);
            LbStorage.Items.Add(st.Model + " ×" + qty);
            UpdateSummary();
        }

        private void BtnAddCaseCooling_Click(object sender, RoutedEventArgs e)
        {
            CaseCooling cc = LvCaseCooling.SelectedItem as CaseCooling;
            if (cc == null) return;
            int qty;
            if (!int.TryParse(TbCaseCoolingQty.Text, out qty) || qty < 1) return;

            BuildCaseCooling bc = new BuildCaseCooling();
            bc.CaseCoolingID = cc.CaseCoolingID;
            bc.Quantity = qty;
            bc.CaseCooling = cc;
            _selCaseFans.Add(bc);
            LbCaseFans.Items.Add(cc.Model + " ×" + qty);
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            SpSummary.Children.Clear();
            decimal total = 0;

            Action<string, decimal> add = (label, price) =>
            {
                TextBlock t = new TextBlock();
                t.Text = label + ": ₽" + price.ToString("N2");
                t.FontSize = 14;
                t.Margin = new Thickness(0, 2, 0, 2);
                SpSummary.Children.Add(t);
                total += price;
            };

            if (_selCpu != null) add(_selCpu.Model, _selCpu.Price);
            if (_selMb != null) add(_selMb.Model, _selMb.Price);
            if (_selGpu != null) add(_selGpu.Model, _selGpu.Price);
            if (_selPsu != null) add(_selPsu.Model, _selPsu.Price);
            if (_selCase != null) add(_selCase.Model, _selCase.Price);

            foreach (BuildRAMs r in _selRams)
                add(r.RAMs.Model + "×" + r.Quantity, r.RAMs.Price * r.Quantity);
            foreach (BuildStorages s in _selStorages)
                add(s.Storages.Model + "×" + s.Quantity, s.Storages.Price * s.Quantity);
            foreach (BuildCaseCooling f in _selCaseFans)
                add(f.CaseCooling.Model + "×" + f.Quantity, f.CaseCooling.Price * f.Quantity);

            TbTotalPrice.Text = "Итого: ₽" + total.ToString("N2");
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            _selCpu = null;
            _selMb = null;
            _selGpu = null;
            _selPsu = null;
            _selCase = null;
            _selRams.Clear();
            _selStorages.Clear();
            _selCaseFans.Clear();

            LvCPU.SelectedIndex = -1;
            LvMB.SelectedIndex = -1;
            LvGPU.SelectedIndex = -1;
            LvPowerSupply.SelectedIndex = -1;
            LvCase.SelectedIndex = -1;

            LbRAM.Items.Clear();
            LbStorage.Items.Clear();
            LbCaseFans.Items.Clear();

            UpdateSummary();
        }

        private void BtnSaveBuild_Click(object sender, RoutedEventArgs e)
        {
            if (_selCpu == null || _selMb == null || _selGpu == null || _selPsu == null || _selCase == null)
            {
                MessageBox.Show("Выберите все основные компоненты!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Builds build = new Builds();
            build.BuildName = "Сборка " + DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            build.CPUID = _selCpu.CPUID;
            build.MotherboardID = _selMb.MotherboardID;
            build.GPUID = _selGpu.GPUID;
            build.PowerSupplyID = _selPsu.PowerSupplyID;
            build.CaseID = _selCase.CaseID;

            _context.Builds.Add(build);
            _context.SaveChanges();

            foreach (BuildRAMs r in _selRams)
            {
                r.BuildID = build.BuildID;
                _context.BuildRAMs.Add(r);
            }
            foreach (BuildStorages s in _selStorages)
            {
                s.BuildID = build.BuildID;
                _context.BuildStorages.Add(s);
            }
            foreach (BuildCaseCooling f in _selCaseFans)
            {
                f.BuildID = build.BuildID;
                _context.BuildCaseCooling.Add(f);
            }

            _context.SaveChanges();
            MessageBox.Show("Сборка сохранена", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}