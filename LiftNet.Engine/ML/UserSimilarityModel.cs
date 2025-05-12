using LiftNet.Domain.Enums;
using LiftNet.Engine.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Engine.ML
{
    public class UserSimilarityModel
    {
        private const double AGE_WEIGHT = 0.2;
        private const double ROLE_WEIGHT = 0.3;
        private const double LOCATION_WEIGHT = 0.2;
        private const double FOLLOWING_WEIGHT = 0.3;

        public float ComputeScore(UserSimilarityFeature user1, UserSimilarityFeature user2)
        {
            var ageSimilarity = CalculateAgeSimilarity(user1.Age, user2.Age);
            var roleSimilarity = CalculateRoleSimilarity(user1.Role, user2.Role);
            var locationSimilarity = CalculateLocationSimilarity(user1.Lat, user1.Lng, user2.Lat, user2.Lng);
            var followingSimilarity = CalculateFollowingSimilarity(user1.FollowingIds, user2.FollowingIds);

            return (float)(
                AGE_WEIGHT * ageSimilarity +
                ROLE_WEIGHT * roleSimilarity +
                LOCATION_WEIGHT * locationSimilarity +
                FOLLOWING_WEIGHT * followingSimilarity
            );
        }

        private double CalculateAgeSimilarity(int age1, int age2)
        {
            var ageDiff = Math.Abs(age1 - age2);
            return Math.Exp(-ageDiff / 10.0);
        }

        private double CalculateRoleSimilarity(LiftNetRoleEnum role1, LiftNetRoleEnum role2)
        {
            return role1 == role2 ? 1.0 : 0.0;
        }

        private double CalculateLocationSimilarity(double lat1, double lng1, double lat2, double lng2)
        {
            const double EARTH_RADIUS = 6371;
            var dLat = ToRadians(lat2 - lat1);
            var dLng = ToRadians(lng2 - lng1);
            
            var a = Math.Sin(dLat/2) * Math.Sin(dLat/2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLng/2) * Math.Sin(dLng/2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
            var distance = EARTH_RADIUS * c;
            
            return Math.Exp(-distance / 50.0);
        }

        private double CalculateFollowingSimilarity(List<string> following1, List<string> following2)
        {
            if (!following1.Any() || !following2.Any()) return 0;
            
            var intersection = following1.Intersect(following2).Count();
            var union = following1.Union(following2).Count();
            
            return (double)intersection / union;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
