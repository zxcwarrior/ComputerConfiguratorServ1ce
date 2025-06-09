using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            CbBuild.ItemsSource = _context.Builds.OrderBy(b => b.BuildName).ToList();
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
            DpDate.SelectedDate = System.DateTime.Today;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LVOrders.SelectedItem is Orders o)
            {
                _current = _context.Orders.Find(o.OrderID);
                CbBuild.SelectedItem = _current.Builds;
                DpDate.SelectedDate = _current.OrderDate;
                TbCost.Text = _current.Cost.ToString("N0");
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
            if (!decimal.TryParse(TbCost.Text, out var cost) || cost <= 0) sb.AppendLine("• Укажите корректную стоимость.");
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
            TbCost.Clear();
            _current = null;
        }

        private void NumericOnly(object sender, TextCompositionEventArgs e)
            => e.Handled = !e.Text.All(char.IsDigit);
    }
}
