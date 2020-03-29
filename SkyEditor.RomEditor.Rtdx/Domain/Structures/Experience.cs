using SkyEditor.IO.Binary;
using System.Collections.Generic;

namespace SkyEditor.RomEditor.Rtdx.Domain.Structures
{
    public class Experience
    {
        public IReadOnlyList<ExperienceEntry> Entries { get; }

        public Experience(IReadOnlyBinaryDataAccessor data, IReadOnlyBinaryDataAccessor entryList)
        {
            var entryCount = checked((int)entryList.Length / sizeof(int));
            var entries = new List<ExperienceEntry>(entryCount);
            for (int i = 0; i < entryCount - 1; i++)
            {
                var entryOffset = entryList.ReadInt32(i * sizeof(int));
                var entryEnd = entryList.ReadInt32((i + 1) * sizeof(int));
                entries.Add(new ExperienceEntry(data.Slice(entryOffset, entryEnd - entryOffset)));
            }
            this.Entries = entries;
        }

        public class ExperienceEntry
        {
            private const int EntrySize = 0x0C;

            public IReadOnlyList<Level> Levels { get; }

            public ExperienceEntry(IReadOnlyBinaryDataAccessor data)
            {
                var levelCount = checked((int)data.Length / EntrySize);
                var levels = new List<Level>(levelCount);
                for (int i = 0; i < levelCount; i++)
                    levels.Add(new Level(data.Slice(i * EntrySize, EntrySize)));
                this.Levels = levels;
            }

            public class Level
            {
                public int MinimumExperience { get; }
                public byte HitPointsGained { get; }
                public byte AttackGained { get; }
                public byte DefenseGained { get; }
                public byte SpecialAttackGained { get; }
                public byte SpecialDefenseGained { get; }
                public byte SpeedGained { get; }
                public byte LevelsGained { get; }

                public Level(IReadOnlyBinaryDataAccessor data)
                {
                    MinimumExperience = data.ReadInt32(0);
                    HitPointsGained = data.ReadByte(5);
                    AttackGained = data.ReadByte(6);
                    DefenseGained = data.ReadByte(7);
                    SpecialAttackGained = data.ReadByte(8);
                    SpecialDefenseGained = data.ReadByte(9);
                    SpeedGained = data.ReadByte(0xA);
                    LevelsGained = data.ReadByte(0xB);
                }
            }
        }
    }
}