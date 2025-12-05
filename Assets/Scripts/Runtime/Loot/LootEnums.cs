using System;

namespace NexusProtocol.Loot
{
    /// <summary>
    /// Types of loot sources in the game
    /// </summary>
    public enum LootSourceType
    {
        // Enemy types from GDD
        Enemy_Scrapper = 0,         // Basic enemy
        Enemy_Drone = 1,            // Flying enemy
        Enemy_Bruiser = 2,          // Heavy enemy
        Enemy_Sniper = 3,           // Ranged enemy
        Enemy_Technician = 4,       // Support enemy
        Enemy_Elite = 5,            // Elite variant
        Enemy_Badass = 6,           // Badass variant
        Enemy_MiniBoss = 7,         // Mini-boss
        Enemy_Boss = 8,             // Full boss
        Enemy_RaidBoss = 9,         // Raid boss

        // Container types
        Chest_Common = 20,          // Basic chest
        Chest_Uncommon = 21,        // Better chest
        Chest_Rare = 22,            // Rare chest
        Chest_Epic = 23,            // Epic chest
        Chest_Legendary = 24,       // Legendary chest (red chest)
        Chest_Vault = 25,           // Vault chest (end of area)

        // Quest and mission rewards
        Quest_Side = 40,            // Side quest reward
        Quest_Main = 41,            // Main story quest reward
        Quest_Daily = 42,           // Daily challenge reward
        Quest_Weekly = 43,          // Weekly challenge reward

        // World drops
        World_Common = 60,          // Standard world drop
        World_Zone = 61,            // Zone-specific drop
        World_Event = 62,           // Event-specific drop

        // Vendor
        Vendor_Standard = 80,       // Standard vendor inventory
        Vendor_Special = 81,        // Special vendor (legendary)

        // Endgame
        Endgame_ProvingGround = 100,
        Endgame_CircleOfSlaughter = 101,
        Endgame_Raid = 102,
        Endgame_MayhemBonus = 103,
        Endgame_Spiral = 104        // Infinite dungeon
    }

    /// <summary>
    /// Types of items that can drop
    /// </summary>
    public enum LootItemType
    {
        // Weapons
        Weapon_AssaultRifle = 0,
        Weapon_SMG = 1,
        Weapon_Pistol = 2,
        Weapon_Shotgun = 3,
        Weapon_SniperRifle = 4,
        Weapon_RocketLauncher = 5,
        Weapon_Special = 6,

        // Equipment
        Equipment_Shield = 20,
        Equipment_GrenadeMod = 21,
        Equipment_ClassMod = 22,
        Equipment_Relic = 23,

        // Consumables
        Consumable_Health = 40,
        Consumable_Ammo = 41,
        Consumable_Currency = 42,
        Consumable_Eridium = 43,    // Special currency

        // Cosmetics
        Cosmetic_WeaponSkin = 60,
        Cosmetic_CharacterSkin = 61,
        Cosmetic_VehicleSkin = 62,
        Cosmetic_HeadCustomization = 63,

        // Special
        Special_QuestItem = 80,
        Special_KeyItem = 81,
        Special_LegendaryHunt = 82  // Legendary hunt specific
    }

    /// <summary>
    /// Chest tier configuration per GDD
    /// </summary>
    public enum ChestTier
    {
        White = 0,      // Common items only
        Green = 1,      // Up to Uncommon
        Blue = 2,       // Up to Rare
        Purple = 3,     // Up to Epic
        Orange = 4,     // Up to Legendary (Red Chest)
        Cyan = 5,       // Up to Pearlescent (Vault)
        Red = 6         // Apocalypse possible
    }

    /// <summary>
    /// Zones in the game per GDD Level Design
    /// </summary>
    public enum GameZone
    {
        HavenCity = 0,              // Starting area
        FracturedCoast = 1,         // Level 1-10
        ScorchedPlateau = 2,        // Level 10-20
        CrystalCaverns = 3,         // Level 20-30
        NexusWasteland = 4,         // Level 30-40
        CoreFacility = 5,           // Level 40-50

        // Endgame zones
        ProvingGround_Alpha = 10,
        ProvingGround_Beta = 11,
        ProvingGround_Gamma = 12,
        CircleOfSlaughter = 20,
        Raid_NexusCore = 30,
        Raid_TemporalParadox = 31,
        Raid_Singularity = 32,
        TheSpiral = 40              // Infinite dungeon
    }

    /// <summary>
    /// Mayhem levels for endgame scaling
    /// </summary>
    public enum MayhemLevel
    {
        None = 0,
        Mayhem1 = 1,
        Mayhem2 = 2,
        Mayhem3 = 3,
        Mayhem4 = 4,
        Mayhem5 = 5,
        Mayhem6 = 6,    // Exclusive legendaries start here
        Mayhem7 = 7,
        Mayhem8 = 8,
        Mayhem9 = 9,
        Mayhem10 = 10
    }

    /// <summary>
    /// Drop conditions for conditional loot
    /// </summary>
    [Flags]
    public enum DropCondition
    {
        None = 0,
        FirstKill = 1 << 0,         // Only on first kill
        Coop = 1 << 1,              // Only in co-op
        Solo = 1 << 2,              // Only in solo
        MayhemMode = 1 << 3,        // Only in Mayhem mode
        Mayhem6Plus = 1 << 4,       // Mayhem 6 or higher
        QuestActive = 1 << 5,       // Specific quest must be active
        QuestComplete = 1 << 6,     // Specific quest must be complete
        TimeOfDay = 1 << 7,         // Specific time (darkness bonus)
        LowHealth = 1 << 8,         // Player below 25% health
        NoDeaths = 1 << 9           // No deaths in encounter
    }
}
