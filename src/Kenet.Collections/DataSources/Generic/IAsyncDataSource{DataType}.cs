// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;

namespace Kenet.Collections.DataSources.Generic
{
    public interface IAsyncDataSource<out DataType> : IDataSource<DataType>
    {
        IAsyncEnumerable<DataType> EnumerateAsync(CancellationToken cancellationToken = default);
    }
}
