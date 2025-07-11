﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Enums.Payment
{
    public enum LiftNetTransactionStatus
    {
        None = 0,
        Pending = 1, // Payment is pending
        Success = 2, // Payment was successful
        Failed = 3, // Payment failed
        Hold = 4, // Payment is on hold
    }
}
