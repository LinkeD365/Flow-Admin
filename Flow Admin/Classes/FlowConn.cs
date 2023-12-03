namespace LinkeD365.FlowAdmin
{
    public class FlowConn : APIConn
    {
        //public string Name;
        //public string AppId;// = string.Empty;
        //public string TenantId = string.Empty;
        //public string ReturnURL = string.Empty;
        //public string Environment = string.Empty;
        //public bool UseDev;
        public new string Type = "Flow";
    }

    /// <summary>
    /// Legacy version of settings
    /// </summary>
    public class FlowConnection
    {
        public string AppId;// = string.Empty;
        public string TenantId = string.Empty;
        public string ReturnURL = string.Empty;
        public string Environment = string.Empty;
        public bool UseDev;
        public string SubscriptionId = string.Empty;

        public string LATenantId = string.Empty;
        public string LAAppId = string.Empty;
        public string LAReturnURL = string.Empty;
        public bool LAUseDev;
    }
}