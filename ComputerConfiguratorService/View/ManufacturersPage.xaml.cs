using ComputerConfiguratorService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ComputerConfiguratorService.View
{
    /// <summary>
    /// Логика взаимодействия для ManufacturersPage.xaml
    /// </summary>
    public partial class ManufacturersPage : Page
    {
        private Manufacturers selectedManufacturer = null;
        private bool isNewRecord = false;
        public ManufacturersPage()
        {
            InitializeComponent();
            LoadManufacturers();
        }
        // Загрузка данных в DataGrid
        private void LoadManufacturers()
        {
            DGManufacturers.ItemsSource = DatabaseEntities.GetContext().Manufacturers.ToList();
        }

        // Кнопка "Добавить новый"
        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            selectedManufacturer = null; // Ничего не выбрано
            isNewRecord = true; // Это новая запись
            tbName.Text = ""; // Поле для названия пустое
            EditPanel.Visibility = Visibility.Visible; // Показываем форму
        }

        // Кнопка "Редактировать"
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            selectedManufacturer = (sender as Button).DataContext as Manufacturers;
            if (selectedManufacturer != null)
            {
                isNewRecord = false; // Это редактирование
                tbName.Text = selectedManufacturer.ManufacturerName;
                EditPanel.Visibility = Visibility.Visible; // Показываем форму
            }
        }

        // Кнопка "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = DatabaseEntities.GetContext();
            if (isNewRecord) // Новая запись
            {
                Manufacturers newManufacturer = new Manufacturers
                {
                    ManufacturerName = tbName.Text
                };
                context.Manufacturers.Add(newManufacturer);
            }
            else if (selectedManufacturer != null) // Редактирование
            {
                selectedManufacturer.ManufacturerName = tbName.Text;
            }
            context.SaveChanges(); // Сохраняем в базу
            LoadManufacturers(); // Обновляем DataGrid
            EditPanel.Visibility = Visibility.Collapsed; // Скрываем форму
        }

        // Кнопка "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed; // Просто закрываем
        }

        // Кнопка "Удалить"
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var manufacturer = (sender as Button).DataContext as Manufacturers;
            if (manufacturer != null && MessageBox.Show("Удалить этого производителя?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DatabaseEntities.GetContext().Manufacturers.Remove(manufacturer);
                DatabaseEntities.GetContext().SaveChanges();
                LoadManufacturers();
            }
        }
    }
}
