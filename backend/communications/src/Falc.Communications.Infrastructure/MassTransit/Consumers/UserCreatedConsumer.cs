using Falc.Communications.Application.Commands;
using Falc.Identity.Contracts;
using MassTransit;

namespace Falc.Communications.Infrastructure.MassTransit.Consumers;

public class UserCreatedConsumer(CreateUser.ICommandHandler createUserCommandHandler) : IConsumer<UserCreated>
{
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var command = new CreateUser.Command(Guid.Parse(context.Message.Id), context.Message.Email);
        await createUserCommandHandler.ExecuteAsync(command, context.CancellationToken);
    }
}
