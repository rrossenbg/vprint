﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FintraxPTFImages.AuthenticationRef {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AuthenticationRef.AuthenticationSoap")]
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
        FintraxPTFImages.AuthenticationRef.RetrieveUserResponse RetrieveUser(FintraxPTFImages.AuthenticationRef.RetrieveUserRequest request);
        
        // CODEGEN: Generating message contract since message CreateUserRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CreateUser", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        FintraxPTFImages.AuthenticationRef.CreateUserResponse CreateUser(FintraxPTFImages.AuthenticationRef.CreateUserRequest request);
        
        // CODEGEN: Generating message contract since message EncryptPasswordRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/EncryptPassword", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        FintraxPTFImages.AuthenticationRef.EncryptPasswordResponse EncryptPassword(FintraxPTFImages.AuthenticationRef.EncryptPasswordRequest request);
        
        // CODEGEN: Generating message contract since message DecryptPasswordRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/DecryptPassword", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        FintraxPTFImages.AuthenticationRef.DecryptPasswordResponse DecryptPassword(FintraxPTFImages.AuthenticationRef.DecryptPasswordRequest request);
        
        // CODEGEN: Generating message contract since message GenerateSaltRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GenerateSalt", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        FintraxPTFImages.AuthenticationRef.GenerateSaltResponse GenerateSalt(FintraxPTFImages.AuthenticationRef.GenerateSaltRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
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
        public FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int countryId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string userName;
        
        public RetrieveUserRequest() {
        }
        
        public RetrieveUserRequest(FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader, int countryId, string userName) {
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
        public FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int countryId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string userName;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public string password;
        
        public CreateUserRequest() {
        }
        
        public CreateUserRequest(FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader, int countryId, string userName, string password) {
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
        public FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string password;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string salt;
        
        public EncryptPasswordRequest() {
        }
        
        public EncryptPasswordRequest(FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader, string password, string salt) {
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
        public FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string encryptedPassword;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string salt;
        
        public DecryptPasswordRequest() {
        }
        
        public DecryptPasswordRequest(FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader, string encryptedPassword, string salt) {
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
        public FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader;
        
        public GenerateSaltRequest() {
        }
        
        public GenerateSaltRequest(FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader) {
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
    public interface AuthenticationSoapChannel : FintraxPTFImages.AuthenticationRef.AuthenticationSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AuthenticationSoapClient : System.ServiceModel.ClientBase<FintraxPTFImages.AuthenticationRef.AuthenticationSoap>, FintraxPTFImages.AuthenticationRef.AuthenticationSoap {
        
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
        FintraxPTFImages.AuthenticationRef.RetrieveUserResponse FintraxPTFImages.AuthenticationRef.AuthenticationSoap.RetrieveUser(FintraxPTFImages.AuthenticationRef.RetrieveUserRequest request) {
            return base.Channel.RetrieveUser(request);
        }
        
        public int RetrieveUser(FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader, int countryId, string userName) {
            FintraxPTFImages.AuthenticationRef.RetrieveUserRequest inValue = new FintraxPTFImages.AuthenticationRef.RetrieveUserRequest();
            inValue.AuthenticationHeader = AuthenticationHeader;
            inValue.countryId = countryId;
            inValue.userName = userName;
            FintraxPTFImages.AuthenticationRef.RetrieveUserResponse retVal = ((FintraxPTFImages.AuthenticationRef.AuthenticationSoap)(this)).RetrieveUser(inValue);
            return retVal.RetrieveUserResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        FintraxPTFImages.AuthenticationRef.CreateUserResponse FintraxPTFImages.AuthenticationRef.AuthenticationSoap.CreateUser(FintraxPTFImages.AuthenticationRef.CreateUserRequest request) {
            return base.Channel.CreateUser(request);
        }
        
        public int CreateUser(FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader, int countryId, string userName, string password) {
            FintraxPTFImages.AuthenticationRef.CreateUserRequest inValue = new FintraxPTFImages.AuthenticationRef.CreateUserRequest();
            inValue.AuthenticationHeader = AuthenticationHeader;
            inValue.countryId = countryId;
            inValue.userName = userName;
            inValue.password = password;
            FintraxPTFImages.AuthenticationRef.CreateUserResponse retVal = ((FintraxPTFImages.AuthenticationRef.AuthenticationSoap)(this)).CreateUser(inValue);
            return retVal.CreateUserResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        FintraxPTFImages.AuthenticationRef.EncryptPasswordResponse FintraxPTFImages.AuthenticationRef.AuthenticationSoap.EncryptPassword(FintraxPTFImages.AuthenticationRef.EncryptPasswordRequest request) {
            return base.Channel.EncryptPassword(request);
        }
        
        public string EncryptPassword(FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader, string password, string salt) {
            FintraxPTFImages.AuthenticationRef.EncryptPasswordRequest inValue = new FintraxPTFImages.AuthenticationRef.EncryptPasswordRequest();
            inValue.AuthenticationHeader = AuthenticationHeader;
            inValue.password = password;
            inValue.salt = salt;
            FintraxPTFImages.AuthenticationRef.EncryptPasswordResponse retVal = ((FintraxPTFImages.AuthenticationRef.AuthenticationSoap)(this)).EncryptPassword(inValue);
            return retVal.EncryptPasswordResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        FintraxPTFImages.AuthenticationRef.DecryptPasswordResponse FintraxPTFImages.AuthenticationRef.AuthenticationSoap.DecryptPassword(FintraxPTFImages.AuthenticationRef.DecryptPasswordRequest request) {
            return base.Channel.DecryptPassword(request);
        }
        
        public string DecryptPassword(FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader, string encryptedPassword, string salt) {
            FintraxPTFImages.AuthenticationRef.DecryptPasswordRequest inValue = new FintraxPTFImages.AuthenticationRef.DecryptPasswordRequest();
            inValue.AuthenticationHeader = AuthenticationHeader;
            inValue.encryptedPassword = encryptedPassword;
            inValue.salt = salt;
            FintraxPTFImages.AuthenticationRef.DecryptPasswordResponse retVal = ((FintraxPTFImages.AuthenticationRef.AuthenticationSoap)(this)).DecryptPassword(inValue);
            return retVal.DecryptPasswordResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        FintraxPTFImages.AuthenticationRef.GenerateSaltResponse FintraxPTFImages.AuthenticationRef.AuthenticationSoap.GenerateSalt(FintraxPTFImages.AuthenticationRef.GenerateSaltRequest request) {
            return base.Channel.GenerateSalt(request);
        }
        
        public string GenerateSalt(FintraxPTFImages.AuthenticationRef.AuthenticationHeader AuthenticationHeader) {
            FintraxPTFImages.AuthenticationRef.GenerateSaltRequest inValue = new FintraxPTFImages.AuthenticationRef.GenerateSaltRequest();
            inValue.AuthenticationHeader = AuthenticationHeader;
            FintraxPTFImages.AuthenticationRef.GenerateSaltResponse retVal = ((FintraxPTFImages.AuthenticationRef.AuthenticationSoap)(this)).GenerateSalt(inValue);
            return retVal.GenerateSaltResult;
        }
    }
}
