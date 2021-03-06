﻿using NsoElfConverterDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using CreatureIndex = SkyEditor.RomEditor.Rtdx.Reverse.Const.creature.Index;
using FixedCreatureIndex = SkyEditor.RomEditor.Rtdx.Reverse.Const.fixed_creature.Index;

namespace SkyEditor.RomEditor.Rtdx
{
    public interface IMainExecutable
    {
        IReadOnlyList<StarterFixedPokemonMap> StarterFixedPokemonMaps { get; }
        byte[] ToElf();
        byte[] ToNso(INsoElfConverter? nsoElfConverter = null);
    }

    public class MainExecutable : IMainExecutable
    {
        public static MainExecutable LoadFromNso(byte[] file, INsoElfConverter? nsoElfConverter = null)
        {
            nsoElfConverter ??= NsoElfConverter.Instance;
            var data = nsoElfConverter.ConvertNsoToElf(file);
            return new MainExecutable(data);
        }

        public MainExecutable(byte[] elfData)
        {
            if (elfData == null)
            {
                throw new ArgumentNullException(nameof(elfData));
            }
            if (elfData.Length <= (0x04BA3B0C + 0x90))
            {
                throw new ArgumentException("Data is not long enough to contain known sections", nameof(elfData));
            }

            this.Data = elfData ?? throw new ArgumentNullException(nameof(elfData));

            Init();
        }

        const int starterFixedPokemonMapOffset = 0x04BA3B0C;

        private void Init()
        {
            var starterFixedPokemonMaps = new List<StarterFixedPokemonMap>();
            for (int i = 0; i < 16; i++)
            {
                var offset = starterFixedPokemonMapOffset + (8 * i);
                starterFixedPokemonMaps.Add(new StarterFixedPokemonMap
                {
                    PokemonId = (CreatureIndex)BitConverter.ToInt32(Data, offset),
                    FixedPokemonId = (FixedCreatureIndex)BitConverter.ToInt32(Data, offset + 4)
                });
            }
            this.StarterFixedPokemonMaps = starterFixedPokemonMaps;
        }

        public byte[] ToElf()
        {
            for (int i = 0; i < 16; i++)
            {
                var map = StarterFixedPokemonMaps[i];
                var offset = starterFixedPokemonMapOffset + (8 * i);
                BitConverter.GetBytes((int)map.PokemonId).CopyTo(Data, offset);
                BitConverter.GetBytes((int)map.FixedPokemonId).CopyTo(Data, offset + 4);
            }
            return this.Data;
        }

        public byte[] ToNso(INsoElfConverter? nsoElfConverter = null)
        {
            nsoElfConverter ??= NsoElfConverter.Instance;
            return nsoElfConverter.ConvertElfToNso(ToElf());
        }

        private byte[] Data { get; }
        public IReadOnlyList<StarterFixedPokemonMap> StarterFixedPokemonMaps { get; private set; } = default!;        
    }

    [DebuggerDisplay("StarterFixedPokemonMap: {PokemonId} -> {FixedPokemonId}")]
    public class StarterFixedPokemonMap
    {
        public CreatureIndex PokemonId { get; set; }
        public FixedCreatureIndex FixedPokemonId { get; set; }
    }
}
