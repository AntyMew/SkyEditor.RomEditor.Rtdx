using SkyEditor.IO.Binary;
using System.Collections.Generic;

using CampIndex = SkyEditor.RomEditor.Rtdx.Reverse.Const.camp.Index;
using CreatureIndex = SkyEditor.RomEditor.Rtdx.Reverse.Const.creature.Index;

namespace SkyEditor.RomEditor.Rtdx.Domain.Structures
{
    public class CampHabitat
    {
        public IReadOnlyDictionary<CreatureIndex, CampIndex> Entries { get; }

        public CampHabitat(IReadOnlyBinaryDataAccessor data)
        {
            var entryCount = checked((int)data.Length / sizeof(int));
            var entries = new Dictionary<CreatureIndex, CampIndex>(entryCount);
            for (int i = 0; i < entryCount; i++)
                entries.Add((CreatureIndex)i, (CampIndex)data.ReadInt32(i * sizeof(int)));
            this.Entries = entries;
        }
    }
}