﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18034
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PremierTaxFree.PTFLib.AuthenticationProxy {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AuthenticationProxy.AuthenticationSoap")]
    public interface AuthenticationSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AuthenticateUser", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string AuthenticateUser(int countryId, string userName, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ValidateUser", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool ValidateUser(int countryId, string userName, string encryptedPassword, string salt);
        
        // CODEGEN: Generating message contract since message RetrieveUserRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/RetrieveUser", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        PremierTaxFree.PTFLib.AuthenticationProxy.RetrieveUserResponse RetrieveUser(PremierTaxFree.PTFLib.AuthenticationProxy.RetrieveUserRequest request);
        
        // CODEGEN: Generating message contract since message CreateUserRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CreateUser", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        PremierTaxFree.PTFLib.AuthenticationProxy.CreateUserResponse CreateUser(PremierTaxFree.PTFLib.AuthenticationProxy.CreateUserRequest request);
        
        // CODEGEN: Generating message contract since message EncryptPasswordRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/EncryptPassword", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        PremierTaxFree.PTFLib.AuthenticationProxy.EncryptPasswordResponse EncryptPassword(PremierTaxFree.PTFLib.AuthenticationProxy.EncryptPasswordRequest request);
        
        // CODEGEN: Generating message contract since message DecryptPasswordRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/DecryptPassword", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        PremierTaxFree.PTFLib.AuthenticationProxy.DecryptPasswordResponse DecryptPassword(PremierTaxFree.PTFLib.AuthenticationProxy.DecryptPasswordRequest request);
        
        // CODEGEN: Generating message contract since message GenerateSaltRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GenerateSalt", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        PremierTaxFree.PTFLib.AuthenticationProxy.GenerateSaltResponse GenerateSalt(PremierTaxFree.PTFLib.AuthenticationProxy.GenerateSaltRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18034")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class AuthenticationHeader : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string userNameField;
        
        private string passwordField;
        
        private string saltField;
        
        private int countryIdField;
        
        private System.Xml.XmlAttribute[] anyAttrField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string UserName {
            get {
                return this.userNameField;
            }
            set {
                this.userNameField = value;
                this.RaisePropertyChanged("UserName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string Password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
                this.RaisePropertyChanged("Password");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string Salt {
            get {
                return this.saltField;
            }
            set {
                this.saltField = value;
                this.RaisePropertyChanged("Salt");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public int CountryId {
            get {
                return this.countryIdField;
            }
            set {
                this.countryIdField = value;
                this.RaisePropertyChanged("CountryId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr {
            get {
                return this.anyAttrField;
            }
            set {
                this.anyAttrField = value;
                this.RaisePropertyChanged("AnyAttr");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RetrieveUser", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class RetrieveUserRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int countryId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string userName;
        
        public RetrieveUserRequest() {
        }
        
        public RetrieveUserRequest(PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader, int countryId, string userName) {
            this.AuthenticationHeader = AuthenticationHeader;
            this.countryId = countryId;
            this.userName = userName;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RetrieveUserResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class RetrieveUserResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int RetrieveUserResult;
        
        public RetrieveUserResponse() {
        }
        
        public RetrieveUserResponse(int RetrieveUserResult) {
            this.RetrieveUserResult = RetrieveUserResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CreateUser", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class CreateUserRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int countryId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string userName;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public string password;
        
        public CreateUserRequest() {
        }
        
        public CreateUserRequest(PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader, int countryId, string userName, string password) {
            this.AuthenticationHeader = AuthenticationHeader;
            this.countryId = countryId;
            this.userName = userName;
            this.password = password;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CreateUserResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class CreateUserResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int CreateUserResult;
        
        public CreateUserResponse() {
        }
        
        public CreateUserResponse(int CreateUserResult) {
            this.CreateUserResult = CreateUserResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="EncryptPassword", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class EncryptPasswordRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string password;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string salt;
        
        public EncryptPasswordRequest() {
        }
        
        public EncryptPasswordRequest(PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader, string password, string salt) {
            this.AuthenticationHeader = AuthenticationHeader;
            this.password = password;
            this.salt = salt;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="EncryptPasswordResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class EncryptPasswordResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string EncryptPasswordResult;
        
        public EncryptPasswordResponse() {
        }
        
        public EncryptPasswordResponse(string EncryptPasswordResult) {
            this.EncryptPasswordResult = EncryptPasswordResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DecryptPassword", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class DecryptPasswordRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string encryptedPassword;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string salt;
        
        public DecryptPasswordRequest() {
        }
        
        public DecryptPasswordRequest(PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader, string encryptedPassword, string salt) {
            this.AuthenticationHeader = AuthenticationHeader;
            this.encryptedPassword = encryptedPassword;
            this.salt = salt;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DecryptPasswordResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class DecryptPasswordResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string DecryptPasswordResult;
        
        public DecryptPasswordResponse() {
        }
        
        public DecryptPasswordResponse(string DecryptPasswordResult) {
            this.DecryptPasswordResult = DecryptPasswordResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GenerateSalt", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GenerateSaltRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader;
        
        public GenerateSaltRequest() {
        }
        
        public GenerateSaltRequest(PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader) {
            this.AuthenticationHeader = AuthenticationHeader;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GenerateSaltResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GenerateSaltResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string GenerateSaltResult;
        
        public GenerateSaltResponse() {
        }
        
        public GenerateSaltResponse(string GenerateSaltResult) {
            this.GenerateSaltResult = GenerateSaltResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface AuthenticationSoapChannel : PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AuthenticationSoapClient : System.ServiceModel.ClientBase<PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationSoap>, PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationSoap {
        
        public AuthenticationSoapClient() {
        }
        
        public AuthenticationSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AuthenticationSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AuthenticationSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AuthenticationSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string AuthenticateUser(int countryId, string userName, string password) {
            return base.Channel.AuthenticateUser(countryId, userName, password);
        }
        
        public bool ValidateUser(int countryId, string userName, string encryptedPassword, string salt) {
            return base.Channel.ValidateUser(countryId, userName, encryptedPassword, salt);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PremierTaxFree.PTFLib.AuthenticationProxy.RetrieveUserResponse PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationSoap.RetrieveUser(PremierTaxFree.PTFLib.AuthenticationProxy.RetrieveUserRequest request) {
            return base.Channel.RetrieveUser(request);
        }
        
        public int RetrieveUser(PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader, int countryId, string userName) {
            PremierTaxFree.PTFLib.AuthenticationProxy.RetrieveUserRequest inValue = new PremierTaxFree.PTFLib.AuthenticationProxy.RetrieveUserRequest();
            inValue.AuthenticationHeader = AuthenticationHeader;
            inValue.countryId = countryId;
            inValue.userName = userName;
            PremierTaxFree.PTFLib.AuthenticationProxy.RetrieveUserResponse retVal = ((PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationSoap)(this)).RetrieveUser(inValue);
            return retVal.RetrieveUserResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PremierTaxFree.PTFLib.AuthenticationProxy.CreateUserResponse PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationSoap.CreateUser(PremierTaxFree.PTFLib.AuthenticationProxy.CreateUserRequest request) {
            return base.Channel.CreateUser(request);
        }
        
        public int CreateUser(PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader, int countryId, string userName, string password) {
            PremierTaxFree.PTFLib.AuthenticationProxy.CreateUserRequest inValue = new PremierTaxFree.PTFLib.AuthenticationProxy.CreateUserRequest();
            inValue.AuthenticationHeader = AuthenticationHeader;
            inValue.countryId = countryId;
            inValue.userName = userName;
            inValue.password = password;
            PremierTaxFree.PTFLib.AuthenticationProxy.CreateUserResponse retVal = ((PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationSoap)(this)).CreateUser(inValue);
            return retVal.CreateUserResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PremierTaxFree.PTFLib.AuthenticationProxy.EncryptPasswordResponse PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationSoap.EncryptPassword(PremierTaxFree.PTFLib.AuthenticationProxy.EncryptPasswordRequest request) {
            return base.Channel.EncryptPassword(request);
        }
        
        public string EncryptPassword(PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader, string password, string salt) {
            PremierTaxFree.PTFLib.AuthenticationProxy.EncryptPasswordRequest inValue = new PremierTaxFree.PTFLib.AuthenticationProxy.EncryptPasswordRequest();
            inValue.AuthenticationHeader = AuthenticationHeader;
            inValue.password = password;
            inValue.salt = salt;
            PremierTaxFree.PTFLib.AuthenticationProxy.EncryptPasswordResponse retVal = ((PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationSoap)(this)).EncryptPassword(inValue);
            return retVal.EncryptPasswordResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PremierTaxFree.PTFLib.AuthenticationProxy.DecryptPasswordResponse PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationSoap.DecryptPassword(PremierTaxFree.PTFLib.AuthenticationProxy.DecryptPasswordRequest request) {
            return base.Channel.DecryptPassword(request);
        }
        
        public string DecryptPassword(PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader, string encryptedPassword, string salt) {
            PremierTaxFree.PTFLib.AuthenticationProxy.DecryptPasswordRequest inValue = new PremierTaxFree.PTFLib.AuthenticationProxy.DecryptPasswordRequest();
            inValue.AuthenticationHeader = AuthenticationHeader;
            inValue.encryptedPassword = encryptedPassword;
            inValue.salt = salt;
            PremierTaxFree.PTFLib.AuthenticationProxy.DecryptPasswordResponse retVal = ((PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationSoap)(this)).DecryptPassword(inValue);
            return retVal.DecryptPasswordResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PremierTaxFree.PTFLib.AuthenticationProxy.GenerateSaltResponse PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationSoap.GenerateSalt(PremierTaxFree.PTFLib.AuthenticationProxy.GenerateSaltRequest request) {
            return base.Channel.GenerateSalt(request);
        }
        
        public string GenerateSalt(PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationHeader AuthenticationHeader) {
            PremierTaxFree.PTFLib.AuthenticationProxy.GenerateSaltRequest inValue = new PremierTaxFree.PTFLib.AuthenticationProxy.GenerateSaltRequest();
            inValue.AuthenticationHeader = AuthenticationHeader;
            PremierTaxFree.PTFLib.AuthenticationProxy.GenerateSaltResponse retVal = ((PremierTaxFree.PTFLib.AuthenticationProxy.AuthenticationSoap)(this)).GenerateSalt(inValue);
            return retVal.GenerateSaltResult;
        }
    }
}
