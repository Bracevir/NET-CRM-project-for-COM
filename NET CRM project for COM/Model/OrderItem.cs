using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_CRM_project_for_COM.Model
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public string FileName { get; set; }
        public string Material { get; set; }
        public string Equipment { get; set; }
        public decimal Cost { get; set; }
        public DateTime WorkTime { get; set; }

        // Связь с заказом
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
