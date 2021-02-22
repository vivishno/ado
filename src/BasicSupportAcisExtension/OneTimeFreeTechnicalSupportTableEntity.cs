//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace BasicSupportAcisExtension
{
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// One Time Free Technical support entity class
    /// </summary>
    class OneTimeFreeTechnicalSupportTableEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets Subscription Id
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets IsFreeSupportUsed
        /// </summary>
        public bool IsFreeSupportUsed { get; set; }

        /// <summary>
        /// Gets or sets the SupportCount 
        /// </summary>
        public int SupportCount { get; set; }

        /// <summary>
        /// Gets or sets the Cohort 
        /// </summary>
        public string Cohort { get; set; }
    }
}
