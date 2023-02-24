﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Teronis.Collections.DataSources.Generic
{
    public class ConsecutiveDataSources<DataType> : AsyncDataSource<DataType>
    {
        private readonly IEnumerable<IAsyncDataSource<DataType>> asyncDataSources;

        public ConsecutiveDataSources(IEnumerable<IAsyncDataSource<DataType>> asyncDataSources, ILogger logger)
            : base(logger)
            => this.asyncDataSources = asyncDataSources ?? throw new ArgumentNullException(nameof(asyncDataSources));

        protected override async IAsyncEnumerable<DataType> EnumerateAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var currentAsyncDataSource in asyncDataSources) {
                await foreach (var data in currentAsyncDataSource.EnumerateAsync(cancellationToken)) {
                    yield return data;
                }
            }
        }
    }
}
