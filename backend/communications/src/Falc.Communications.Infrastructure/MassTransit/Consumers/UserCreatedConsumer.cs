using Falc.Communications.Application.Commands;
using Falc.Identity.Contracts;
using MassTransit;
using MediatR;

namespace Falc.Communications.Infrastructure.MassTransit.Consumers;

public class UserCreatedConsumer(IMediator mediator) : IConsumer<UserCreated>
{
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var dto = new CreateUser.Command(Guid.Parse(context.Message.Id), context.Message.Email);
        await mediator.Send(dto, context.CancellationToken);
    }
}