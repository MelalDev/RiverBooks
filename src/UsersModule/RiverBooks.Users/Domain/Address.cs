namespace RiverBooks.Users.Domain;

/*
* There's no reason not to use the same record type for this address, it's duplicate of what we have in Order processing
* module, and that's fine, if we need to change it on either side we can, but at the momment they need the same thing,
* so there's no reason not to use the same definition. We could consider moving this into a SharedKernel project, which
* we will have here soon, but I'd be real cautions about doing that with one of these just because the chance that they
* might evolve separately is probably decent and I wouldn't want that to be difficult to do because they were using a
* shared library.
*/
public record Address(string Street1,
                        string Street2,
                        string City,
                        string State,
                        string PostalCode,
                        string Country);
