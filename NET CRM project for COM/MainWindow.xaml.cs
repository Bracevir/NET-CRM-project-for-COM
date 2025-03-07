using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NET_CRM_project_for_COM.Model;
using PrinterBaseProject;

namespace CRMproject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApplicationContext _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = new ApplicationContext();
            if (!_context.DatabaseExists())
            {
                MessageBox.Show("База не существует. Создаю новую базу...");
                _context.Database.EnsureCreated(); // Создает базу данных, если она не существует
            }

            LoadOrders();
        }

        private void LoadOrders()
        {
            var orders = _context.Orders.Include(o => o.OrderItems).ToList();
            OrdersListView.ItemsSource = orders; // Устанавливаем источник данных для ListView
        }

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var orderForm = new OrderForm(_context);
            orderForm.ShowDialog();
            LoadOrders(); // Обновляем список после добавления заказа
        }

        private void OpenSearchFilterWindow_Click(object sender, RoutedEventArgs e)
        {
            var searchFilterWindow = new SearchFilterWindow();
            if (searchFilterWindow.ShowDialog() == true) // Проверяем результат диалога
            {
                ApplyFilters(searchFilterWindow); // Применяем фильтры к списку заказов
            }
        }

        private void ApplyFilters(SearchFilterWindow filter)
        {
            var filteredOrders = _context.Orders.Include(o => o.OrderItems).AsQueryable();

            if (!string.IsNullOrEmpty(filter.FileNameFilter))
            {
                filteredOrders = filteredOrders.Where(o => o.OrderItems.Any(i => i.FileName.Contains(filter.FileNameFilter)));
            }

            if (!string.IsNullOrEmpty(filter.MaterialFilter))
            {
                filteredOrders = filteredOrders.Where(o => o.OrderItems.Any(i => i.Material.Contains(filter.MaterialFilter)));
            }

            if (!string.IsNullOrEmpty(filter.EquipmentFilter))
            {
                filteredOrders = filteredOrders.Where(o => o.OrderItems.Any(i => i.Equipment.Contains(filter.EquipmentFilter)));
            }

            if (!string.IsNullOrEmpty(filter.CostFilter) && decimal.TryParse(filter.CostFilter, out decimal cost))
            {
                filteredOrders = filteredOrders.Where(o => o.OrderItems.Any(i => i.Cost == cost));
            }

            OrdersListView.ItemsSource = filteredOrders.ToList(); // Обновляем источник данных ListView
        }

        private void OrdersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OrdersListView.SelectedItem is Order selectedOrder)
            {
                var orderDetailsWindow = new OrderDetailsWindow(selectedOrder);
                orderDetailsWindow.ShowDialog();
            }
        }
        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersListView.SelectedItem is Order selectedOrder)
            {
                var orderForm = new OrderForm(_context, selectedOrder);
                orderForm.ShowDialog();
                LoadOrders(); // Обновляем список после редактирования заказа
            }
            else
            {
                MessageBox.Show("Выберите заказ для редактирования");
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersListView.SelectedItem is Order selectedOrder)
            {
                var result = MessageBox.Show("Вы согласны удалить заказ?", "Подтвердите удаление", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Orders.Remove(selectedOrder);
                    _context.SaveChanges();
                    LoadOrders(); // Обновляем список после удаления заказа
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для удаления.");
            }
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersListView.SelectedItem is Order selectedOrder)
            {
                var orderDetailsWindow = new OrderDetailsWindow(selectedOrder);
                orderDetailsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите заказ для просмотра деталей.");
            }
        }

        private void ExportToCsv_Click(object sender, RoutedEventArgs e)
        {
            // Логика экспорта в CSV
        }

        private void ImportFromCsv_Click(object sender, RoutedEventArgs e)
        {
            // Логика импорта из CSV
        }

        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            // Логика печати отчета
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("CNC Tracking System\nVersion 1.0\nDeveloped by me", "About", MessageBoxButton.OK);
        }
        private void OrdersListView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, был ли клик на элементе списка
            if (OrdersListView.SelectedItem != null)
            {
                // Получаем контекстное меню из ресурсов
                ContextMenu cm = this.FindResource("GridContextMenu") as ContextMenu;

                // Устанавливаем целевой элемент для контекстного меню
                cm.PlacementTarget = OrdersListView;
                cm.IsOpen = true; // Открываем контекстное меню
            }
        }

        private void OpenCamWindow_Click(object sender, RoutedEventArgs e)
        {
            var CAMWindow = new PrinterBaseProject.MainWindow();
            CAMWindow.Owner = this;
            CAMWindow.Show();
        }
    }
}