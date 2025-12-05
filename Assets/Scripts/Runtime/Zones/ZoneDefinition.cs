using System;
using System.Collections.Generic;
using UnityEngine;

namespace NexusProtocol.Zones
{
    /// <summary>
    /// Defines a zone in the game world with all configuration data.
    /// Zones contain level ranges, spawn points, hazards, and encounter zones.
    /// </summary>
    [CreateAssetMenu(fileName = "NewZone", menuName = "Nexus Protocol/Zones/Zone Definition")]
    public class ZoneDefinition : ScriptableObject
    {
        [Header("Zone Identity")]
        [Tooltip("Unique identifier for this zone")]
        public string zoneId;

        [Tooltip("Display name of the zone")]
        public string zoneName;

        [Tooltip("Type of zone")]
        public ZoneType zoneType;

        [Tooltip("Description of the zone")]
        [TextArea(2, 4)]
        public string description;

        [Header("Level Configuration")]
        [Tooltip("Minimum player level for this zone")]
        [Range(1, 50)]
        public int minLevel = 1;

        [Tooltip("Maximum player level for this zone")]
        [Range(1, 50)]
        public int maxLevel = 10;

        [Tooltip("Recommended player level")]
        [Range(1, 50)]
        public int recommendedLevel = 5;

        [Header("Zone Features")]
        [Tooltip("Features available in this zone")]
        public ZoneFeatures features;

        [Tooltip("Atmosphere/weather type")]
        public ZoneAtmosphere atmosphere = ZoneAtmosphere.Clear;

        [Header("Spawn Points")]
        [Tooltip("Player spawn/respawn points in this zone")]
        public SpawnPointData[] spawnPoints;

        [Header("Motor Pool Stations")]
        [Tooltip("Vehicle spawn stations in this zone")]
        public MotorPoolData[] motorPools;

        [Header("Cover Points")]
        [Tooltip("Tactical cover positions (can be auto-generated)")]
        public CoverPointData[] coverPoints;

        [Header("Environmental Hazards")]
        [Tooltip("Hazard areas in this zone")]
        public HazardAreaData[] hazardAreas;

        [Header("Encounter Zones")]
        [Tooltip("Random encounter spawn zones")]
        public EncounterZoneData[] encounterZones;

        [Header("Points of Interest")]
        [Tooltip("Important locations in the zone")]
        public POIData[] pointsOfInterest;

        [Header("Connections")]
        [Tooltip("Connections to other zones")]
        public ZoneConnectionData[] connections;

        [Header("Sub-Areas")]
        [Tooltip("Sub-areas within this zone")]
        public SubAreaData[] subAreas;

        [Header("Visual")]
        [Tooltip("Zone icon for map display")]
        public Sprite zoneIcon;

        [Tooltip("Zone minimap image")]
        public Texture2D minimapTexture;

        [Tooltip("Primary color for zone UI")]
        public Color zoneColor = Color.white;

        /// <summary>
        /// Get the level range as a formatted string
        /// </summary>
        public string GetLevelRangeString()
        {
            return $"Level {minLevel}-{maxLevel}";
        }

        /// <summary>
        /// Check if a player level is appropriate for this zone
        /// </summary>
        public bool IsLevelAppropriate(int playerLevel)
        {
            return playerLevel >= minLevel - 3 && playerLevel <= maxLevel + 5;
        }

        /// <summary>
        /// Get scaled enemy level based on player level
        /// </summary>
        public int GetEnemyLevel(int playerLevel)
        {
            return Mathf.Clamp(playerLevel, minLevel, maxLevel);
        }

        /// <summary>
        /// Validate the zone definition
        /// </summary>
        public bool Validate(out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrEmpty(zoneId))
                errors.Add("Zone ID is required");

            if (string.IsNullOrEmpty(zoneName))
                errors.Add("Zone name is required");

            if (minLevel > maxLevel)
                errors.Add("Min level cannot be greater than max level");

            if (recommendedLevel < minLevel || recommendedLevel > maxLevel)
                errors.Add("Recommended level must be within min-max range");

            // Check for at least one spawn point in non-endgame zones
            if (!features.HasFlag(ZoneFeatures.IsEndgame) &&
                (spawnPoints == null || spawnPoints.Length == 0))
            {
                errors.Add("Zone requires at least one spawn point");
            }

            // Validate spawn points
            if (spawnPoints != null)
            {
                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    if (string.IsNullOrEmpty(spawnPoints[i].pointId))
                        errors.Add($"Spawn point {i} requires an ID");
                }
            }

            // Validate motor pools if feature enabled
            if (features.HasFlag(ZoneFeatures.HasMotorPool) &&
                (motorPools == null || motorPools.Length == 0))
            {
                errors.Add("Zone has motor pool feature but no motor pool stations defined");
            }

            // Validate connections
            if (connections != null)
            {
                foreach (var connection in connections)
                {
                    if (connection.targetZone == null)
                        errors.Add($"Connection '{connection.connectionId}' has no target zone");
                }
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Get color based on zone type
        /// </summary>
        public Color GetZoneTypeColor()
        {
            return zoneType switch
            {
                ZoneType.HavenCity => new Color(0.2f, 0.8f, 0.2f),        // Green (safe)
                ZoneType.FracturedCoast => new Color(0.4f, 0.6f, 0.9f),   // Light blue
                ZoneType.ScorchedPlateau => new Color(0.9f, 0.5f, 0.2f),  // Orange
                ZoneType.CrystalCaverns => new Color(0.6f, 0.3f, 0.9f),   // Purple
                ZoneType.NexusWasteland => new Color(0.5f, 0.5f, 0.5f),   // Gray
                ZoneType.CoreFacility => new Color(0.9f, 0.2f, 0.2f),     // Red
                ZoneType.CircleOfSlaughter => new Color(0.8f, 0.1f, 0.1f),// Dark red
                ZoneType.TheSpiral => new Color(0.1f, 0.1f, 0.3f),        // Dark blue
                _ when (int)zoneType >= 10 && (int)zoneType < 20 => new Color(0.9f, 0.9f, 0.2f), // Yellow (proving grounds)
                _ when (int)zoneType >= 30 && (int)zoneType < 40 => new Color(0.9f, 0.4f, 0.9f), // Pink (raids)
                _ => Color.white
            };
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Auto-generate ID if empty
            if (string.IsNullOrEmpty(zoneId) && !string.IsNullOrEmpty(zoneName))
            {
                zoneId = zoneName.Replace(" ", "_").ToLower();
            }

            // Ensure level range is valid
            if (minLevel > maxLevel)
            {
                maxLevel = minLevel;
            }

            // Clamp recommended level
            recommendedLevel = Mathf.Clamp(recommendedLevel, minLevel, maxLevel);
        }
#endif
    }

    /// <summary>
    /// Data for a player spawn point
    /// </summary>
    [Serializable]
    public class SpawnPointData
    {
        [Tooltip("Unique identifier")]
        public string pointId;

        [Tooltip("Display name")]
        public string pointName;

        [Tooltip("Type of spawn point")]
        public SpawnPointType spawnType;

        [Tooltip("World position")]
        public Vector3 position;

        [Tooltip("Spawn rotation")]
        public Vector3 rotation;

        [Tooltip("Is this the default spawn for the zone?")]
        public bool isDefault;

        [Tooltip("Can be used for fast travel")]
        public bool fastTravelEnabled;

        [Tooltip("Must be discovered before use")]
        public bool requiresDiscovery;
    }

    /// <summary>
    /// Data for a Motor Pool vehicle spawn station
    /// </summary>
    [Serializable]
    public class MotorPoolData
    {
        [Tooltip("Unique identifier")]
        public string stationId;

        [Tooltip("Display name")]
        public string stationName;

        [Tooltip("Type of motor pool")]
        public MotorPoolType motorPoolType;

        [Tooltip("World position")]
        public Vector3 position;

        [Tooltip("Vehicle spawn direction")]
        public Vector3 spawnDirection;

        [Tooltip("Vehicle types available")]
        public string[] availableVehicles;

        [Tooltip("Must be discovered before use")]
        public bool requiresDiscovery;

        [Tooltip("Fast travel destination")]
        public bool hasFastTravel;
    }

    /// <summary>
    /// Data for a tactical cover point
    /// </summary>
    [Serializable]
    public class CoverPointData
    {
        [Tooltip("Cover type")]
        public CoverType coverType;

        [Tooltip("World position")]
        public Vector3 position;

        [Tooltip("Cover facing direction")]
        public Vector3 forward;

        [Tooltip("Width of cover")]
        public float width = 1f;

        [Tooltip("Height of cover")]
        public float height = 1f;

        [Tooltip("Can be destroyed")]
        public bool isDestructible;

        [Tooltip("Health if destructible")]
        public float health = 100f;
    }

    /// <summary>
    /// Data for an environmental hazard area
    /// </summary>
    [Serializable]
    public class HazardAreaData
    {
        [Tooltip("Unique identifier")]
        public string hazardId;

        [Tooltip("Type of hazard")]
        public HazardType hazardType;

        [Tooltip("Center position")]
        public Vector3 position;

        [Tooltip("Radius of effect")]
        public float radius = 5f;

        [Tooltip("Height of effect")]
        public float height = 3f;

        [Tooltip("Damage per second")]
        public float damagePerSecond = 10f;

        [Tooltip("Is hazard always active?")]
        public bool alwaysActive = true;

        [Tooltip("Activation trigger (if not always active)")]
        public string triggerCondition;

        [Tooltip("Can be disabled/cleared")]
        public bool canBeDisabled;

        [Tooltip("Warning time before damage starts")]
        public float warningTime = 1f;
    }

    /// <summary>
    /// Data for a random encounter zone
    /// </summary>
    [Serializable]
    public class EncounterZoneData
    {
        [Tooltip("Unique identifier")]
        public string encounterId;

        [Tooltip("Encounter type")]
        public EncounterType encounterType;

        [Tooltip("Base difficulty")]
        public EncounterDifficulty difficulty;

        [Tooltip("Center position")]
        public Vector3 position;

        [Tooltip("Trigger radius")]
        public float triggerRadius = 10f;

        [Tooltip("Spawn positions for enemies")]
        public Vector3[] spawnPositions;

        [Tooltip("Minimum enemies")]
        public int minEnemies = 3;

        [Tooltip("Maximum enemies")]
        public int maxEnemies = 8;

        [Tooltip("Chance to trigger when player enters (0-1)")]
        [Range(0f, 1f)]
        public float triggerChance = 0.5f;

        [Tooltip("Cooldown before encounter can trigger again")]
        public float cooldownTime = 300f;

        [Tooltip("Enemy types allowed")]
        public string[] allowedEnemyTypes;

        [Tooltip("Badass enemy chance")]
        [Range(0f, 1f)]
        public float badassChance = 0.1f;

        [Tooltip("Is this a one-time encounter?")]
        public bool oneTimeOnly;
    }

    /// <summary>
    /// Data for a point of interest
    /// </summary>
    [Serializable]
    public class POIData
    {
        [Tooltip("Unique identifier")]
        public string poiId;

        [Tooltip("Display name")]
        public string poiName;

        [Tooltip("Type of POI")]
        public POIType poiType;

        [Tooltip("World position")]
        public Vector3 position;

        [Tooltip("Interaction radius")]
        public float interactionRadius = 2f;

        [Tooltip("Shows on minimap")]
        public bool showOnMinimap = true;

        [Tooltip("Shows on world map")]
        public bool showOnWorldMap = true;

        [Tooltip("Icon override")]
        public Sprite iconOverride;

        [Tooltip("Associated quest ID (if any)")]
        public string associatedQuestId;
    }

    /// <summary>
    /// Data for a connection to another zone
    /// </summary>
    [Serializable]
    public class ZoneConnectionData
    {
        [Tooltip("Unique identifier")]
        public string connectionId;

        [Tooltip("Connection type")]
        public ConnectionType connectionType;

        [Tooltip("Target zone")]
        public ZoneDefinition targetZone;

        [Tooltip("Entry point ID in target zone")]
        public string targetSpawnPointId;

        [Tooltip("Position of connection trigger")]
        public Vector3 position;

        [Tooltip("Size of trigger area")]
        public Vector3 triggerSize = new Vector3(3f, 3f, 1f);

        [Tooltip("Requires confirmation to use")]
        public bool requiresConfirmation;

        [Tooltip("Only accessible with vehicle")]
        public bool vehicleOnly;

        [Tooltip("Minimum level to access")]
        public int minLevelRequired;

        [Tooltip("Quest required to unlock")]
        public string requiredQuestId;
    }

    /// <summary>
    /// Data for a sub-area within a zone
    /// </summary>
    [Serializable]
    public class SubAreaData
    {
        [Tooltip("Unique identifier")]
        public string subAreaId;

        [Tooltip("Display name")]
        public string subAreaName;

        [Tooltip("Type of sub-area")]
        public SubAreaType subAreaType;

        [Tooltip("Center position")]
        public Vector3 position;

        [Tooltip("Bounds of sub-area")]
        public Vector3 bounds = new Vector3(50f, 20f, 50f);

        [Tooltip("Level modifier (added to zone level)")]
        public int levelModifier;

        [Tooltip("Custom atmosphere override")]
        public ZoneAtmosphere atmosphereOverride;

        [Tooltip("Use atmosphere override")]
        public bool useAtmosphereOverride;

        [Tooltip("Custom features for this sub-area")]
        public ZoneFeatures subAreaFeatures;
    }
}
