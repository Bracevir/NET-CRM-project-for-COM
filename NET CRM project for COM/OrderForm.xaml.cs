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
    public partial class OrderForm : Window
    {
        private ApplicationContext _context;
        private Order _order;

        public OrderForm(ApplicationContext context, Order order = null)
        {
            InitializeComponent();
            _context = context;
            _order = order ?? new Order();

            if (_order != null)
            {
                PrefixTextBox.Text = _order.Prefix;
                LoadOrderItems();
            }
        }

        private void LoadOrderItems()
        {
            // Загружаем позиции заказа в ListView
            OrderItemsListView.ItemsSource = _order.OrderItems.ToList();
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            var orderItem = new OrderItem
            {
                FileName = FileNameTextBox.Text,
                Material = MaterialTextBox.Text,
                Equipment = EquipmentTextBox.Text,
                Cost = decimal.Parse(CostTextBox.Text),
                WorkTime = WorkTimePicker.SelectedDate ?? DateTime.Now
            };

            _order.OrderItems.Add(orderItem);
            LoadOrderItems(); // Обновляем список позиций

            // Очистка полей после добавления позиции
            FileNameTextBox.Clear();
            MaterialTextBox.Clear();
            EquipmentTextBox.Clear();
            CostTextBox.Clear();
        }

        private void SaveOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PrefixTextBox.Text) || !_order.OrderItems.Any())
            {
                MessageBox.Show("Пожалуйста, заполните префикс и добавьте минимум одно изделие.");
                return;
            }

            _order.Prefix = PrefixTextBox.Text;

            if (_order.OrderId == 0) // Новый заказ
                _context.Orders.Add(_order);

            else // Обновление существующего заказа
                _context.Orders.Update(_order);

            _context.SaveChanges();
            MessageBox.Show("Успешно сохранено!");
            this.Close();
        }
    }
}
