﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Teronis.Collections.Synchronization
{
    internal static class CollectionModificationThrowHelper
    {
        public static ArgumentException NewItemsWereNullException() =>
            new ArgumentException("The new-items were null that cannot be used during collection modification.");

        public static ArgumentException OldItemsWereNullException() =>
            new ArgumentException("The old-items were null and cannot be used during collection modification.");
    }
}
