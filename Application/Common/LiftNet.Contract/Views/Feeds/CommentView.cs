using LiftNet.Contract.Views.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Feeds
{
    public class CommentView
    {
        public string Id
        {
            get; set;
        }

        public UserOverview User
        {
            get; set;
        }

        public string Comment
        {
            get; set;
        }
        
        public DateTimeOffset CreatedAt
        {
            get; set;
        }

        public DateTimeOffset ModifiedAt
        {
            get; set;
        }
    }
}
