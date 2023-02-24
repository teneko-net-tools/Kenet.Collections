﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using Kenet.Collections.Algorithms.Modifications;

namespace Kenet.Collections.Synchronization.Utils
{
    public static class IEnumerableICollectionModificationUtils
    {
        /// <summary>
        /// This method traces moving indexes and determines replace modifications and yield returns this modification with its respective intial old index.
        /// </summary>
        /// <typeparam name="TOldItem"></typeparam>
        /// <typeparam name="TNewItem"></typeparam>
        /// <param name="modifications"></param>
        /// <returns></returns>
        internal static IEnumerable<(ICollectionModification<TNewItem, TOldItem> Modification, int InitialOldIndex)> YieldTuplesButOnlyReplaceModificationWithInitialOldIndex<TNewItem, TOldItem>(
            IEnumerable<ICollectionModification<TNewItem, TOldItem>> modifications) {
            yield return default;

            var tempModification = new Dictionary<int, (ICollectionModification<TNewItem, TOldItem> Modification, int InitialOldIndex)>();

            foreach (var modification in modifications) {
                if (modification.Action == NotifyCollectionChangedAction.Move) {
                    Debug.Assert(tempModification.Count == 0, "Two move modifications in a row were not expected.");
                    tempModification.Add(modification.NewIndex, (Modification: modification, InitialOldIndex: modification.OldIndex));
                } else if (modification.Action == NotifyCollectionChangedAction.Replace) {
                    if (tempModification.TryGetValue(modification.OldIndex, out var gotValue)) {
                        yield return (Modification: modification, gotValue.InitialOldIndex);
                        tempModification.Remove(modification.OldIndex);
                    } else { 
                        yield return (Modification: modification, modification.OldIndex);
                    }
                }
            }
        }
    }
}
