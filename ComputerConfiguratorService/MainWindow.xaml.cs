using System;
using System.Windows;
using System.Windows.Threading;
using ComputerConfiguratorService.View;
using ComputerConfiguratorService.Utilities;

namespace ComputerConfiguratorService
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            UpdateDateTime();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Обновление каждую секунду
            timer.Tick += Timer_Tick;
            timer.Start();

            Manager.MainFrame = MainFrame;
            BackButton.CommandTarget = Manager.MainFrame;
            Manager.MainFrame.Navigate(new AuthPage());
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            DateTimeText.Text = DateTime.Now.ToString("dd MMMM yyyy, HH:mm");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            if (Manager.MainFrame.CanGoBack)
            {
                BackButton.Visibility = Visibility.Visible;
            }
            else
            {
                BackButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
