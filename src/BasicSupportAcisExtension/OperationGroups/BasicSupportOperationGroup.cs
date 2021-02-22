//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace BasicSupportAcisExtension.OperationGroups
{
    using Microsoft.WindowsAzure.Wapd.Acis.Contracts;

    /// <summary>
    /// Basic support operation group class
    /// </summary>
    class BasicSupportOperationGroup : AcisSMEOperationGroup
    {
        /// <summary>
        /// Basic support operation group name
        /// </summary>
        public override string Name
        {
            get { return "Basic Support Operations"; }
        }
    }
}
