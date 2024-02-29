using Ardalis.Result;
using MediatR;

namespace RiverBooks.EmailSending.Integrations;

internal class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, Result<Guid>>
{
    private readonly ISendEmail _sendEmail;

    public SendEmailCommandHandler(ISendEmail sendEmail)
    {
        _sendEmail = sendEmail;
    }

    public async Task<Result<Guid>> Handle(SendEmailCommand request, CancellationToken ct)
    {
        await _sendEmail.SendEmailAsync(request.To, request.From, request.Subject, request.Body);

        return Guid.Empty;
    }
}
