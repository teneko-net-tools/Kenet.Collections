// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Kenet.Collections
{
    internal static class CollectionTools
    {
        /// <summary>
        /// Gets the move affected range of items between <paramref name="fromIndex"/> and
        /// <paramref name="toIndex"/> with simultaneous consideration of the amount of the
        /// to be moved items.
        /// </summary>
        /// <param name="fromIndex">The index of first item where you would start the move.</param>
        /// <param name="toIndex">The index where the new items would be if the old items would be not there already.</param>
        /// <param name="itemsCount">The amount of items you want move including first item.</param>
        /// <returns>The lower index where move would start and starting there the distance.</returns>
        public static (int LowerIndex, int Distance) GetMoveRange(int fromIndex, int toIndex, int itemsCount)
        {
            int amountOfAffectedItems = Math.Abs(fromIndex - toIndex) + itemsCount;
            int startIndexOfAffectedItems = Math.Min(fromIndex, toIndex);
            return (LowerIndex: startIndexOfAffectedItems, Distance: amountOfAffectedItems);
        }

        /// <summary>
        /// Checks whether <paramref name="itemIndex"/> is within the move affected range of
        /// items between <paramref name="fromIndex"/> and <paramref name="toIndex"/> with
        /// simultaneous consideration of the amount of the to be moved items.
        /// </summary>
        /// <param name="fromIndex">The index of first item where you would start the move.</param>
        /// <param name="toIndex">The index where the new items would be if the old items would be not there already.</param>
        /// <param name="itemsCount">The amount of items you want move including first item.</param>
        /// <param name="itemIndex"></param>
        /// <returns>The lower index where move would start and starting there the distance.</returns>
        public static bool MoveRangeContains(int fromIndex, int toIndex, int itemsCount, int itemIndex)
        {
            (int StartIndexOfAffectedItems, int AmountOfAffectedItems) = GetMoveRange(fromIndex, toIndex, itemsCount);
            return itemIndex >= StartIndexOfAffectedItems && itemIndex < StartIndexOfAffectedItems + AmountOfAffectedItems;
        }
    }
}
