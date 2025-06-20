using LiftNet.Contract.Views.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Feeds
{
    public class FeedViewModel
    {
        public string Id 
        {
            get; set; 
        }
        public UserOverview UserOverview
        {
            get; set;
        }
        public string Content
        { 
            get; set; 
        }
        public List<string> Medias 
        { 
            get; set; 
        }
        public DateTime CreatedAt 
        { 
            get; set; 
        }
        public DateTime ModifiedAt 
        { 
            get; set;
        }
        public int LikeCount
        {
            get; set;
        }
        public int CommentCount
        {
            get; set;
        }
        public bool IsLiked 
        {
            get; set; 
        }
    }
}
