//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace BasicSupportAcisExtension.Operations
{
    using System;
    using System.Collections.Generic;
    using BasicSupportAcisExtension.OperationGroups;
    using Microsoft.WindowsAzure.Wapd.Acis.Contracts;

    /// <summary>
    /// Class for check One Time Free Technical support entitlement
    /// </summary>
    class CheckOneTimeFreeTechnicalSupportOperation : AcisSMEOperation
    {
        /// <summary>
        /// Claims for check One Time Free Technical support entitlement
        /// </summary>
        public override IEnumerable<AcisUserClaim> ClaimsRequired
        {
            get
            {
                return new[] { AcisSMESecurityGroup.ClientPlatformServiceOperator("Azcoms"),
                                 AcisSMESecurityGroup.ClientPlatformServiceOperator("ASMSQA"),
                                 AcisSMESecurityGroup.ClientCustomerServiceOperator("Subscription"),
                                 AcisSMESecurityGroup.ClientPlatformServiceAdministrator("BasicSupport")
                };
            }
        }

        /// <summary>
        /// Operation group
        /// </summary>
        public override IAcisSMEOperationGroup OperationGroup
        {
            get { return new BasicSupportOperationGroup(); }
        }

        /// <summary>
        /// Operation name
        /// </summary>
        public override string OperationName
        {
            get { return "Check one time free technical support"; }
        }

        /// <summary>
        /// Required parameter for operation - SubscriptionId
        /// </summary>
        public override IEnumerable<IAcisSMEParameterRef> Parameters
        {
            get { return new[] { AcisWellKnownParameters.Get(ParameterName.SubscriptionId) }; }
        }

        /// <summary>
        /// Check given subscriptionId is present into the One Time Free Technical table or not
        /// </summary>
        /// <param name="subscriptionId">The user's subscription Id</param>
        /// <param name="extension">The extension context</param>
        /// <param name="endpoint">The extension endpoint</param>
        /// <returns></returns>
        public IAcisSMEOperationResponse CheckOneTimeFreeTechnicalSupport(string subscriptionId, IAcisServiceManagementExtension extension, IAcisSMEEndpoint endpoint)
        {
            string storageKey = string.Empty;
            subscriptionId = subscriptionId.ToLower();
            try
            {
                string storageName = endpoint.Configuration.GetConfigurationValue("StorageName");
                storageKey = endpoint.Secrets.GetSecretAsync("storagekey").Result;
                string storageTableName = endpoint.Configuration.GetConfigurationValue("OneTimeFreeTechnicalSupportStorageTableName");

                extension.Logger.LogVerbose(string.Format("Supplied Subscription Id {0}", subscriptionId));

                // Create table connection
                StorageTableDataAccess<OneTimeFreeTechnicalSupportTableEntity> astDataAccessor = new StorageTableDataAccess<OneTimeFreeTechnicalSupportTableEntity>(storageName, storageKey, storageTableName);

                // Make Partition key
                var partitionKey = WhiteListConstants.SubscriptionHeader + subscriptionId.Substring(subscriptionId.Length - WhiteListConstants.PartitionStringLength);
                var existingEntity = astDataAccessor.RetrieveOneTimeTechnicalSupportEntity(partitionKey, subscriptionId);

                // Check if subscription is already present in the table
                if (existingEntity != null)
                {
                    if (!existingEntity.IsFreeSupportUsed)
                    {
                        extension.Logger.LogVerbose(string.Format("Subscription {0} is already entitled for one-time free technical support. Note: This customer has been entitled for one-time free technical support {1} time(s) in the past and the most recent entitlement has not been used.", subscriptionId, existingEntity.SupportCount));
                        return AcisSMEOperationResponseExtensions.BuildSuccess(string.Format("Subscription {0} is already entitled for one-time free technical support. Note: This customer has been entitled for one-time free technical support {1} time(s) in the past and the most recent entitlement has not been used.", subscriptionId, existingEntity.SupportCount));
                    }
                    else
                    {
                        extension.Logger.LogVerbose(string.Format("Subscription {0} is eligible for one-time free technical support. Note: This customer has been entitled for one-time free technical support {1} time(s) in the past.", subscriptionId, existingEntity.SupportCount));
                        return AcisSMEOperationResponseExtensions.BuildSuccess(string.Format("Subscription {0} is eligible for one-time free technical support. Note: This customer has been entitled for one-time free technical support {1} time(s) in the past.", subscriptionId, existingEntity.SupportCount));
                    }
                }
                else
                {
                    extension.Logger.LogVerbose(string.Format("Subscription {0} is eligible for one-time free technical support.", subscriptionId));
                    return AcisSMEOperationResponseExtensions.BuildSuccess(string.Format("Subscription {0} is eligible for one-time free technical support.", subscriptionId));
                }
            }
            catch (Exception ex)
            {
                extension.Logger.LogVerbose(string.Format("Error. Unable to verify whether subscription {0} is eligible for one time free technical support.", subscriptionId));
                return AcisSMEOperationResponseExtensions.BuildException(ex, string.Format("Error. Unable to verify whether subscription {0} is eligible for one time free technical support.", subscriptionId));
            }
        }

        /// <summary>
        /// Data access level
        /// </summary>
        public override DataAccessLevel SystemMetadata
        {
            get
            {
                return DataAccessLevel.ReadOnly;
            }
        }
    }
}
