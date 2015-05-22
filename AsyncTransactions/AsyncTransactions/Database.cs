using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using Newtonsoft.Json;

namespace AsyncTransactions
{
    public class Database
    {
        readonly List<object> stored = new List<object>();
        private readonly DatabaseStore store;

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

            // await Task.FromResult(transaction.EnlistVolatile(new SynchronousSaveResourceManager(SaveInternalAsync), EnlistmentOptions.None));

            await Task.FromResult(transaction.EnlistVolatile(new DangerousResourceManager(SaveInternalAsync), EnlistmentOptions.None));

            // await Task.FromResult(transaction.EnlistVolatile(new AsynchronousBlockingResourceManager(SaveInternalAsync), EnlistmentOptions.None));
        }

        private async Task SaveInternalAsync()
        {
            foreach (var o in stored)
            {
                throw new DirectoryNotFoundException();
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
}
