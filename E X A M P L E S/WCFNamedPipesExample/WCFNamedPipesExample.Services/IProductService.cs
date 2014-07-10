using System.ServiceModel;

namespace WCFNamedPipesExample.Services
{
    /// <summary>
    /// Service contract for the product service.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required,
        CallbackContract = typeof(IProductServiceCallback))]
    public interface IProductService
    {
        /// <summary>
        /// Register a client for notification of added products.
        /// </summary>
        [OperationContract]
        void RegisterForAddedProducts();

        /// <summary>
        /// Request additional product information.
        /// </summary>
        /// <param name="productNumber"></param>
        [OperationContract]
        void GetProductInformation(string productNumber);
    }
}
