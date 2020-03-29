using SkyEditor.IO.Binary;
using System.Collections.Generic;
using System.Text;

using CampIndex = SkyEditor.RomEditor.Rtdx.Reverse.Const.camp.Index;

namespace SkyEditor.RomEditor.Rtdx.Domain.Structures
{
    public class Camp
    {
        private const int EntrySize = 0x114;

        public IDictionary<CampIndex, CampEntry> Entries { get; }

        public Camp(IReadOnlyBinaryDataAccessor data)
        {
            var entryCount = checked((int)data.Length / EntrySize);
            var entries = new Dictionary<CampIndex, CampEntry>(entryCount);
            for (int i = 0; i < entryCount; i++)
                entries.Add((CampIndex)i, new CampEntry(data.Slice(i * EntrySize, EntrySize)));
            this.Entries = entries;
        }

        public class CampEntry
        {
            public string Lineup { get; }
            public string UnlockCondition { get; }
            public int Price { get; }
            public string BackgroundTexture { get; }
            public string BackgroundMusic { get; }

            public CampEntry(IReadOnlyBinaryDataAccessor data)
            {
                Lineup = data.ReadString(2, 0x40, Encoding.ASCII);
                UnlockCondition = data.ReadString(0x42, 0x40, Encoding.ASCII);
                Price = data.ReadInt32(0x84);
                BackgroundTexture = data.ReadString(0x94, 0x40, Encoding.ASCII);
                BackgroundMusic = data.ReadString(0xD4, 0x40, Encoding.ASCII);
            }
        }
    }
}