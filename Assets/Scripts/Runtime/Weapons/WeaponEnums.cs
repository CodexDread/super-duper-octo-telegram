using System;

namespace NexusProtocol.Weapons
{
    /// <summary>
    /// The 6 weapon part categories as defined in GDD_v3
    /// Note: Ammo is handled separately through the ammo system
    /// </summary>
    public enum WeaponPartCategory
    {
        Receiver = 0,   // Determines Manufacturer
        Barrel = 1,     // Projectile Behavior
        Magazine = 2,   // Ammunition Management
        Grip = 3,       // Handling
        Stock = 4,      // Stability
        Sight = 5       // Targeting
    }

    /// <summary>
    /// The 7 rarity tiers as defined in GDD_v3
    /// Each tier has specific stat improvements and drop rates
    /// </summary>
    public enum WeaponRarity
    {
        Common = 1,         // Gray - Base stats, no bonus effects, 60% drop rate
        Uncommon = 2,       // Green - +5-10% stat improvement, 25% drop rate
        Rare = 3,           // Blue - +10-20% stat improvement, minor bonus effects possible, 10% drop rate
        Epic = 4,           // Purple - +20-35% stat improvement, guaranteed minor effect, 4% drop rate
        Legendary = 5,      // Orange - +35-50% stat improvement, major unique effect, 0.9% drop rate
        Pearlescent = 6,    // Cyan - +50-75% stat improvement, multiple effects, 0.09% drop rate
        Apocalypse = 7      // Red - +75-100% stat improvement, game-breaking effects, 0.01% drop rate
    }

    /// <summary>
    /// Part type tiers within each rarity
    /// </summary>
    public enum WeaponPartType
    {
        Standard = 0,   // Basic version, normal stat modifiers, common within rarity
        Special = 1,    // Enhanced version, additional minor effect, uncommon within rarity
        Exotic = 2      // Radical modification, completely changes behavior, rare within rarity
    }

    /// <summary>
    /// The 7 weapon manufacturers as defined in GDD_v3 and Editor_Tools_Development_Prompt
    /// </summary>
    public enum WeaponManufacturer
    {
        KDC = 0,            // Kinetic Dynamics Corporation - Reliability through simplicity
        FrontierArms = 1,   // Frontier Arms - Replaced Wasteland Works per changelog
        TekCorp = 2,        // TekCorp Industries - Smart weapons for a smart soldier
        QuantumDynamics = 3,// Quantum Dynamics - Why follow physics when you can rewrite them?
        VoidIndustries = 4, // Void Industries - Embrace the void
        NexusSalvage = 5,   // Nexus Salvage Co. - Turn their weapons against them
        RedLine = 6         // RedLine - Per Editor_Tools_Development_Prompt
    }

    /// <summary>
    /// Stat types that can be modified by weapon parts
    /// </summary>
    public enum WeaponStatType
    {
        // Core Stats
        Damage,
        FireRate,
        Accuracy,
        ReloadSpeed,
        MagazineSize,
        CriticalChance,
        CriticalDamage,

        // Handling Stats
        Recoil,
        Handling,
        WeaponSwapSpeed,
        ADSSpeed,

        // Range Stats
        Range,
        ProjectileSpeed,
        Penetration,

        // Special Stats
        ElementalChance,
        ElementalDamage,
        SplashRadius,
        MeleeDamage
    }

    /// <summary>
    /// Elemental damage types
    /// </summary>
    public enum ElementType
    {
        None = 0,
        Fire = 1,       // Burn DoT
        Shock = 2,      // Chain lightning, shield damage
        Corrosive = 3,  // Armor damage, DoT
        Cryo = 4,       // Slow/freeze
        Radiation = 5,  // AoE DoT, spreads
        Quantum = 6     // Phase through shields
    }
}
