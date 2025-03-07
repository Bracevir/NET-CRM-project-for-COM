using NET_CRM_project_for_COM.Model;
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
using System.Windows.Shapes;

namespace CRMproject
{
    /// <summary>
    /// Логика взаимодействия для OrderDetailsWindow.xaml
    /// </summary>
    public partial class OrderDetailsWindow : Window
    {
        public OrderDetailsWindow(Order order)
        {
            InitializeComponent();
            PrefixTextBlock.Text = order.Prefix;
            TotalCostTextBlock.Text = order.TotalCost.ToString("C"); // Форматирование как валюты
            OrderItemsListView.ItemsSource = order.OrderItems; // Устанавливаем источник данных для ListView
        }
    }
}
