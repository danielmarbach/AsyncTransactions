using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Transactions;
using ApprovalTests;
using NUnit.Framework;

namespace AsyncTransactions
{
    [TestFixture]
    public class Script
    {
        [Test]
        [TestCase("EntityFramework")]
        public void TheStart(string EntityFramework = null)
        {
            var daniel = new DanielMarbach();
            daniel
                .Is("CEO").Of("tracelight Gmbh").In("Switzerland")
                .and
                .WorkingFor("Particular Software").TheFolksBehind("NServiceBus");

            throw new ArgumentOutOfRangeException(nameof(EntityFramework), "Sorry this presentation is not about Entity Framework");
        }

        [Test]
        public async Task TransactionScopeIntro()
        {
            var slide = new Slide(title: "Transaction Scope Intro");
            await slide
                .BulletPoint("System.Transactions.TransactionScope")
                .BulletPoint("Implicit programing model / Ambient Transactions")

                .Sample(() =>
                {
                    Assert.Null(Transaction.Current);

                    using (var tx = new TransactionScope())
                    {
                        Assert.NotNull(Transaction.Current);

                        SomeMethodInTheCallStack();

                        tx.Complete();
                    }

                    Assert.Null(Transaction.Current);
                })

                .BulletPoint("Only works with async/await with .NET 4.5.1");
        }

        private static void SomeMethodInTheCallStack()
        {
            Assert.NotNull(Transaction.Current);
        }

        [Test]
        public async Task TransactionScopeAsync()
        {
            var slide = new Slide(title: "Transaction Scope Async");
            await slide

                .Sample(async () =>
                {
                    Assert.Null(Transaction.Current);

                    using (var tx = new TransactionScope())
                    {
                        Assert.NotNull(Transaction.Current);

                        await SomeMethodInTheCallStackAsync().ConfigureAwait(false);

                        tx.Complete();
                    }

                    Assert.Null(Transaction.Current);
                });
        }

        [Test]
        public async Task TransactionScopeAsyncProper()
        {
            var slide = new Slide(title: "Transaction Scope Async");
            await slide

                .Sample(async () =>
                {
                    Assert.Null(Transaction.Current);

                    using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        Assert.NotNull(Transaction.Current);

                        await SomeMethodInTheCallStackAsync().ConfigureAwait(false);

                        tx.Complete();
                    }

                    Assert.Null(Transaction.Current);
                });
        }

        private static async Task SomeMethodInTheCallStackAsync()
        {
            await Task.Delay(500).ConfigureAwait(false);

            Assert.NotNull(Transaction.Current);
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task StoreAsync()
        {
            var slide = new Slide(title: "Store Async");
            await slide
                .Sample(async () =>
                {
                    var database = new Database("StoreAsync.received.txt");

                    for (int i = 0; i < 10; i++)
                    {
                        database.Store(new Customer { Name = "Daniel" + i });
                    }

                    await database.SaveAsync();

                    database.Close();

                    // Doesn't seem to work properly, bug in approval tests?
                    // Approvals.VerifyFile("StoreAsync.received.txt");
                });
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task StoreAsyncSupportsAmbientTransactionComplete()
        {
            var slide = new Slide(title: "Store Async supports ambient transactions - complete");
            await slide
                .Sample(async () =>
                {
                    var database = new Database("StoreAsyncSupportsAmbientTransactionComplete.received.txt");

                    for (int i = 0; i < 10; i++)
                    {
                        database.Store(new Customer {Name = "Daniel" + i});
                    }

                    using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await database.SaveAsync();

                        tx.Complete();
                    }

                    database.Close();

                    Approvals.VerifyFile("StoreAsyncSupportsAmbientTransactionComplete.received.txt");
                });
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task StoreAsyncSupportsAmbientTransactionRollback()
        {
            var slide = new Slide(title: "Store Async supports ambient transactions - rollback");
            await slide
                .Sample(async () =>
                {
                    var database = new Database("StoreAsyncSupportsAmbientTransactionRollback.received.txt");

                    for (int i = 0; i < 10; i++)
                    {
                        database.Store(new Customer {Name = "Daniel" + i});
                    }

                    using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await database.SaveAsync();
                    }

                    database.Close();

                    Approvals.VerifyFile("StoreAsyncSupportsAmbientTransactionRollback.received.txt");
                });
        }

        [Test]
        public async Task TheEnd()
        {
            var giveAway = new GiveAway();
            await giveAway.WorthThousandDollars();
        }
    }
}