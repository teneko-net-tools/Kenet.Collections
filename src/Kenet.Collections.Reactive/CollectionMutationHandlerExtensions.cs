using System;

namespace Kenet.Collections.Reactive;

internal static class CollectionMutationHandlerExtensions
{
    public static void InsertItem<TItem>(this IListMutationTarget<TItem> itemsMutationTarget, int insertAt, TItem item) =>
        itemsMutationTarget.Mutate(collection => collection.InsertItem(insertAt, item), disableRedirection: false);

    public static void MoveItems<TItem>(this IListMutationTarget<TItem> itemsMutationTarget, int fromIndex, int toIndex, int count) =>
        itemsMutationTarget.Mutate(collection => collection.MoveItems(fromIndex, toIndex, count), disableRedirection: false);

    public static void RemoveItem<TItem>(this IListMutationTarget<TItem> itemsMutationTarget, int removeAt) =>
        itemsMutationTarget.Mutate(collection => collection.RemoveItem(removeAt), disableRedirection: false);

    public static void ReplaceItem<TItem>(this IListMutationTarget<TItem> itemsMutationTarget, int replaceAt, Func<TItem> getNewItem) =>
        itemsMutationTarget.Mutate(collection => collection.ReplaceItem(replaceAt, getNewItem), disableRedirection: false);

    public static void ResetItems<TItem>(this IListMutationTarget<TItem> itemsMutationTarget) =>
        itemsMutationTarget.Mutate(collection => collection.ResetItems(), disableRedirection: false);
}
