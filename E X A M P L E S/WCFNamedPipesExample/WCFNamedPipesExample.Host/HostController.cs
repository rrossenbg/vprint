using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Threading;
using System.Windows;
using WCFNamedPipesExample.Services;

namespace WCFNamedPipesExample.Host
{
    /// <summary>
    /// The host/server controller
    /// </summary>
    public partial class HostController : Application, IHostApplication
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of the <see cref="HostController"/> class.
        /// </summary>
        public HostController()
        {
            Activated += HostControllerActivated;
        }

        #endregion

        #region Initialization

        private void Init()
        {
            //start a new thread to start the server
            var initializationThread = new Thread(StartHost);
            initializationThread.Start();
        }

        private void StartHost()
        {
            var group =
                ServiceModelSectionGroup.GetSectionGroup(
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None));

            if (group != null)
            {
                //take the first service and endpoint definition
                var service = group.Services.Services[0];
                var baseAddress =
                            service.Endpoints[0].Address.AbsoluteUri.Replace(service.Endpoints[0].Address.AbsolutePath,
                                                                                String.Empty);
                //create service
                var productService = new ProductService(this);
                //create host
                var host = new ServiceHost(productService, new[] { new Uri(baseAddress) });
                host.AddServiceEndpoint(typeof(IProductService),
                                        new NetNamedPipeBinding(), service.Endpoints[0].Address.AbsolutePath);

                try
                {
                    //open host
                    host.Open();
                }
                catch (CommunicationObjectFaultedException cofe)
                {
                    //log exception
                }
                catch (TimeoutException te)
                {
                    //log exception
                }
            }
        }

        #endregion

        #region Event Handler

        public void OnProductAdded(string productNumber)
        {
            var productAdded = ProductAdded;
            if (productAdded != null)
            {
                productAdded(this, new EventArgs<string>(productNumber));
            }
        }

        void HostControllerActivated(object sender, EventArgs e)
        {
            Init();
            MainWindow.DataContext = this;
            Activated -= HostControllerActivated;
        }

        #endregion

        #region IHostApplication members

        /// <summary>
        /// Invoked when a new product was added.
        /// </summary>
        public event EventHandler<EventArgs<string>> ProductAdded;

        /// <summary>
        /// Gets a <see cref="Product"/> for the given product number.
        /// </summary>
        /// <param name="productNumber"></param>
        /// <returns></returns>
        public Product GetProductInformation(string productNumber)
        {
            return new Product
                       {
                           Manufacturer = "Microsoft", 
                           Name = "Visual Studio",
                           Number = productNumber, 
                           Price = 1200.0m
                       };
        }

        #endregion
    }
}
