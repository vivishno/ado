//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace BasicSupportAcisExtension
{
    using Microsoft.WindowsAzure.Wapd.Acis.Contracts;

    public class BasicSupportExtension : AcisServiceManagementExtension
    {
        /// <summary>
        /// Extension name
        /// </summary>
        static public string ServiceNameConst = "Basic Support";

        #region Required for ACIS SME infrastructure
        /// <summary>
        /// SME Service name
        /// Uniquely identifies the SME service.  
        /// In the future the loading of a SME will be managed using  both the Service Name and the Service version.
        /// </summary>
        public override string ServiceName { get { return ServiceNameConst; } }

        /// <summary>
        /// SME Sample Version
        /// In the future this version will allow multiple versions of a SME to be loaded at the same time.
        /// Currently, the extension version is entirely left in the hands of the SME writer.  
        /// There is no requirement to have a specific version or even to have the version increase in a predictable fashion.  
        /// </summary>
        public override string ExtensionVersion { get { return "1.0"; } }

        #endregion
    }
}
