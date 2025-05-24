using LiftNet.Contract.Enums.Finder;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Finders.Commands.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Handler.Finders.Commands
{
    public class ApplyFinderPostHandler : IRequestHandler<ApplyFinderPostCommand, LiftNetRes>
    {
        private readonly ILiftLogger<ApplyFinderPostHandler> _logger;
        private readonly IFinderPostRepo _postRepo;
        private readonly IFinderPostApplicantRepo _applicantRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ApplyFinderPostHandler(
            ILiftLogger<ApplyFinderPostHandler> logger,
            IFinderPostRepo postRepo,
            IFinderPostApplicantRepo applicantRepo,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _postRepo = postRepo;
            _applicantRepo = applicantRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<LiftNetRes> Handle(ApplyFinderPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var post = await _postRepo.GetById(request.PostId);
                if (post == null)
                {
                    return LiftNetRes.ErrorResponse("Post not found");
                }

                if (post.Status != (int)FinderPostStatus.Open)
                {
                    return LiftNetRes.ErrorResponse("Post is not open for applications");
                }

                var existingApplication = await _applicantRepo.GetQueryable()
                                                    .FirstOrDefaultAsync(x => x.PostId == request.PostId && 
                                                                                x.TrainerId == request.UserId); 

                if (existingApplication != null)
                {
                    return LiftNetRes.ErrorResponse("You have already applied to this post");
                }

                var application = new FinderPostApplicant
                {
                    PostId = request.PostId,
                    TrainerId = request.UserId,
                    Message = request.Message,
                    Status = (int)FinderPostApplyingStatus.Applying,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };

                await _applicantRepo.Create(application);
                await _unitOfWork.CommitAsync();

                return LiftNetRes.SuccessResponse();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while applying to finder post");
                return LiftNetRes.ErrorResponse("Error occurred while applying to finder post");
            }
        }
    }
} 