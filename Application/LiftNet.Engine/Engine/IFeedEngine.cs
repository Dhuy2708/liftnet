using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftNet.Domain.Indexes;
using LiftNet.Engine.Data.Feat;

namespace LiftNet.Engine.Engine
{
    public interface IFeedEngine
    {
        Task Train(IEnumerable<UserFeedFeature> trainingData);
        Task IncrementalTrain(IEnumerable<UserFeedFeature> newData);
        float Predict(UserFeedFeature feature);
    }
}
