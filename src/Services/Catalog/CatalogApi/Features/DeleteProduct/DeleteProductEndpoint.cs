using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CatalogApi.Features.DeleteProduct
{
    public record DeleteProductResponse(bool Success);

    public class DeleteProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/products/{id}",  async (Guid id,  ISender sender) =>
            {
                var result = await sender.Send(new DeleteProductCommand(id));
                var response = result.Adapt<DeleteProductResponse>();
                return Results.Ok(response);
            })
            .WithName("DeleteProduct")
            .Produces<DeleteProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete a product")
            .WithDescription("Delete a product from the catalog");
        }
    }
}
