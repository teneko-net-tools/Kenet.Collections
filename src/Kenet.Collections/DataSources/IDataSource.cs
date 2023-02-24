// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Kenet.Collections.DataSources.Generic;

namespace Kenet.Collections.DataSources
{
    public interface IDataSource : IDisposable
    {
        DataSourceEnumerationState EnumerationState { get; }

        /// <summary>
        /// Type of data this data source delivers.
        /// </summary>
        Type DataType { get; }

        public bool IsDisposed { get; }
    }
}
