
using CatalogApi.Models;
using Marten;

namespace CatalogApi.Features.GetProduct
{
     public record GetProductsQuery() : IQuery<GetProductResult>;

    public record GetProductResult(IEnumerable<Product> Products);
 
    internal class GetProductsQueryHandler
            (IDocumentSession session, ILogger<GetProductsQueryHandler> logger)
            : IQueryHandler<GetProductsQuery, GetProductResult>
      {
            public async Task<GetProductResult> Handle(GetProductsQuery request, CancellationToken cancellationToken)
            {
                logger.LogInformation("GetProductsQueryHandler");
                var products = await session.Query<Product>().ToListAsync(cancellationToken);
                return new GetProductResult(products);
            }
       }
    
}
