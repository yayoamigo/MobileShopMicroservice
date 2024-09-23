namespace CatalogApi.Exceptions
{
    public class ProductNotFounException : Exception
    {
        public ProductNotFounException(string productId) : base($"Product with id: {productId} was not found.")
        {
        }
    }
}
