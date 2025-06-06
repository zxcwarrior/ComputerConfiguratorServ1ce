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
    /// Логика взаимодействия для SocketsPage.xaml
    /// </summary>
    public partial class SocketsPage : Page
    {
        private Sockets selectedSocket = null;
        private bool isNewRecord = false;
        public SocketsPage()
        {
            InitializeComponent();
            LoadSockets();
        }
        private void LoadSockets()
        {
            DGSockets.ItemsSource = DatabaseEntities.GetContext().Sockets.ToList();
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            selectedSocket = null;
            isNewRecord = true;
            tbName.Text = "";
            EditPanel.Visibility = Visibility.Visible;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            selectedSocket = (sender as Button).DataContext as Sockets;
            if (selectedSocket != null)
            {
                isNewRecord = false;
                tbName.Text = selectedSocket.SocketName;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = DatabaseEntities.GetContext();
            if (isNewRecord)
            {
                Sockets newSocket = new Sockets
                {
                    SocketName = tbName.Text
                };
                context.Sockets.Add(newSocket);
            }
            else if (selectedSocket != null)
            {
                selectedSocket.SocketName = tbName.Text;
            }
            context.SaveChanges();
            LoadSockets();
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditPanel.Visibility = Visibility.Collapsed;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var socket = (sender as Button).DataContext as Sockets;
            if (socket != null && MessageBox.Show("Удалить этот сокет?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DatabaseEntities.GetContext().Sockets.Remove(socket);
                DatabaseEntities.GetContext().SaveChanges();
                LoadSockets();
            }
        }
    }
}
