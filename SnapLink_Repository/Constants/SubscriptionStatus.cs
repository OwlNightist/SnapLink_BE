using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapLink_Repository.Constants
{
    public static class SubscriptionStatus
    {
        public const string Active = "Active";
        public const string Canceled = "Canceled";
        public const string Expired = "Expired";
        // Nếu muốn mở rộng thêm:
        public const string PendingPayment = "PendingPayment";
        public const string Suspended = "Suspended";

    }
}
