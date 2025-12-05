using System;

namespace NexusProtocol.Zones
{
    /*
     * ZONE SYSTEM DESIGN:
     *
     * Zones are discrete areas in the game world with specific:
     * - Level ranges for enemy scaling
     * - Environmental hazards
     * - Motor Pool locations for vehicle spawning
     * - Respawn points for player revival
     * - Cover points for tactical combat
     * - Random encounter zones for dynamic content
     *
     * ZONE TYPES:
     * - Story Zones: Main progression areas (Fractured Coast to Core Facility)
     * - Hub Zone: Haven City - main player settlement
     * - Endgame Zones: Proving Grounds, Circle of Slaughter, Raids, The Spiral
     *
     * VEHICLE SYSTEM:
     * - Motor Pool stations allow vehicle spawning
     * - Each player can have one vehicle at a time
     * - 30-second cooldown between spawns
     * - Fast travel between discovered Motor Pools
     */

    /// <summary>
    /// Main zone identifiers matching GDD Level Design
    /// </summary>
    public enum ZoneType
    {
        // Hub
        HavenCity = 0,              // Starting area, main settlement

        // Story Zones (Linear Progression)
        FracturedCoast = 1,         // Level 1-10, Act 1
        ScorchedPlateau = 2,        // Level 10-20, Act 2
        CrystalCaverns = 3,         // Level 20-30, Act 2-3
        NexusWasteland = 4,         // Level 30-40, Act 3
        CoreFacility = 5,           // Level 40-50, Act 4

        // Endgame Zones
        ProvingGround_Alpha = 10,   // Time trial arena
        ProvingGround_Beta = 11,    // Time trial arena
        ProvingGround_Gamma = 12,   // Time trial arena
        CircleOfSlaughter = 20,     // Wave-based survival
        Raid_NexusCore = 30,        // 4-player raid
        Raid_TemporalParadox = 31,  // 4-player raid with time mechanics
        Raid_Singularity = 32,      // 4-player raid, final
        TheSpiral = 40              // Infinite procedural dungeon
    }

    /// <summary>
    /// Sub-area types within zones
    /// </summary>
    public enum SubAreaType
    {
        OpenWorld = 0,              // Standard explorable area
        Dungeon = 1,                // Enclosed combat area
        Outpost = 2,                // Player-friendly settlement
        EnemyCamp = 3,              // Enemy stronghold
        VehicleArena = 4,           // Vehicle combat zone
        BossArena = 5,              // Boss fight area
        SafeZone = 6,               // No combat area
        TransitionArea = 7,         // Zone connection
        SecretArea = 8,             // Hidden area
        VaultRoom = 9               // End-of-area treasure room
    }

    /// <summary>
    /// Environmental hazard types
    /// </summary>
    public enum HazardType
    {
        None = 0,
        Radiation = 1,              // Damage over time, reduced by resistance
        Toxic = 2,                  // Corrosive damage, slows movement
        Fire = 3,                   // Burn damage, spreads
        Electric = 4,               // Shock damage, chains to nearby
        Cryo = 5,                   // Freeze effect, slows then immobilizes
        Void = 6,                   // NEXUS corruption, drains shields
        Explosive = 7,              // Instant damage on trigger
        Falling = 8,                // Bottomless pit / fall damage
        Crushing = 9                // Moving environmental hazard
    }

    /// <summary>
    /// Spawn point types for players
    /// </summary>
    public enum SpawnPointType
    {
        ZoneEntry = 0,              // First entry to zone
        Checkpoint = 1,             // Progress checkpoint
        SafeRoom = 2,               // Safe area respawn
        FastTravel = 3,             // Fast travel destination
        BossCheckpoint = 4,         // Before boss encounter
        CoopJoin = 5,               // Co-op player join location
        RespawnAnchor = 6           // Fight-for-your-life respawn
    }

    /// <summary>
    /// Motor Pool station types
    /// </summary>
    public enum MotorPoolType
    {
        Standard = 0,               // Basic vehicle spawner
        Outpost = 1,                // Settlement vehicle station
        Hidden = 2,                 // Discoverable secret station
        Premium = 3,                // Special vehicles available
        Temporary = 4               // Mission-specific, disappears
    }

    /// <summary>
    /// Cover point types for tactical combat
    /// </summary>
    public enum CoverType
    {
        LowCover = 0,               // Crouch required, can peek over
        HighCover = 1,              // Standing cover, side peek only
        DestructibleLow = 2,        // Low cover that can be destroyed
        DestructibleHigh = 3,       // High cover that can be destroyed
        VaultableLow = 4,           // Can vault over
        CornerCover = 5,            // Corner piece for side peeking
        VehicleCover = 6            // Cover from vehicles
    }

    /// <summary>
    /// Random encounter types
    /// </summary>
    public enum EncounterType
    {
        None = 0,
        Patrol = 1,                 // Moving enemy group
        Ambush = 2,                 // Triggered enemy spawn
        DefendPoint = 3,            // Protect location
        Assault = 4,                // Attack enemy position
        Convoy = 5,                 // Vehicle escort/attack
        Boss = 6,                   // Mini-boss encounter
        Treasure = 7,               // Loot-focused encounter
        Rescue = 8,                 // Save NPC encounter
        HuntTarget = 9              // Legendary hunt target
    }

    /// <summary>
    /// Encounter difficulty modifier
    /// </summary>
    public enum EncounterDifficulty
    {
        Trivial = 0,                // Easy, few enemies
        Easy = 1,                   // Below zone level
        Normal = 2,                 // At zone level
        Hard = 3,                   // Above zone level
        Badass = 4,                 // Includes badass enemies
        Elite = 5,                  // All elite enemies
        Boss = 6                    // Boss-tier encounter
    }

    /// <summary>
    /// Point of interest types
    /// </summary>
    public enum POIType
    {
        None = 0,
        Chest = 1,                  // Loot container
        Ammo = 2,                   // Ammo refill
        HealthStation = 3,          // Health refill
        Vendor = 4,                 // Shop
        QuestGiver = 5,             // NPC with quest
        LoreObject = 6,             // Collectible lore
        SecretSwitch = 7,           // Hidden mechanism
        Elevator = 8,               // Vertical transport
        Vehicle = 9,                // Abandoned vehicle (pickup)
        SkillChallenge = 10         // Optional challenge
    }

    /// <summary>
    /// Connection types between zones/areas
    /// </summary>
    public enum ConnectionType
    {
        Door = 0,                   // Standard door
        Gate = 1,                   // Large gate (vehicles)
        Tunnel = 2,                 // Underground passage
        Elevator = 3,               // Vertical connection
        Teleporter = 4,             // Instant transport
        VehicleRamp = 5,            // Vehicle-only path
        ClimbingPath = 6,           // Marked climbing surface
        SecretPassage = 7           // Hidden connection
    }

    /// <summary>
    /// Zone discovery state
    /// </summary>
    public enum DiscoveryState
    {
        Undiscovered = 0,           // Not yet found
        Discovered = 1,             // Found but not explored
        Explored = 2,               // Partially explored
        Completed = 3,              // Fully explored/completed
        Mastered = 4                // All challenges done
    }

    /// <summary>
    /// Flags for zone features
    /// </summary>
    [Flags]
    public enum ZoneFeatures
    {
        None = 0,
        HasVehicles = 1 << 0,       // Vehicles allowed/required
        HasMotorPool = 1 << 1,      // Motor Pool station present
        HasFastTravel = 1 << 2,     // Fast travel available
        HasVendor = 1 << 3,         // Vendor present
        HasBoss = 1 << 4,           // Boss encounter present
        HasRaid = 1 << 5,           // Raid content
        IsEndgame = 1 << 6,         // Endgame zone
        IsSafeZone = 1 << 7,        // No enemies spawn
        HasSecrets = 1 << 8,        // Hidden areas present
        IsProcedurallyGenerated = 1 << 9, // Procedural content
        IsMayhemEnabled = 1 << 10,  // Mayhem mode available
        HasLegendaryHunt = 1 << 11  // Legendary hunt target
    }

    /// <summary>
    /// Weather/atmosphere types for zones
    /// </summary>
    public enum ZoneAtmosphere
    {
        Clear = 0,                  // Normal conditions
        Foggy = 1,                  // Reduced visibility
        Stormy = 2,                 // Rain, lightning hazard
        Sandstorm = 3,              // Desert conditions
        Toxic = 4,                  // Green haze, damage
        Dark = 5,                   // Low light, flashlight needed
        Corrupted = 6,              // NEXUS influence
        Frozen = 7                  // Ice, slippery surfaces
    }
}
