/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Diagnostics;
using System.Reflection;

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
}
