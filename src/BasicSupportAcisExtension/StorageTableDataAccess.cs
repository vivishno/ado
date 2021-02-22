//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace BasicSupportAcisExtension
{
    using System;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Storage table data access class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StorageTableDataAccess<T> where T : class, ITableEntity
    {
        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        public CloudTable table { get; set; }

        /// <summary>
        /// The Storage table name.
        /// </summary>
        private readonly string tableName;

        /// <summary> Initializes a new instance of the <see cref="StorageTableDataAccess{T}"/> class. </summary>
        /// <param name="accountName">The storage account name</param>
        /// <param name="storageKey">The storage key</param>
        /// <param name="tableName"> The table name. </param>
        /// <exception cref="ArgumentException">Throws argument exception if table name is invalid or empty</exception>
        public StorageTableDataAccess(string accountName, string storageKey, string tableName)
        {
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", accountName, storageKey);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            this.tableName = tableName;
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("Invalid storage table name (null or white spaces).");
            }

            this.table = tableClient.GetTableReference(this.tableName);
            table.CreateIfNotExists();
        }

        /// <summary> The retrieve. </summary>
        /// <param name="partitionKey"> The partition key. </param>
        /// <param name="rowKey"> The row key. </param>
        /// <returns> The <see cref="T"/>. </returns>
        public T Retrieve(string partitionKey, string rowKey)
        {
            var startTime = DateTime.Now;

            // Create a retrieve operation.
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            // Execute the retrieve operation.
            TableResult retrievedResult = this.table.Execute(retrieveOperation);
            T result = retrievedResult.Result as T;
            return result;
        }

        /// <summary> The insert. </summary>
        /// <param name="entity"> The entity. </param>
        /// <returns> The <see cref="T"/>. </returns>
        public T Insert(T entity)
        {
            TableOperation op = TableOperation.Insert(entity);
            TableResult tableResult = this.table.Execute(op);
            T result = tableResult.Result as T;

            return result;
        }

        /// <summary> The insert. </summary>
        /// <param name="entity"> The entity. </param>
        /// <returns> The <see cref="T"/>. </returns>
        public T Merge(T entity)
        {
            TableOperation op = TableOperation.Merge(entity);
            TableResult tableResult = this.table.Execute(op);
            T result = tableResult.Result as T;

            return result;
        }

        /// <summary>
        /// Checks if given PUID is present into the table or not
        /// </summary>
        /// <param name="partitionKey">The partition key</param>
        /// <param name="puid">The Puid</param>
        /// <returns>True if puid exists into the table else false</returns>
        public bool CheckIfPuidExists(string partitionKey, string puid)
        {
            var entity = Retrieve(partitionKey, puid);

            if (entity != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if given PUID is present into the table or not
        /// </summary>
        /// <param name="partitionKey">The partition key</param>
        /// <param name="puid">The Puid</param>
        /// <returns>True if puid exists into the table else false</returns>
        public T RetrieveOneTimeTechnicalSupportEntity(string partitionKey, string subscriptionId)
        {
            var entity = Retrieve(partitionKey, subscriptionId);
            return entity;
        }
    }
}
