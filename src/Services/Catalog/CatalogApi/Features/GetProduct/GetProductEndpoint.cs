

namespace CatalogApi.Features.GetProduct
{
    public record GetProductsResponse(IEnumerable<Product> Products);
    public class GetProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (ISender sender) =>
            {
                var query = new GetProductsQuery();
                var result = await sender.Send(query);
                var response = result.Adapt<GetProductsResponse>();
                return Results.Ok(response);
            })
            .WithName("GetProducts")
            .Produces<GetProductsResponse>(200)
            .ProducesProblem(400)
            .WithSummary("Get all products")
            .WithDescription("Get all products in the catalog");
        }
    }
}
