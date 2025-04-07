using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.ServiceBus.Contracts
{
    public class RabbitMqCredentials
    {
        public string Hostname
        {
            get; set;
        } = string.Empty;

        public string Username
        {
            get; set;
        } = string.Empty;

        public string Password
        {
            get; set;
        } = string.Empty;
        public string Url
        {
            get; set;
        } = string.Empty;

        public int Port
        {
            get; set;
        }
    }
}
