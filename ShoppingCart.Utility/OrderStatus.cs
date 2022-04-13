using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Utility
{
    public static class OrderStatus
    {
        public const string StatusPending = "В ожидании";
        public const string StatusRefund = "Возмещено";
        public const string StatusApproved = "Одобрено";
        public const string StatusCancelled = "Отменено";
        public const string StatusInProcess = "Недостаточная обработка";
        public const string StatusShpped = "Отправлено";
    }
}
