/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;

namespace ReceivingServiceLib
{
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
}
