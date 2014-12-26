/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;

namespace ReceivingServiceLib
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class MapExceptionToFaultAttribute : Attribute//, IOperationBehavior
    {
        internal Type ExceptionType { get; set; }
        internal Type FaultType { get; set; }

        private ConstructorInfo _exceptionConstructor;
        private ConstructorInfo _parameterlessConstructor;

        public MapExceptionToFaultAttribute(Type exceptionType, Type faultDetailType)
        {
            ExceptionType = exceptionType;
            FaultType = faultDetailType;

            Debug.Assert(typeof(Exception).IsAssignableFrom(ExceptionType));

            _exceptionConstructor = FaultType.GetConstructor(new Type[] { exceptionType });
            if (_exceptionConstructor == null)
                _parameterlessConstructor = FaultType.GetConstructor(Type.EmptyTypes);
            Debug.Assert(_exceptionConstructor != null || _parameterlessConstructor != null);
        }

        internal object GetFaultDetailForException(Exception exception)
        {
            if (_exceptionConstructor != null)
                return _exceptionConstructor.Invoke(new object[] { exception });
            if (_parameterlessConstructor != null)
                return _parameterlessConstructor.Invoke(new object[] { });

            Debug.Assert(false);
            return null;
        }

        //#region IOperationBehavior Members

        //public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        //{
        //}

        //public void ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation)
        //{
        //}

        //public void ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation)
        //{
        //    string faultName = this.FaultType.Name + "Fault";
        //    string faultAction = operationDescription.DeclaringContract.Namespace + '/' +
        //        operationDescription.DeclaringContract.Name + '/' +
        //        operationDescription.Name + '/' + faultName;
        //    operationDescription.Faults.Add(new FaultDescription(faultAction)
        //    {
        //        DetailType = this.FaultType,
        //        Name = faultName,
        //        Namespace = operationDescription.DeclaringContract.Namespace
        //    });
        //}

        //public void Validate(OperationDescription operationDescription)
        //{
        //}

        //#endregion
    }

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://www.codeproject.com/Articles/26320/WCF-Error-Handling-and-Fault-Conversion"/>
    public sealed class ErrorHandler : IErrorHandler
    {
        public static event ThreadExceptionEventHandler Error;

        private ErrorHandlingBehaviorAttribute m_attrib;
        private IExceptionToFaultConverter m_convert;

        public ErrorHandler(ErrorHandlingBehaviorAttribute attribute)
        {
            m_attrib = attribute;
            if (m_attrib.ExceptionToFaultConverter != null)
                m_convert = (IExceptionToFaultConverter)Activator.CreateInstance(m_attrib.ExceptionToFaultConverter);
        }

        public bool HandleError(Exception error)
        {
            if (Error != null)
                Error(this, new ThreadExceptionEventArgs(error));
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            //If it's a FaultException already, then we have nothing to do
            if (error is FaultException)
                return;

            ServiceEndpoint endpoint = OperationContext.Current.Host.Description.Endpoints.Find(OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri);
            DispatchOperation dispatchOperation = 
                OperationContext.Current.EndpointDispatcher.DispatchRuntime.Operations.Where(
                    op => op.Action == OperationContext.Current.IncomingMessageHeaders.Action).First();
            OperationDescription operationDesc = endpoint.Contract.Operations.Find(dispatchOperation.Name);

            object faultDetail = GetFaultDetail(operationDesc.SyncMethod, operationDesc.Faults, error);
            if (faultDetail != null)
            {
                Type faultExceptionType = typeof(FaultException<>).MakeGenericType(faultDetail.GetType());
                FaultException faultException = (FaultException)Activator.CreateInstance(faultExceptionType, faultDetail, error.Message);
                MessageFault faultMessage = faultException.CreateMessageFault();
                fault = Message.CreateMessage(version, faultMessage, faultException.Action);
            }
        }

        private object GetFaultDetail(MethodInfo method, FaultDescriptionCollection faults, Exception error)
        {
            object faultDetail = null;

            if (m_convert != null)
            {
                faultDetail = m_convert.ConvertExceptionToFaultDetail(error);
            }

            if (faultDetail == null && method != null)
            {
                MapExceptionToFaultAttribute[] mappers = (MapExceptionToFaultAttribute[])
                    method.GetCustomAttributes(typeof(MapExceptionToFaultAttribute), true);
                foreach (MapExceptionToFaultAttribute mapAttribute in mappers)
                {
                    if (mapAttribute.ExceptionType == error.GetType())
                    {
                        faultDetail = mapAttribute.GetFaultDetailForException(error);
                        if (faultDetail != null)
                        {
                            break;
                        }
                    }
                }
            }

            if (faultDetail != null && m_attrib.EnforceFaultContract &&
                !faults.Any(f => f.DetailType == faultDetail.GetType()))
            {
                faultDetail = null;
            }

            if (faultDetail == null)
            {
                foreach (FaultDescription faultDesc in faults)
                {
                    if (faultDesc.DetailType == error.GetType())
                    {
                        faultDetail = error;
                        break;
                    }
                }
            }

            return faultDetail;
        }
    }

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

    public interface IExceptionToFaultConverter
    {
        object ConvertExceptionToFaultDetail(Exception error);
    }

    public class MyServiceFaultProvider : IExceptionToFaultConverter
    {
        public object ConvertExceptionToFaultDetail(Exception error)
        {
            if (error is InvalidOperationException)
                return new InvalidOperationFault(error as InvalidOperationException);
            return null;
        }
    }

    [DataContract]
    public class MyApplicationFault
    {
    }

    [DataContract]
    public class InvalidOperationFault
    {
        [DataMember]
        public InvalidOperationException Exception { get; private set; }

        public InvalidOperationFault(InvalidOperationException e)
        {
            Exception = e;
        }
    }
}
