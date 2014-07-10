using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading;
using System.Windows;
using WCFNamedPipesExample.Services;

namespace WCFNamedPipesExample.Client
{
    /// <summary>
    /// Client controller. Acts as callback client.
    /// </summary>
    public partial class ClientController : Application, IProductServiceCallback
    {
        private IProductService pipeProxy;
        private ChannelFactory<IProductService> pipeFactory;

        private readonly System.Timers.Timer checkConnectionTimer;
        private string currentProductNumber;

        /// <summary>
        /// Creates a new instance of the <see cref="ClientController"/> class.
        /// </summary>
        public ClientController()
        {
            Activated += ClientControllerActivated;

            //init connection timer
            this.checkConnectionTimer = new System.Timers.Timer(5000);
            this.checkConnectionTimer.Elapsed += CheckConnectionTimerElapsed;
        }

        private void Init()
        {
            var initializeConnectionThread = new Thread(Connect);
            initializeConnectionThread.Start();
        }

        private void Connect()
        {
            //get endpoint configuration from app.config.
            var group =
                ServiceModelSectionGroup.GetSectionGroup(
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None));

            if (group != null)
            {
                //create duplex channel factory
                pipeFactory = new DuplexChannelFactory<IProductService>(this, group.Client.Endpoints[0].Name);

                //create a communication channel and register for its events
                pipeProxy = pipeFactory.CreateChannel();
                ((IClientChannel) pipeProxy).Faulted += PipeProxyFaulted;
                ((IClientChannel) pipeProxy).Opened += PipeProxyOpened;

                try
                {
                    //try to open the connection
                    ((IClientChannel) pipeProxy).Open();
                    this.checkConnectionTimer.Stop();

                    //register for added products
                    pipeProxy.RegisterForAddedProducts();

                }
                catch
                {
                    //for example show status text or log; 
                }
            }
        }

        void CheckConnectionTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //if the channel is not already open, call Initialize
            if (pipeProxy == null || ((IClientChannel)pipeProxy).State != CommunicationState.Opened
                || ((IClientChannel)pipeProxy).State != CommunicationState.Opening)
            {
                Connect();
            }
        }  

        void ClientControllerActivated(object sender, EventArgs e)
        {
            Init();
            MainWindow.DataContext = this;
            Activated -= ClientControllerActivated;
        }

        void PipeProxyOpened(object sender, EventArgs e)
        {
            this.checkConnectionTimer.Stop();
        }

        void PipeProxyFaulted(object sender, EventArgs e)
        {
            var proxy = sender as IClientChannel;
            if (proxy != null)
            {
                proxy.Faulted -= PipeProxyFaulted;
                proxy.Opened -= PipeProxyOpened;
                proxy = null;
            }

            this.pipeFactory = null;
            this.checkConnectionTimer.Start();
        }

        public void GetProductInformation()
        {
            if (!String.IsNullOrEmpty(currentProductNumber))
            {
                pipeProxy.GetProductInformation(currentProductNumber);
            }
        }

        /// <summary>
        /// Invoked by the server, when a new product was added to the host
        /// application.
        /// </summary>
        /// <param name="productNumber"></param>
        public void ProductAdded(string productNumber)
        {
            currentProductNumber = productNumber;
            MessageBox.Show("Received Product Number: "
                            + productNumber, "Product Added!");
        }

        /// <summary>
        /// Invoked when additional product information were retrieved.
        /// </summary>
        /// <param name="product"></param>
        public void ProductInformationCallback(Product product)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(product.Name);
            stringBuilder.AppendLine(product.Number);
            stringBuilder.AppendLine(product.Manufacturer);
            stringBuilder.AppendLine(product.Price.ToString("C"));
            MessageBox.Show("Received Product Information. " + stringBuilder);
        }
    }
}
