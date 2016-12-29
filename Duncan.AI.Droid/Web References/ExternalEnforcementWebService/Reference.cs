﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace Duncan.AI.Droid.ExternalEnforcementWebService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="BasicHttpBinding_IcitationsmiamiService", Namespace="http://tempuri.org/")]
    public partial class citationsmiamiService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetCitationDataOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public citationsmiamiService() {
            this.Url = "http://64.132.70.129/CitServ/citationsmiamiService.svc/basic";
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetCitationDataCompletedEventHandler GetCitationDataCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IcitationsmiamiService/GetCitationData", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute(IsNullable=true)]
        [return: System.Xml.Serialization.XmlArrayItemAttribute(Namespace="http://schemas.datacontract.org/2004/07/citationsmiamiService")]
        public VendorTransaction[] GetCitationData([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string EnforcementKey) {
            object[] results = this.Invoke("GetCitationData", new object[] {
                        EnforcementKey});
            return ((VendorTransaction[])(results[0]));
        }
        
        /// <remarks/>
        public void GetCitationDataAsync(string EnforcementKey) {
            this.GetCitationDataAsync(EnforcementKey, null);
        }
        
        /// <remarks/>
        public void GetCitationDataAsync(string EnforcementKey, object userState) {
            if ((this.GetCitationDataOperationCompleted == null)) {
                this.GetCitationDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetCitationDataOperationCompleted);
            }
            this.InvokeAsync("GetCitationData", new object[] {
                        EnforcementKey}, this.GetCitationDataOperationCompleted, userState);
        }
        
        private void OnGetCitationDataOperationCompleted(object arg) {
            if ((this.GetCitationDataCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetCitationDataCompleted(this, new GetCitationDataCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1067.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.datacontract.org/2004/07/citationsmiamiService")]
    public partial class VendorTransaction {
        
        private string crossStreet1Field;
        
        private string crossStreet2Field;
        
        private int customerIdField;
        
        private bool customerIdFieldSpecified;
        
        private string enfHourDescField;
        
        private string enforcementKeyField;
        
        private System.Nullable<System.DateTime> expiredMinutesField;
        
        private bool expiredMinutesFieldSpecified;
        
        private System.Nullable<System.DateTime> expiryTimeField;
        
        private bool expiryTimeFieldSpecified;
        
        private System.Nullable<System.DateTime> meterExpiredMinutesField;
        
        private bool meterExpiredMinutesFieldSpecified;
        
        private string meterNameField;
        
        private string meterStreetField;
        
        private string plateNumberField;
        
        private System.Nullable<System.DateTime> sensorEventTimeField;
        
        private bool sensorEventTimeFieldSpecified;
        
        private int spaceNoField;
        
        private bool spaceNoFieldSpecified;
        
        private string spaceStatusField;
        
        private string streetTypeField;
        
        private string vMakeField;
        
        private string vModelField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string CrossStreet1 {
            get {
                return this.crossStreet1Field;
            }
            set {
                this.crossStreet1Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string CrossStreet2 {
            get {
                return this.crossStreet2Field;
            }
            set {
                this.crossStreet2Field = value;
            }
        }
        
        /// <remarks/>
        public int CustomerId {
            get {
                return this.customerIdField;
            }
            set {
                this.customerIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CustomerIdSpecified {
            get {
                return this.customerIdFieldSpecified;
            }
            set {
                this.customerIdFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string EnfHourDesc {
            get {
                return this.enfHourDescField;
            }
            set {
                this.enfHourDescField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string EnforcementKey {
            get {
                return this.enforcementKeyField;
            }
            set {
                this.enforcementKeyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> ExpiredMinutes {
            get {
                return this.expiredMinutesField;
            }
            set {
                this.expiredMinutesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ExpiredMinutesSpecified {
            get {
                return this.expiredMinutesFieldSpecified;
            }
            set {
                this.expiredMinutesFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> ExpiryTime {
            get {
                return this.expiryTimeField;
            }
            set {
                this.expiryTimeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ExpiryTimeSpecified {
            get {
                return this.expiryTimeFieldSpecified;
            }
            set {
                this.expiryTimeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> MeterExpiredMinutes {
            get {
                return this.meterExpiredMinutesField;
            }
            set {
                this.meterExpiredMinutesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MeterExpiredMinutesSpecified {
            get {
                return this.meterExpiredMinutesFieldSpecified;
            }
            set {
                this.meterExpiredMinutesFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string MeterName {
            get {
                return this.meterNameField;
            }
            set {
                this.meterNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string MeterStreet {
            get {
                return this.meterStreetField;
            }
            set {
                this.meterStreetField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string PlateNumber {
            get {
                return this.plateNumberField;
            }
            set {
                this.plateNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> SensorEventTime {
            get {
                return this.sensorEventTimeField;
            }
            set {
                this.sensorEventTimeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SensorEventTimeSpecified {
            get {
                return this.sensorEventTimeFieldSpecified;
            }
            set {
                this.sensorEventTimeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public int SpaceNo {
            get {
                return this.spaceNoField;
            }
            set {
                this.spaceNoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SpaceNoSpecified {
            get {
                return this.spaceNoFieldSpecified;
            }
            set {
                this.spaceNoFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string SpaceStatus {
            get {
                return this.spaceStatusField;
            }
            set {
                this.spaceStatusField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string StreetType {
            get {
                return this.streetTypeField;
            }
            set {
                this.streetTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string VMake {
            get {
                return this.vMakeField;
            }
            set {
                this.vMakeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string VModel {
            get {
                return this.vModelField;
            }
            set {
                this.vModelField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void GetCitationDataCompletedEventHandler(object sender, GetCitationDataCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetCitationDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetCitationDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public VendorTransaction[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((VendorTransaction[])(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591