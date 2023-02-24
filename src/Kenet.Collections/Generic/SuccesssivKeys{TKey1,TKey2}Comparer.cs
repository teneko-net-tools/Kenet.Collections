﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Kenet.Collections.Generic
{
    public class SuccessiveKeysComparer<TKey1, TKey2> : Comparer<SuccessiveKeys<TKey1, TKey2>>, IComparer<SuccessiveKeys<TKey1, TKey2>?>
    {
        public new static SuccessiveKeysComparer<TKey1, TKey2> Default = new SuccessiveKeysComparer<TKey1, TKey2>();

        public override int Compare([AllowNull] SuccessiveKeys<TKey1, TKey2> x, [AllowNull] SuccessiveKeys<TKey1, TKey2> y) =>
            new IOrderedKeysProviderComparer().Compare(x, y);

        public int Compare([AllowNull] SuccessiveKeys<TKey1, TKey2>? x, [AllowNull] SuccessiveKeys<TKey1, TKey2>? y) =>
            new IOrderedKeysProviderComparer().Compare(x, y);
    }
}
