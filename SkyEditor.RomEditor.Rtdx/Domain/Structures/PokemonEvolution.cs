using SkyEditor.IO.Binary;
using System;
using System.Collections.Generic;

using CreatureIndex = SkyEditor.RomEditor.Rtdx.Reverse.Const.creature.Index;
using ItemIndex = SkyEditor.RomEditor.Rtdx.Reverse.Const.item.Index;

namespace SkyEditor.RomEditor.Rtdx.Domain.Structures
{
    public class PokemonEvolution
    {
        private const int EntrySize = 0x10;

        public IDictionary<CreatureIndex, PokemonEvolutionEntry> Entries { get; }

        public PokemonEvolution(IReadOnlyBinaryDataAccessor data)
        {
            var sir0 = new Sir0(data);
            var indexOffset = checked((int)sir0.SubHeader.ReadInt64(0));
            var entryCount = sir0.SubHeader.ReadInt32(8);
            var entries = new Dictionary<CreatureIndex, PokemonEvolutionEntry>(entryCount);
            for (int i = 0; i < entryCount; i++)
                entries.Add((CreatureIndex)i, new PokemonEvolutionEntry(sir0, sir0.Data.Slice(indexOffset + i * EntrySize, EntrySize)));
            this.Entries = entries;
        }

        public class PokemonEvolutionEntry
        {
            public IReadOnlyList<PokemonEvolutionBranch> Branches { get; }
            public (CreatureIndex First, CreatureIndex Second) MegaEvos { get; }

            public PokemonEvolutionEntry(Sir0 sir0, IReadOnlyBinaryDataAccessor data)
            {
                int branchOffset = checked((int)data.ReadInt64(0));
                int branchCount = data.ReadInt32(8);
                var branches = new List<PokemonEvolutionBranch>(branchCount);
                for (int i = 0; i < branchCount; i++)
                    branches.Add(new PokemonEvolutionBranch(sir0.Data.Slice(branchOffset + i * 0x14, 0x14)));
                this.Branches = branches;
                MegaEvos = (
                    (CreatureIndex)data.ReadInt16(0xC),
                    (CreatureIndex)data.ReadInt16(0xE)
                );
            }
        }

        public class PokemonEvolutionBranch
        {
            private readonly Requirements flags;

            public CreatureIndex Evolution { get; }

            public ItemIndex EvolutionItem { get; }

            public short ItemsRequired { get; }

            public byte MinimumLevel { get; }

            public bool HasMinimumLevel => flags.HasFlag(Requirements.Level);

            public bool RequiresItem => flags.HasFlag(Requirements.Item);

            [Flags]
            private enum Requirements
            {
                None = 0,
                Level = 0x01,
                Item = 0x02
            }

            public PokemonEvolutionBranch(IReadOnlyBinaryDataAccessor accessor)
            {
                Evolution = (CreatureIndex)accessor.ReadInt16(4);
                EvolutionItem = (ItemIndex)accessor.ReadInt16(6);
                ItemsRequired = accessor.ReadInt16(8);
                MinimumLevel = accessor.ReadByte(0x10);
                flags = (Requirements)accessor.ReadByte(0x11);
            }
        }
    }
}