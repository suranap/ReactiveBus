using System;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveBus;

namespace ReactiveBusTest
{
    [TestClass]
    public class RxBusTest
    {
        [TestMethod]
        public void Initialize_RxBus()
        {
            var bus = new RxBus();
            Assert.IsNotNull(bus);
        }

        [TestMethod]
        public void Use_with_one_publisher_and_subscriber()
        {
            var bus = new RxBus();

            int result = -1;
            var rx = Observable.Return(1);
            bus.GetObservable<int>().Subscribe(i => { result = i; });
            rx.Subscribe(bus.GetObserver<int>());

            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void Check_DeadEvent_with_one_publisher()
        {
            var bus = new RxBus();

            DeadEvent result = null;
            var rx = Observable.Return(1);
            bus.GetObservable<DeadEvent>().Subscribe(i => { result = i; });
            rx.Subscribe(bus.GetObserver<int>());

            Assert.AreEqual(result.Value, 1);
        }
    }
}
