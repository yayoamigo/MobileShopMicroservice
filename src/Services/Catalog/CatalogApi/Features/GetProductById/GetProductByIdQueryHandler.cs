

namespace CatalogApi.Features.GetProductById
{
    public record GetProductByIdQuery(Guid id) : IQuery<GetProductByIdResult>;

    public record GetProductByIdResult(Product Product);
    internal class GetProductByIdQueryHandler(IDocumentSession session, ILogger<GetProductByIdQueryHandler> logger)
        : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            if(request.id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty");
            }

            logger.LogInformation("GetProductByIdQueryHandler");
            var product = await session.LoadAsync<Product>(request.id, cancellationToken);
            if(product == null) {
                throw new ProductNotFounException(request.id.ToString());
            }
            return new GetProductByIdResult(product);
        }
    }
}
