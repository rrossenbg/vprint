using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace ErrorHandlingBehaviorLibrary
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ErrorHandlingBehaviorAttribute : Attribute, IServiceBehavior
    {
        private Type _exceptionToFaultConverterType;

        public bool EnforceFaultContract { get; set; }
        public Type ExceptionToFaultConverter
        {
            get
            {
                return _exceptionToFaultConverterType;
            }
            set
            {
                if (!typeof(IExceptionToFaultConverter).IsAssignableFrom(value))
                    throw new ArgumentException("Fault converter doesn't implement IExceptionToFaultConverter.", "value");
                _exceptionToFaultConverterType = value;
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, 
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase chanDispBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = chanDispBase as ChannelDispatcher;
                if (channelDispatcher == null)
                    continue;
                channelDispatcher.ErrorHandlers.Add(new ErrorHandler(this));
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}
