using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NexusProtocol.Weapons
{
    /// <summary>
    /// Database containing all weapon part definitions
    /// Used by the weapon generation system to roll parts
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponPartDatabase", menuName = "Nexus Protocol/Weapons/Weapon Part Database")]
    public class WeaponPartDatabase : ScriptableObject
    {
        [Header("Part Collections")]
        [Tooltip("All weapon parts in the database")]
        public List<WeaponPartDefinition> allParts = new List<WeaponPartDefinition>();

        // Cached lookups for performance
        private Dictionary<WeaponPartCategory, List<WeaponPartDefinition>> _partsByCategory;
        private Dictionary<WeaponRarity, List<WeaponPartDefinition>> _partsByRarity;
        private Dictionary<WeaponManufacturer, List<WeaponPartDefinition>> _partsByManufacturer;
        private Dictionary<string, WeaponPartDefinition> _partsById;
        private bool _cacheInitialized = false;

        /// <summary>
        /// Initialize the lookup caches
        /// </summary>
        public void InitializeCache()
        {
            _partsByCategory = new Dictionary<WeaponPartCategory, List<WeaponPartDefinition>>();
            _partsByRarity = new Dictionary<WeaponRarity, List<WeaponPartDefinition>>();
            _partsByManufacturer = new Dictionary<WeaponManufacturer, List<WeaponPartDefinition>>();
            _partsById = new Dictionary<string, WeaponPartDefinition>();

            foreach (WeaponPartCategory category in Enum.GetValues(typeof(WeaponPartCategory)))
            {
                _partsByCategory[category] = new List<WeaponPartDefinition>();
            }

            foreach (WeaponRarity rarity in Enum.GetValues(typeof(WeaponRarity)))
            {
                _partsByRarity[rarity] = new List<WeaponPartDefinition>();
            }

            foreach (WeaponManufacturer manufacturer in Enum.GetValues(typeof(WeaponManufacturer)))
            {
                _partsByManufacturer[manufacturer] = new List<WeaponPartDefinition>();
            }

            foreach (var part in allParts)
            {
                if (part == null) continue;

                _partsByCategory[part.category].Add(part);
                _partsByRarity[part.rarity].Add(part);

                if (part.category == WeaponPartCategory.Receiver)
                {
                    _partsByManufacturer[part.manufacturer].Add(part);
                }

                if (!string.IsNullOrEmpty(part.partID))
                {
                    _partsById[part.partID] = part;
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
        /// Get all parts in a specific category
        /// </summary>
        public List<WeaponPartDefinition> GetPartsByCategory(WeaponPartCategory category)
        {
            EnsureCacheInitialized();
            return _partsByCategory.TryGetValue(category, out var parts) ? parts : new List<WeaponPartDefinition>();
        }

        /// <summary>
        /// Get all parts of a specific rarity
        /// </summary>
        public List<WeaponPartDefinition> GetPartsByRarity(WeaponRarity rarity)
        {
            EnsureCacheInitialized();
            return _partsByRarity.TryGetValue(rarity, out var parts) ? parts : new List<WeaponPartDefinition>();
        }

        /// <summary>
        /// Get all receivers for a specific manufacturer
        /// </summary>
        public List<WeaponPartDefinition> GetReceiverByManufacturer(WeaponManufacturer manufacturer)
        {
            EnsureCacheInitialized();
            return _partsByManufacturer.TryGetValue(manufacturer, out var parts) ? parts : new List<WeaponPartDefinition>();
        }

        /// <summary>
        /// Get a part by its ID
        /// </summary>
        public WeaponPartDefinition GetPartById(string partId)
        {
            EnsureCacheInitialized();
            return _partsById.TryGetValue(partId, out var part) ? part : null;
        }

        /// <summary>
        /// Get parts filtered by category and rarity
        /// </summary>
        public List<WeaponPartDefinition> GetParts(WeaponPartCategory category, WeaponRarity rarity)
        {
            EnsureCacheInitialized();
            return _partsByCategory[category]
                .Where(p => p.rarity == rarity)
                .ToList();
        }

        /// <summary>
        /// Get parts filtered by category, rarity, and type
        /// </summary>
        public List<WeaponPartDefinition> GetParts(WeaponPartCategory category, WeaponRarity rarity, WeaponPartType partType)
        {
            EnsureCacheInitialized();
            return _partsByCategory[category]
                .Where(p => p.rarity == rarity && p.partType == partType)
                .ToList();
        }

        /// <summary>
        /// Get a weighted random part from a category
        /// </summary>
        public WeaponPartDefinition GetRandomPart(WeaponPartCategory category, System.Random random = null)
        {
            EnsureCacheInitialized();
            random ??= new System.Random();

            var parts = _partsByCategory[category].Where(p => p.worldDropEnabled).ToList();
            if (parts.Count == 0) return null;

            float totalWeight = parts.Sum(p => p.dropWeight);
            float roll = (float)random.NextDouble() * totalWeight;

            float current = 0f;
            foreach (var part in parts)
            {
                current += part.dropWeight;
                if (roll <= current)
                {
                    return part;
                }
            }

            return parts.Last();
        }

        /// <summary>
        /// Get a random part based on rarity drop rates
        /// </summary>
        public WeaponPartDefinition GetRandomPartByRarity(WeaponPartCategory category, System.Random random = null)
        {
            EnsureCacheInitialized();
            random ??= new System.Random();

            // Roll for rarity first based on GDD drop rates
            float rarityRoll = (float)random.NextDouble();
            WeaponRarity targetRarity;

            if (rarityRoll < 0.0001f)
                targetRarity = WeaponRarity.Apocalypse;
            else if (rarityRoll < 0.001f)
                targetRarity = WeaponRarity.Pearlescent;
            else if (rarityRoll < 0.01f)
                targetRarity = WeaponRarity.Legendary;
            else if (rarityRoll < 0.05f)
                targetRarity = WeaponRarity.Epic;
            else if (rarityRoll < 0.15f)
                targetRarity = WeaponRarity.Rare;
            else if (rarityRoll < 0.40f)
                targetRarity = WeaponRarity.Uncommon;
            else
                targetRarity = WeaponRarity.Common;

            var parts = GetParts(category, targetRarity).Where(p => p.worldDropEnabled).ToList();

            // Fall back to lower rarity if none available
            while (parts.Count == 0 && targetRarity > WeaponRarity.Common)
            {
                targetRarity--;
                parts = GetParts(category, targetRarity).Where(p => p.worldDropEnabled).ToList();
            }

            if (parts.Count == 0) return null;

            return parts[random.Next(parts.Count)];
        }

        /// <summary>
        /// Get statistics about the database
        /// </summary>
        public DatabaseStatistics GetStatistics()
        {
            EnsureCacheInitialized();

            var stats = new DatabaseStatistics
            {
                TotalParts = allParts.Count,
                PartsByCategory = new Dictionary<WeaponPartCategory, int>(),
                PartsByRarity = new Dictionary<WeaponRarity, int>(),
                PartsByManufacturer = new Dictionary<WeaponManufacturer, int>()
            };

            foreach (var kvp in _partsByCategory)
            {
                stats.PartsByCategory[kvp.Key] = kvp.Value.Count;
            }

            foreach (var kvp in _partsByRarity)
            {
                stats.PartsByRarity[kvp.Key] = kvp.Value.Count;
            }

            foreach (var kvp in _partsByManufacturer)
            {
                stats.PartsByManufacturer[kvp.Key] = kvp.Value.Count;
            }

            return stats;
        }

        /// <summary>
        /// Validate all parts in the database
        /// </summary>
        public List<(WeaponPartDefinition part, List<string> errors)> ValidateAll()
        {
            var results = new List<(WeaponPartDefinition, List<string>)>();

            foreach (var part in allParts)
            {
                if (part == null)
                {
                    results.Add((null, new List<string> { "Null part reference in database" }));
                    continue;
                }

                if (!part.Validate(out var errors))
                {
                    results.Add((part, errors));
                }
            }

            return results;
        }

        /// <summary>
        /// Add a part to the database
        /// </summary>
        public void AddPart(WeaponPartDefinition part)
        {
            if (part == null || allParts.Contains(part)) return;

            allParts.Add(part);
            _cacheInitialized = false; // Force cache rebuild
        }

        /// <summary>
        /// Remove a part from the database
        /// </summary>
        public void RemovePart(WeaponPartDefinition part)
        {
            if (part == null) return;

            allParts.Remove(part);
            _cacheInitialized = false; // Force cache rebuild
        }

        /// <summary>
        /// Clear the cache (call when data changes)
        /// </summary>
        public void ClearCache()
        {
            _cacheInitialized = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _cacheInitialized = false;
        }
#endif
    }

    /// <summary>
    /// Statistics about the weapon part database
    /// </summary>
    [Serializable]
    public class DatabaseStatistics
    {
        public int TotalParts;
        public Dictionary<WeaponPartCategory, int> PartsByCategory;
        public Dictionary<WeaponRarity, int> PartsByRarity;
        public Dictionary<WeaponManufacturer, int> PartsByManufacturer;
    }
}
