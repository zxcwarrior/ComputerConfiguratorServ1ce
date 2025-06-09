using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ComputerConfiguratorService.Model;

namespace ComputerConfiguratorService.View
{
    public partial class CompleteBuildsPage : Page
    {
        private readonly DatabaseEntities _context;
        private List<BuildViewModel> _items;

        public CompleteBuildsPage()
        {
            InitializeComponent();
            _context = DatabaseEntities.GetContext();
            LoadBuilds();
        }

        private void LoadBuilds()
        {
            _items = _context.Builds
                .Include("Cases")
                .Include("CPUs")
                .Include("GPUs")
                .Include("Motherboards")
                .Include("PowerSupplies")
                .Select(b => new BuildViewModel
                {
                    BuildID = b.BuildID,
                    BuildName = b.BuildName,
                    ImagePath = b.Cases.ImagePath,
                    TotalPrice =
                        b.CPUs.Price
                      + b.GPUs.Price
                      + b.Motherboards.Price
                      + b.PowerSupplies.Price
                      + b.Cases.Price
                      + b.BuildRAMs.Sum(r => r.RAMs.Price * r.Quantity)
                      + b.BuildStorages.Sum(s => s.Storages.Price * s.Quantity)
                      + b.BuildCaseCooling.Sum(c => c.CaseCooling.Price * c.Quantity)
                })
                .ToList();

            LVBuilds.ItemsSource = _items;
            BtnDelete.IsEnabled = false;
        }

        private void LVBuilds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnDelete.IsEnabled = LVBuilds.SelectedItem != null;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVBuilds.SelectedItem is BuildViewModel vm)
            {
                var build = _context.Builds.Find(vm.BuildID);
                if (build == null) return;

                if (MessageBox.Show($"Удалить сборку «{vm.BuildName}»?",
                    "Подтвердите удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    != MessageBoxResult.Yes) return;

                // Удаляем связанные записи
                _context.BuildRAMs.RemoveRange(_context.BuildRAMs.Where(r => r.BuildID == vm.BuildID));
                _context.BuildStorages.RemoveRange(_context.BuildStorages.Where(s => s.BuildID == vm.BuildID));
                _context.BuildCaseCooling.RemoveRange(_context.BuildCaseCooling.Where(c => c.BuildID == vm.BuildID));
                _context.Builds.Remove(build);
                _context.SaveChanges();

                LoadBuilds();
            }
        }

        private class BuildViewModel
        {
            public int BuildID { get; set; }
            public string BuildName { get; set; }
            public decimal TotalPrice { get; set; }
            public string ImagePath { get; set; }
        }
    }
}
