using LiftNet.Contract.Enums.Finder;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
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
    public class ResponseApplicantHandler : IRequestHandler<ResponseApplicantCommand, LiftNetRes>
    {
        private readonly ILiftLogger<ResponseApplicantHandler> _logger;
        private readonly IFinderPostRepo _postRepo;
        private readonly IFinderPostApplicantRepo _applicantRepo;
        private readonly IUserRepo _userRepo;
        private readonly IUserService _userService;
        public ResponseApplicantHandler(ILiftLogger<ResponseApplicantHandler> logger,
                                        IFinderPostRepo postRepo,
                                        IFinderPostApplicantRepo applicantRepo,
                                        IUserRepo userRepo,
                                        IUserService userService)
        {
            _logger = logger;
            _postRepo = postRepo;
            _applicantRepo = applicantRepo;
            _userRepo = userRepo;
            _userService = userService;
        }
        public async Task<LiftNetRes> Handle(ResponseApplicantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryable = _applicantRepo.GetQueryable()
                                              .Include(x => x.Post);

                var applicant = await queryable.FirstOrDefaultAsync(x => x.Id == request.ApplicantId);
                if (applicant == null)
                {
                    return LiftNetRes.ErrorResponse("Applicant not found");
                }

                if (applicant.Post.UserId != request.UserId)
                {
                    return LiftNetRes.ErrorResponse("You don't have permission to respond to this applicant");
                }

                if (applicant.Status != (int)FinderApplyingStatus.Applying)
                {
                    return LiftNetRes.ErrorResponse("Applicant is not in a pending state");
                }

                if (request.Status == FinderPostResponseType.Accept)
                {
                    applicant.Status = (int)FinderPostResponseType.Accept;
                    applicant.Post.Status = (int)FinderPostStatus.Closed;

                    var otherApplicants = await _applicantRepo.GetQueryable()
                        .Where(x => x.PostId == applicant.PostId
                                    && x.Id != applicant.Id
                                    && x.Status == (int)FinderApplyingStatus.Applying)
                        .ToListAsync(cancellationToken);

                    foreach (var other in otherApplicants)
                    {
                        other.Status = (int)FinderPostResponseType.Reject;
                    }

                    await _applicantRepo.UpdateRange(otherApplicants);
                    await _applicantRepo.Update(applicant);
                    await _postRepo.Update(applicant.Post);
                    await _applicantRepo.SaveChangesAsync();
                }
                else if (request.Status == FinderPostResponseType.Reject)
                {
                    applicant.Status = (int)FinderPostResponseType.Reject;
                    await _applicantRepo.Update(applicant);
                    await _applicantRepo.SaveChangesAsync();
                }
                else
                {
                    return LiftNetRes.ErrorResponse("Invalid status");
                }

                return LiftNetRes.SuccessResponse();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while responding to applicant");
                return LiftNetRes.ErrorResponse("An error occurred while processing your request.");
            }
        }
    }
}
