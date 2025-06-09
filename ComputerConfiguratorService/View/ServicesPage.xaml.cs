using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ComputerConfiguratorService.Model;

namespace ComputerConfiguratorService.View
{
    public partial class ServicesPage : Page
    {
        private readonly DatabaseEntities _context;
        private Services _current;

        public ServicesPage()
        {
            InitializeComponent();
            _context = DatabaseEntities.GetContext();
            LoadAll();
        }

        private void LoadAll()
        {
            LVServices.ItemsSource = _context.Services
                .OrderBy(s => s.ServiceName)
                .ToList();
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = false;
        }

        private void LV_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            bool sel = LVServices.SelectedItem != null;
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = sel;
        }

        private void BtnAdd_Click(object s, RoutedEventArgs e)
        {
            _current = null;
            ClearForm();
            EditPanel.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object s, RoutedEventArgs e)
        {
            if (LVServices.SelectedItem is Services svc)
            {
                _current = _context.Services.Find(svc.ServiceID);
                TbName.Text = _current.ServiceName;
                TbPrice.Text = _current.Price.ToString("N0");
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void BtnDelete_Click(object s, RoutedEventArgs e)
        {
            if (LVServices.SelectedItem is Services svc)
            {
                var toDel = _context.Services.Find(svc.ServiceID);
                if (MessageBox.Show($"Удалить услугу «{toDel.ServiceName}»?", "Подтвердите",
                                    MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    == MessageBoxResult.Yes)
                {
                    _context.Services.Remove(toDel);
                    _context.SaveChanges();
                    LoadAll();
                }
            }
        }

        private void BtnSave_Click(object s, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(TbName.Text)) sb.AppendLine("• Введите название услуги.");
            if (!decimal.TryParse(TbPrice.Text, out var pr) || pr <= 0) sb.AppendLine("• Укажите цену.");
            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_current == null)
            {
                _current = new Services
                {
                    ServiceName = TbName.Text.Trim(),
                    Price = pr
                };
                _context.Services.Add(_current);
            }
            else
            {
                _current.ServiceName = TbName.Text.Trim();
                _current.Price = pr;
            }

            _context.SaveChanges();
            LoadAll();
            EditPanel.Visibility = Visibility.Collapsed;
            ClearForm();
        }

        private void BtnCancel_Click(object s, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
            ClearForm();
        }

        private void ClearForm()
        {
            TbName.Clear();
            TbPrice.Clear();
            _current = null;
        }

        private void NumericOnly(object sender, TextCompositionEventArgs e)
            => e.Handled = !e.Text.All(char.IsDigit);
    }
}
