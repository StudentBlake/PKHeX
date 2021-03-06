﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary> Generation 5 <see cref="PKM"/> format. </summary>
    public sealed class PK5 : PKM, IRibbonSetEvent3, IRibbonSetEvent4, IRibbonSetUnique3, IRibbonSetUnique4, IRibbonSetCommon3, IRibbonSetCommon4, IContestStats
    {
        private static readonly byte[] Unused =
        {
            0x87, // PokeStar Fame -- this is first to prevent 0x42 from being the first ExtraByte as this byte has GUI functionality
            0x42, // Hidden Ability/NPokemon
            0x43, 0x44, 0x45, 0x46, 0x47,
            0x5E, // unused
            0x63, // last 8 bits of a 32bit ribbonset
            0x64, 0x65, 0x66, 0x67, // unused 32bit ribbonset?
            0x86, // unused
        };

        public override IReadOnlyList<byte> ExtraBytes => Unused;

        public override int SIZE_PARTY => PKX.SIZE_5PARTY;
        public override int SIZE_STORED => PKX.SIZE_5STORED;
        public override int Format => 5;
        public override PersonalInfo PersonalInfo => PersonalTable.B2W2.GetFormeEntry(Species, AltForm);

        public override byte[] Data { get; }
        public PK5() => Data = new byte[PKX.SIZE_5PARTY];

        public PK5(byte[] data)
        {
            PKX.CheckEncrypted(ref data, Format);
            if (data.Length != PKX.SIZE_5PARTY)
                Array.Resize(ref data, PKX.SIZE_5PARTY);
            Data = data;
        }

        public override PKM Clone() => new PK5((byte[])Data.Clone()){Identifier = Identifier};

        private string GetString(int Offset, int Count) => StringConverter.GetString5(Data, Offset, Count);
        private byte[] SetString(string value, int maxLength) => StringConverter.SetString5(value, maxLength);

        // Trash Bytes
        public override byte[] Nickname_Trash { get => GetData(0x48, 22); set { if (value?.Length == 22) value.CopyTo(Data, 0x48); } }
        public override byte[] OT_Trash { get => GetData(0x68, 16); set { if (value?.Length == 16) value.CopyTo(Data, 0x68); } }

        // Future Attributes
        public override uint EncryptionConstant { get => PID; set { } }
        public override int CurrentFriendship { get => OT_Friendship; set => OT_Friendship = value; }
        public override int CurrentHandler { get => 0; set { } }
        public override int AbilityNumber { get => HiddenAbility ? 4 : 1 << PIDAbility; set { } }

        // Structure
        public override uint PID { get => BitConverter.ToUInt32(Data, 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, 0x00); }
        public override ushort Sanity { get => BitConverter.ToUInt16(Data, 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, 0x04); }
        public override ushort Checksum { get => BitConverter.ToUInt16(Data, 0x06); set => BitConverter.GetBytes(value).CopyTo(Data, 0x06); }

        #region Block A
        public override int Species { get => BitConverter.ToUInt16(Data, 0x08); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x08); }
        public override int HeldItem { get => BitConverter.ToUInt16(Data, 0x0A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0A); }
        public override int TID { get => BitConverter.ToUInt16(Data, 0x0C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0C); }
        public override int SID { get => BitConverter.ToUInt16(Data, 0x0E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0E); }
        public override uint EXP { get => BitConverter.ToUInt32(Data, 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, 0x10); }
        public override int OT_Friendship { get => Data[0x14]; set => Data[0x14] = (byte)value; }
        public override int Ability { get => Data[0x15]; set => Data[0x15] = (byte)value; }
        public override int MarkValue { get => Data[0x16]; protected set => Data[0x16] = (byte)value; }
        public override int Language { get => Data[0x17]; set => Data[0x17] = (byte)value; }
        public override int EV_HP { get => Data[0x18]; set => Data[0x18] = (byte)value; }
        public override int EV_ATK { get => Data[0x19]; set => Data[0x19] = (byte)value; }
        public override int EV_DEF { get => Data[0x1A]; set => Data[0x1A] = (byte)value; }
        public override int EV_SPE { get => Data[0x1B]; set => Data[0x1B] = (byte)value; }
        public override int EV_SPA { get => Data[0x1C]; set => Data[0x1C] = (byte)value; }
        public override int EV_SPD { get => Data[0x1D]; set => Data[0x1D] = (byte)value; }
        public int CNT_Cool { get => Data[0x1E]; set => Data[0x1E] = (byte)value; }
        public int CNT_Beauty { get => Data[0x1F]; set => Data[0x1F] = (byte)value; }
        public int CNT_Cute { get => Data[0x20]; set => Data[0x20] = (byte)value; }
        public int CNT_Smart { get => Data[0x21]; set => Data[0x21] = (byte)value; }
        public int CNT_Tough { get => Data[0x22]; set => Data[0x22] = (byte)value; }
        public int CNT_Sheen { get => Data[0x23]; set => Data[0x23] = (byte)value; }

        private byte RIB0 { get => Data[0x24]; set => Data[0x24] = value; } // Sinnoh 1
        private byte RIB1 { get => Data[0x25]; set => Data[0x25] = value; } // Sinnoh 2
        private byte RIB2 { get => Data[0x26]; set => Data[0x26] = value; } // Unova 1
        private byte RIB3 { get => Data[0x27]; set => Data[0x27] = value; } // Unova 2
        public bool RibbonChampionSinnoh    { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)((RIB0 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonAbility           { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)((RIB0 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonAbilityGreat      { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)((RIB0 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonAbilityDouble     { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)((RIB0 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonAbilityMulti      { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)((RIB0 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonAbilityPair       { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)((RIB0 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonAbilityWorld      { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)((RIB0 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RibbonAlert             { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)((RIB0 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public bool RibbonShock             { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)((RIB1 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonDowncast          { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)((RIB1 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonCareless          { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)((RIB1 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonRelax             { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)((RIB1 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonSnooze            { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)((RIB1 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonSmile             { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)((RIB1 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonGorgeous          { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)((RIB1 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RibbonRoyal             { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)((RIB1 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public bool RibbonGorgeousRoyal     { get => (RIB2 & (1 << 0)) == 1 << 0; set => RIB2 = (byte)((RIB2 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonFootprint         { get => (RIB2 & (1 << 1)) == 1 << 1; set => RIB2 = (byte)((RIB2 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonRecord            { get => (RIB2 & (1 << 2)) == 1 << 2; set => RIB2 = (byte)((RIB2 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonEvent             { get => (RIB2 & (1 << 3)) == 1 << 3; set => RIB2 = (byte)((RIB2 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonLegend            { get => (RIB2 & (1 << 4)) == 1 << 4; set => RIB2 = (byte)((RIB2 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonChampionWorld     { get => (RIB2 & (1 << 5)) == 1 << 5; set => RIB2 = (byte)((RIB2 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonBirthday          { get => (RIB2 & (1 << 6)) == 1 << 6; set => RIB2 = (byte)((RIB2 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RibbonSpecial           { get => (RIB2 & (1 << 7)) == 1 << 7; set => RIB2 = (byte)((RIB2 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public bool RibbonSouvenir          { get => (RIB3 & (1 << 0)) == 1 << 0; set => RIB3 = (byte)((RIB3 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonWishing           { get => (RIB3 & (1 << 1)) == 1 << 1; set => RIB3 = (byte)((RIB3 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonClassic           { get => (RIB3 & (1 << 2)) == 1 << 2; set => RIB3 = (byte)((RIB3 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonPremier           { get => (RIB3 & (1 << 3)) == 1 << 3; set => RIB3 = (byte)((RIB3 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RIB3_4 { get => (RIB3 & (1 << 4)) == 1 << 4; set => RIB3 = (byte)((RIB3 & ~(1 << 4)) | (value ? 1 << 4 : 0)); } // Unused
        public bool RIB3_5 { get => (RIB3 & (1 << 5)) == 1 << 5; set => RIB3 = (byte)((RIB3 & ~(1 << 5)) | (value ? 1 << 5 : 0)); } // Unused
        public bool RIB3_6 { get => (RIB3 & (1 << 6)) == 1 << 6; set => RIB3 = (byte)((RIB3 & ~(1 << 6)) | (value ? 1 << 6 : 0)); } // Unused
        public bool RIB3_7 { get => (RIB3 & (1 << 7)) == 1 << 7; set => RIB3 = (byte)((RIB3 & ~(1 << 7)) | (value ? 1 << 7 : 0)); } // Unused
        #endregion

        #region Block B
        public override int Move1 { get => BitConverter.ToUInt16(Data, 0x28); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x28); }
        public override int Move2 { get => BitConverter.ToUInt16(Data, 0x2A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2A); }
        public override int Move3 { get => BitConverter.ToUInt16(Data, 0x2C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2C); }
        public override int Move4 { get => BitConverter.ToUInt16(Data, 0x2E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2E); }
        public override int Move1_PP { get => Data[0x30]; set => Data[0x30] = (byte)value; }
        public override int Move2_PP { get => Data[0x31]; set => Data[0x31] = (byte)value; }
        public override int Move3_PP { get => Data[0x32]; set => Data[0x32] = (byte)value; }
        public override int Move4_PP { get => Data[0x33]; set => Data[0x33] = (byte)value; }
        public override int Move1_PPUps { get => Data[0x34]; set => Data[0x34] = (byte)value; }
        public override int Move2_PPUps { get => Data[0x35]; set => Data[0x35] = (byte)value; }
        public override int Move3_PPUps { get => Data[0x36]; set => Data[0x36] = (byte)value; }
        public override int Move4_PPUps { get => Data[0x37]; set => Data[0x37] = (byte)value; }
        private uint IV32 { get => BitConverter.ToUInt32(Data, 0x38); set => BitConverter.GetBytes(value).CopyTo(Data, 0x38); }
        public override int IV_HP  { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 00)) | ((value > 31 ? 31u : (uint)value) << 00); }
        public override int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 05)) | ((value > 31 ? 31u : (uint)value) << 05); }
        public override int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 10)) | ((value > 31 ? 31u : (uint)value) << 10); }
        public override int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 15)) | ((value > 31 ? 31u : (uint)value) << 15); }
        public override int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 20)) | ((value > 31 ? 31u : (uint)value) << 20); }
        public override int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 25)) | ((value > 31 ? 31u : (uint)value) << 25); }
        public override bool IsEgg { get => ((IV32 >> 30) & 1) == 1; set => IV32 = (IV32 & ~0x40000000u) | (value ? 0x40000000u : 0u); }
        public override bool IsNicknamed { get => ((IV32 >> 31) & 1) == 1; set => IV32 = (IV32 & 0x7FFFFFFFu) | (value ? 0x80000000u : 0u); }

        private byte RIB4 { get => Data[0x3C]; set => Data[0x3C] = value; } // Hoenn 1a
        private byte RIB5 { get => Data[0x3D]; set => Data[0x3D] = value; } // Hoenn 1b
        private byte RIB6 { get => Data[0x3E]; set => Data[0x3E] = value; } // Hoenn 2a
        private byte RIB7 { get => Data[0x3F]; set => Data[0x3F] = value; } // Hoenn 2b
        public bool RibbonG3Cool            { get => (RIB4 & (1 << 0)) == 1 << 0; set => RIB4 = (byte)((RIB4 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonG3CoolSuper       { get => (RIB4 & (1 << 1)) == 1 << 1; set => RIB4 = (byte)((RIB4 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonG3CoolHyper       { get => (RIB4 & (1 << 2)) == 1 << 2; set => RIB4 = (byte)((RIB4 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonG3CoolMaster      { get => (RIB4 & (1 << 3)) == 1 << 3; set => RIB4 = (byte)((RIB4 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonG3Beauty          { get => (RIB4 & (1 << 4)) == 1 << 4; set => RIB4 = (byte)((RIB4 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonG3BeautySuper     { get => (RIB4 & (1 << 5)) == 1 << 5; set => RIB4 = (byte)((RIB4 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonG3BeautyHyper     { get => (RIB4 & (1 << 6)) == 1 << 6; set => RIB4 = (byte)((RIB4 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RibbonG3BeautyMaster    { get => (RIB4 & (1 << 7)) == 1 << 7; set => RIB4 = (byte)((RIB4 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public bool RibbonG3Cute            { get => (RIB5 & (1 << 0)) == 1 << 0; set => RIB5 = (byte)((RIB5 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonG3CuteSuper       { get => (RIB5 & (1 << 1)) == 1 << 1; set => RIB5 = (byte)((RIB5 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonG3CuteHyper       { get => (RIB5 & (1 << 2)) == 1 << 2; set => RIB5 = (byte)((RIB5 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonG3CuteMaster      { get => (RIB5 & (1 << 3)) == 1 << 3; set => RIB5 = (byte)((RIB5 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonG3Smart           { get => (RIB5 & (1 << 4)) == 1 << 4; set => RIB5 = (byte)((RIB5 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonG3SmartSuper      { get => (RIB5 & (1 << 5)) == 1 << 5; set => RIB5 = (byte)((RIB5 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonG3SmartHyper      { get => (RIB5 & (1 << 6)) == 1 << 6; set => RIB5 = (byte)((RIB5 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RibbonG3SmartMaster     { get => (RIB5 & (1 << 7)) == 1 << 7; set => RIB5 = (byte)((RIB5 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public bool RibbonG3Tough           { get => (RIB6 & (1 << 0)) == 1 << 0; set => RIB6 = (byte)((RIB6 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonG3ToughSuper      { get => (RIB6 & (1 << 1)) == 1 << 1; set => RIB6 = (byte)((RIB6 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonG3ToughHyper      { get => (RIB6 & (1 << 2)) == 1 << 2; set => RIB6 = (byte)((RIB6 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonG3ToughMaster     { get => (RIB6 & (1 << 3)) == 1 << 3; set => RIB6 = (byte)((RIB6 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonChampionG3Hoenn   { get => (RIB6 & (1 << 4)) == 1 << 4; set => RIB6 = (byte)((RIB6 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonWinning           { get => (RIB6 & (1 << 5)) == 1 << 5; set => RIB6 = (byte)((RIB6 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonVictory           { get => (RIB6 & (1 << 6)) == 1 << 6; set => RIB6 = (byte)((RIB6 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RibbonArtist            { get => (RIB6 & (1 << 7)) == 1 << 7; set => RIB6 = (byte)((RIB6 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public bool RibbonEffort            { get => (RIB7 & (1 << 0)) == 1 << 0; set => RIB7 = (byte)((RIB7 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonChampionBattle    { get => (RIB7 & (1 << 1)) == 1 << 1; set => RIB7 = (byte)((RIB7 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonChampionRegional  { get => (RIB7 & (1 << 2)) == 1 << 2; set => RIB7 = (byte)((RIB7 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonChampionNational  { get => (RIB7 & (1 << 3)) == 1 << 3; set => RIB7 = (byte)((RIB7 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonCountry           { get => (RIB7 & (1 << 4)) == 1 << 4; set => RIB7 = (byte)((RIB7 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonNational          { get => (RIB7 & (1 << 5)) == 1 << 5; set => RIB7 = (byte)((RIB7 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonEarth             { get => (RIB7 & (1 << 6)) == 1 << 6; set => RIB7 = (byte)((RIB7 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RibbonWorld             { get => (RIB7 & (1 << 7)) == 1 << 7; set => RIB7 = (byte)((RIB7 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }

        public override bool FatefulEncounter { get => (Data[0x40] & 1) == 1; set => Data[0x40] = (byte)((Data[0x40] & ~0x01) | (value ? 1 : 0)); }
        public override int Gender { get => (Data[0x40] >> 1) & 0x3; set => Data[0x40] = (byte)((Data[0x40] & ~0x06) | (value << 1)); }
        public override int AltForm { get => Data[0x40] >> 3; set => Data[0x40] = (byte)((Data[0x40] & 0x07) | (value << 3)); }
        public override int Nature { get => Data[0x41]; set => Data[0x41] = (byte)value; }
        public bool HiddenAbility { get => (Data[0x42] & 1) == 1; set => Data[0x42] = (byte)((Data[0x42] & ~0x01) | (value ? 1 : 0)); }
        public bool NPokémon { get => (Data[0x42] & 2) == 2; set => Data[0x42] = (byte)((Data[0x42] & ~0x02) | (value ? 2 : 0)); }
        // 0x43-0x47 Unused
        #endregion

        #region Block C
        public override string Nickname { get => GetString(0x48, 22); set => SetString(value, 11).CopyTo(Data, 0x48); }
        // 0x5E unused
        public override int Version { get => Data[0x5F]; set => Data[0x5F] = (byte)value; }
        private byte RIB8 { get => Data[0x60]; set => Data[0x60] = value; } // Sinnoh 3
        private byte RIB9 { get => Data[0x61]; set => Data[0x61] = value; } // Sinnoh 4
        private byte RIBA { get => Data[0x62]; set => Data[0x62] = value; } // Sinnoh 5
        private byte RIBB { get => Data[0x63]; set => Data[0x63] = value; } // Sinnoh 6
        public bool RibbonG4Cool            { get => (RIB8 & (1 << 0)) == 1 << 0; set => RIB8 = (byte)((RIB8 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonG4CoolGreat       { get => (RIB8 & (1 << 1)) == 1 << 1; set => RIB8 = (byte)((RIB8 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonG4CoolUltra       { get => (RIB8 & (1 << 2)) == 1 << 2; set => RIB8 = (byte)((RIB8 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonG4CoolMaster      { get => (RIB8 & (1 << 3)) == 1 << 3; set => RIB8 = (byte)((RIB8 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonG4Beauty          { get => (RIB8 & (1 << 4)) == 1 << 4; set => RIB8 = (byte)((RIB8 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonG4BeautyGreat     { get => (RIB8 & (1 << 5)) == 1 << 5; set => RIB8 = (byte)((RIB8 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonG4BeautyUltra     { get => (RIB8 & (1 << 6)) == 1 << 6; set => RIB8 = (byte)((RIB8 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RibbonG4BeautyMaster    { get => (RIB8 & (1 << 7)) == 1 << 7; set => RIB8 = (byte)((RIB8 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public bool RibbonG4Cute            { get => (RIB9 & (1 << 0)) == 1 << 0; set => RIB9 = (byte)((RIB9 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonG4CuteGreat       { get => (RIB9 & (1 << 1)) == 1 << 1; set => RIB9 = (byte)((RIB9 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonG4CuteUltra       { get => (RIB9 & (1 << 2)) == 1 << 2; set => RIB9 = (byte)((RIB9 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonG4CuteMaster      { get => (RIB9 & (1 << 3)) == 1 << 3; set => RIB9 = (byte)((RIB9 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonG4Smart           { get => (RIB9 & (1 << 4)) == 1 << 4; set => RIB9 = (byte)((RIB9 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonG4SmartGreat      { get => (RIB9 & (1 << 5)) == 1 << 5; set => RIB9 = (byte)((RIB9 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonG4SmartUltra      { get => (RIB9 & (1 << 6)) == 1 << 6; set => RIB9 = (byte)((RIB9 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RibbonG4SmartMaster     { get => (RIB9 & (1 << 7)) == 1 << 7; set => RIB9 = (byte)((RIB9 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public bool RibbonG4Tough           { get => (RIBA & (1 << 0)) == 1 << 0; set => RIBA = (byte)((RIBA & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonG4ToughGreat      { get => (RIBA & (1 << 1)) == 1 << 1; set => RIBA = (byte)((RIBA & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonG4ToughUltra      { get => (RIBA & (1 << 2)) == 1 << 2; set => RIBA = (byte)((RIBA & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonG4ToughMaster     { get => (RIBA & (1 << 3)) == 1 << 3; set => RIBA = (byte)((RIBA & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RIBA_4 { get => (RIBA & (1 << 4)) == 1 << 4; set => RIBA = (byte)((RIBA & ~(1 << 4)) | (value ? 1 << 4 : 0)); } // Unused
        public bool RIBA_5 { get => (RIBA & (1 << 5)) == 1 << 5; set => RIBA = (byte)((RIBA & ~(1 << 5)) | (value ? 1 << 5 : 0)); } // Unused
        public bool RIBA_6 { get => (RIBA & (1 << 6)) == 1 << 6; set => RIBA = (byte)((RIBA & ~(1 << 6)) | (value ? 1 << 6 : 0)); } // Unused
        public bool RIBA_7 { get => (RIBA & (1 << 7)) == 1 << 7; set => RIBA = (byte)((RIBA & ~(1 << 7)) | (value ? 1 << 7 : 0)); } // Unused
        public bool RIBB_0 { get => (RIBB & (1 << 0)) == 1 << 0; set => RIBB = (byte)((RIBB & ~(1 << 0)) | (value ? 1 << 0 : 0)); } // Unused
        public bool RIBB_1 { get => (RIBB & (1 << 1)) == 1 << 1; set => RIBB = (byte)((RIBB & ~(1 << 1)) | (value ? 1 << 1 : 0)); } // Unused
        public bool RIBB_2 { get => (RIBB & (1 << 2)) == 1 << 2; set => RIBB = (byte)((RIBB & ~(1 << 2)) | (value ? 1 << 2 : 0)); } // Unused
        public bool RIBB_3 { get => (RIBB & (1 << 3)) == 1 << 3; set => RIBB = (byte)((RIBB & ~(1 << 3)) | (value ? 1 << 3 : 0)); } // Unused
        public bool RIBB_4 { get => (RIBB & (1 << 4)) == 1 << 4; set => RIBB = (byte)((RIBB & ~(1 << 4)) | (value ? 1 << 4 : 0)); } // Unused
        public bool RIBB_5 { get => (RIBB & (1 << 5)) == 1 << 5; set => RIBB = (byte)((RIBB & ~(1 << 5)) | (value ? 1 << 5 : 0)); } // Unused
        public bool RIBB_6 { get => (RIBB & (1 << 6)) == 1 << 6; set => RIBB = (byte)((RIBB & ~(1 << 6)) | (value ? 1 << 6 : 0)); } // Unused
        public bool RIBB_7 { get => (RIBB & (1 << 7)) == 1 << 7; set => RIBB = (byte)((RIBB & ~(1 << 7)) | (value ? 1 << 7 : 0)); } // Unused
        // 0x64-0x67 Unused
        #endregion

        #region Block D
        public override string OT_Name { get => GetString(0x68, 0x16); set => SetString(value, 7).CopyTo(Data, 0x68); }
        public override int Egg_Year { get => Data[0x78]; set => Data[0x78] = (byte)value; }
        public override int Egg_Month { get => Data[0x79]; set => Data[0x79] = (byte)value; }
        public override int Egg_Day { get => Data[0x7A]; set => Data[0x7A] = (byte)value; }
        public override int Met_Year { get => Data[0x7B]; set => Data[0x7B] = (byte)value; }
        public override int Met_Month { get => Data[0x7C]; set => Data[0x7C] = (byte)value; }
        public override int Met_Day { get => Data[0x7D]; set => Data[0x7D] = (byte)value; }
        public override int Egg_Location { get => BitConverter.ToUInt16(Data, 0x7E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7E); }
        public override int Met_Location { get => BitConverter.ToUInt16(Data, 0x80); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x80); }
        private byte PKRS { get => Data[0x82]; set => Data[0x82] = value; }
        public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)((PKRS & ~0xF) | value); }
        public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)((PKRS & 0xF) | (value << 4)); }
        public override int Ball { get => Data[0x83]; set => Data[0x83] = (byte)value; }
        public override int Met_Level { get => Data[0x84] & ~0x80; set => Data[0x84] = (byte)((Data[0x84] & 0x80) | value); }
        public override int OT_Gender { get => Data[0x84] >> 7; set => Data[0x84] = (byte)((Data[0x84] & ~0x80) | value << 7); }
        public override int EncounterType { get => Data[0x85]; set => Data[0x85] = (byte)value; }
        // 0x86 Unused
        public byte PokeStarFame { get => Data[0x87]; set => Data[0x87] = value; }
        public bool IsPokeStar { get => PokeStarFame > 250; set => PokeStarFame = (byte)(value ? 255 : 0); }
        #endregion

        #region Battle Stats
        public override int Status_Condition { get => BitConverter.ToInt32(Data, 0x88); set => BitConverter.GetBytes(value).CopyTo(Data, 0x88); }
        public override int Stat_Level { get => Data[0x8C]; set => Data[0x8C] = (byte)value; }
        public override int Stat_HPCurrent { get => BitConverter.ToUInt16(Data, 0x8E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x8E); }
        public override int Stat_HPMax { get => BitConverter.ToUInt16(Data, 0x90); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x90); }
        public override int Stat_ATK { get => BitConverter.ToUInt16(Data, 0x92); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x92); }
        public override int Stat_DEF { get => BitConverter.ToUInt16(Data, 0x94); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x94); }
        public override int Stat_SPE { get => BitConverter.ToUInt16(Data, 0x96); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x96); }
        public override int Stat_SPA { get => BitConverter.ToUInt16(Data, 0x98); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x98); }
        public override int Stat_SPD { get => BitConverter.ToUInt16(Data, 0x9A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x9A); }
        public byte[] HeldMailData { get => Data.Skip(0x9C).Take(0x38).ToArray(); set => value.CopyTo(Data, 0x9C); }
        #endregion

        // Generated Attributes
        public override int PSV => (int)((PID >> 16 ^ (PID & 0xFFFF)) >> 3);
        public override int TSV => (TID ^ SID) >> 3;

        public override int Characteristic
        {
            get
            {
                int pm6 = (int)(PID % 6); // PID
                int maxIV = MaximumIV;
                int pm6stat = 0;
                for (int i = 0; i < 6; i++)
                {
                    pm6stat = (pm6 + i) % 6;
                    if (GetIV(pm6stat) == maxIV)
                        break;
                }
                return (pm6stat * 5) + (maxIV % 5);
            }
        }

        // Maximums
        public override int MaxMoveID => Legal.MaxMoveID_5;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_5;
        public override int MaxAbilityID => Legal.MaxAbilityID_5;
        public override int MaxItemID => Legal.MaxItemID_5_B2W2;
        public override int MaxBallID => Legal.MaxBallID_5;
        public override int MaxGameID => Legal.MaxGameID_5; // B2
        public override int MaxIV => 31;
        public override int MaxEV => 255;
        public override int OTLength => 7;
        public override int NickLength => 10;

        // Methods
        protected override byte[] Encrypt()
        {
            RefreshChecksum();
            return PKX.EncryptArray45(Data);
        }

        // Synthetic Trading Logic
        public bool Trade(string SAV_Trainer, int SAV_TID, int SAV_SID, int SAV_GENDER, int Day = 1, int Month = 1, int Year = 2013)
        {
            if (IsEgg && !(SAV_Trainer == OT_Name && SAV_TID == TID && SAV_SID == SID && SAV_GENDER == OT_Gender))
            {
                SetLinkTradeEgg(Day, Month, Year, Locations.LinkTrade5);
                return true;
            }
            return false;
        }

        public PK6 ConvertToPK6()
        {
            PK6 pk6 = new PK6 // Convert away!
            {
                EncryptionConstant = PID,
                Species = Species,
                TID = TID,
                SID = SID,
                EXP = EXP,
                PID = PID,
                Ability = Ability
            };

            int[] abilities = PersonalInfo.Abilities;
            int abilval = Array.IndexOf(abilities, Ability);
            if (abilval >= 0 && abilities[abilval] == abilities[2] && HiddenAbility)
                abilval = 2; // hidden ability shared with a regular ability
            if (abilval >= 0)
            {
                pk6.AbilityNumber = 1 << abilval;
            }
            else // Fallback (shouldn't happen)
            {
                if (HiddenAbility) pk6.AbilityNumber = 4; // Hidden, else G5 or G3/4 correlation.
                else pk6.AbilityNumber = Gen5 ? 1 << (int)(PID >> 16 & 1) : 1 << (int)(PID & 1);
            }
            pk6.Markings = Markings;
            pk6.Language = Math.Max((int)LanguageID.Japanese, Language); // Hacked or Bad IngameTrade (Japanese B/W)

            pk6.CNT_Cool = CNT_Cool;
            pk6.CNT_Beauty = CNT_Beauty;
            pk6.CNT_Cute = CNT_Cute;
            pk6.CNT_Smart = CNT_Smart;
            pk6.CNT_Tough = CNT_Tough;
            pk6.CNT_Sheen = CNT_Sheen;

            // Cap EVs
            pk6.EV_HP = EV_HP > 252 ? 252 : EV_HP;
            pk6.EV_ATK = EV_ATK > 252 ? 252 : EV_ATK;
            pk6.EV_DEF = EV_DEF > 252 ? 252 : EV_DEF;
            pk6.EV_SPA = EV_SPA > 252 ? 252 : EV_SPA;
            pk6.EV_SPD = EV_SPD > 252 ? 252 : EV_SPD;
            pk6.EV_SPE = EV_SPE > 252 ? 252 : EV_SPE;

            pk6.Move1 = Move1;
            pk6.Move2 = Move2;
            pk6.Move3 = Move3;
            pk6.Move4 = Move4;

            pk6.Move1_PPUps = Move1_PPUps;
            pk6.Move2_PPUps = Move2_PPUps;
            pk6.Move3_PPUps = Move3_PPUps;
            pk6.Move4_PPUps = Move4_PPUps;

            // Fix PP
            pk6.HealPP();

            pk6.IV_HP = IV_HP;
            pk6.IV_ATK = IV_ATK;
            pk6.IV_DEF = IV_DEF;
            pk6.IV_SPA = IV_SPA;
            pk6.IV_SPD = IV_SPD;
            pk6.IV_SPE = IV_SPE;
            pk6.IsEgg = IsEgg;
            pk6.IsNicknamed = IsNicknamed;

            pk6.FatefulEncounter = FatefulEncounter;
            pk6.Gender = Gender;
            pk6.AltForm = AltForm;
            pk6.Nature = Nature;

            // Apply trash bytes for species name of current app language -- default to PKM's language if no match
            int curLang = SpeciesName.GetSpeciesNameLanguage(Species, Nickname, Format);
            pk6.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, curLang < 0 ? Language : curLang, pk6.Format);
            if (IsNicknamed)
                pk6.Nickname = Nickname;

            pk6.Version = Version;

            pk6.OT_Name = OT_Name;

            // Dates are kept upon transfer
            pk6.MetDate = MetDate;
            pk6.EggMetDate = EggMetDate;

            // Locations are kept upon transfer
            pk6.Met_Location = Met_Location;
            pk6.Egg_Location = Egg_Location;

            pk6.PKRS_Strain = PKRS_Strain;
            pk6.PKRS_Days = PKRS_Days;
            pk6.Ball = Ball;

            // OT Gender & Encounter Level
            pk6.Met_Level = Met_Level;
            pk6.OT_Gender = OT_Gender;
            pk6.EncounterType = EncounterType;

            // Ribbon Decomposer (Contest & Battle)
            byte contestribbons = 0;
            byte battleribbons = 0;

            // Contest Ribbon Counter
            for (int i = 0; i < 8; i++) // Sinnoh 3, Hoenn 1
            {
                if ((Data[0x60] >> i & 1) == 1) contestribbons++;
                if (((Data[0x61] >> i) & 1) == 1) contestribbons++;
                if (((Data[0x3C] >> i) & 1) == 1) contestribbons++;
                if (((Data[0x3D] >> i) & 1) == 1) contestribbons++;
            }
            for (int i = 0; i < 4; i++) // Sinnoh 4, Hoenn 2
            {
                if (((Data[0x62] >> i) & 1) == 1) contestribbons++;
                if (((Data[0x3E] >> i) & 1) == 1) contestribbons++;
            }

            // Battle Ribbon Counter
            if (RibbonWinning) battleribbons++;
            if (RibbonVictory) battleribbons++;
            for (int i = 1; i < 7; i++)     // Sinnoh Battle Ribbons
                if (((Data[0x24] >> i) & 1) == 1) battleribbons++;

            // Fill the Ribbon Counter Bytes
            pk6.RibbonCountMemoryContest = contestribbons;
            pk6.RibbonCountMemoryBattle = battleribbons;

            // Copy Ribbons to their new locations.
            pk6.RibbonChampionG3Hoenn = RibbonChampionG3Hoenn;
            pk6.RibbonChampionSinnoh = RibbonChampionSinnoh;
            pk6.RibbonEffort = RibbonEffort;

            pk6.RibbonAlert = RibbonAlert;
            pk6.RibbonShock = RibbonShock;
            pk6.RibbonDowncast = RibbonDowncast;
            pk6.RibbonCareless = RibbonCareless;
            pk6.RibbonRelax = RibbonRelax;
            pk6.RibbonSnooze = RibbonSnooze;
            pk6.RibbonSmile = RibbonSmile;
            pk6.RibbonGorgeous = RibbonGorgeous;

            pk6.RibbonRoyal = RibbonRoyal;
            pk6.RibbonGorgeousRoyal = RibbonGorgeousRoyal;
            pk6.RibbonArtist = RibbonArtist;
            pk6.RibbonFootprint = RibbonFootprint;
            pk6.RibbonRecord = RibbonRecord;
            pk6.RibbonLegend = RibbonLegend;
            pk6.RibbonCountry = RibbonCountry;
            pk6.RibbonNational = RibbonNational;

            pk6.RibbonEarth = RibbonEarth;
            pk6.RibbonWorld = RibbonWorld;
            pk6.RibbonClassic = RibbonClassic;
            pk6.RibbonPremier = RibbonPremier;
            pk6.RibbonEvent = RibbonEvent;
            pk6.RibbonBirthday = RibbonBirthday;
            pk6.RibbonSpecial = RibbonSpecial;
            pk6.RibbonSouvenir = RibbonSouvenir;

            pk6.RibbonWishing = RibbonWishing;
            pk6.RibbonChampionBattle = RibbonChampionBattle;
            pk6.RibbonChampionRegional = RibbonChampionRegional;
            pk6.RibbonChampionNational = RibbonChampionNational;
            pk6.RibbonChampionWorld = RibbonChampionWorld;

            // Write Transfer Location - location is dependent on 3DS system that transfers.
            pk6.Country = PKMConverter.Country;
            pk6.Region = PKMConverter.Region;
            pk6.ConsoleRegion = PKMConverter.ConsoleRegion;

            // Write the Memories, Friendship, and Origin!
            pk6.CurrentHandler = 1;
            pk6.HT_Name = PKMConverter.OT_Name;
            pk6.HT_Gender = PKMConverter.OT_Gender;
            pk6.Geo1_Region = PKMConverter.Region;
            pk6.Geo1_Country = PKMConverter.Country;
            pk6.HT_Intensity = 1;
            pk6.HT_Memory = 4;
            pk6.HT_Feeling = Memories.GetRandomFeeling(pk6.HT_Memory);
            // When transferred, friendship gets reset.
            pk6.OT_Friendship = pk6.HT_Friendship = PersonalInfo.BaseFriendship;

            // Gen6 changed the shiny correlation to have 2x the rate.
            // If the current PID would be shiny with those increased odds, fix it.
            if ((PSV ^ TSV) == 1)
                pk6.PID ^= 0x80000000;

            // HMs are not deleted 5->6, transfer away (but fix if blank spots?)
            pk6.FixMoves();

            // Fix Name Strings
            pk6.Nickname = StringConverter345.TransferGlyphs56(pk6.Nickname);
            pk6.OT_Name = StringConverter345.TransferGlyphs56(pk6.OT_Name);

            // Fix Checksum
            pk6.RefreshChecksum();

            return pk6; // Done!
        }
    }
}
