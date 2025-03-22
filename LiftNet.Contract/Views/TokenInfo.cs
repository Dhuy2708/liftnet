using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views
{
    public class TokenInfo
    {
        public string Token
        {
            get; set;
        }
        public DateTime ExpiresAt
        {
            get; set;
        }
    }
}
