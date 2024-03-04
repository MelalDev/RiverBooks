using Ardalis.Result;
using RiverBooks.EmailSending.Contracts;

namespace RiverBooks.EmailSending.Integrations;

// this is not an handler from mediator anymore due to using outbox pattern.
// internal class SendEmailCommandHandler //: IRequestHandler<SendEmailCommand, Result<Guid>>
// {
//     private readonly ISendEmail _emailSender;

//     public SendEmailCommandHandler(ISendEmail emailSender)
//     {
//         _emailSender = emailSender;
//     }

//     public async Task<Result<Guid>> HandleAsync(SendEmailCommand request, CancellationToken ct)
//     {
//         await _emailSender.SendEmailAsync(request.To, request.From, request.Subject, request.Body);

//         return Guid.Empty;
//     }
// }
