using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IEmailService emailService, ILogger<DeleteOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = await _orderRepository.GetByIdAsync(request.Id);

            if (orderEntity == null)
            {
                _logger.LogError($"Order {request.Id} does not exist in the database.");

                throw new NotFoundException(nameof(Order), request.Id);
            }

            await _orderRepository.DeleteAsync(orderEntity);

            _logger.LogInformation($"Order {orderEntity.Id} has been deleted.");

            return Unit.Value;
        }
    }
}
