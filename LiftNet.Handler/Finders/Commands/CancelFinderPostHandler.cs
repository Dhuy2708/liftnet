using LiftNet.Contract.Enums.Finder;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Finders.Commands.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Finders.Commands
{
    public class CancelFinderPostHandler : IRequestHandler<CancelFinderPostCommand, LiftNetRes>
    {
        private readonly ILiftLogger<CancelFinderPostHandler> _logger;
        private readonly IFinderPostApplicantRepo _applicantRepo;

        public CancelFinderPostHandler(ILiftLogger<CancelFinderPostHandler> logger, IFinderPostApplicantRepo applicantRepo)
        {
            _logger = logger;
            _applicantRepo = applicantRepo;
        }

        public async Task<LiftNetRes> Handle(CancelFinderPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Info("begin cancel finder post");
                var applicant = await _applicantRepo.GetQueryable()
                                        .FirstOrDefaultAsync(x => x.TrainerId == request.UserId &&
                                                                  x.PostId == request.PostId);
                if (applicant == null)
                {
                    return LiftNetRes.ErrorResponse("You have not applied for this post");
                }

                if (applicant.Status != (int)FinderPostApplyingStatus.Applying)
                {
                    return LiftNetRes.ErrorResponse("You can only cancel posts that are currently applying");
                }

                applicant.Status = (int)FinderPostApplyingStatus.Canceled;
                applicant.CancelReason = request.Reason;
                await _applicantRepo.Update(applicant);

                return LiftNetRes.SuccessResponse("Post cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error cancelling finder post");
                return LiftNetRes.ErrorResponse("An error occurred while cancelling the post");
            }
        }
    }
}
