using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Commands.Requests;
using LiftNet.Handler.Appointments.Commands.Validators;
using LiftNet.SharedKenel.Extensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands
{
    public class AppointmentActionHandler : IRequestHandler<AppointmentActionCommand, LiftNetRes>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILiftLogger<AppointmentActionHandler> _logger;

        public AppointmentActionHandler(IUnitOfWork uow, ILiftLogger<AppointmentActionHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(AppointmentActionCommand request, CancellationToken cancellationToken)
        {
            await new AppointmentActionValidator().ValidateAndThrowAsync(request);
            
            _logger.Info("begin to handle appointment action command");
            throw new NotImplementedException();
        }
    }
}
