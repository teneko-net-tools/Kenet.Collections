namespace Kenet.Collections.Reactive;

/// <summary>
/// Represents the delegate for the handler that is called from <see cref="CollectionModificationIterationHelper.IteratorBuilder.Iterate"/>.
/// </summary>
public delegate void CollectionModificationIterationHandler(CollectionModificationIterationContext iterationContext);
