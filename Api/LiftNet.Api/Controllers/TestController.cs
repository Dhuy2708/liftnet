using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using LiftNet.Domain.Response;
using LiftNet.Redis.Interface;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Api.Requests.Feeds;

namespace LiftNet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : LiftNetControllerBase
    {
        private IFeedIndexService feedService => _serviceProvider.GetRequiredService<IFeedIndexService>();
        private IRedisCacheService redisCacheService => _serviceProvider.GetRequiredService<IRedisCacheService>();
        public TestController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("cache/set")]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetCache([FromBody] CacheRequest request)
        {
            if (request.Expiration.HasValue)
            {
                await redisCacheService.SetAsync(request.Key, request.Value, request.Expiration.Value);
            }
            else
            {
                await redisCacheService.SetAsync(request.Key, request.Value);
            }
            return Ok(LiftNetRes.SuccessResponse("Cache set successfully"));
        }

        [HttpGet("cache/get/{key}")]
        [ProducesResponseType(typeof(LiftNetRes<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCache([FromRoute] string key)
        {
            var value = await redisCacheService.GetObjectAsync<string>(key);
            if (value == null)
            {
                return NotFound(LiftNetRes<string>.ErrorResponse("Key not found"));
            }
            return Ok(LiftNetRes<string>.SuccessResponse(value));
        }

        [HttpDelete("cache/delete/{key}")]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteCache([FromRoute] string key)
        {
            await redisCacheService.RemoveAsync(key);
            return Ok(LiftNetRes.SuccessResponse("Cache deleted successfully"));
        }

        [HttpGet("cache/exists/{key}")]
        [ProducesResponseType(typeof(LiftNetRes<bool>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckCacheExists([FromRoute] string key)
        {
            var exists = await redisCacheService.ExistsAsync(key);
            return Ok(LiftNetRes<bool>.SuccessResponse(exists));
        }

        [HttpPost("cache/expire/{key}")]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetCacheExpiration([FromRoute] string key, [FromBody] ExpirationRequest request)
        {
            var success = await redisCacheService.SetExpirationAsync(key, request.Expiration);
            if (!success)
            {
                return NotFound(LiftNetRes.ErrorResponse("Key not found"));
            }
            return Ok(LiftNetRes.SuccessResponse("Expiration set successfully"));
        }

        [HttpPost("feed/post")]
        public async Task<IActionResult> PostFeedAsync(TestPostFeedRequest req)
        {
            var feedRequests = new List<TestPostFeedRequest>
            {
                new TestPostFeedRequest
                {
                    Content = @"When I first started my fitness journey, I was overwhelmed by conflicting advice. I tried fad diets and extreme workout routines, but nothing seemed sustainable. Over time, I learned that fitness is less about perfection and more about consistency. It’s about showing up even on the days you don’t feel motivated. Your progress isn’t defined by one workout but by the sum of many small efforts repeated daily. Nutrition, rest, and mindset play equally important roles. Fitness taught me patience, discipline, and the importance of listening to my body. It’s a lifelong journey of learning and growth."
                },
                new TestPostFeedRequest
                {
                    Content = @"Understanding how your body responds to different types of exercise can transform your results. For example, strength training stimulates muscle growth by creating small tears in muscle fibers, which then repair and grow stronger. Cardiovascular exercises improve heart health and endurance by increasing oxygen capacity. Both are crucial, but balancing them based on your goals is key. Recovery is just as important—without adequate rest, your progress stalls and injury risk rises. Remember, fitness isn’t a race; it’s a marathon. Building a sustainable routine tailored to your lifestyle ensures long-term success."
                },
                new TestPostFeedRequest
                {
                    Content = @"I used to think working out meant hours in the gym every day. But I discovered that short, focused sessions can be just as effective if done with intensity and proper form. For instance, high-intensity interval training (HIIT) challenges your cardiovascular system and builds strength in a fraction of the time. Equally important is setting realistic goals. Instead of chasing quick fixes, aim for incremental improvements. Track your progress with measurable goals, whether it’s lifting heavier weights, running longer distances, or simply feeling more energetic. Celebrate these wins to stay motivated."
                },
                new TestPostFeedRequest
                {
                    Content = @"Mental resilience is often the unsung hero in fitness. Your mind will frequently tell you to quit long before your body reaches its limit. Training your mental toughness can help you push past plateaus and achieve breakthroughs. Techniques like visualization, positive self-talk, and mindfulness can make a huge difference. I started meditating for 10 minutes a day, which surprisingly enhanced my focus during workouts and improved recovery. Remember, physical fitness is inseparable from mental health. Cultivate both for holistic well-being."
                },
                new TestPostFeedRequest
                {
                    Content = @"One of the most valuable lessons I’ve learned is that rest and recovery are essential parts of fitness, not signs of weakness. Overtraining can lead to burnout, injuries, and stalled progress. Your muscles grow and repair during rest days, so incorporating active recovery like walking or gentle stretching keeps your body moving without strain. Sleep also plays a critical role in recovery and hormone regulation. Prioritize 7-9 hours of quality sleep per night to maximize gains. Balancing training and recovery creates a sustainable routine and long-term health benefits."
                },
                new TestPostFeedRequest
                {
                    Content = @"Fitness is not just about physical transformation but about changing your lifestyle and mindset. It teaches you discipline, time management, and resilience. Through challenges and setbacks, you learn to adapt and overcome. It has improved my confidence and energy, which positively impacts other areas of life. Whether it’s waking up early for a workout or choosing nutritious meals, these choices ripple through your day. Remember, every small decision adds up. Embrace the process and focus on becoming the best version of yourself, not just physically but mentally and emotionally."
                },
                new TestPostFeedRequest
                {
                    Content = @"Nutrition often gets overlooked in favor of workouts, but it’s the foundation of fitness. Your body needs fuel to perform and recover. Focusing on whole, minimally processed foods rich in nutrients supports muscle repair, energy production, and immune function. Experiment with your diet to find what works best—some thrive on higher protein, others on balanced macros. Avoid restrictive diets that cause stress or fatigue. Instead, adopt sustainable habits like meal prepping and mindful eating. Hydration is another crucial piece—drink plenty of water daily to aid digestion and muscle function."
                },
                new TestPostFeedRequest
                {
                    Content = @"Tracking your fitness journey through journals or apps can significantly boost motivation and accountability. Recording workouts, nutrition, and how you feel after training helps identify patterns and areas for improvement. Celebrate small victories like hitting a new personal best or consistently working out for a week. Use setbacks as learning opportunities, not reasons to quit. The journey is full of ups and downs, but maintaining perspective and staying committed makes all the difference."
                },
                new TestPostFeedRequest
                {
                    Content = @"Working out with a community or partner provides support, encouragement, and fun. I found that joining group classes or having a workout buddy increased my consistency and enjoyment. Sharing goals and challenges makes the journey less lonely and helps you stay accountable. Plus, friendly competition can push you to perform better. If you’re new to fitness, find local groups or online communities that align with your interests. The social aspect can be as rewarding as the physical gains."
                },
                new TestPostFeedRequest
                {
                    Content = @"Fitness is deeply personal and what works for one person may not for another. Don’t compare your progress or routine to others. Focus on your body’s signals and preferences. Experiment with different types of exercise—from yoga and pilates to weightlifting and running—until you find what you enjoy. Enjoyment is crucial for long-term adherence. The goal is a balanced routine that fits your lifestyle and brings you joy, not just results on the scale."
                },
                new TestPostFeedRequest
                {
                    Content = @"Strength training changed my life. I started with just bodyweight exercises but soon realized the importance of progressive overload to build real strength. Lifting weights not only reshaped my body but also boosted my metabolism and confidence. The key is to focus on compound movements like squats, deadlifts, and presses, which work multiple muscle groups and provide the best return on investment. Don’t be afraid to lift heavy with proper form, and always prioritize recovery. The gains you make will surprise you."
                },
                new TestPostFeedRequest
                {
                    Content = @"Cardio can be intimidating if you’re new, but it’s one of the best ways to improve heart health and burn calories. I recommend starting with walking or cycling and gradually increasing intensity. Mixing steady-state cardio with HIIT workouts can keep things interesting and maximize fat loss. Remember, cardio is only one piece of the puzzle; pairing it with strength training and proper nutrition yields the best overall results."
                },
                new TestPostFeedRequest
                {
                    Content = @"Flexibility and mobility are often neglected but are essential for long-term fitness and injury prevention. Incorporate stretching routines or yoga into your weekly plan to improve range of motion and reduce muscle tightness. Good mobility supports better form during lifts and reduces the risk of joint pain. Taking just 10-15 minutes daily for mobility work can greatly enhance your performance and recovery."
                },
                new TestPostFeedRequest
                {
                    Content = @"Supplements can support your fitness goals but shouldn’t replace a balanced diet. I use protein powder to meet my daily protein needs on busy days and occasionally take omega-3s for joint health. Before trying any supplement, research its benefits and consult a healthcare professional. Focus primarily on nutrient-dense foods and hydration first."
                },
                new TestPostFeedRequest
                {
                    Content = @"One major lesson is that mental health and fitness are intertwined. Regular exercise helped reduce my anxiety and improved my sleep quality. The endorphin release after workouts creates a natural mood boost. Even on tough days, moving your body—even for a short walk—can positively impact your mindset. Never underestimate the psychological benefits of physical activity."
                },
                new TestPostFeedRequest
                {
                    Content = @"Setting a routine that fits your lifestyle is crucial. I used to try morning workouts but found evenings worked better for my energy levels and schedule. Listen to your body and adjust accordingly. Consistency beats intensity if you want sustainable results. Find times that work and make workouts a non-negotiable part of your day."
                },
                new TestPostFeedRequest
                {
                    Content = @"Injuries are setbacks, but they don’t have to end your fitness journey. When I injured my shoulder, I focused on rehab exercises, adjusted my routine, and practiced patience. This experience taught me to listen more carefully to my body and incorporate preventive exercises. Rest and proper form are crucial for avoiding injuries."
                },
                new TestPostFeedRequest
                {
                    Content = @"Celebrate progress beyond the scale. Increased strength, better endurance, improved mood, and confidence are all indicators of success. Fitness is multifaceted. Avoid getting discouraged by minor weight fluctuations or perceived plateaus. Embrace the holistic benefits and keep pushing forward."
                },
                new TestPostFeedRequest
                {
                    Content = @"Remember, every fitness journey is unique. Focus on your progress and enjoy the process. Celebrate the small wins, learn from challenges, and keep your eyes on long-term health and happiness. Fitness is more than aesthetics; it’s about living your best life."
                }
            };

            var postTasks = feedRequests.Select(feed =>
                    feedService.PostFeedAsync(UserId, feed.Content, feed.Medias ?? new List<string>())
                );

            await Task.WhenAll(postTasks);
            return Ok();
        }
    }

    public class CacheRequest
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public TimeSpan? Expiration { get; set; }
    }

    public class ExpirationRequest
    {
        public TimeSpan Expiration { get; set; }
    }
}
