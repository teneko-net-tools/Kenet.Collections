// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Kenet.Collections.Reactive.SynchronizationMethods;

internal class IndexBasedEnumerator<TItem> : IIndexBasedEnumerator<TItem>
{
    public TItem Current { get; private set; }

    object IEnumerator.Current => Current!;

    private readonly IReadOnlyList<TItem> _list;
    private readonly int _lastIndex = -1;

    public IndexBasedEnumerator(IReadOnlyList<TItem> list)
    {
        Current = default!;
        _list = list;
    }

    public bool MoveNext() =>
        throw new NotImplementedException();

    public bool MoveTo(int index)
    {
        if (index < 0) {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (index == _lastIndex) {
            return true;
        }

        if (index < _list.Count) {
            Current = _list[index];
            return true;
        } else {
            return false;
        }
    }

    public void Reset() =>
        throw new NotImplementedException();

    public void Dispose() { }
}
