using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace ErrorHandlingBehaviorLibrary
{
    sealed class ErrorHandler : IErrorHandler
    {
        private ErrorHandlingBehaviorAttribute _attribute;
        private IExceptionToFaultConverter _converter;

        public ErrorHandler(ErrorHandlingBehaviorAttribute attribute)
        {
            _attribute = attribute;
            if (_attribute.ExceptionToFaultConverter != null)
                _converter = (IExceptionToFaultConverter)Activator.CreateInstance(_attribute.ExceptionToFaultConverter);
        }

        public bool HandleError(Exception error)
        {
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            //If it's a FaultException already, then we have nothing to do
            if (error is FaultException)
                return;

            ServiceEndpoint endpoint =
                OperationContext.Current.Host.Description.Endpoints.Find(
                    OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri);
            DispatchOperation dispatchOperation =
                OperationContext.Current.EndpointDispatcher.DispatchRuntime.Operations.Where(
                    op => op.Action == OperationContext.Current.IncomingMessageHeaders.Action).First();
            OperationDescription operationDesc =
                endpoint.Contract.Operations.Find(dispatchOperation.Name);

            object faultDetail = GetFaultDetail(operationDesc.SyncMethod, operationDesc.Faults, error);
            if (faultDetail != null)
            {
                Type faultExceptionType =
                    typeof(FaultException<>).MakeGenericType(faultDetail.GetType());
                FaultException faultException =
                    (FaultException)Activator.CreateInstance(faultExceptionType, faultDetail, error.Message);
                MessageFault faultMessage = faultException.CreateMessageFault();
                fault = Message.CreateMessage(version, faultMessage, faultException.Action);
            }
        }

        private object GetFaultDetail(MethodInfo method, FaultDescriptionCollection faults, Exception error)
        {
            object faultDetail = null;

            if (_converter != null)
            {
                faultDetail = _converter.ConvertExceptionToFaultDetail(error);
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

            if (faultDetail != null && _attribute.EnforceFaultContract &&
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
