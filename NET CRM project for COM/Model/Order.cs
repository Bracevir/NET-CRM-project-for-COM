namespace NET_CRM_project_for_COM.Model
{
    public class Order
    {
        public int OrderId { get; set; }
        public string Prefix { get; set; } // Префикс для идентификации заказа
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Общая стоимость заказа
        public decimal TotalCost => OrderItems.Sum(item => item.Cost);
        public string TotalETE
        {
            get
            {
                // Суммируем все секунды из WorkTime
                long totalSeconds = OrderItems.Sum(item => (long)item.WorkTime.TimeOfDay.TotalSeconds);

                // Вычисляем часы, минуты и секунды
                int hours = (int)(totalSeconds / 3600);
                int minutes = (int)(totalSeconds % 3600 / 60);
                int seconds = (int)(totalSeconds % 60);

                // Форматируем строку для вывода
                return $"{hours} ч {minutes} мин {seconds} сек";
            }
        }
    }
}