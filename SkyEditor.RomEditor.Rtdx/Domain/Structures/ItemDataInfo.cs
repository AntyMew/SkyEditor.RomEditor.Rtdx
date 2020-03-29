using SkyEditor.IO.Binary;
using System.Collections.Generic;
using System.Text;

using ItemIndex = SkyEditor.RomEditor.Rtdx.Reverse.Const.item.Index;
using WazaIndex = SkyEditor.RomEditor.Rtdx.Reverse.Const.waza.Index;

namespace SkyEditor.RomEditor.Rtdx.Domain.Structures
{
    public class ItemDataInfo
    {
        private const int EntrySize = 0x64;

        public IReadOnlyDictionary<ItemIndex, ItemDataInfoEntry> Entries { get; }

        public ItemDataInfo(IReadOnlyBinaryDataAccessor data)
        {
            var entryCount = checked((int)data.Length / EntrySize);
            var entries = new Dictionary<ItemIndex, ItemDataInfoEntry>(entryCount);
            for (int i = 0; i < entryCount; i++)
                entries.Add((ItemIndex)i, new ItemDataInfoEntry(data.Slice(i * EntrySize, EntrySize)));
            this.Entries = entries;
        }

        public class ItemDataInfoEntry
        {
            public short BuyPrice { get; }
            public short SellPrice { get; }
            public WazaIndex MoveTaught { get; }
            public string Name { get; }

            public ItemDataInfoEntry(IReadOnlyBinaryDataAccessor data)
            {
                BuyPrice = data.ReadInt16(6);
                SellPrice = data.ReadInt16(8);
                MoveTaught = (WazaIndex)data.ReadInt16(0xA);
                Name = data.ReadString(0x22, 0x42, Encoding.ASCII);
            }
        }
    }
}