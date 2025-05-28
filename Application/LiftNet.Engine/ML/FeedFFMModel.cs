using LiftNet.Contract.Constants;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Engine.Data.Normalized;
using LiftNet.Engine.Data.Vectorized;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System.Collections.Generic;

namespace LiftNet.Engine.ML
{
    internal class FeedFFMModel
    {
        private readonly MLContext _mlContext;
        private readonly IBlobService _blobService;
        private ITransformer _model;
        private PredictionEngine<VectorizedFeedFFMData, FeedFFMPrediction> _predictionEngine;
        private const double MAX_DISTANCE_KM = 50.0;
        private const string MODEL_FILE = "feed_ffm_model.zip";

        public FeedFFMModel(IBlobService blobService)
        {
            _mlContext = new MLContext(seed: 1);
            _blobService = blobService;
            LoadModelAsync().Wait();
        }

        public async Task Train(IEnumerable<NormalizedUserFeedFeature> trainingData)
        {
            var trainData = _mlContext.Data.LoadFromEnumerable(trainingData.Select(ConvertToFFMData));

            var pipeline = _mlContext.Transforms.NormalizeMinMax(
                     new[]
                     {
                        new InputOutputColumnPair("UserId"),
                        new InputOutputColumnPair("FeedId"),
                        new InputOutputColumnPair("UserAgeRangeOneHot"),
                        new InputOutputColumnPair("UserRoleOneHot"),
                        new InputOutputColumnPair("UserGenderOneHot"),
                        new InputOutputColumnPair("LocationProximity"),
                        new InputOutputColumnPair("HasLocation"),
                        new InputOutputColumnPair("HasAge"),
                        new InputOutputColumnPair("HasGender"),
                        new InputOutputColumnPair("FeedTimeAgo")
                     })
                 .Append(_mlContext.BinaryClassification.Trainers.FieldAwareFactorizationMachine(
                     new FieldAwareFactorizationMachineTrainer.Options
                     {
                         FeatureColumnName = "UserId",
                         ExtraFeatureColumns = new[]
                         {
                            "FeedId",
                            "UserRoleOneHot",
                            "UserGenderOneHot",
                            "UserAgeRangeOneHot",
                            "LocationProximity",
                            "HasLocation",
                            "HasAge",
                            "HasGender",
                            "FeedTimeAgo"
                         },
                         LabelColumnName = "Label",
                         NumberOfIterations = 20,
                         LearningRate = 0.01f,
                         LambdaLinear = 0.0001f,
                         LambdaLatent = 0.0001f
                     }));

            _model = pipeline.Fit(trainData);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<VectorizedFeedFFMData, FeedFFMPrediction>(_model);
            await SaveModelAsync();
        }

        public async Task TrainIncremental(IEnumerable<NormalizedUserFeedFeature> newData)
        {
            if (_model == null)
            {
                await Train(newData);
                return;
            }

            var trainData = _mlContext.Data.LoadFromEnumerable(newData.Select(ConvertToFFMData));
            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    "UserAgeRangeOneHot", "UserRoleOneHot", "UserGenderOneHot",
                    "LocationProximity", "HasLocation", "HasAge", "HasGender",
                    "FeedLikes", "FeedTimeAgo")
                .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(_mlContext.BinaryClassification.Trainers.FieldAwareFactorizationMachine(
                    new FieldAwareFactorizationMachineTrainer.Options
                    {
                        FeatureColumnName = "Features",
                        LabelColumnName = "Label",
                        NumberOfIterations = 10,
                        LearningRate = 0.005f,
                        LambdaLinear = 0.0001f,
                        LambdaLatent = 0.0001f
                    }));

            _model = pipeline.Fit(trainData);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<VectorizedFeedFFMData, FeedFFMPrediction>(_model);
            await SaveModelAsync();
        }

        public float Predict(NormalizedUserFeedFeature feature)
        {
            var input = ConvertToFFMData(feature);
            var prediction = _predictionEngine.Predict(input);
            return prediction.Score;
        }

        private async Task SaveModelAsync()
        {
            if (_model == null) return;

            using var memoryStream = new MemoryStream();
            _mlContext.Model.Save(_model, null, memoryStream);
            memoryStream.Position = 0;

            var containerClient = await _blobService.GetContainerClient(BlobContainerName.FFM_MODEL);
            var blobClient = containerClient.GetBlobClient(MODEL_FILE);
            await blobClient.UploadAsync(memoryStream, true);
        }

        private async Task LoadModelAsync()
        {
            try
            {
                var modelBytes = await _blobService.DownloadFileAsync(MODEL_FILE, BlobContainerName.FFM_MODEL);
                using var memoryStream = new MemoryStream(modelBytes);
                _model = _mlContext.Model.Load(memoryStream, out var _);
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<VectorizedFeedFFMData, FeedFFMPrediction>(_model);
            }
            catch (FileNotFoundException)
            {
                // Model doesn't exist yet, that's okay
            }
        }

        private VectorizedFeedFFMData ConvertToFFMData(NormalizedUserFeedFeature feature)
        {
            var hasLocation = feature.User.NormalizedLat != 0 || feature.User.NormalizedLng != 0 ? 1.0f : 0.0f;
            var hasAge = feature.User.AgeRangeOneHot.Any(x => x > 0) ? 1.0f : 0.0f;
            var hasGender = feature.User.GenderOneHot.Any(x => x > 0) ? 1.0f : 0.0f;

            return new VectorizedFeedFFMData
            {
                UserId = new[] { HashStringToFloat(feature.User.UserId) },
                FeedId = new[] { HashStringToFloat(feature.Feed.FeedId) },
                UserAgeRangeOneHot = feature.User.AgeRangeOneHot,
                UserRoleOneHot = feature.User.RoleOneHot,
                UserGenderOneHot = feature.User.GenderOneHot,
                LocationProximity = new[] { hasLocation > 0 ? CalculateLocationProximity(
                    feature.User.NormalizedLat,
                    feature.User.NormalizedLng) : 0.0f },
                HasLocation = new[] { hasLocation },
                HasAge = new[] { hasAge },
                HasGender = new[] { hasGender },
                FeedTimeAgo = new[] { feature.Feed.NormalizedTimeAgo },
                Label = feature.Label > 0.5f
            };
        }

        private float HashStringToFloat(string input)
        {
            if (string.IsNullOrEmpty(input)) return 0f;
            return (float)(Math.Abs(input.GetHashCode()) / (double)int.MaxValue);
        }

        private float CalculateLocationProximity(double userLat, double userLng)
        {
            var actualLat = (userLat * 180) - 90;
            var actualLng = (userLng * 360) - 180;

            const double EARTH_RADIUS = 6371; // km
            var dLat = ToRadians(actualLat);
            var dLng = ToRadians(actualLng);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(actualLat)) * Math.Cos(ToRadians(actualLat)) *
                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = EARTH_RADIUS * c;

            return (float)Math.Exp(-distance / MAX_DISTANCE_KM);
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
