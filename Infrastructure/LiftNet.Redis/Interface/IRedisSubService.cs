using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Redis.Interface
{
    public interface IRedisSubService
    {
        Task SubscribeAsync(string channel, Action<RedisChannel, RedisValue> handler);
        Task UnsubscribeAsync(string channel);
    }
}
