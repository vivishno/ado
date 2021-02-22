//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace BasicSupportAcisExtension.Operations
{
    using System;
    using System.Collections.Generic;
    using BasicSupportAcisExtension.OperationGroups;
    using Microsoft.WindowsAzure.Wapd.Acis.Contracts;
    using Microsoft.WindowsAzure.Wapd.Acis.Contracts.SimplificationClasses;

    /// <summary>
    /// Class for Add One Time Free Technical support entitlement operation
    /// </summary>
    class AddOneTimeFreeTechnicalSupportOperation : AcisSMEOperation
    {
        /// <summary>
        /// Claims for add One Time Free Technical support entitlement operation
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
            get { return "Add OneTime Free Technical Support"; }
        }

        /// <summary>
        /// Required parameter for operation - SubscriptionId
        /// </summary>
        public override IEnumerable<IAcisSMEParameterRef> Parameters
        {
            get { return new[] { AcisWellKnownParameters.Get(ParameterName.SubscriptionId) }; }
        }

        /// <summary>
        /// Adds subscriptionId into the One Time Free Technical support  table in order to entitle One Time Free Technical support
        /// </summary>
        /// <param name="subscriptionId">The user's SubscriptionId</param>
        /// <param name="extension">The extension context</param>
        /// <param name="endpoint">The extension endpoint name</param>
        /// <returns>Returns response string</returns>
        public IAcisSMEOperationResponse AddOneTimeFreeTechnicalSupport(string subscriptionId, IAcisServiceManagementExtension extension, IAcisSMEEndpoint endpoint, OperationExecutionContext context)
        {
            try
            {
                string storageName = endpoint.Configuration.GetConfigurationValue("StorageName");
                string storageKey = endpoint.Secrets.GetSecretAsync("storagekey").Result;
                string storageTableName = endpoint.Configuration.GetConfigurationValue("OneTimeFreeTechnicalSupportStorageTableName");
                subscriptionId = subscriptionId.ToLower();
                
                // Create table connection
                StorageTableDataAccess<OneTimeFreeTechnicalSupportTableEntity> astDataAccessor = new StorageTableDataAccess<OneTimeFreeTechnicalSupportTableEntity>(storageName, storageKey, storageTableName);
                // Make Partition key
                var partitionKey = WhiteListConstants.SubscriptionHeader + subscriptionId.Substring(subscriptionId.Length - WhiteListConstants.PartitionStringLength);
                var existingEntity = astDataAccessor.RetrieveOneTimeTechnicalSupportEntity(partitionKey, subscriptionId);

                if (existingEntity != null)
                {
                    if (existingEntity.IsFreeSupportUsed)
                    {
                        existingEntity.IsFreeSupportUsed = false;
                        existingEntity.SupportCount++;
                        var entity = astDataAccessor.Merge(existingEntity);
                        if (entity != null)
                        {
                            extension.Logger.LogVerbose(string.Format("Success. Subscription {0} is now entitled for one time free technical support. Note: This customer has also been entitled for one-time technical support {1} time(s) in the past.", subscriptionId, existingEntity.SupportCount));
                            return AcisSMEOperationResponseExtensions.BuildSuccess(string.Format("Success. Subscription {0} is now entitled for one time free technical support. Note: This customer has also been entitled for one-time technical support {1} time(s) in the past.", subscriptionId, existingEntity.SupportCount));
                        }
                        else
                        {
                            extension.Logger.LogVerbose(string.Format("Error. Entity returned after update is null. One time free support may not have been given to subscription {0}. Check entitlement using the CheckOneTimeFreeTechnicalSupport operation.", subscriptionId));
                            return AcisSMEOperationResponseExtensions.BuildException(new Exception("Update operation failed"), string.Format("Error. Entity returned after update is null. One time free support may not have been given to subscription {0}. Check entitlement using the CheckOneTimeFreeTechnicalSupport operation.", subscriptionId));
                        }
                    }
                    else
                    {
                        extension.Logger.LogVerbose(string.Format("Subscription {0} is already entitled for one time free technical support. No new entitlements were made. Note: This customer has also been entitled for one-time technical support {1} time(s) in the past.", subscriptionId, existingEntity.SupportCount));
                        return AcisSMEOperationResponseExtensions.BuildSuccess(string.Format("Subscription {0} is already entitled for one time free technical support. No new entitlements were made. Note: This customer has also been entitled for one-time technical support {1} time(s) in the past.", subscriptionId, existingEntity.SupportCount));
                    }
                }
                else
                {
                    // Insert into table storage
                    OneTimeFreeTechnicalSupportTableEntity otEntity = new OneTimeFreeTechnicalSupportTableEntity();
                    otEntity.PartitionKey = partitionKey;
                    otEntity.SubscriptionId = subscriptionId;
                    otEntity.IsFreeSupportUsed = false;
                    otEntity.SupportCount = 1;
                    otEntity.RowKey = subscriptionId;
                    otEntity.Cohort = context.CurrentUser.Name;
                    var entity = astDataAccessor.Insert(otEntity);
                    if (entity != null)
                    {
                        extension.Logger.LogVerbose(string.Format("Success. Subscription {0} is now entitled for one time free technical support. Note: This is the first time this subscription has been entitled for one time free technical support.", subscriptionId));
                        return AcisSMEOperationResponseExtensions.BuildSuccess(string.Format("Success. Subscription {0} is now entitled for one time free technical support. Note: This is the first time this subscription has been entitled for one time free technical support.", subscriptionId));
                    }
                    else
                    {
                        extension.Logger.LogVerbose(string.Format("Error. The insert operation failed. One time free support was not given to subscription {0}", subscriptionId));
                        return AcisSMEOperationResponseExtensions.BuildException(new Exception("Insert operation failed"), string.Format("Error. The insert operation failed. One time free support was not given to subscription {0}", subscriptionId));

                    }
                }
            }
            catch (Exception ex)
            {
                extension.Logger.LogVerbose(string.Format("Error. One time free support may not have been given to subscription {0}. Check entitlement using the CheckOneTimeFreeTechnicalSupport operation.", subscriptionId));
                return AcisSMEOperationResponseExtensions.BuildException(ex, string.Format("Error. One time free support may not have been given to subscription {0}. Check entitlement using the CheckOneTimeFreeTechnicalSupport operation.", subscriptionId));
            }
        }

        /// <summary>
        /// Data access level
        /// </summary>
        public override DataAccessLevel SystemMetadata
        {
            get
            {
                return DataAccessLevel.ReadWrite;
            }
        }
    }
}
