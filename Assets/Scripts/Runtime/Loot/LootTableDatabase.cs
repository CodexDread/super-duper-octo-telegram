using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NexusProtocol.Weapons;

namespace NexusProtocol.Loot
{
    /// <summary>
    /// Database managing all loot tables in the game
    /// </summary>
    [CreateAssetMenu(fileName = "LootTableDatabase", menuName = "Nexus Protocol/Loot/Loot Table Database")]
    public class LootTableDatabase : ScriptableObject
    {
        [Header("All Loot Tables")]
        [Tooltip("All loot tables in the database")]
        public List<LootTable> allTables = new List<LootTable>();

        [Header("Global Settings")]
        [Tooltip("Global luck modifier applied to all drops")]
        [Range(-0.5f, 2f)]
        public float globalLuckModifier = 0f;

        [Tooltip("Global rarity boost (multiplier)")]
        [Range(0.5f, 3f)]
        public float globalRarityBoost = 1f;

        [Header("Chest Tier Configuration")]
        [Tooltip("Loot tables for each chest tier")]
        public ChestTierConfig[] chestTierConfigs;

        [Header("World Drop Tables")]
        [Tooltip("Default world drop table")]
        public LootTable defaultWorldDropTable;

        [Tooltip("Zone-specific world drop tables")]
        public ZoneDropConfig[] zoneDropConfigs;

        // Cached lookups
        private Dictionary<string, LootTable> _tablesByID;
        private Dictionary<LootSourceType, List<LootTable>> _tablesBySource;
        private Dictionary<GameZone, List<LootTable>> _tablesByZone;
        private bool _cacheInitialized = false;

        /// <summary>
        /// Initialize lookup caches
        /// </summary>
        public void InitializeCache()
        {
            _tablesByID = new Dictionary<string, LootTable>();
            _tablesBySource = new Dictionary<LootSourceType, List<LootTable>>();
            _tablesByZone = new Dictionary<GameZone, List<LootTable>>();

            foreach (LootSourceType source in Enum.GetValues(typeof(LootSourceType)))
            {
                _tablesBySource[source] = new List<LootTable>();
            }

            foreach (GameZone zone in Enum.GetValues(typeof(GameZone)))
            {
                _tablesByZone[zone] = new List<LootTable>();
            }

            foreach (var table in allTables)
            {
                if (table == null) continue;

                if (!string.IsNullOrEmpty(table.tableId))
                {
                    _tablesByID[table.tableId] = table;
                }

                _tablesBySource[table.sourceType].Add(table);

                if (table.applicableZones != null)
                {
                    foreach (var zone in table.applicableZones)
                    {
                        _tablesByZone[zone].Add(table);
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
        /// Get a loot table by ID
        /// </summary>
        public LootTable GetTableById(string tableId)
        {
            EnsureCacheInitialized();
            return _tablesByID.TryGetValue(tableId, out var table) ? table : null;
        }

        /// <summary>
        /// Get all tables for a source type
        /// </summary>
        public List<LootTable> GetTablesBySource(LootSourceType source)
        {
            EnsureCacheInitialized();
            return _tablesBySource.TryGetValue(source, out var tables) ? tables : new List<LootTable>();
        }

        /// <summary>
        /// Get all tables for a zone
        /// </summary>
        public List<LootTable> GetTablesByZone(GameZone zone)
        {
            EnsureCacheInitialized();
            return _tablesByZone.TryGetValue(zone, out var tables) ? tables : new List<LootTable>();
        }

        /// <summary>
        /// Get the appropriate table for a chest tier
        /// </summary>
        public LootTable GetChestTable(ChestTier tier)
        {
            if (chestTierConfigs == null) return null;

            var config = chestTierConfigs.FirstOrDefault(c => c.tier == tier);
            return config.lootTable;
        }

        /// <summary>
        /// Get the world drop table for a zone
        /// </summary>
        public LootTable GetWorldDropTable(GameZone zone)
        {
            if (zoneDropConfigs != null)
            {
                var config = zoneDropConfigs.FirstOrDefault(c => c.zone == zone);
                if (config.worldDropTable != null)
                {
                    return config.worldDropTable;
                }
            }

            return defaultWorldDropTable;
        }

        /// <summary>
        /// Roll drops from a specific source
        /// </summary>
        public List<LootDrop> RollDrops(LootSourceType source, string specificId, int playerLevel,
            float playerLuck, MayhemLevel mayhem, DropCondition conditions, GameZone zone, System.Random random = null)
        {
            EnsureCacheInitialized();
            random ??= new System.Random();

            var drops = new List<LootDrop>();
            float effectiveLuck = playerLuck + globalLuckModifier;

            // Find matching tables
            var tables = _tablesBySource[source]
                .Where(t => string.IsNullOrEmpty(specificId) || t.specificSourceId == specificId)
                .Where(t => t.applicableZones == null || t.applicableZones.Length == 0 || t.applicableZones.Contains(zone))
                .ToList();

            // Roll from each matching table
            foreach (var table in tables)
            {
                var tableDrops = table.RollDrops(playerLevel, effectiveLuck, mayhem, conditions, random);
                drops.AddRange(tableDrops);
            }

            return drops;
        }

        /// <summary>
        /// Roll drops from a chest
        /// </summary>
        public List<LootDrop> RollChestDrops(ChestTier tier, int playerLevel, float playerLuck,
            MayhemLevel mayhem, GameZone zone, System.Random random = null)
        {
            var table = GetChestTable(tier);
            if (table == null) return new List<LootDrop>();

            float effectiveLuck = playerLuck + globalLuckModifier;
            return table.RollDrops(playerLevel, effectiveLuck, mayhem, DropCondition.None, random);
        }

        /// <summary>
        /// Roll world drops
        /// </summary>
        public List<LootDrop> RollWorldDrops(GameZone zone, int playerLevel, float playerLuck,
            MayhemLevel mayhem, System.Random random = null)
        {
            var table = GetWorldDropTable(zone);
            if (table == null) return new List<LootDrop>();

            float effectiveLuck = playerLuck + globalLuckModifier;
            return table.RollDrops(playerLevel, effectiveLuck, mayhem, DropCondition.None, random);
        }

        /// <summary>
        /// Add a table to the database
        /// </summary>
        public void AddTable(LootTable table)
        {
            if (table == null || allTables.Contains(table)) return;
            allTables.Add(table);
            _cacheInitialized = false;
        }

        /// <summary>
        /// Remove a table from the database
        /// </summary>
        public void RemoveTable(LootTable table)
        {
            if (table == null) return;
            allTables.Remove(table);
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
        public DatabaseStats GetStatistics()
        {
            EnsureCacheInitialized();

            var stats = new DatabaseStats
            {
                TotalTables = allTables.Count,
                TablesBySource = new Dictionary<LootSourceType, int>(),
                TablesByZone = new Dictionary<GameZone, int>(),
                TotalEntries = 0,
                TotalWeight = 0
            };

            foreach (var kvp in _tablesBySource)
            {
                stats.TablesBySource[kvp.Key] = kvp.Value.Count;
            }

            foreach (var kvp in _tablesByZone)
            {
                stats.TablesByZone[kvp.Key] = kvp.Value.Count;
            }

            foreach (var table in allTables)
            {
                if (table == null) continue;
                stats.TotalEntries += table.entries.Count + table.mayhemExclusiveEntries.Count;
                stats.TotalWeight += table.entries.Sum(e => e.weight);
            }

            return stats;
        }

        /// <summary>
        /// Validate all tables in the database
        /// </summary>
        public List<(LootTable table, List<string> errors)> ValidateAll()
        {
            var results = new List<(LootTable, List<string>)>();

            foreach (var table in allTables)
            {
                if (table == null)
                {
                    results.Add((null, new List<string> { "Null table reference" }));
                    continue;
                }

                if (!table.Validate(out var errors))
                {
                    results.Add((table, errors));
                }
            }

            return results;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _cacheInitialized = false;
        }
#endif
    }

    /// <summary>
    /// Configuration for a chest tier
    /// </summary>
    [Serializable]
    public struct ChestTierConfig
    {
        public ChestTier tier;
        public LootTable lootTable;
        [Range(1, 10)]
        public int minItems;
        [Range(1, 20)]
        public int maxItems;
        public WeaponRarity maxRarity;
    }

    /// <summary>
    /// Configuration for zone-specific drops
    /// </summary>
    [Serializable]
    public struct ZoneDropConfig
    {
        public GameZone zone;
        public LootTable worldDropTable;
        public LootTable bossDropTable;
        [Range(1, 50)]
        public int minLevel;
        [Range(1, 50)]
        public int maxLevel;
    }

    /// <summary>
    /// Statistics about the loot database
    /// </summary>
    public class DatabaseStats
    {
        public int TotalTables;
        public Dictionary<LootSourceType, int> TablesBySource;
        public Dictionary<GameZone, int> TablesByZone;
        public int TotalEntries;
        public float TotalWeight;
    }
}
