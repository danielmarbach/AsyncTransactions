using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Newtonsoft.Json;

namespace AsyncTransactions
{
    public class Database
    {
        List<object> stored = new List<object>();
        private DatabaseStore store;

        public Database(string storePath)
        {
            store = new DatabaseStore(storePath);
        }

        public void Store(object entity)
        {
            stored.Add(entity);            
        }

        public Task SaveAsync()
        {
            return SaveAsync(Transaction.Current);
        }

        public async Task SaveAsync(Transaction transaction)
        {
            if (transaction == null)
            {
                await SaveInternalAsync();
                return;
            }

            await Task.FromResult(transaction.EnlistVolatile(new SaveResourceManager(SaveInternalAsync), EnlistmentOptions.None));
        }

        private async Task SaveInternalAsync()
        {
            foreach (var o in stored)
            {
                using(var stream = new MemoryStream())
                using (var writer = new JsonTextWriter(new StreamWriter(stream)))
                {
                    var serializer = JsonSerializer.Create();
                    serializer.Serialize(writer, o);
                    writer.Flush();
                    stream.Position = 0;

                    await store.AppendAsync(stream);

                    writer.Close();
                }
            }
        }

        public void Close()
        {
            store.Close();
        }
    }

    public class SaveResourceManager : IEnlistmentNotification
    {
        private readonly Func<Task> operation;

        public SaveResourceManager(Func<Task> operation)
        {
            this.operation = operation;
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        public void Commit(Enlistment enlistment)
        {
            operation().Wait();
            enlistment.Done();
        }

        public void Rollback(Enlistment enlistment)
        {
            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }
    }

    public class DatabaseStore
    {
        private FileStream fileStore;

        public DatabaseStore(string storePath)
        {
            fileStore = File.OpenWrite(storePath);
        }

        public async Task AppendAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            await stream.CopyToAsync(fileStore, 4096, cancellationToken);
            await fileStore.FlushAsync(cancellationToken);
        }

        public void Close()
        {
            fileStore.Close();
        }
    }

    internal class ResourceManager
    {
        
    }
}
