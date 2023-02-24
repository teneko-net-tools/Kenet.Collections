﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Kenet.Collections.Specialized
{
    public sealed class IndexDirectoryEntry
    {
        public int Index { get; internal set; }
        public IndexDirectoryEntryMode Mode { get; }

        public IndexDirectoryEntry(int index, IndexDirectoryEntryMode mode)
        {
            Index = index;
            Mode = mode;
        }

        public override string ToString() =>
            $"[{Index}, {Enum.GetName(typeof(IndexDirectoryEntryMode), Mode)}]";

        public static implicit operator int(IndexDirectoryEntry entry) =>
            entry.Index;
    }
}
