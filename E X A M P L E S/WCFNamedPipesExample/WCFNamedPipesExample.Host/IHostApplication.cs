using System;
using WCFNamedPipesExample.Services;

namespace WCFNamedPipesExample.Host
{
    /// <summary>
    /// Interface for defining the host application.
    /// </summary>
    public interface IHostApplication
    {
        /// <summary>
        /// Invoked when a new product was added.
        /// </summary>
        event EventHandler<EventArgs<string>> ProductAdded;

        /// <summary>
        /// Gets a <see cref="Product"/> for the given product number.
        /// </summary>
        /// <param name="productNumber"></param>
        /// <returns></returns>
        Product GetProductInformation(string productNumber);
    }
}
