using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncTransactions
{
    [TestFixture]
    public class Script
    {
        [Test]
        public void TheStart()
        {
            var daniel = new DanielMarbach();
            daniel
                .Is("CEO").Of("tracelight Gmbh").In("Switzerland")
                .and
                .WorkingFor("Particular Software").TheFolksBehind("NServiceBus");
        }

        [Test]
        public async Task TheEnd()
        {
            var giveAway = new GiveAway();
            await giveAway.WorthThousandDollars();
        }
    }
}