using LiftNet.Redis.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Redis.Service
{
    public class RedisSubService : IRedisSubService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ISubscriber _subscriber;

        public RedisSubService(IConnectionMultiplexer redis)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _subscriber = _redis.GetSubscriber();
        }

        public async Task SubscribeAsync(string channel, Action<RedisChannel, RedisValue> handler)
        {
            if (string.IsNullOrEmpty(channel))
                throw new ArgumentNullException(nameof(channel));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var redisChannel = RedisChannel.Literal(channel);  
            await _subscriber.SubscribeAsync(redisChannel, (redisChannel, redisValue) =>
            {
                handler(redisChannel, redisValue);
            });
        }

        public async Task UnsubscribeAsync(string channel)
        {
            if (string.IsNullOrEmpty(channel))
                throw new ArgumentNullException(nameof(channel));

            var redisChannel = RedisChannel.Literal(channel); 
            await _subscriber.UnsubscribeAsync(redisChannel);
        }
    }
}
