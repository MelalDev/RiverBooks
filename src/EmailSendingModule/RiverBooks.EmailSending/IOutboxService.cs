using Ardalis.Result;

namespace RiverBooks.EmailSending;

// please check TheEmailSendingModule/UsingVerticalSliceArchitecture
/*
* So first, we're going to need some way to persist these messages. I think the simplest thing will be to just refer to this
* as an outbox service. And we'll have some methods for queuing things up on the outbox and also checking for things from the
* outbox. All right, we're going to need an outbox service, and it's going to have a way to queue up an email for sending.
* And then we'll pass that in an email. we need some type to represent that. We'll call it an email outbox entity.
*/
//internal interface IOutboxService
//{
//    Task QueueEmailForSending(EmailOutboxEntity entity);

//    /*
//   * We could go and get all of the items that need to be processed, but just for demonstration purposes, we're going
//   * to try do one by one so you can see them happening.
//   */
//    Task<Result<EmailOutboxEntity>> GetUnprocessedEmailEntity();
//}
