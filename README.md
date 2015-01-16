# ReactiveBus
A .NET EventBus implemented with Reactive Extensions. 

This is inspired by [Guava's EventBus](https://code.google.com/p/guava-libraries/wiki/EventBusExplained). While there are many implementations of EventBuses with Reactive Extensions, I haven't run across any that implement all the features found in Guava. 

* A subscriber to type T also gets all messages posted to subtypes of T (including interfaces). 
* If there are no subscribers for a message, it is posted to DeadEvent.
* Several .NET EventBus implementations don't handle concurrency. 
