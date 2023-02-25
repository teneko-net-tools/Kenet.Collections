using System;

namespace Kenet.Collections.Reactive;

internal static class CollectionMutationHandlerExtensions
{
    public static void InsertItem<TItem>(this IMutableList<TItem> collectionMutationHandler, int insertAt, TItem item) =>
        collectionMutationHandler.Mutate(collection => collection.InsertItem(insertAt, item), disableRedirection: false);

    public static void MoveItems<TItem>(this IMutableList<TItem> collectionMutationHandler, int fromIndex, int toIndex, int count) =>
        collectionMutationHandler.Mutate(collection => collection.MoveItems(fromIndex, toIndex, count), disableRedirection: false);

    public static void RemoveItem<TItem>(this IMutableList<TItem> collectionMutationHandler, int removeAt) =>
        collectionMutationHandler.Mutate(collection => collection.RemoveItem(removeAt), disableRedirection: false);

    public static void ReplaceItem<TItem>(this IMutableList<TItem> collectionMutationHandler, int replaceAt, Func<TItem> getNewItem) =>
        collectionMutationHandler.Mutate(collection => collection.ReplaceItem(replaceAt, getNewItem), disableRedirection: false);

    public static void ResetItems<TItem>(this IMutableList<TItem> collectionMutationHandler) =>
        collectionMutationHandler.Mutate(collection => collection.ResetItems(), disableRedirection: false);
}
