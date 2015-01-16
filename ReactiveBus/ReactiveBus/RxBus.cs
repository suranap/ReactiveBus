using System;
using System.Diagnostics.Contracts;
using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace ReactiveBus
{
    /// <summary>
    /// An in-memory EventBus implemented using Reactive Extensions.
    /// </summary>
    public class RxBus
    {
        // TODO: Cache the type search for parent/interface types

        // The value is a Subject<T> where T is the key
        private readonly ConcurrentDictionary<Type, object> _routeTable = new ConcurrentDictionary<Type, object>();

        // This gets data from publisher and forwards it to client
        public class BusObserver<T> : IObserver<T>
        {
            private readonly ConcurrentDictionary<Type, object> _rt;

            public void OnCompleted()
            {
                object subject;
                foreach (var t in typeof(T).GetSuperTypes())
                    if (_rt.TryGetValue(t, out subject))
                    {
                        ((Subject<T>)subject).OnCompleted();
                    }
            }

            public void OnNext(T value)
            {
                object subject;
                bool liveEvent = false;
                foreach (var t in typeof(T).GetSuperTypes())
                    if (_rt.TryGetValue(t, out subject))
                    {
                        liveEvent = true;
                        ((Subject<T>)subject).OnNext(value);
                    }
                if (liveEvent) return;
                var exists = _rt.TryGetValue(typeof (DeadEvent), out subject);
// ReSharper disable once RedundantBoolCompare
                Contract.Assert(exists == true);
                Contract.Assume(subject != null);
// ReSharper disable once PossibleNullReferenceException
                ((Subject<DeadEvent>)subject).OnNext(new DeadEvent(value));
            }

            public void OnError(Exception e)
            {
                object subject;
                foreach (var t in typeof(T).GetSuperTypes())
                    if (_rt.TryGetValue(t, out subject)) {
                        ((Subject<T>)subject).OnError(e);
                    }
            }

            internal BusObserver(ConcurrentDictionary<Type, object> routeTable)
            {
                Contract.Requires<ArgumentNullException>(routeTable != null);
                _rt = routeTable;
            }
        }

        public class BusObservable<T> : IObservable<T>
        {
            private readonly Subject<T> _next;

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return _next.Subscribe(observer);
            }

            internal BusObservable(Subject<T> subject)
            {
                _next = subject;
            }
        }

        // These extract type-safe Observ(ers)/(ables) for clients to connect to.
        public IObserver<T> GetObserver<T>()
        {
            return new BusObserver<T>(_routeTable);
        }

        public IObservable<T> GetObservable<T>()
        {
            var subject = (Subject<T>)_routeTable.GetOrAdd(typeof (T), t => new Subject<T>());
            return new BusObservable<T>(subject);
        }

        public RxBus()
        {
            _routeTable[typeof (DeadEvent)] = new Subject<DeadEvent>(); 
        }
    }
}
