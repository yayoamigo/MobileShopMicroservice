namespace CatalogApi.Features.GetProductByCategory
{
    public record GetProductsByCategoryQuery(string category) : IQuery<GetProductsByCategoryResult>;

    public record GetProductsByCategoryResult(IEnumerable<Product> Products);
    internal class GetProductsByCategoryQueryHandler(IDocumentSession session, ILogger<GetProductsByCategoryQueryHandler> logger)
        : IQueryHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
    {
        public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("GetProductsByCategoryQueryHandler");
            var products = await session.Query<Product>().Where(p => p.Categories.Contains(request.category)).ToListAsync(cancellationToken);
            return new GetProductsByCategoryResult(products);
        }
    }
   
}
