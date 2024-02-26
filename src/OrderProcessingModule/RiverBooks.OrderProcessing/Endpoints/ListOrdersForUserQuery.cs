using Ardalis.Result;
using MediatR;

namespace RiverBooks.OrderProcessing;

internal record ListOrdersForUserQuery(string EmailAddress) : IRequest<Result<List<OrderSummary>>>;
