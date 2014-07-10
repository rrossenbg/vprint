using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ErrorHandlingBehaviorLibrary;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace TesterService
{
    [DataContract]
    class InvalidOperationFault
    {
        [DataMember]
        public InvalidOperationException Exception { get; private set; }

        public InvalidOperationFault(InvalidOperationException e)
        {
            Exception = e;
        }
    }

    [DataContract]
    public class MyApplicationFault
    {
    }

    [ServiceContract]
    interface IMyService
    {
        [OperationContract]
        [FaultContract(typeof(InvalidOperationFault))]
        void MethodWithFaultContract();

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        [MapExceptionToFault(typeof(ApplicationException), typeof(MyApplicationFault))]
        void MethodWithFaultContractAndMapExceptionAttribute();

        [OperationContract]
        void MethodWithoutFaultContract();
    }

    [ErrorHandlingBehaviorAttribute(EnforceFaultContract=false,
        ExceptionToFaultConverter=typeof(MyServiceFaultProvider))]
    class MyService : IMyService
    {
        class MyServiceFaultProvider : IExceptionToFaultConverter
        {
            #region IExceptionToFaultConverter Members

            public object ConvertExceptionToFaultDetail(Exception error)
            {
                if (error is InvalidOperationException)
                    return new InvalidOperationFault(error as InvalidOperationException);
                return null;
            }

            #endregion
        }

        #region IMyService Members

        public void MethodWithFaultContract()
        {
            throw new InvalidOperationException();
        }

        public void MethodWithFaultContractAndMapExceptionAttribute()
        {
            throw new ApplicationException();
        }

        public void MethodWithoutFaultContract()
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    class ServiceMain
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(MyService));
            host.AddServiceEndpoint(typeof(IMyService), new WSHttpBinding(),
                "http://localhost:8080/MyService");
            host.Open();

            CallMethodOnService(s => s.MethodWithFaultContract());
            CallMethodOnService(s => s.MethodWithFaultContractAndMapExceptionAttribute());
            CallMethodOnService(s => s.MethodWithoutFaultContract());

            host.Close();
        }

        private static void CallMethodOnService(Action<IMyService> action)
        {
            Console.WriteLine("-----------------------------------------------------");
            IMyService proxy = ChannelFactory<IMyService>.CreateChannel(new WSHttpBinding(), new EndpointAddress("http://localhost:8080/MyService"));
            try
            {
                action(proxy);
                ((ICommunicationObject)proxy).Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("-----------------------------------------------------");
        }
    }
}
