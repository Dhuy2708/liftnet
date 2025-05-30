using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views;
using LiftNet.Contract.Views.Finders;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Finders.Queries.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Finders.Queries
{
    public class GetAppliedMessageHandler : IRequestHandler<GetAppliedMessageQuery, LiftNetRes<AppliedFinderPostMessage>>
    {
        private readonly ILiftLogger<GetAppliedMessageHandler> _logger;
        private readonly IFinderPostApplicantRepo _applicationRepo;

        public GetAppliedMessageHandler(ILiftLogger<GetAppliedMessageHandler> logger, IFinderPostApplicantRepo applicationRepo)
        {
            _logger = logger;
            _applicationRepo = applicationRepo;
        }

        public async Task<LiftNetRes<AppliedFinderPostMessage>> Handle(GetAppliedMessageQuery request, CancellationToken cancellationToken)
        {
            _logger.Info("Handling GetAppliedFinderPostDetailQuery");
            try
            {
                var applicant = await _applicationRepo.GetQueryable()
                                    .FirstOrDefaultAsync(x => x.TrainerId == request.UserId &&
                                                              x.PostId == request.PostId);

                if (applicant == null)
                {
                    return LiftNetRes<AppliedFinderPostMessage>.ErrorResponse("No applied message found for the given post and user.");
                }

                var message = new AppliedFinderPostMessage
                {
                    PostId = applicant.PostId,
                    ApplyMessage = applicant.Message,
                    CancelReason = applicant.CancelReason,
                    ModifiedAt = applicant.ModifiedAt
                };
                return LiftNetRes<AppliedFinderPostMessage>.SuccessResponse(message, "Applied message retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error handling GetAppliedFinderPostDetailQuery");
                return LiftNetRes<AppliedFinderPostMessage>.ErrorResponse("An error occurred while processing your request.");
            }
        }
    }
}
