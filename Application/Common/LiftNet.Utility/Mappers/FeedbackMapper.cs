using LiftNet.Contract.Views.Appointments;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Mappers
{
    public static class FeedbackMapper
    {
        /// <summary>
        /// Should include reviewer and userRoleMapping
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static FeedbackView ToView(this Feedback entity, Dictionary<string, LiftNetRoleEnum>? userRoleMapping = null)
        {
            return new FeedbackView
            {
                Id = entity.Id,
                Content = entity.Content,
                Medias = entity.Medias == null ?
                    JsonConvert.DeserializeObject<List<string>>(entity.Medias) : null,
                Star = entity.Star,
                User = entity.Reviewer.ToOverview(userRoleMapping),
            };
        }

        /// <summary>
        /// Should include reviewer and userRoleMapping
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<FeedbackView> ToViews(this List<Feedback> entities, Dictionary<string, LiftNetRoleEnum>? userRoleMapping = null)
        {
            return entities.Select(x => x.ToView(userRoleMapping)).ToList();
        }
    }
}
