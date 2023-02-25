// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kenet.Collections.Reactive.SynchronizationMethods;

public interface IIndexBasedEnumerator<TItem> : IEnumerator<TItem>
{
    bool MoveTo(int index);
}
