using SkyEditor.IO.Binary;
using System.Collections.Generic;
using System.Text;

using RankIndex = SkyEditor.RomEditor.Rtdx.Reverse.Const.rank.Index;

namespace SkyEditor.RomEditor.Rtdx.Domain.Structures
{
    public class Rank
    {
        private const int EntrySize = 0x20;

        public IReadOnlyDictionary<RankIndex, RankEntry> Entries { get; }

        public Rank(IReadOnlyBinaryDataAccessor data)
        {
            var sir0 = new Sir0(data);
            var entryCount = checked((int)data.Length / EntrySize);
            var entries = new Dictionary<RankIndex, RankEntry>(entryCount);
            for (int i = 0; i < entryCount; i++)
                entries.Add((RankIndex)i, new RankEntry(sir0, sir0.Data.Slice(i * EntrySize, EntrySize)));
            this.Entries = entries;
        }

        public class RankEntry
        {
            public string RewardStatue { get; }
            public int MinPoints { get; }
            public short Unknown { get; }
            public short ToolboxSize { get; }
            public short CampCapacity { get; }
            public short TeamPresets { get; }
            public short JobLimit { get; }

            public RankEntry(Sir0 sir0, IReadOnlyBinaryDataAccessor data)
            {
                {
                    int offset = checked((int)data.ReadInt64(0));
                    RewardStatue = sir0.Data.ReadString(offset, offset + 0x10, Encoding.ASCII);
                }
                MinPoints = data.ReadInt32(8);
                Unknown = data.ReadInt16(0xC);
                ToolboxSize = data.ReadInt16(0xE);
                CampCapacity = data.ReadInt16(0x10);
                TeamPresets = data.ReadInt16(0x12);
                JobLimit = data.ReadInt16(0x14);
            }
        }
    }
}