namespace RiverBooks.Books;

/*
* One question you may have is, why are we using book and not our Dto here? The reason is that our repository abstraction
* is part of our domain model, so it should only work with domain model types like book. Dtos aren't part of domain model
* typically. It's going to be the application services layer's job to map between our domain model types and our 
* application types, specifically these DTOs. Now normally I would do this on a feature by feature basis, and so I would
* be working in vertical slices, and we'd be doing one of these operations like adding a book all the way from the Api
* endpoint, through the application service, and into the repository. However, in the interest of time for this course,
* I'm doing these in horizontal slices, and that's not how you'd want to do it in a real world situation. It's just 
* faster in this case because I already know where I'm going, and I already know that it all works because I've already
* built it.
* If you want to learn more about vertical slices architecture, check out this article here on DevIQ: deviq.com/practices/vertical-slices
*/
internal interface IBookRepository : IReadOnlyBookRepository
{
    Task AddAsync(Book book);
    Task DeleteAsync(Book book);
    Task SaveChangesAsync();
}
