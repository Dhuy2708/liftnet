using System;
using System.Collections.Generic;

namespace LiftNet.Domain.ViewModels
{
    public class FeedViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public List<string> Medias { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int LikeCount { get; set; }
        public bool IsLiked { get; set; }
    }
} 