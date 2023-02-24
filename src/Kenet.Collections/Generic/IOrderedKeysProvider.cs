// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kenet.Collections.Generic
{
    public interface IOrderedKeysProvider
    {
        public int KeysLength { get; }

        public IList<object?> GetOrderedKeys();
    }
}
