//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace BasicSupportAcisExtension
{
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Basic support entity class
    /// </summary>
    class BasicSupportWhiteListTableEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets Puid
        /// </summary>
        public string Puid { get; set; }

        /// <summary>
        /// Gets or sets Subscription Id
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the Cohort 
        /// (PRE when entitlement via bulk import, ACIS when entitled via ACIS, PORTAL when auto entitled).
        /// </summary>
        public string Cohort { get; set; }
    }
}
