namespace Kenet.Collections.Reactive;

public class SynchronizableCollectionOptions
{
    public static SynchronizableCollectionOptions<TItem> Create<TItem>()
            where TItem : notnull =>
            new();
}
