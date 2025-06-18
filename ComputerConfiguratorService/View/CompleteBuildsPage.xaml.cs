// CompleteBuildsPage.xaml.cs
using ComputerConfiguratorService.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ComputerConfiguratorService.View
{
    public partial class CompleteBuildsPage : Page
    {
        private readonly DatabaseEntities _context;
        private BuildViewModel _selected;

        public CompleteBuildsPage()
        {
            InitializeComponent();
            _context = DatabaseEntities.GetContext();
            LoadBuilds();
        }

        private void LoadBuilds()
        {
            var items = _context.Builds
                .Include(b => b.Cases)
                .Include(b => b.CPUs)
                .Include(b => b.GPUs)
                .Include(b => b.Motherboards)
                .Include(b => b.PowerSupplies)
                .Include(b => b.CPUCooling)
                .Include(b => b.BuildRAMs.Select(r => r.RAMs))
                .Include(b => b.BuildStorages.Select(s => s.Storages))
                .Include(b => b.BuildCaseCooling.Select(c => c.CaseCooling))
                .Include(b => b.BuildHeadphones.Select(h => h.Headphones))
                .Include(b => b.BuildKeyboards.Select(k => k.Keyboards))
                .Include(b => b.BuildMouses.Select(m => m.Mouses))
                .Include(b => b.BuildMonitors.Select(m => m.Monitors))
                .Include(b => b.BuildMicrophones.Select(m => m.Microphones))
                .Include(b => b.BuildServices.Select(s => s.Services))
                .ToList()
                .Select(b => new BuildViewModel
                {
                    BuildID = b.BuildID,
                    BuildName = b.BuildName,
                    ImagePath = b.Cases?.ImagePath,
                    CPUs = b.CPUs,
                    Motherboard = b.Motherboards,
                    GPU = b.GPUs,
                    PSU = b.PowerSupplies,
                    CPUCool = b.CPUCooling,
                    Case = b.Cases,
                    RAMs = b.BuildRAMs.ToList(),
                    Storages = b.BuildStorages.ToList(),
                    CaseFans = b.BuildCaseCooling.ToList(),
                    Headphones = b.BuildHeadphones.ToList(),
                    Keyboards = b.BuildKeyboards.ToList(),
                    Mouses = b.BuildMouses.ToList(),
                    Monitors = b.BuildMonitors.ToList(),
                    Microphones = b.BuildMicrophones.ToList(),
                    Services = b.BuildServices.ToList()
                })
                .Select(vm =>
                {
                    vm.TotalPrice =
                        (vm.CPUs?.Price ?? 0m)
                      + (vm.Motherboard?.Price ?? 0m)
                      + (vm.GPU?.Price ?? 0m)
                      + (vm.PSU?.Price ?? 0m)
                      + (vm.CPUCool?.Price ?? 0m)
                      + (vm.Case?.Price ?? 0m)
                      + vm.RAMs.Sum(r => (r.RAMs?.Price ?? 0m) * r.Quantity)
                      + vm.Storages.Sum(s => (s.Storages?.Price ?? 0m) * s.Quantity)
                      + vm.CaseFans.Sum(c => (c.CaseCooling?.Price ?? 0m) * c.Quantity)
                      + vm.Headphones.Sum(h => (h.Headphones?.Price ?? 0m))
                      + vm.Keyboards.Sum(k => k.Keyboards?.Price ?? 0m)
                      + vm.Mouses.Sum(m => m.Mouses?.Price ?? 0m)
                      + vm.Monitors.Sum(m => m.Monitors?.Price ?? 0m)
                      + vm.Microphones.Sum(m => m.Microphones?.Price ?? 0m)
                      + vm.Services.Sum(s => s.Services?.Price ?? 0m);
                    return vm;
                })
                .ToList();

            LVBuilds.ItemsSource = items;
            BtnDelete.IsEnabled = BtnDetails.IsEnabled = false;
        }

        private void LVBuilds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = LVBuilds.SelectedItem as BuildViewModel;
            bool ok = _selected != null;
            BtnDelete.IsEnabled = BtnDetails.IsEnabled = ok;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            if (MessageBox.Show($"Удалить сборку «{_selected.BuildName}»?",
                "Подтвердите удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes) return;

            var entity = _context.Builds.Find(_selected.BuildID);
            // удаляем все связанные записи
            _context.BuildRAMs.RemoveRange(_context.BuildRAMs.Where(r => r.BuildID == _selected.BuildID));
            _context.BuildStorages.RemoveRange(_context.BuildStorages.Where(s => s.BuildID == _selected.BuildID));
            _context.BuildCaseCooling.RemoveRange(_context.BuildCaseCooling.Where(c => c.BuildID == _selected.BuildID));
            _context.BuildHeadphones.RemoveRange(_context.BuildHeadphones.Where(h => h.BuildID == _selected.BuildID));
            _context.BuildKeyboards.RemoveRange(_context.BuildKeyboards.Where(k => k.BuildID == _selected.BuildID));
            _context.BuildMouses.RemoveRange(_context.BuildMouses.Where(m => m.BuildID == _selected.BuildID));
            _context.BuildMonitors.RemoveRange(_context.BuildMonitors.Where(m => m.BuildID == _selected.BuildID));
            _context.BuildMicrophones.RemoveRange(_context.BuildMicrophones.Where(m => m.BuildID == _selected.BuildID));
            _context.BuildServices.RemoveRange(_context.BuildServices.Where(s => s.BuildID == _selected.BuildID));
            _context.Builds.Remove(entity);
            _context.SaveChanges();
            LoadBuilds();
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;

            var sb = new StringBuilder();
            sb.AppendLine($"Сборка: {_selected.BuildName} (ID {_selected.BuildID})");
            sb.AppendLine(new string('-', 40));
            void Line(string name, decimal price) => sb.AppendLine($"{name}: ₽{price:N2}");

            if (_selected.CPUs != null) Line($"Процессор: {_selected.CPUs.Model}", _selected.CPUs.Price);
            if (_selected.Motherboard != null) Line($"Мат.плата: {_selected.Motherboard.Model}", _selected.Motherboard.Price);
            if (_selected.GPU != null) Line($"Видеокарта: {_selected.GPU.Model}", _selected.GPU.Price);
            if (_selected.PSU != null) Line($"Блок питания: {_selected.PSU.Model}", _selected.PSU.Price);
            if (_selected.CPUCool != null) Line($"Охлаждение процессора: {_selected.CPUCool.Model}", _selected.CPUCool.Price);
            if (_selected.Case != null) Line($"Корпус: {_selected.Case.Model}", _selected.Case.Price);

            foreach (var r in _selected.RAMs)
                Line($"Оперативная память ×{r.Quantity}: {r.RAMs.Model}", r.RAMs.Price * r.Quantity);
            foreach (var s in _selected.Storages)
                Line($"Хранилища ×{s.Quantity}: {s.Storages.Model}", s.Storages.Price * s.Quantity);
            foreach (var c in _selected.CaseFans)
                Line($"Охлаждение корпуса ×{c.Quantity}: {c.CaseCooling.Model}", c.CaseCooling.Price * c.Quantity);

            foreach (var h in _selected.Headphones)
                Line($"Наушники : {h.Headphones.Model}", h.Headphones.Price);
            foreach (var k in _selected.Keyboards)
                Line($"Клавиатура: {k.Keyboards.Model}", k.Keyboards.Price);
            foreach (var m in _selected.Mouses)
                Line($"Мышь: {m.Mouses.Model}", m.Mouses.Price);
            foreach (var m in _selected.Monitors)
                Line($"Монитор: {m.Monitors.Model}", m.Monitors.Price);
            foreach (var m in _selected.Microphones)
                Line($"Микрофон: {m.Microphones.Model}", m.Microphones.Price);
            foreach (var s in _selected.Services)
                Line($"Услуги: {s.Services.ServiceName}", s.Services.Price);

            sb.AppendLine(new string('-', 40));
            sb.AppendLine($"Итого: ₽{_selected.TotalPrice:N2}");

            MessageBox.Show(sb.ToString(), "Подробности сборки", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private class BuildViewModel
        {
            public int BuildID { get; set; }
            public string BuildName { get; set; }
            public string ImagePath { get; set; }
            public decimal TotalPrice { get; set; }

            // Чтобы формировать «Подробнее»
            public CPUs CPUs { get; set; }
            public Motherboards Motherboard { get; set; }
            public GPUs GPU { get; set; }
            public PowerSupplies PSU { get; set; }
            public CPUCooling CPUCool { get; set; }
            public Cases Case { get; set; }
            public List<BuildRAMs> RAMs { get; set; }
            public List<BuildStorages> Storages { get; set; }
            public List<BuildCaseCooling> CaseFans { get; set; }
            public List<BuildHeadphones> Headphones { get; set; }
            public List<BuildKeyboards> Keyboards { get; set; }
            public List<BuildMouses> Mouses { get; set; }
            public List<BuildMonitors> Monitors { get; set; }
            public List<BuildMicrophones> Microphones { get; set; }
            public List<BuildServices> Services { get; set; }
        }
    }
}
