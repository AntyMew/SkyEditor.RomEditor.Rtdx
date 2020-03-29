using SkyEditor.IO.Binary;
using System.Collections.Generic;

using DungeonIndex = SkyEditor.RomEditor.Rtdx.Reverse.Const.dungeon.Index;

namespace SkyEditor.RomEditor.Rtdx.Domain.Structures
{
    public class DungeonDataInfo
    {
        private const int EntrySize = 0x1C;

        public IDictionary<DungeonIndex, DungeonDataInfoEntry> Entries { get; }

        public DungeonDataInfo(IReadOnlyBinaryDataAccessor data)
        {
            var entryCount = checked((int)data.Length / EntrySize);
            var entries = new Dictionary<DungeonIndex, DungeonDataInfoEntry>(entryCount);
            for (int i = 0; i < entryCount; i++)
                entries.Add((DungeonIndex)i, new DungeonDataInfoEntry(data.Slice(i * EntrySize, EntrySize)));
            this.Entries = entries;
        }
    }

    public class DungeonDataInfoEntry
    {

        public DungeonIndex Index { get; }

        public int SortPriority { get; }
        
        public DungeonDataInfoEntry(IReadOnlyBinaryDataAccessor data)
        {
            Index = (DungeonIndex)data.ReadInt32(4);
            SortPriority = data.ReadInt32(0xC);
        }
    }
}