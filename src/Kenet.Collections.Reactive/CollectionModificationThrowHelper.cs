// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Kenet.Collections.Reactive
{
    internal static class CollectionModificationThrowHelper
    {
        public static ArgumentException NewItemsWereNullException() =>
            new("The new-items were null that cannot be used during collection modification.");

        public static ArgumentException OldItemsWereNullException() =>
            new("The old-items were null and cannot be used during collection modification.");
    }
}
