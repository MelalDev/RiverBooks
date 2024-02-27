using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing;

// This is the materialized view's data model.
/*
* We could think about OrderAddress. This is for the materialized view. I don't think it necessarily belongs in a domain model
* because it's kind of separate. So, I'll probably put that in with the infrastructure is using it.
*/
internal record OrderAddress(Guid Id, Address Address);
