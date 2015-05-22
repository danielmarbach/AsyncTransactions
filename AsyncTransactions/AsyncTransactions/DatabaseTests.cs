using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Transactions;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Namers.StackTraceParsers;
using ApprovalTests.Reporters;
using ApprovalTests.StackTraceParsers;
using NUnit.Framework;

namespace AsyncTransactions
{
    [TestFixture]
    [UseReporter(typeof(DiffReporter))]
    public class DatabaseTests
    {
        private Database database;

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task StoreAsync()
        {
            database = new Database("StoreAsync.received.txt");

            for (int i = 0; i < 10; i++)
            {
                database.Store(new Customer { Name = "Daniel" + i });
            }
            
            await database.SaveAsync();

            database.Close();

            // Doesn't seem to work properly
            // Approvals.VerifyFile("StoreAsync.received.txt");
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task StoreAsyncSupportsAmbientTransactionComplete()
        {
            database = new Database("StoreAsyncSupportsAmbientTransactionComplete.received.txt");

            for (int i = 0; i < 10; i++)
            {
                database.Store(new Customer { Name = "Daniel" + i });
            }

            using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await database.SaveAsync();

                tx.Complete();
            }

            database.Close();

            Approvals.VerifyFile("StoreAsyncSupportsAmbientTransactionComplete.received.txt");
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task StoreAsyncSupportsAmbientTransactionRollback()
        {
            database = new Database("StoreAsyncSupportsAmbientTransactionRollback.received.txt");

            for (int i = 0; i < 10; i++)
            {
                database.Store(new Customer { Name = "Daniel" + i });
            }

            using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await database.SaveAsync();
            }

            database.Close();

            Approvals.VerifyFile("StoreAsyncSupportsAmbientTransactionRollback.received.txt");
        }

        private class Customer
        {
            public string Name { get; set; }
        }
    }
}