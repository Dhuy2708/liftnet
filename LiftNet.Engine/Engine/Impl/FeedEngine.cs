using LiftNet.Contract.Constants;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Engine.Data.Feat;
using LiftNet.Engine.ML;
using LiftNet.Engine.Util;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Engine.Engine.Impl
{
    public class FeedEngine : IFeedEngine, IDependency
    {
        private readonly FeedFFMModel _model;

        public FeedEngine(IBlobService blobService)
        {
            _model = new FeedFFMModel(blobService);
        }

        public async Task Train(IEnumerable<UserFeedFeature> trainingData)
        {
            var normalizedData = trainingData.Select(x => FeedNormalizationUtil.NormalizeUserFeedFeature(x))
                                             .ToList();
            await _model.Train(normalizedData);
        }

        public async Task IncrementalTrain(IEnumerable<UserFeedFeature> newData)
        {
            var normalizedData = newData.Select(x => FeedNormalizationUtil.NormalizeUserFeedFeature(x)).ToList();
            await _model.TrainIncremental(normalizedData);
        }

        public float Predict(UserFeedFeature feature)
        {
            var normalizedFeature = FeedNormalizationUtil.NormalizeUserFeedFeature(feature);
            return _model.Predict(normalizedFeature);
        }
    }
}
