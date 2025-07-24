using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using xampl.Models.Xampl;
using xampl.Services.RepositoryService;

namespace xampl.GraphQL
{
    public record AddDocumentInput(int UserId, string Title, string Content);
    public record UpdateDocumentInput(int UserId, int DocumentId, string Title, string Content);

    public class Mutation
    {
        public async Task<Document?> AddDocument(
            [Service] IRepository<XamplContext> repo,
            AddDocumentInput input
        )
        {
            var user = repo.GetAllAsQueryable<User>().FirstOrDefaultAsync(x => x.Id == input.UserId)
                ?? throw new ArgumentException($"No user with Id: {input.UserId}");

            var document = new Document
            {
                Title = input.Title,
                Content = input.Content,
                CreatedBy = user.Id,
                LastUpdatedBy = user.Id,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };
            var validationContext = new ValidationContext(document);
            var validationErrors = new List<ValidationResult>();
            var documentIsValid = Validator.TryValidateObject(
                document,
                validationContext,
                validationErrors,
                true
            );
            if (documentIsValid)
            {
                await repo.CreateAsync(document);
                return document;
            }

            throw new ValidationException($"Invalid input\n{string.Join('\n', validationErrors)}");
        }

        public async Task<Document> UpdateDocument(
            [Service] IRepository<XamplContext> repo,
            UpdateDocumentInput input
        )
        {
            var user = repo.GetAllAsQueryable<User>().FirstOrDefaultAsync(x => x.Id == input.UserId)
                ?? throw new ArgumentException($"No user with Id: {input.UserId}");
            var document = await repo.GetAllAsQueryable<Document>().FirstOrDefaultAsync(x => x.Id == input.DocumentId)
                ?? throw new ArgumentException($"No document with Id: {input.DocumentId}");

            document.Title = input.Title;
            document.Content = input.Content;
            document.LastUpdatedBy = user.Id;
            document.LastUpdatedAt = DateTime.UtcNow;

            var validationContext = new ValidationContext(document);
            var validationErrors = new List<ValidationResult>();
            var documentIsValid = Validator.TryValidateObject(
                document,
                validationContext,
                validationErrors,
                true
            );
            if (documentIsValid)
            {
                await repo.CreateAsync(document);
                return document;
            }

            throw new ValidationException($"Invalid input\n{string.Join('\n', validationErrors)}");
        }
    }
}
