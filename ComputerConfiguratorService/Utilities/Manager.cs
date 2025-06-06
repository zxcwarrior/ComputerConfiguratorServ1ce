using ComputerConfiguratorService.Model;
using System.Windows.Controls;

namespace ComputerConfiguratorService.Utilities
{
    class Manager
    {
        public static Frame MainFrame { get; set; }
        public static Frame MenuFrame { get; set; }
        public static Frame DetailFrame { get; set; }
        public static Users AuthUser { get; set; }
    }
}
