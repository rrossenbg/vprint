using System.ServiceModel;

namespace WCFNamedPipesExample.Services
{
    /// <summary>
    /// Callback definition for clients.
    /// </summary>
    public interface IProductServiceCallback
    {
        /// <summary>
        /// Invoked by the server, when a new product was added to the host
        /// application.
        /// </summary>
        /// <param name="productNumber"></param>
        [OperationContract(IsOneWay = true)]
        void ProductAdded(string productNumber);

        /// <summary>
        /// Invoked when additional product information were retrieved.
        /// </summary>
        /// <param name="product"></param>
        [OperationContract(IsOneWay = true)]
        void ProductInformationCallback(Product product);
    }
}
