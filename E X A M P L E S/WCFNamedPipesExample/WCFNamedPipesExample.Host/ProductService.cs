using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using WCFNamedPipesExample.Services;

namespace WCFNamedPipesExample.Host
{
    /// <summary>
    /// Product service implementation.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ProductService : IProductService
    {
        //a list holding all callbacks for clients that get notified
        //when a new product was added.
        private readonly IList<IProductServiceCallback> addedProductsCallbackList;
        
        //reference to the host application for delegating calls.
        private readonly IHostApplication hostApplication;
        
        /// <summary>
        /// Creates a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="hostApplication"></param>
        public ProductService(IHostApplication hostApplication)
        {
            addedProductsCallbackList = new List<IProductServiceCallback>();
            this.hostApplication = hostApplication;
            //subscribe to host event
            this.hostApplication.ProductAdded += HostApplicationProductAdded;
        }

        void HostApplicationProductAdded(object sender, EventArgs<string> e)
        {
            //notify all clients
            foreach (var productServiceCallback in addedProductsCallbackList)
            {
                productServiceCallback.ProductAdded(e.Data);
            }
        }

        #region Implementation of IProductService

        /// <summary>
        /// Register a client for notification of added products.
        /// </summary>
        public void RegisterForAddedProducts()
        {
            //get callback from operation context
            var callback = OperationContext.Current.GetCallbackChannel<IProductServiceCallback>();
            if (callback != null)
            {
                this.addedProductsCallbackList.Add(callback);
            }
        }

        /// <summary>
        /// Request additional product information.
        /// </summary>
        /// <param name="productNumber"></param>
        public void GetProductInformation(string productNumber)
        {
            if (this.hostApplication == null) 
                return;
            
            var callback = OperationContext.Current.GetCallbackChannel<IProductServiceCallback>();
            if (callback != null)
            {
                //e.g. start a backgroundworker for long running processes
                var backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += BackgroundWorkerDoWork;
                backgroundWorker.RunWorkerCompleted += BackgroundWorkerRunWorkerCompleted;
                backgroundWorker.RunWorkerAsync(new ArgumentObject {Callback = callback, ProductNumber = productNumber});
            }
        }

        #endregion

        void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var args = e.Result as ArgumentObject;
            if (args != null)
            {
                //notify the client
                args.Callback.ProductInformationCallback(args.Product);
            }
        }

        void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as ArgumentObject;
            if(args != null)
            {
                //get the product from the host application
                args.Product = hostApplication.GetProductInformation(args.ProductNumber);
            }

            e.Result = args;
        }
    }

    /// <summary>
    /// Helper class for transfering data in the background worker
    /// </summary>
    internal class ArgumentObject
    {
        internal IProductServiceCallback Callback { get; set; }
        internal string ProductNumber { get; set; }
        internal Product Product { get; set; }
    }
}
