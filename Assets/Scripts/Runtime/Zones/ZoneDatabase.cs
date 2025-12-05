using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NexusProtocol.Zones
{
    /// <summary>
    /// Database managing all zones in the game
    /// </summary>
    [CreateAssetMenu(fileName = "ZoneDatabase", menuName = "Nexus Protocol/Zones/Zone Database")]
    public class ZoneDatabase : ScriptableObject
    {
        [Header("All Zones")]
        [Tooltip("All zone definitions in the database")]
        public List<ZoneDefinition> allZones = new List<ZoneDefinition>();

        [Header("Default Configuration")]
        [Tooltip("Starting zone for new players")]
        public ZoneDefinition startingZone;

        [Tooltip("Hub zone (Haven City)")]
        public ZoneDefinition hubZone;

        [Header("Global Settings")]
        [Tooltip("Global level scaling factor")]
        [Range(0.5f, 2f)]
        public float globalLevelScale = 1f;

        [Tooltip("Encounter density multiplier")]
        [Range(0.5f, 2f)]
        public float encounterDensity = 1f;

        [Tooltip("Hazard damage multiplier")]
        [Range(0.5f, 2f)]
        public float hazardDamageScale = 1f;

        // Cached lookups
        private Dictionary<string, ZoneDefinition> _zonesById;
        private Dictionary<ZoneType, ZoneDefinition> _zonesByType;
        private Dictionary<int, List<ZoneDefinition>> _zonesByLevel;
        private bool _cacheInitialized;

        /// <summary>
        /// Initialize lookup caches
        /// </summary>
        public void InitializeCache()
        {
            _zonesById = new Dictionary<string, ZoneDefinition>();
            _zonesByType = new Dictionary<ZoneType, ZoneDefinition>();
            _zonesByLevel = new Dictionary<int, List<ZoneDefinition>>();

            // Initialize level buckets
            for (int i = 1; i <= 50; i++)
            {
                _zonesByLevel[i] = new List<ZoneDefinition>();
            }

            foreach (var zone in allZones)
            {
                if (zone == null) continue;

                // Cache by ID
                if (!string.IsNullOrEmpty(zone.zoneId))
                {
                    _zonesById[zone.zoneId] = zone;
                }

                // Cache by type
                _zonesByType[zone.zoneType] = zone;

                // Cache by level range
                for (int level = zone.minLevel; level <= zone.maxLevel; level++)
                {
                    if (level >= 1 && level <= 50)
                    {
                        _zonesByLevel[level].Add(zone);
                    }
                }
            }

            _cacheInitialized = true;
        }

        private void EnsureCacheInitialized()
        {
            if (!_cacheInitialized)
            {
                InitializeCache();
            }
        }

        /// <summary>
        /// Get a zone by ID
        /// </summary>
        public ZoneDefinition GetZoneById(string zoneId)
        {
            EnsureCacheInitialized();
            return _zonesById.TryGetValue(zoneId, out var zone) ? zone : null;
        }

        /// <summary>
        /// Get a zone by type
        /// </summary>
        public ZoneDefinition GetZoneByType(ZoneType zoneType)
        {
            EnsureCacheInitialized();
            return _zonesByType.TryGetValue(zoneType, out var zone) ? zone : null;
        }

        /// <summary>
        /// Get all zones appropriate for a player level
        /// </summary>
        public List<ZoneDefinition> GetZonesForLevel(int playerLevel)
        {
            EnsureCacheInitialized();
            playerLevel = Mathf.Clamp(playerLevel, 1, 50);
            return _zonesByLevel.TryGetValue(playerLevel, out var zones) ? zones : new List<ZoneDefinition>();
        }

        /// <summary>
        /// Get all story zones in order
        /// </summary>
        public List<ZoneDefinition> GetStoryZonesInOrder()
        {
            EnsureCacheInitialized();
            return allZones
                .Where(z => z != null && (int)z.zoneType >= 0 && (int)z.zoneType <= 5)
                .OrderBy(z => z.minLevel)
                .ToList();
        }

        /// <summary>
        /// Get all endgame zones
        /// </summary>
        public List<ZoneDefinition> GetEndgameZones()
        {
            EnsureCacheInitialized();
            return allZones
                .Where(z => z != null && z.features.HasFlag(ZoneFeatures.IsEndgame))
                .ToList();
        }

        /// <summary>
        /// Get all zones with Motor Pool stations
        /// </summary>
        public List<ZoneDefinition> GetZonesWithMotorPools()
        {
            return allZones
                .Where(z => z != null && z.features.HasFlag(ZoneFeatures.HasMotorPool))
                .ToList();
        }

        /// <summary>
        /// Get all zones with fast travel points
        /// </summary>
        public List<ZoneDefinition> GetZonesWithFastTravel()
        {
            return allZones
                .Where(z => z != null && z.features.HasFlag(ZoneFeatures.HasFastTravel))
                .ToList();
        }

        /// <summary>
        /// Get recommended zone for player level
        /// </summary>
        public ZoneDefinition GetRecommendedZone(int playerLevel)
        {
            EnsureCacheInitialized();
            var zones = GetZonesForLevel(playerLevel);

            // Prefer zones where player is near recommended level
            return zones
                .Where(z => !z.features.HasFlag(ZoneFeatures.IsEndgame))
                .OrderBy(z => Mathf.Abs(z.recommendedLevel - playerLevel))
                .FirstOrDefault();
        }

        /// <summary>
        /// Add a zone to the database
        /// </summary>
        public void AddZone(ZoneDefinition zone)
        {
            if (zone == null || allZones.Contains(zone)) return;
            allZones.Add(zone);
            _cacheInitialized = false;
        }

        /// <summary>
        /// Remove a zone from the database
        /// </summary>
        public void RemoveZone(ZoneDefinition zone)
        {
            if (zone == null) return;
            allZones.Remove(zone);
            _cacheInitialized = false;
        }

        /// <summary>
        /// Clear the cache
        /// </summary>
        public void ClearCache()
        {
            _cacheInitialized = false;
        }

        /// <summary>
        /// Get database statistics
        /// </summary>
        public ZoneDatabaseStats GetStatistics()
        {
            EnsureCacheInitialized();

            var stats = new ZoneDatabaseStats
            {
                TotalZones = allZones.Count(z => z != null),
                StoryZones = allZones.Count(z => z != null && (int)z.zoneType >= 0 && (int)z.zoneType <= 5),
                EndgameZones = allZones.Count(z => z != null && z.features.HasFlag(ZoneFeatures.IsEndgame)),
                TotalSpawnPoints = 0,
                TotalMotorPools = 0,
                TotalCoverPoints = 0,
                TotalHazardAreas = 0,
                TotalEncounterZones = 0,
                TotalPOIs = 0,
                TotalConnections = 0
            };

            foreach (var zone in allZones)
            {
                if (zone == null) continue;

                stats.TotalSpawnPoints += zone.spawnPoints?.Length ?? 0;
                stats.TotalMotorPools += zone.motorPools?.Length ?? 0;
                stats.TotalCoverPoints += zone.coverPoints?.Length ?? 0;
                stats.TotalHazardAreas += zone.hazardAreas?.Length ?? 0;
                stats.TotalEncounterZones += zone.encounterZones?.Length ?? 0;
                stats.TotalPOIs += zone.pointsOfInterest?.Length ?? 0;
                stats.TotalConnections += zone.connections?.Length ?? 0;
            }

            return stats;
        }

        /// <summary>
        /// Validate all zones in the database
        /// </summary>
        public List<(ZoneDefinition zone, List<string> errors)> ValidateAll()
        {
            var results = new List<(ZoneDefinition, List<string>)>();

            foreach (var zone in allZones)
            {
                if (zone == null)
                {
                    results.Add((null, new List<string> { "Null zone reference" }));
                    continue;
                }

                if (!zone.Validate(out var errors))
                {
                    results.Add((zone, errors));
                }
            }

            // Check for duplicate IDs
            var duplicateIds = allZones
                .Where(z => z != null && !string.IsNullOrEmpty(z.zoneId))
                .GroupBy(z => z.zoneId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var dupId in duplicateIds)
            {
                var zone = allZones.First(z => z != null && z.zoneId == dupId);
                results.Add((zone, new List<string> { $"Duplicate zone ID: {dupId}" }));
            }

            // Check for missing starting zone
            if (startingZone == null)
            {
                results.Add((null, new List<string> { "No starting zone configured" }));
            }

            // Check for missing hub zone
            if (hubZone == null)
            {
                results.Add((null, new List<string> { "No hub zone (Haven City) configured" }));
            }

            // Check zone connections
            foreach (var zone in allZones)
            {
                if (zone?.connections == null) continue;

                foreach (var connection in zone.connections)
                {
                    if (connection.targetZone == null)
                    {
                        results.Add((zone, new List<string> { $"Connection '{connection.connectionId}' has no target zone" }));
                    }
                    else if (!allZones.Contains(connection.targetZone))
                    {
                        results.Add((zone, new List<string> { $"Connection '{connection.connectionId}' targets zone not in database" }));
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Get all spawn points across all zones
        /// </summary>
        public List<(ZoneDefinition zone, SpawnPointData point)> GetAllSpawnPoints()
        {
            var result = new List<(ZoneDefinition, SpawnPointData)>();

            foreach (var zone in allZones)
            {
                if (zone?.spawnPoints == null) continue;

                foreach (var point in zone.spawnPoints)
                {
                    result.Add((zone, point));
                }
            }

            return result;
        }

        /// <summary>
        /// Get all Motor Pool stations across all zones
        /// </summary>
        public List<(ZoneDefinition zone, MotorPoolData motorPool)> GetAllMotorPools()
        {
            var result = new List<(ZoneDefinition, MotorPoolData)>();

            foreach (var zone in allZones)
            {
                if (zone?.motorPools == null) continue;

                foreach (var pool in zone.motorPools)
                {
                    result.Add((zone, pool));
                }
            }

            return result;
        }

        /// <summary>
        /// Find zones that connect to the specified zone
        /// </summary>
        public List<ZoneDefinition> GetConnectedZones(ZoneDefinition zone)
        {
            var connected = new List<ZoneDefinition>();

            // Get zones this zone connects to
            if (zone?.connections != null)
            {
                foreach (var connection in zone.connections)
                {
                    if (connection.targetZone != null && !connected.Contains(connection.targetZone))
                    {
                        connected.Add(connection.targetZone);
                    }
                }
            }

            // Get zones that connect to this zone
            foreach (var otherZone in allZones)
            {
                if (otherZone == null || otherZone == zone || otherZone.connections == null) continue;

                if (otherZone.connections.Any(c => c.targetZone == zone) && !connected.Contains(otherZone))
                {
                    connected.Add(otherZone);
                }
            }

            return connected;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _cacheInitialized = false;
        }
#endif
    }

    /// <summary>
    /// Statistics about the zone database
    /// </summary>
    public class ZoneDatabaseStats
    {
        public int TotalZones;
        public int StoryZones;
        public int EndgameZones;
        public int TotalSpawnPoints;
        public int TotalMotorPools;
        public int TotalCoverPoints;
        public int TotalHazardAreas;
        public int TotalEncounterZones;
        public int TotalPOIs;
        public int TotalConnections;
    }
}
