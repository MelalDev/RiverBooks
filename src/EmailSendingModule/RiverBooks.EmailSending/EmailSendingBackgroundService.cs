using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RiverBooks.EmailSending;

internal class EmailSendingBackgroundService : BackgroundService
{
    private readonly ILogger<EmailSendingBackgroundService> _logger;
    private readonly ISendEmailsFromOutboxService _sendEmailsFromOutboxService;

    public EmailSendingBackgroundService(ILogger<EmailSendingBackgroundService> logger, ISendEmailsFromOutboxService sendEmailsFromOutboxService)
    {
        _logger = logger;
        _sendEmailsFromOutboxService = sendEmailsFromOutboxService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        /*
        * We're going to have this run every 10 seconds just so that we can see it working. Normally, this would be a much
        * smaller number, maybe every 50 milliseconds or 100 milliseconds. Again, we don't want to unnecessarily delay
        * sending emails, but you also don't want this thing to juse be in a tight loop queueing up CPU, so some delay
        * is useful.
        */
        int delayMilliseconds = 10_000; // 10 seconds
        _logger.LogInformation("{servicename} starting ...", nameof(EmailSendingBackgroundService));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                /*
                * Now, we need something that's going to actually do the work and process the outbox. Now we could use our
                * IOutboxService, but that I'm thinking of more as a way to insert individual items or pull individual items
                * off. This is more like what do we want to do when we get one of the things off the outbox, and so, this is
                * actually the email sending service that we want to use. So, I'm thinking this is an SendEmail from outbox
                * kind of service, so let's name it that (ISendEmailsFromOutboxService).
                */
                await _sendEmailsFromOutboxService.CheckForAndSendEmailsAsync();
            }
            catch (Exception ex)
            {
                /*
                * we're going to catch any errors, and assuming you don't want your background worker to just die, we're 
                * going to not rethrow those exceptions, but we will log them.
                */
                _logger.LogError("Error processing outbox: {message}", ex.Message);
            }
            finally
            {
                /*
                * in the finally block, that's where we're going to await task.delay and then that milliseconds. If you wanted 
                * to, you could use a configuration file to configure the duration of the wait time. In this case, we're just
                * hard coding it.
                */
                await Task.Delay(delayMilliseconds, stoppingToken);
            }
        }

        _logger.LogInformation("{servicename} stopping.", nameof(EmailSendingBackgroundService));
    }
}
