
using FluentValidation;

namespace CatalogApi.Features.UpdateProduct
{
    public record UpdateProductCommand(Guid Id, string Name,List<string> Categories, string Description, string ImageFile, decimal Price) : IRequest<UpdateProductResult>;

    public record UpdateProductResult(bool success);

    public class UpdateProductResulValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductResulValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required").Length(2,259);
            RuleFor(x => x.Categories).NotEmpty().WithMessage("categories can not be empty");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
            RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is required");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
    internal class UpdateProductCommandHandler(IDocumentSession session, ILogger<UpdateProductCommandHandler> logger)
        : IRequestHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productToUpdate = await session.LoadAsync<Product>(request.Id);
            if (productToUpdate == null)
            {
                logger.LogWarning("Product with id {Id} not found", request.Id);
                throw new ProductNotFounException(request.Id.ToString());
            }

            productToUpdate.Name = request.Name;
            productToUpdate.Categories = request.Categories;
            productToUpdate.Description = request.Description;
            productToUpdate.ImageFile = request.ImageFile;
            productToUpdate.Price = request.Price;
            
            session.Update(productToUpdate);
           await session.SaveChangesAsync();

            return new UpdateProductResult(true);

        }
    }
}
