namespace CatalogApi.Features.GetProductById
{
    public record GetProductByIdResponse(Product Product);
    public class GetPorductByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        { 
            app.MapGet("/products/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByIdQuery(id));
                var response = result.Adapt<GetProductByIdResponse>();
                return Results.Ok(response);
            })
            .WithName("GetProductById")
            .Produces<GetProductByIdResponse>(200)
            .ProducesProblem(400)
            .WithSummary("Get a product by id")
            .WithDescription("Get a product by id from the catalog");
        }
    }
    
}
