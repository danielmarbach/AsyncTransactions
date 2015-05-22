using System;
using System.Threading.Tasks;
using System.Transactions;

namespace AsyncTransactions
{
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
}