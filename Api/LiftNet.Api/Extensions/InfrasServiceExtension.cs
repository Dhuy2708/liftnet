using dotenv.net;
using LiftNet.Api.Utils;
using LiftNet.AzureBlob.Services;
using LiftNet.Cloudinary.Contracts;
using LiftNet.Cloudinary.Services;
using LiftNet.Cloudinary.Services.Impl;
using LiftNet.Contract.Constants;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.CosmosDb.Contracts;
using LiftNet.CosmosDb.Services;
using LiftNet.Domain.Constants;
using LiftNet.ExerciseSDK.Core;
using LiftNet.MapSDK.Contracts;
using LiftNet.Redis.Interface;
using LiftNet.Redis.Service;
using LiftNet.ServiceBus.Contracts;
using LiftNet.ServiceBus.Core.Impl;
using LiftNet.ServiceBus.Interfaces;
using LiftNet.Timer.Service;
using Microsoft.Azure.Cosmos;
using Quartz;
using StackExchange.Redis;
using VNPAY.NET;

namespace LiftNet.Api.Extensions
{
    public static class InfrasServiceExtension
    {
        public static IServiceCollection RegisterInfras(this IServiceCollection services)
        {
            DotEnv.Load();

            #region blob
            var blobCnnStr = Environment.GetEnvironmentVariable(EnvKeys.BLOB_CONNECTION_STRING)!;
            services.AddSingleton<IBlobService>(provider =>
                                    new BlobService(provider.GetRequiredService<ILogger<BlobService>>(), blobCnnStr));
            #endregion

            #region cosmos
            var cosmosCnnStr = Environment.GetEnvironmentVariable(EnvKeys.COSMOS_CONNECTION_STRING) ?? throw new ArgumentNullException("COSMOS_CONNECTION_STRING is null.");
            var cosmosDbId = Environment.GetEnvironmentVariable(EnvKeys.COSMOS_DATABASE_ID) ?? throw new ArgumentNullException("COSMOS_DATABASE_ID is null");

            var cosmosClient = new CosmosClient(cosmosCnnStr);
            services.AddSingleton(provider => new CosmosCredential(cosmosClient, cosmosDbId));

            services.AddScoped(typeof(IIndexBaseService<>), typeof(IndexBaseService<>));
            services.AddDependencies(typeof(CosmosDb.CosmosDbAssemblyRef).Assembly);
            #endregion

            #region mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            #endregion

            #region sdk
            // province
            services.AddDependencies(typeof(ProvinceSDK.ProvinceSdkAssemblyRef).Assembly);

            // map
            services.AddSingleton(new MapApiKey
            {
                Key = Environment.GetEnvironmentVariable(EnvKeys.GOONG_MAP_API_KEY)!
            });
            services.AddDependencies(typeof(MapSDK.MapSdkAssemblyRef).Assembly);

            // exercise
            services.AddScoped(sp =>
            {
                var apiKey = Environment.GetEnvironmentVariable(EnvKeys.EXERCISE_API_KEY);
                return new ExerciseApiClient(apiKey!);
            });

            // cloudinary
            var cloudinaryKeyName = Environment.GetEnvironmentVariable(EnvKeys.CLOUDINARY_CLOUD_NAME);
            var cloudinaryApiKey = Environment.GetEnvironmentVariable(EnvKeys.CLOUDINARY_API_KEY);
            var cloudinaryApiSecret = Environment.GetEnvironmentVariable(EnvKeys.CLOUDINARY_API_SECRET);
            if (string.IsNullOrEmpty(cloudinaryKeyName) || string.IsNullOrEmpty(cloudinaryApiKey) || string.IsNullOrEmpty(cloudinaryApiSecret))
            {
                throw new ArgumentNullException("Cloudinary credentials not found.");
            }
            services.AddSingleton(x => new CloudinaryAppSetting
            {
                CloudName = cloudinaryKeyName,
                ApiKey = cloudinaryApiKey,
                ApiSecret = cloudinaryApiSecret
            });
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            #endregion

            #region quartz
            services.RegisterQuartzService();
            #endregion

            #region rabbitmq
            services.RegisterRabbitMq();
            #endregion

            #region redis 
            var redisDbHostName = Environment.GetEnvironmentVariable(EnvKeys.REDIS_HOST_NAME)!;
            var redisDbPort = Environment.GetEnvironmentVariable(EnvKeys.REDIS_PORT)!;
            var redisDbUser = Environment.GetEnvironmentVariable(EnvKeys.REDIS_USER)!;
            var redisDbPassword = Environment.GetEnvironmentVariable(EnvKeys.REDIS_PASSWORD)!;

            if (string.IsNullOrEmpty(redisDbHostName) ||
                string.IsNullOrEmpty(redisDbPort) ||
                string.IsNullOrEmpty(redisDbUser) ||
                string.IsNullOrEmpty(redisDbPassword))
            {
                throw new ArgumentNullException("Redis credentials not found or lacks info.");
            }

            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { { redisDbHostName, int.Parse(redisDbPort) } },
                User = redisDbUser,
                Password = redisDbPassword,
            };

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = configurationOptions;
            });

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(configurationOptions);
            });

            services.AddSingleton<IRedisCacheService, RedisCacheService>();
            services.AddScoped<IRedisSubService, RedisSubService>();
            #endregion

            #region vnpay
            services.RegisterVNPay();
            #endregion

            return services;
        }

        private static IServiceCollection RegisterRabbitMq(this IServiceCollection services)
        {
            string rabbitmqHostName, rabbitmqUsername, rabbitmqPassword, rabbitmqUrl, rabbitmqPort;

#if DEBUG
            rabbitmqHostName = Environment.GetEnvironmentVariable(EnvKeys.DEV_RABBITMQ_HOST_NAME)!;
            rabbitmqUsername = Environment.GetEnvironmentVariable(EnvKeys.DEV_RABBITMQ_USERNAME)!;
            rabbitmqPassword = Environment.GetEnvironmentVariable(EnvKeys.DEV_RABBITMQ_PASSWORD)!;
            rabbitmqUrl = Environment.GetEnvironmentVariable(EnvKeys.DEV_RABBITMQ_URL)!;
            rabbitmqPort = Environment.GetEnvironmentVariable(EnvKeys.DEV_RABBITMQ_PORT)!;
#else
            rabbitmqHostName = Environment.GetEnvironmentVariable(EnvKeys.TEST_RABBITMQ_HOST_NAME)!;
            rabbitmqUsername = Environment.GetEnvironmentVariable(EnvKeys.TEST_RABBITMQ_USERNAME)!;
            rabbitmqPassword = Environment.GetEnvironmentVariable(EnvKeys.TEST_RABBITMQ_PASSWORD)!;
            rabbitmqUrl = Environment.GetEnvironmentVariable(EnvKeys.TEST_RABBITMQ_URL)!;
            rabbitmqPort = Environment.GetEnvironmentVariable(EnvKeys.TEST_RABBITMQ_PORT)!;
#endif
            if (string.IsNullOrEmpty(rabbitmqHostName) || string.IsNullOrEmpty(rabbitmqUsername) || string.IsNullOrEmpty(rabbitmqPassword))
            {
                throw new ArgumentNullException("RabbitMQ credentials not found.");
            }
            services.AddSingleton(new RabbitMqCredentials
            {
                Hostname = rabbitmqHostName,
                Username = rabbitmqUsername,
                Password = rabbitmqPassword,
                Url = rabbitmqUrl,
                Port = int.Parse(rabbitmqPort)
            });
            services.AddSingleton<IEventBusService, EventBusService>();
            services.AddSingleton<IEventConsumer, EventConsumer>();
            return services;
        }

        private static IServiceCollection RegisterVNPay(this IServiceCollection services)
        {
            var tmnCode = Environment.GetEnvironmentVariable(EnvKeys.VNP_TMNCODE);
            var hashSecret = Environment.GetEnvironmentVariable(EnvKeys.VNP_HASH_SECRET);
            var sandbox = Environment.GetEnvironmentVariable(EnvKeys.VNP_SANDBOX_URL);
            var callBackUrl = Environment.GetEnvironmentVariable(EnvKeys.VNP_CALLBACK_URL);

#if DEBUG
            callBackUrl = Environment.GetEnvironmentVariable(EnvKeys.LOCAL_VNP_CALLBACK_URL);
#endif

            if (string.IsNullOrEmpty(tmnCode) || string.IsNullOrEmpty(hashSecret) || string.IsNullOrEmpty(sandbox) || string.IsNullOrEmpty(callBackUrl))
            {
                throw new ArgumentNullException("VNPay credentials not found.");
            }
            services.AddScoped<IVnpay>(x =>
            {
                var vnpay = new Vnpay();
                vnpay.Initialize(tmnCode, hashSecret, sandbox, callBackUrl);    
                return vnpay;
            });
            return services;
        }

        private static IServiceCollection RegisterQuartzService(this IServiceCollection services)
        {
#if !DEBUG
            services.AddQuartz(q =>
            {
                q.AddJob<ProvinceDiscService>(opts => opts.WithIdentity("ProvinceDiscJob", "DiscJobs"));

                q.AddTrigger(opts => opts
                    .ForJob("ProvinceDiscJob", "DiscJobs")
                    .WithIdentity("ProvinceDiscTrigger", "DiscTriggers")
                    .StartNow()
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInHours(JobIntervalHour.PROVINCE_DISC)
                        .RepeatForever()
                    )
                );
            });

            services.AddQuartz(q =>
            {
                q.AddJob<SocialScoreService>(opts => opts.WithIdentity("SocialScoreJob", "MLJobs"));

                q.AddTrigger(opts => opts
                    .ForJob("SocialScoreJob", "MLJobs")
                    .WithIdentity("SocialScoreJobTrigger", "MLTriggers")
                    .StartNow()
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInHours(JobIntervalHour.UPDATE_SOCIAL_SCORE)
                        .RepeatForever()
                    )
                );
            });

             services.AddQuartz(q =>
            {
                q.AddJob<TrainFFMModelService>(opts => opts.WithIdentity("TrainFFMModelJob", "MLJobs"));

                q.AddTrigger(opts => opts
                    .ForJob("TrainFFMModelJob", "MLJobs")
                    .WithIdentity("TrainFFMModelJobTrigger", "MLTriggers")
                    .StartNow()
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInHours(JobIntervalHour.UPDATE_SOCIAL_SCORE)
                        .RepeatForever()
                    )
                );
            });

             services.AddQuartz(q =>
            {
                q.AddJob<UpdateConfirmationRequestsService>(opts => opts.WithIdentity("UpdateConfirmationRequestsJob", "UpJobs"));

                q.AddTrigger(opts => opts
                    .ForJob("UpdateConfirmationRequestsJob", "UpJobs")
                    .WithIdentity("UpdateConfirmationRequestsTrigger", "UpTriggers")
                    .StartNow()
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInMinutes(5)
                        .RepeatForever()
                    )
                );
            });

            services.AddQuartz(q =>
            {
                q.AddJob<ExerciseDiscService>(opts => opts.WithIdentity("ExerciseDiscJob", "DiscJobs"));

                q.AddTrigger(opts => opts
                    .ForJob("ExerciseDiscJob", "DiscJobs")
                    .WithIdentity("ExerciseDiscTrigger", "DiscTriggers")
                    .StartNow()
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInHours(JobIntervalHour.EXERCISE_DISC)
                        .RepeatForever()
                    )
                );
            });
#endif



#if !DEBUG
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            services.AddTransient<TrainFFMModelService>();
            services.AddTransient<ProvinceDiscService>();
            services.AddTransient<SocialScoreService>();
            services.AddTransient<UpdateConfirmationRequestsService>();
            services.AddTransient<ExerciseDiscService>();
#endif

            return services;
        }
    }
}
