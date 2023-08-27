using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LinkeD365.FlowAdmin
{
    internal class Utils
    {
        private static AppInsights ai;

        private const string aiEndpoint = "https://dc.services.visualstudio.com/v2/track";

        private const string aiKey = "cc383234-dfdb-429a-a970-d17847361df3";

        public static AppInsights Ai
        {
            get
            {
                if (ai == null)
                {
                    ai = new AppInsights(aiEndpoint, aiKey, Assembly.GetExecutingAssembly());
                    ai.WriteEvent("Control Loaded");
                }
                return ai;
            }
        }
    }
}