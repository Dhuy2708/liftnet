using LiftNet.Contract.Views.Plannings;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Plannings.Queries.Requests
{
    public class ListExercisesQuery : IRequest<LiftNetRes<List<ExerciseView>>>
    {
        public string Search
        {
            get; set;
        }

        public int PageNumber
        {
            get; set;
        } = 1;

        public int PageSize
        {
            get; set;
        } = 10;
    }
}
