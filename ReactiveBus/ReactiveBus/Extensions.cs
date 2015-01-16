using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveBus
{
    static class Extensions
    {
        public static IEnumerable<R> UnFold<T, R>(this IEnumerable<T> seeds, Predicate<T> stop,
                                                 Func<T, R> map, Func<T, IEnumerable<T>> next)
        {
            Contract.Requires<ArgumentNullException>(stop != null);
            Contract.Requires<ArgumentNullException>(map != null);
            Contract.Requires<ArgumentNullException>(next != null);

            foreach (var seed in seeds.Where(seed => !stop(seed))) {
                yield return map(seed);
                foreach (var val in next(seed).UnFold(stop, map, next))
                    yield return val;
            }
        }

        /// <summary>
        /// Iterates up the type heirarchy starting from baseType.
        /// </summary>
        /// <param name="baseType">Any non-null type object</param>
        /// <returns>All the parent types and interfaces all the way to Object</returns>
        /// <remarks>
        /// This implementation is a bit more complicated because it's using the 
        /// limited Type class available for the Portable library.
        /// </remarks>
        public static IEnumerable<Type> GetSuperTypes(this Type baseType)
        {
            Contract.Requires<ArgumentNullException>(baseType != null);

            var parents = new[] { baseType }
                .UnFold(t => t == null, t => t,
                        t => t.GetTypeInfo().ImplementedInterfaces.Concat(new[] { t.GetTypeInfo().BaseType }))
                .Distinct();
            return parents;
        }
    }
}
