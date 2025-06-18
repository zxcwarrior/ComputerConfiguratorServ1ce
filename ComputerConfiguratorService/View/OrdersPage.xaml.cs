using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity; // Для EF6
using ComputerConfiguratorService.Model;

namespace ComputerConfiguratorService.View
{
    public partial class OrdersPage : Page
    {
        private readonly DatabaseEntities _context;
        private Orders _current;

        public OrdersPage()
        {
            InitializeComponent();
            _context = DatabaseEntities.GetContext();
            LoadAll();
            LoadBuilds();
            CbBuild.SelectionChanged += CbBuild_SelectionChanged;
        }

        private void LoadAll()
        {
            LVOrders.ItemsSource = _context.Orders
                .Include("Builds")
                .OrderBy(o => o.OrderID)
                .ToList();
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = false;
        }

        private void LoadBuilds()
        {
            CbBuild.ItemsSource = _context.Builds
                .OrderBy(b => b.BuildName)
                .ToList();
        }

        private void LVOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool sel = LVOrders.SelectedItem != null;
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = sel;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _current = null;
            ClearForm();
            EditPanel.Visibility = Visibility.Visible;
            DpDate.SelectedDate = DateTime.Today;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LVOrders.SelectedItem is Orders o)
            {
                _current = _context.Orders.Find(o.OrderID);
                CbBuild.SelectedItem = _current.Builds;
                DpDate.SelectedDate = _current.OrderDate;
                TbCost.Text = _current.Cost.ToString("N2");
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LVOrders.SelectedItem is Orders o)
            {
                var toDel = _context.Orders.Find(o.OrderID);
                if (MessageBox.Show($"Удалить заказ #{toDel.OrderID}?", "Подтвердите",
                                    MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    == MessageBoxResult.Yes)
                {
                    _context.Orders.Remove(toDel);
                    _context.SaveChanges();
                    LoadAll();
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            if (CbBuild.SelectedItem == null) sb.AppendLine("• Выберите сборку.");
            if (!DpDate.SelectedDate.HasValue) sb.AppendLine("• Укажите дату.");
            if (!decimal.TryParse(TbCost.Text, out decimal cost) || cost <= 0)
            {
                sb.AppendLine("• Стоимость не рассчитана или некорректна.");
            }

            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var build = CbBuild.SelectedItem as Builds;
            var date = DpDate.SelectedDate.Value;

            if (_current == null)
            {
                _current = new Orders
                {
                    BuildID = build.BuildID,
                    OrderDate = date,
                    Cost = cost
                };
                _context.Orders.Add(_current);
            }
            else
            {
                _current.BuildID = build.BuildID;
                _current.OrderDate = date;
                _current.Cost = cost;
            }

            _context.SaveChanges();
            LoadAll();
            EditPanel.Visibility = Visibility.Collapsed;
            ClearForm();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
            ClearForm();
        }

        private void ClearForm()
        {
            CbBuild.SelectedIndex = -1;
            DpDate.SelectedDate = null;
            TbCost.Text = string.Empty;
            _current = null;
        }

        private void CbBuild_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbBuild.SelectedItem is Builds build)
            {
                var buildCost = CalculateBuildCost(build.BuildID);
                var servicesAndAccessoriesCost = CalculateServicesAndAccessoriesCost(build.BuildID);
                var orderCost = (buildCost * 1.07m) + servicesAndAccessoriesCost;
                TbCost.Text = orderCost.ToString("N2");
            }
            else
            {
                TbCost.Text = string.Empty;
            }
        }

        private decimal CalculateBuildCost(int buildId)
        {
            var build = _context.Builds
                .Include("CPUs")
                .Include("Motherboards")
                .Include("BuildRAMs.RAMs")
                .Include("CPUCooling")
                .Include("GPUs")
                .Include("PowerSupplies")
                .Include("BuildStorages.Storages")
                .Include("Cases")
                .Include("BuildCaseCooling.CaseCooling")
                .AsNoTracking()
                .FirstOrDefault(b => b.BuildID == buildId);

            if (build == null) return 0m;

            return
                (build.CPUs?.Price ?? 0m)
              + (build.Motherboards?.Price ?? 0m)
              + (build.BuildRAMs.Any() ? build.BuildRAMs.Sum(r => (r.RAMs?.Price ?? 0m) * r.Quantity) : 0m)
              + (build.CPUCooling?.Price ?? 0m)
              + (build.GPUs?.Price ?? 0m)
              + (build.PowerSupplies?.Price ?? 0m)
              + (build.BuildStorages.Any() ? build.BuildStorages.Sum(s => (s.Storages?.Price ?? 0m) * s.Quantity) : 0m)
              + (build.Cases?.Price ?? 0m)
              + (build.BuildCaseCooling.Any() ? build.BuildCaseCooling.Sum(c => (c.CaseCooling?.Price ?? 0m) * c.Quantity) : 0m);
        }

        private decimal CalculateServicesAndAccessoriesCost(int buildId)
        {
            var build = _context.Builds
                .Include("BuildServices.Services")
                .Include("BuildKeyboards.Keyboards")
                .Include("BuildMouses.Mouses")
                .Include("BuildHeadphones.Headphones")
                .Include("BuildMicrophones.Microphones")
                .Include("BuildMonitors.Monitors")
                .AsNoTracking()
                .FirstOrDefault(b => b.BuildID == buildId);

            if (build == null) return 0m;

            return
                (build.BuildServices.Any() ? build.BuildServices.Sum(s => s.Services?.Price ?? 0m) : 0m)
              + (build.BuildKeyboards.Any() ? build.BuildKeyboards.Sum(k => k.Keyboards?.Price ?? 0m) : 0m)
              + (build.BuildMouses.Any() ? build.BuildMouses.Sum(m => m.Mouses?.Price ?? 0m) : 0m)
              + (build.BuildHeadphones.Any() ? build.BuildHeadphones.Sum(h => h.Headphones?.Price ?? 0m) : 0m)
              + (build.BuildMicrophones.Any() ? build.BuildMicrophones.Sum(m => m.Microphones?.Price ?? 0m) : 0m)
              + (build.BuildMonitors.Any() ? build.BuildMonitors.Sum(m => m.Monitors?.Price ?? 0m) : 0m);
        }
    }
}