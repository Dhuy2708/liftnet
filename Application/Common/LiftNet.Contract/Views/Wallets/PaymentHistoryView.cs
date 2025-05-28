using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Wallets
{
    public class PaymentHistoryView
    {
        public string TransactionId
        {
            get; set;
        }

        public double Amount
        {
            get; set;
        }
    }
}
