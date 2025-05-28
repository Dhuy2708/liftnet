using LiftNet.Contract.Enums;
using LiftNet.Domain.Enums;
using LiftNet.Engine.Data.Feat;
using LiftNet.Engine.Data.Normalized;
using LiftNet.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Engine.Util
{
    public class FeedNormalizationUtil
    {
        private const int MAX_LIKES = 10000;
        private const double MAX_TIME_AGO_DAYS = 365.0;

        public static NormalizedUserFeedFeature NormalizeUserFeedFeature(UserFeedFeature feature)
        {
            var result = new NormalizedUserFeedFeature
            {
                User = NormalizeUserFields(feature.User),
                Feed = NormalizeFeedFields(feature.Feed),
                Label = feature.Label
            };
            return result;
        }

        private static NormalizedUserFieldAware NormalizeUserFields(UserFieldAware user)
        {
            return new NormalizedUserFieldAware
            {
                UserId = user.UserId,
                AgeRangeOneHot = OneHotEncodeAgeRange(UserUtil.GetAgeRange(user.Age)),
                RoleOneHot = OneHotEncodeRole(user.Role),
                GenderOneHot = OneHotEncodeGender(user.Gender),
                NormalizedLat = NormalizeLatitude(user.Location?.Lat ?? 0),
                NormalizedLng = NormalizeLongitude(user.Location?.Lng ?? 0)
            };
        }

        private static NormalizedFeedFieldAware NormalizeFeedFields(FeedFieldAware feed)
        {
            return new NormalizedFeedFieldAware
            {
                FeedId = feed.FeedId,
                NormalizedTimeAgo = NormalizeTimeAgo(feed.CreatedAt)
            };
        }

        private static float[] OneHotEncodeAgeRange(UserAgeRange ageRange)
        {
            var ranges = Enum.GetValues(typeof(UserAgeRange)).Length;
            var encoding = new float[ranges];
            encoding[(int)ageRange] = 1f;
            return encoding;
        }

        private static float[] OneHotEncodeRole(LiftNetRoleEnum role)
        {
            var roles = Enum.GetValues(typeof(LiftNetRoleEnum)).Length - 1; // remove admin
            var encoding = new float[roles];
            encoding[(int)role] = 1f;
            return encoding;
        }

        private static float[] OneHotEncodeGender(LiftNetGender gender)
        {
            var genders = Enum.GetValues(typeof(LiftNetGender)).Length;
            var encoding = new float[genders];
            encoding[(int)gender] = 1f;
            return encoding;
        }

        private static float NormalizeLatitude(double lat)
        {
            return (float)((lat + 90) / 180);
        }

        private static float NormalizeLongitude(double lng)
        {
            return (float)((lng + 180) / 360);
        }

        private static float NormalizeLikes(int likes)
        {
            return Math.Clamp((float)likes / MAX_LIKES, 0f, 1f);
        }

        private static float NormalizeTimeAgo(DateTime createdAt)
        {
            var timeAgo = (DateTime.UtcNow - createdAt).TotalDays;
            return Math.Clamp((float)(timeAgo / MAX_TIME_AGO_DAYS), 0f, 1f);
        }
    }
}
