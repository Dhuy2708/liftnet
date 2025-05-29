using LiftNet.Contract.Enums.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Wallets
{
    public class PaymentHistoryView
    {
        public string Id
        {
            get; set;
        }

        public string? TransactionId // for external payment system
        {
            get; set;
        }

        public double Amount
        {
            get; set;
        }

        public string Description
        {
            get; set;
        } = string.Empty;

        public PaymentType Type
        {
            get; set;
        } // 1: Topup, 2: Transfer, 3: Withdraw

        public int? PaymentMethod
        {
            get; set;
        }

        public PaymentStatus Status
        {
            get; set;
        }
    }
}
