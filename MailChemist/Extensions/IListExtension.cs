//using System;
//using System.Collections.Generic;

//namespace MailChemist.Extensions
//{
//    internal static class IListExtension
//    {
//        /// <summary>
//        /// This is internal so we don't expand the .NET methods for the developers
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="source"></param>
//        /// <param name="target"></param>
//        internal static void AddRange<T>(this IList<T> source, IEnumerable<T> target)
//        {
//            if (source is null)
//                throw new ArgumentNullException(nameof(source));

//            if (target is null)
//                throw new ArgumentNullException(nameof(target));

//            if (source is IList<T> concreteList)
//            {
//                concreteList.AddRange(target);
//                return;
//            }

//            var tempTarget = new List<T>();

//            foreach (var item in source)
//                tempTarget.Add(item);

//            target = tempTarget;
//        }
//    }
//}