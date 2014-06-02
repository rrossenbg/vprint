﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VPrinting.ScanServiceLocalRef {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ScanServiceLocalRef.IScanServiceLocal")]
    public interface IScanServiceLocal {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IScanServiceLocal/StartScan", ReplyAction="http://tempuri.org/IScanServiceLocal/StartScanResponse")]
        void StartScan(string directory);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IScanServiceLocal/StartScan", ReplyAction="http://tempuri.org/IScanServiceLocal/StartScanResponse")]
        System.IAsyncResult BeginStartScan(string directory, System.AsyncCallback callback, object asyncState);
        
        void EndStartScan(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IScanServiceLocal/StopScan", ReplyAction="http://tempuri.org/IScanServiceLocal/StopScanResponse")]
        void StopScan();
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IScanServiceLocal/StopScan", ReplyAction="http://tempuri.org/IScanServiceLocal/StopScanResponse")]
        System.IAsyncResult BeginStopScan(System.AsyncCallback callback, object asyncState);
        
        void EndStopScan(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IScanServiceLocalChannel : VPrinting.ScanServiceLocalRef.IScanServiceLocal, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ScanServiceLocalClient : System.ServiceModel.ClientBase<VPrinting.ScanServiceLocalRef.IScanServiceLocal>, VPrinting.ScanServiceLocalRef.IScanServiceLocal {
        
        private BeginOperationDelegate onBeginStartScanDelegate;
        
        private EndOperationDelegate onEndStartScanDelegate;
        
        private System.Threading.SendOrPostCallback onStartScanCompletedDelegate;
        
        private BeginOperationDelegate onBeginStopScanDelegate;
        
        private EndOperationDelegate onEndStopScanDelegate;
        
        private System.Threading.SendOrPostCallback onStopScanCompletedDelegate;
        
        public ScanServiceLocalClient() {
        }
        
        public ScanServiceLocalClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ScanServiceLocalClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ScanServiceLocalClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ScanServiceLocalClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> StartScanCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> StopScanCompleted;
        
        public void StartScan(string directory) {
            base.Channel.StartScan(directory);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginStartScan(string directory, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginStartScan(directory, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndStartScan(System.IAsyncResult result) {
            base.Channel.EndStartScan(result);
        }
        
        private System.IAsyncResult OnBeginStartScan(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string directory = ((string)(inValues[0]));
            return this.BeginStartScan(directory, callback, asyncState);
        }
        
        private object[] OnEndStartScan(System.IAsyncResult result) {
            this.EndStartScan(result);
            return null;
        }
        
        private void OnStartScanCompleted(object state) {
            if ((this.StartScanCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.StartScanCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void StartScanAsync(string directory) {
            this.StartScanAsync(directory, null);
        }
        
        public void StartScanAsync(string directory, object userState) {
            if ((this.onBeginStartScanDelegate == null)) {
                this.onBeginStartScanDelegate = new BeginOperationDelegate(this.OnBeginStartScan);
            }
            if ((this.onEndStartScanDelegate == null)) {
                this.onEndStartScanDelegate = new EndOperationDelegate(this.OnEndStartScan);
            }
            if ((this.onStartScanCompletedDelegate == null)) {
                this.onStartScanCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnStartScanCompleted);
            }
            base.InvokeAsync(this.onBeginStartScanDelegate, new object[] {
                        directory}, this.onEndStartScanDelegate, this.onStartScanCompletedDelegate, userState);
        }
        
        public void StopScan() {
            base.Channel.StopScan();
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginStopScan(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginStopScan(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndStopScan(System.IAsyncResult result) {
            base.Channel.EndStopScan(result);
        }
        
        private System.IAsyncResult OnBeginStopScan(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return this.BeginStopScan(callback, asyncState);
        }
        
        private object[] OnEndStopScan(System.IAsyncResult result) {
            this.EndStopScan(result);
            return null;
        }
        
        private void OnStopScanCompleted(object state) {
            if ((this.StopScanCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.StopScanCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void StopScanAsync() {
            this.StopScanAsync(null);
        }
        
        public void StopScanAsync(object userState) {
            if ((this.onBeginStopScanDelegate == null)) {
                this.onBeginStopScanDelegate = new BeginOperationDelegate(this.OnBeginStopScan);
            }
            if ((this.onEndStopScanDelegate == null)) {
                this.onEndStopScanDelegate = new EndOperationDelegate(this.OnEndStopScan);
            }
            if ((this.onStopScanCompletedDelegate == null)) {
                this.onStopScanCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnStopScanCompleted);
            }
            base.InvokeAsync(this.onBeginStopScanDelegate, null, this.onEndStopScanDelegate, this.onStopScanCompletedDelegate, userState);
        }
    }
}
