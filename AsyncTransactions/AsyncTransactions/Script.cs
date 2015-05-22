using System;
using System.Threading.Tasks;
using System.Transactions;
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
        public async Task TransactionScope()
        {
            var slide = new Slide(title: "Transaction Scope");
            await slide
                .BulletPoint("System.Transactions.TransactionScope")
                .BulletPoint("Implicit programing model / Ambient Transactions")

                .Sample(() =>
                {
                    Assert.Null(Transaction.Current);

                    using (var tx = new TransactionScope())
                    {
                        Assert.NotNull(Transaction.Current);

                        tx.Complete();
                    }

                    Assert.Null(Transaction.Current);
                })

                .BulletPoint("Only works with async/await with .NET 4.5.1");
        }

        [Test]
        public async Task ShowHowAwesome()
        {
            var slide = new Slide(title: "Transaction Scope");
            await slide
                .Sample(async () =>
                {
                    var database = new Database("StoreAsync.received.txt");

                    for (int i = 0; i < 10; i++)
                    {
                        database.Store(new DatabaseTests.Customer {Name = "Daniel" + i});
                    }

                    await database.SaveAsync();

                    database.Close();
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