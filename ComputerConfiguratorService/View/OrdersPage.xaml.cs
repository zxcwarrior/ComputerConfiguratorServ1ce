using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
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
                .Include(o => o.Builds)
                .OrderBy(o => o.OrderID)
                .ToList();
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = BtnDetails.IsEnabled = false;
        }

        private void LoadBuilds()
        {
            CbBuild.ItemsSource = _context.Builds
                .OrderBy(b => b.BuildName)
                .ToList();
        }

        private void LVOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool sel = LVOrders.SelectedItem is Orders o;
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = BtnDetails.IsEnabled = sel;
            _current = sel ? (Orders)LVOrders.SelectedItem : null;
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
            if (_current == null) return;
            CbBuild.SelectedItem = _current.Builds;
            DpDate.SelectedDate = _current.OrderDate;
            TbCost.Text = _current.Cost.ToString("N2");
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;
            if (MessageBox.Show($"Удалить заказ №{_current.OrderID}?", "Подтвердите удаление",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes) return;

            _context.Orders.Remove(_current);
            _context.SaveChanges();
            LoadAll();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            if (CbBuild.SelectedItem == null) sb.AppendLine("• Выберите сборку.");
            if (!DpDate.SelectedDate.HasValue) sb.AppendLine("• Укажите дату.");
            if (!decimal.TryParse(TbCost.Text, out decimal cost) || cost <= 0)
                sb.AppendLine("• Стоимость не рассчитана или некорректна.");

            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var build = (Builds)CbBuild.SelectedItem;
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
                // например, наценка 7% на сборку
                var orderCost = buildCost * 1.07m + servicesAndAccessoriesCost;
                TbCost.Text = orderCost.ToString("N2");
            }
            else
            {
                TbCost.Text = string.Empty;
            }
        }

        private decimal CalculateBuildCost(int buildId)
        {
            var b = _context.Builds
                .Include(x => x.CPUs)
                .Include(x => x.Motherboards)
                .Include(x => x.BuildRAMs.Select(r => r.RAMs))
                .Include(x => x.CPUCooling)
                .Include(x => x.GPUs)
                .Include(x => x.PowerSupplies)
                .Include(x => x.BuildStorages.Select(s => s.Storages))
                .Include(x => x.Cases)
                .Include(x => x.BuildCaseCooling.Select(c => c.CaseCooling))
                .AsNoTracking()
                .FirstOrDefault(x => x.BuildID == buildId);
            if (b == null) return 0m;

            return (b.CPUs?.Price ?? 0m)
                 + (b.Motherboards?.Price ?? 0m)
                 + b.BuildRAMs.Sum(r => (r.RAMs?.Price ?? 0m) * r.Quantity)
                 + (b.CPUCooling?.Price ?? 0m)
                 + (b.GPUs?.Price ?? 0m)
                 + (b.PowerSupplies?.Price ?? 0m)
                 + b.BuildStorages.Sum(s => (s.Storages?.Price ?? 0m) * s.Quantity)
                 + (b.Cases?.Price ?? 0m)
                 + b.BuildCaseCooling.Sum(c => (c.CaseCooling?.Price ?? 0m) * c.Quantity);
        }

        private decimal CalculateServicesAndAccessoriesCost(int buildId)
        {
            var b = _context.Builds
                .Include(x => x.BuildServices.Select(s => s.Services))
                .Include(x => x.BuildKeyboards.Select(k => k.Keyboards))
                .Include(x => x.BuildMouses.Select(m => m.Mouses))
                .Include(x => x.BuildHeadphones.Select(h => h.Headphones))
                .Include(x => x.BuildMicrophones.Select(m => m.Microphones))
                .Include(x => x.BuildMonitors.Select(m => m.Monitors))
                .AsNoTracking()
                .FirstOrDefault(x => x.BuildID == buildId);
            if (b == null) return 0m;

            return b.BuildServices.Sum(s => s.Services?.Price ?? 0m)
                 + b.BuildKeyboards.Sum(k => k.Keyboards?.Price ?? 0m)
                 + b.BuildMouses.Sum(m => m.Mouses?.Price ?? 0m)
                 + b.BuildHeadphones.Sum(h => h.Headphones?.Price ?? 0m)
                 + b.BuildMicrophones.Sum(m => m.Microphones?.Price ?? 0m)
                 + b.BuildMonitors.Sum(m => m.Monitors?.Price ?? 0m);
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {

            if (_current == null) return;

            // Загрузим полностью запись заказа с навигацией
            var order = _context.Orders
                .Include(o => o.Builds.CPUs)
                .Include(o => o.Builds.Motherboards)
                .Include(o => o.Builds.GPUs)
                .Include(o => o.Builds.PowerSupplies)
                .Include(o => o.Builds.CPUCooling)
                .Include(o => o.Builds.Cases)
                .Include(o => o.Builds.BuildRAMs.Select(r => r.RAMs))
                .Include(o => o.Builds.BuildStorages.Select(s => s.Storages))
                .Include(o => o.Builds.BuildCaseCooling.Select(c => c.CaseCooling))
                .Include(o => o.Builds.BuildServices.Select(s => s.Services))
                .Include(o => o.Builds.BuildKeyboards.Select(k => k.Keyboards))
                .Include(o => o.Builds.BuildMouses.Select(m => m.Mouses))
                .Include(o => o.Builds.BuildHeadphones.Select(h => h.Headphones))
                .Include(o => o.Builds.BuildMicrophones.Select(m => m.Microphones))
                .Include(o => o.Builds.BuildMonitors.Select(m => m.Monitors))
                .FirstOrDefault(o => o.OrderID == _current.OrderID);

            if (order == null) return;

            decimal baseCost = CalculateBuildCost(order.BuildID);
            decimal serviceFee = baseCost * 0.07m;

            var sb = new StringBuilder();
            sb.AppendLine($"Заказ #{order.OrderID}");
            sb.AppendLine($"Дата: {order.OrderDate:dd.MM.yyyy}");
            sb.AppendLine($"Стоимость сборки ПК: ₽{serviceFee:N2}");
            sb.AppendLine($"Итоговая стоимость: ₽{order.Cost:N2}");
            sb.AppendLine(new string('-', 40));
            sb.AppendLine("Состав сборки:");
            void Line(string name, decimal price) => sb.AppendLine($"  • {name}: ₽{price:N2}");

            var b = order.Builds;
            if (b.CPUs != null) Line($"Процессор {b.CPUs.Model}", b.CPUs.Price);
            if (b.Motherboards != null) Line($"Материнская плата {b.Motherboards.Model}", b.Motherboards.Price);
            if (b.GPUs != null) Line($"Видеокарта {b.GPUs.Model}", b.GPUs.Price);
            if (b.PowerSupplies != null) Line($"Блок питания {b.PowerSupplies.Model}", b.PowerSupplies.Price);
            if (b.CPUCooling != null) Line($"Охлаждение CPU {b.CPUCooling.Model}", b.CPUCooling.Price);
            if (b.Cases != null) Line($"Корпус {b.Cases.Model}", b.Cases.Price);

            foreach (var r in b.BuildRAMs)
                Line($"RAM ×{r.Quantity} {r.RAMs.Model}", r.RAMs.Price * r.Quantity);
            foreach (var s in b.BuildStorages)
                Line($"Storage ×{s.Quantity} {s.Storages.Model}", s.Storages.Price * s.Quantity);
            foreach (var c in b.BuildCaseCooling)
                Line($"CaseFan ×{c.Quantity} {c.CaseCooling.Model}", c.CaseCooling.Price * c.Quantity);

            sb.AppendLine("Аксессуары и услуги:");
            foreach (var svc in order.Builds.BuildServices)
                Line($"Услуга {svc.Services.ServiceName}", svc.Services.Price);
            foreach (var kbd in order.Builds.BuildKeyboards)
                Line($"Клавиатура {kbd.Keyboards.Model}", kbd.Keyboards.Price);
            foreach (var ms in order.Builds.BuildMouses)
                Line($"Мышь {ms.Mouses.Model}", ms.Mouses.Price);
            foreach (var hp in order.Builds.BuildHeadphones)
                Line($"Наушники {hp.Headphones.Model}", hp.Headphones.Price);
            foreach (var mic in order.Builds.BuildMicrophones)
                Line($"Микрофон {mic.Microphones.Model}", mic.Microphones.Price);
            foreach (var mon in order.Builds.BuildMonitors)
                Line($"Монитор {mon.Monitors.Model}", mon.Monitors.Price);

            MessageBox.Show(sb.ToString(), "Детали заказа", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
