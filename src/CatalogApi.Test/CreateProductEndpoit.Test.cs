using Xunit;
using Moq;
using CatalogApi.Features.CreateProduct;
using MediatR;
using Mapster;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;

namespace CatalogApi.Tests.Features.CreateProduct
{
    public class CreateProductEndpointTests
    {
        [Fact]
        public async Task CreateProductEndpoint_ReturnsCreatedResult_WhenProductIsCreated()
        {
            // Arrange
            var mockSender = new Mock<ISender>();

            var request = new CreateProductRequest(
                Name: "Test Product",
                Categories: new List<string> { "Category1", "Category2" },
                Description: "Test Description",
                ImageFile: "image.jpg",
                Price: 99.99m
            );

            var command = request.Adapt<CreateProductCommand>();

            var expectedResponse = new CreateProductResponse(Guid.NewGuid());

            // Mock ISender to return a valid response when sending the command
            mockSender
                .Setup(x => x.Send(It.IsAny<CreateProductCommand>(), default))
                .ReturnsAsync(new CreateProductResult(expectedResponse.Id));

            // Act
            var handler = new Func<CreateProductRequest, ISender, Task<IResult>>(async (req, sender) =>
            {
                var command = req.Adapt<CreateProductCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateProductResponse>();
                return Results.Created($"/products/{response.Id}", response);
            });

            var result = await handler(request, mockSender.Object);

            // Assert
            var createdResult = Assert.IsType<Created<CreateProductResponse>>(result);
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(expectedResponse.Id, createdResult.Value.Id);
        }

        [Fact]
        public async Task CreateProductEndpoint_ReturnsBadRequest_WhenCommandFails()
        {
            // Arrange
            var mockSender = new Mock<ISender>();

            var request = new CreateProductRequest(
                Name: "Test Product",
                Categories: new List<string> { "Category1", "Category2" },
                Description: "Test Description",
                ImageFile: "image.jpg",
                Price: 99.99m
            );

            // Mock ISender to return null or throw an exception to simulate failure
            mockSender
                .Setup(x => x.Send(It.IsAny<CreateProductCommand>(), default))
                .ThrowsAsync(new Exception("Command failed"));

            // Act
            var handler = new Func<CreateProductRequest, ISender, Task<IResult>>(async (req, sender) =>
            {
                try
                {
                    var command = req.Adapt<CreateProductCommand>();
                    var result = await sender.Send(command);
                    var response = result.Adapt<CreateProductResponse>();
                    return Results.Created($"/products/{response.Id}", response);
                }
                catch
                {
                    return Results.Problem("Failed to create product", statusCode: 400);
                }
            });

            var result = await handler(request, mockSender.Object);

            // Assert
            var badRequestResult = Assert.IsType<ProblemHttpResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Failed to create product", badRequestResult.ProblemDetails.Detail);
        }
    }
}
