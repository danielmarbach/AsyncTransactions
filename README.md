# AsyncTransactions
This is the code for the AnzCoders conference. 

### The secret of transactions and async/await
`Wed May 27 21:30:00 2015`

Did you ever ask yourself, when using async/await together with Entity Framework and transactions to load data from your database, what kind of black magic Entity Framework is pulling off behind the scenes? Or do you need to write custom code which participates asynchronously within a transaction? Together we explore the inner workings of transactions. We learn how to leverage transactions with asynchronous code and get to know why there is no way around async void when working with transactions. After this session there will be no more black magic.

#### Intro
My name is Daniel Marbach. I'm the CEO of tracelight GmbH in Switzerland and also working as a Solutions Architect for Particular Software, the folks behind NServiceBus.

Disclaimer first: If you are attending this presentation to learn something about Entity Framework then shutdown your computer and got to bed! This presentation is not about EntityFramework. It is more a deep dive into the curiosity of TransactionScope and async/await. I'm sure you will learn a lot of unnecessary stuff which makes you the king story teller in your local geek bar.

#### Brief refresh
The .NET platform has under `System.Transactions` a very nifty class called `TransactionScope`. The `TransactionScope` allows you to wrap your database code, your infrastructure code and sometimes even third-party code (if supported by the third-party library) inside a transaction and only perform the actions when you actually want to commit (or complete) the transaction. As long as all the code inside the `TransactionScope` is executed on the same thread, all the code on the callstack can participate with the `TransactionScope` defined in your code. You can nest scopes, create new independent scopes inside a parent transaction scope or even creates clones of a `TransactionScope` and pass the clone to another thread and join back onto the calling thread but all this is not part of this talk. By wrapping your code with a transaction scope you are using an implicit transaction model or also called ambient transactions. The benefits and also drawbacks of the `TransactionScope` (depends how you want to see it) is that the local transaction automatically escalates to a distributed transaction if necessary. The scope also simplifies programming with transactions if you favour implicit over explicit. There are a few caveats:

* It has a limited usefulness in cloud scenarios because the cloud doesn't support distributed transactions **phew! We can finally get rid of that beast** 
* It only works with async/await in .NET 4.5.1

#### TransactionScope and Async
When async/await was introduced with C# 5.0 and .NET 4.5 Microsoft completely forgot one tiny little detail. The statemachine introduced didn't properly "float" the transaction around when an async method was called under a wrapping `TransactionScope`. So in order to make `TransactionScope` and async work properly you need to upgrade to .NET 4.5.1.

#### TransactionScope and Async (proper)
With .NET 4.5.1 the `TransactionScope` has a new enum which can be provided in the constructor. 

`TransactionScopeAsyncFlowOption.Enabled`
Specifies that transaction flow across thread continuations is enabled.

the default is `Suppress` (Non breaking changes you know!)

#### Store Async and THE database
For the sake of this presentation let's assume we are building our own NoSQL database. Side note: Never, ever do that unless you really know what you are doing! The database needs to store objects in memory and as soon as `SaveAsync` is called it should write it into the underlying storage. Let us briefly explore the code.

#### Store Async and TransactionScope
Now suppose we want to add ambient transaction support to our NoSQL database. How could we do that? Well nothing simpler than that. We need to write an `IEnlistmentNotification` implementation which we enlist volatile (meaning it cannot recover from failures in case when something unforeseen happened to the resource manager). I don't want to see heads exploding. Therefore I'm not diving more into resource manager, single phase and multi phase commits...

Let us briefly explore the code.

Note: The AsyncPump implementation could still be tweaked. For example if we know that in the majority of case we invoke multiple asynchronous methods we could extend the pump to support an enumerable of tasks. If course this change would also affect the resource manager implementations.

#### What did we just learn
* Async void is evil! use it carefully.
* In a cloud first and async world (no pun intended) try to avoid TransactionScope
* Modern frameworks like Entity Framework 6 and higher support their own transactions which is the recommended way and actually works properly with async.
* Use the AsyncPump if you need `TransactionScope` support in order to enlist your own transactional stuff asynchronously.
* If you are writing an async enabled library then `ConfigureAwait(false)` is your friend

#### Resources
* [Six Essential Tips for Async](http://channel9.msdn.com/Series/Three-Essential-Tips-for-Async)
* [Best Practices in Asynchronous Programming](https://msdn.microsoft.com/en-us/magazine/jj991977.aspx)
* [Participating in TransactionScopes and Async/Await](http://www.planetgeek.ch/2014/12/07/participating-in-transactionscopes-and-asyncawait-introduction/)
* [Working with Transactions (EF6 Onwards)](https://msdn.microsoft.com/en-us/data/dn456843.aspx)
* [Enlisting Resources as Participants in a Transaction](https://msdn.microsoft.com/en-us/library/ms172153.aspx)