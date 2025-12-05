using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NexusProtocol.Weapons;

namespace NexusProtocol.Loot
{
    /// <summary>
    /// A single entry in a loot table defining what can drop and its probability
    /// </summary>
    [Serializable]
    public class LootTableEntry
    {
        [Header("Item Definition")]
        [Tooltip("Type of item this entry represents")]
        public LootItemType itemType;

        [Tooltip("Specific item ID (for unique/legendary items)")]
        public string specificItemId;

        [Tooltip("Display name for this entry")]
        public string entryName;

        [Header("Drop Settings")]
        [Tooltip("Base weight for this drop (higher = more likely)")]
        [Range(0f, 1000f)]
        public float weight = 10f;

        [Tooltip("Base drop chance (0-1) before modifiers")]
        [Range(0f, 1f)]
        public float baseDropChance = 0.1f;

        [Tooltip("Guaranteed drop (ignores chance calculation)")]
        public bool guaranteed = false;

        [Header("Rarity Settings")]
        [Tooltip("Minimum rarity for this drop")]
        public WeaponRarity minRarity = WeaponRarity.Common;

        [Tooltip("Maximum rarity for this drop")]
        public WeaponRarity maxRarity = WeaponRarity.Legendary;

        [Tooltip("Custom rarity weights (overrides global)")]
        public bool useCustomRarityWeights = false;

        [Tooltip("Custom weights per rarity tier")]
        public RarityWeightOverride[] customRarityWeights;

        [Header("Manufacturer Bias")]
        [Tooltip("Enable manufacturer bias for weapon drops")]
        public bool useManufacturerBias = false;

        [Tooltip("Manufacturer weight overrides")]
        public ManufacturerWeightOverride[] manufacturerWeights;

        [Header("Level Scaling")]
        [Tooltip("Minimum player level for this drop")]
        [Range(1, 50)]
        public int minLevel = 1;

        [Tooltip("Maximum player level for this drop (0 = no max)")]
        [Range(0, 50)]
        public int maxLevel = 0;

        [Tooltip("Level scaling curve (how drop chance changes with level)")]
        public AnimationCurve levelScalingCurve = AnimationCurve.Linear(0, 1, 1, 1);

        [Header("Conditions")]
        [Tooltip("Conditions required for this drop")]
        public DropCondition conditions = DropCondition.None;

        [Tooltip("Quest ID required (if QuestActive or QuestComplete condition)")]
        public string requiredQuestId;

        [Header("Quantity")]
        [Tooltip("Minimum quantity to drop")]
        [Range(1, 100)]
        public int minQuantity = 1;

        [Tooltip("Maximum quantity to drop")]
        [Range(1, 100)]
        public int maxQuantity = 1;

        /// <summary>
        /// Calculate effective drop chance with all modifiers
        /// </summary>
        public float CalculateEffectiveChance(int playerLevel, float luckModifier, MayhemLevel mayhem)
        {
            if (guaranteed) return 1f;

            float chance = baseDropChance;

            // Level scaling
            if (minLevel > 0 || maxLevel > 0)
            {
                float levelProgress = maxLevel > minLevel
                    ? Mathf.InverseLerp(minLevel, maxLevel, playerLevel)
                    : 1f;
                chance *= levelScalingCurve.Evaluate(levelProgress);
            }

            // Luck modifier
            chance *= (1f + luckModifier);

            // Mayhem bonus (20% per level per GDD)
            if (mayhem > MayhemLevel.None)
            {
                chance *= 1f + ((int)mayhem * 0.20f);
            }

            return Mathf.Clamp01(chance);
        }

        /// <summary>
        /// Check if conditions are met for this drop
        /// </summary>
        public bool CheckConditions(DropCondition currentConditions)
        {
            if (conditions == DropCondition.None) return true;
            return (conditions & currentConditions) == conditions;
        }
    }

    /// <summary>
    /// Override for rarity weight
    /// </summary>
    [Serializable]
    public struct RarityWeightOverride
    {
        public WeaponRarity rarity;
        [Range(0f, 100f)]
        public float weight;
    }

    /// <summary>
    /// Override for manufacturer weight
    /// </summary>
    [Serializable]
    public struct ManufacturerWeightOverride
    {
        public WeaponManufacturer manufacturer;
        [Range(0f, 100f)]
        public float weight;
    }

    /// <summary>
    /// ScriptableObject defining a complete loot table for a source
    /// </summary>
    [CreateAssetMenu(fileName = "NewLootTable", menuName = "Nexus Protocol/Loot/Loot Table")]
    public class LootTable : ScriptableObject
    {
        [Header("Loot Table Identification")]
        [Tooltip("Unique identifier for this loot table")]
        public string tableId;

        [Tooltip("Display name for this loot table")]
        public string tableName;

        [Tooltip("Description of what uses this table")]
        [TextArea(2, 4)]
        public string description;

        [Header("Source Configuration")]
        [Tooltip("Type of source this table is for")]
        public LootSourceType sourceType;

        [Tooltip("Specific source ID (enemy ID, chest ID, etc.)")]
        public string specificSourceId;

        [Tooltip("Zones where this table applies")]
        public GameZone[] applicableZones;

        [Header("Global Rarity Weights")]
        [Tooltip("Base rarity distribution for this table")]
        public RarityDistribution rarityDistribution = new RarityDistribution();

        [Header("Global Manufacturer Bias")]
        [Tooltip("Enable global manufacturer bias")]
        public bool useGlobalManufacturerBias = false;

        [Tooltip("Manufacturer weight distribution")]
        public ManufacturerWeightOverride[] globalManufacturerWeights;

        [Header("Loot Entries")]
        [Tooltip("All possible drops from this source")]
        public List<LootTableEntry> entries = new List<LootTableEntry>();

        [Header("Drop Count")]
        [Tooltip("Minimum items to drop")]
        [Range(0, 20)]
        public int minDrops = 1;

        [Tooltip("Maximum items to drop")]
        [Range(0, 20)]
        public int maxDrops = 3;

        [Tooltip("Chance for bonus drop")]
        [Range(0f, 1f)]
        public float bonusDropChance = 0.1f;

        [Header("Level Requirements")]
        [Tooltip("Minimum player level for any drops")]
        [Range(1, 50)]
        public int minimumPlayerLevel = 1;

        [Tooltip("Level range for drops (+/- from player level)")]
        [Range(0, 10)]
        public int levelRange = 3;

        [Header("Mayhem Scaling")]
        [Tooltip("Minimum Mayhem level required")]
        public MayhemLevel minimumMayhemLevel = MayhemLevel.None;

        [Tooltip("Mayhem-exclusive entries (only drop at Mayhem 6+)")]
        public List<LootTableEntry> mayhemExclusiveEntries = new List<LootTableEntry>();

        [Header("Special Settings")]
        [Tooltip("Always drop at least one item")]
        public bool guaranteedDrop = true;

        [Tooltip("Can drop multiple of the same item")]
        public bool allowDuplicates = true;

        [Tooltip("Inherit from parent table")]
        public LootTable parentTable;

        /// <summary>
        /// Roll drops from this table
        /// </summary>
        public List<LootDrop> RollDrops(int playerLevel, float luckModifier, MayhemLevel mayhem, DropCondition conditions, System.Random random = null)
        {
            random ??= new System.Random();
            var drops = new List<LootDrop>();

            // Check minimum requirements
            if (playerLevel < minimumPlayerLevel) return drops;
            if (mayhem < minimumMayhemLevel) return drops;

            // Inherit from parent
            var allEntries = new List<LootTableEntry>(entries);
            if (parentTable != null)
            {
                allEntries.AddRange(parentTable.entries);
            }

            // Add mayhem exclusive if applicable
            if (mayhem >= MayhemLevel.Mayhem6)
            {
                allEntries.AddRange(mayhemExclusiveEntries);
            }

            // Calculate drop count
            int dropCount = random.Next(minDrops, maxDrops + 1);

            // Bonus drop check
            if (random.NextDouble() < bonusDropChance * (1f + luckModifier))
            {
                dropCount++;
            }

            // Mayhem bonus drops
            if (mayhem > MayhemLevel.None)
            {
                dropCount += (int)mayhem / 3; // +1 drop every 3 mayhem levels
            }

            // Roll for each drop
            HashSet<string> droppedItems = new HashSet<string>();

            for (int i = 0; i < dropCount; i++)
            {
                var drop = RollSingleDrop(allEntries, playerLevel, luckModifier, mayhem, conditions, random);

                if (drop != null)
                {
                    if (allowDuplicates || !droppedItems.Contains(drop.itemId))
                    {
                        drops.Add(drop);
                        droppedItems.Add(drop.itemId);
                    }
                }
            }

            // Guaranteed drop check
            if (guaranteedDrop && drops.Count == 0 && allEntries.Count > 0)
            {
                var guaranteedEntries = allEntries.Where(e => e.guaranteed).ToList();
                if (guaranteedEntries.Count > 0)
                {
                    var entry = guaranteedEntries[random.Next(guaranteedEntries.Count)];
                    drops.Add(CreateDropFromEntry(entry, playerLevel, random));
                }
                else
                {
                    // Pick random entry as fallback
                    var entry = allEntries[random.Next(allEntries.Count)];
                    drops.Add(CreateDropFromEntry(entry, playerLevel, random));
                }
            }

            return drops;
        }

        private LootDrop RollSingleDrop(List<LootTableEntry> entries, int playerLevel, float luckModifier,
            MayhemLevel mayhem, DropCondition conditions, System.Random random)
        {
            // Filter valid entries
            var validEntries = entries
                .Where(e => e.CheckConditions(conditions))
                .Where(e => playerLevel >= e.minLevel)
                .Where(e => e.maxLevel == 0 || playerLevel <= e.maxLevel)
                .ToList();

            if (validEntries.Count == 0) return null;

            // Calculate total weight
            float totalWeight = validEntries.Sum(e => e.weight);
            float roll = (float)random.NextDouble() * totalWeight;

            // Select entry by weight
            float currentWeight = 0f;
            LootTableEntry selectedEntry = null;

            foreach (var entry in validEntries)
            {
                currentWeight += entry.weight;
                if (roll <= currentWeight)
                {
                    selectedEntry = entry;
                    break;
                }
            }

            if (selectedEntry == null)
                selectedEntry = validEntries.Last();

            // Check drop chance
            float effectiveChance = selectedEntry.CalculateEffectiveChance(playerLevel, luckModifier, mayhem);
            if (random.NextDouble() > effectiveChance && !selectedEntry.guaranteed)
                return null;

            return CreateDropFromEntry(selectedEntry, playerLevel, random);
        }

        private LootDrop CreateDropFromEntry(LootTableEntry entry, int playerLevel, System.Random random)
        {
            // Roll rarity
            WeaponRarity rarity = RollRarity(entry, random);

            // Roll manufacturer if applicable
            WeaponManufacturer? manufacturer = null;
            if (IsWeaponType(entry.itemType))
            {
                manufacturer = RollManufacturer(entry, random);
            }

            // Roll quantity
            int quantity = random.Next(entry.minQuantity, entry.maxQuantity + 1);

            // Calculate item level
            int itemLevel = Mathf.Clamp(
                playerLevel + random.Next(-levelRange, levelRange + 1),
                1, 50);

            return new LootDrop
            {
                itemType = entry.itemType,
                itemId = entry.specificItemId ?? GenerateItemId(entry, rarity),
                displayName = entry.entryName,
                rarity = rarity,
                manufacturer = manufacturer,
                quantity = quantity,
                itemLevel = itemLevel
            };
        }

        private WeaponRarity RollRarity(LootTableEntry entry, System.Random random)
        {
            float roll = (float)random.NextDouble();
            float cumulative = 0f;

            // Use custom weights if defined
            if (entry.useCustomRarityWeights && entry.customRarityWeights != null && entry.customRarityWeights.Length > 0)
            {
                float totalWeight = entry.customRarityWeights.Sum(w => w.weight);
                roll *= totalWeight;

                foreach (var weight in entry.customRarityWeights)
                {
                    cumulative += weight.weight;
                    if (roll <= cumulative && weight.rarity >= entry.minRarity && weight.rarity <= entry.maxRarity)
                    {
                        return weight.rarity;
                    }
                }
            }

            // Use table's rarity distribution
            var distribution = rarityDistribution;

            if (roll < distribution.apocalypseWeight && entry.maxRarity >= WeaponRarity.Apocalypse)
                return WeaponRarity.Apocalypse;
            cumulative = distribution.apocalypseWeight;

            if (roll < cumulative + distribution.pearlescentWeight && entry.maxRarity >= WeaponRarity.Pearlescent)
                return WeaponRarity.Pearlescent;
            cumulative += distribution.pearlescentWeight;

            if (roll < cumulative + distribution.legendaryWeight && entry.maxRarity >= WeaponRarity.Legendary)
                return WeaponRarity.Legendary;
            cumulative += distribution.legendaryWeight;

            if (roll < cumulative + distribution.epicWeight && entry.maxRarity >= WeaponRarity.Epic)
                return WeaponRarity.Epic;
            cumulative += distribution.epicWeight;

            if (roll < cumulative + distribution.rareWeight && entry.maxRarity >= WeaponRarity.Rare)
                return WeaponRarity.Rare;
            cumulative += distribution.rareWeight;

            if (roll < cumulative + distribution.uncommonWeight && entry.maxRarity >= WeaponRarity.Uncommon)
                return WeaponRarity.Uncommon;

            return Mathf.Max(WeaponRarity.Common, entry.minRarity);
        }

        private WeaponManufacturer RollManufacturer(LootTableEntry entry, System.Random random)
        {
            ManufacturerWeightOverride[] weights = null;

            if (entry.useManufacturerBias && entry.manufacturerWeights != null && entry.manufacturerWeights.Length > 0)
            {
                weights = entry.manufacturerWeights;
            }
            else if (useGlobalManufacturerBias && globalManufacturerWeights != null && globalManufacturerWeights.Length > 0)
            {
                weights = globalManufacturerWeights;
            }

            if (weights == null || weights.Length == 0)
            {
                // Equal distribution
                return (WeaponManufacturer)random.Next(7);
            }

            float totalWeight = weights.Sum(w => w.weight);
            float roll = (float)random.NextDouble() * totalWeight;
            float cumulative = 0f;

            foreach (var weight in weights)
            {
                cumulative += weight.weight;
                if (roll <= cumulative)
                {
                    return weight.manufacturer;
                }
            }

            return weights.Last().manufacturer;
        }

        private bool IsWeaponType(LootItemType type)
        {
            return type >= LootItemType.Weapon_AssaultRifle && type <= LootItemType.Weapon_Special;
        }

        private string GenerateItemId(LootTableEntry entry, WeaponRarity rarity)
        {
            return $"{entry.itemType}_{rarity}_{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        /// <summary>
        /// Get statistics about this loot table
        /// </summary>
        public LootTableStatistics GetStatistics()
        {
            var stats = new LootTableStatistics
            {
                TotalEntries = entries.Count + mayhemExclusiveEntries.Count,
                EntriesByType = new Dictionary<LootItemType, int>(),
                EntriesByRarity = new Dictionary<WeaponRarity, int>(),
                TotalWeight = entries.Sum(e => e.weight),
                GuaranteedDrops = entries.Count(e => e.guaranteed),
                ConditionalDrops = entries.Count(e => e.conditions != DropCondition.None)
            };

            foreach (var entry in entries.Concat(mayhemExclusiveEntries))
            {
                // Count by type
                if (!stats.EntriesByType.ContainsKey(entry.itemType))
                    stats.EntriesByType[entry.itemType] = 0;
                stats.EntriesByType[entry.itemType]++;

                // Count by max rarity
                if (!stats.EntriesByRarity.ContainsKey(entry.maxRarity))
                    stats.EntriesByRarity[entry.maxRarity] = 0;
                stats.EntriesByRarity[entry.maxRarity]++;
            }

            return stats;
        }

        /// <summary>
        /// Validate this loot table
        /// </summary>
        public bool Validate(out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrEmpty(tableId))
                errors.Add("Table ID is required");

            if (string.IsNullOrEmpty(tableName))
                errors.Add("Table name is required");

            if (entries.Count == 0 && mayhemExclusiveEntries.Count == 0 && parentTable == null)
                errors.Add("Table has no entries and no parent table");

            if (minDrops > maxDrops)
                errors.Add("Min drops cannot be greater than max drops");

            foreach (var entry in entries.Concat(mayhemExclusiveEntries))
            {
                if (entry.minRarity > entry.maxRarity)
                    errors.Add($"Entry '{entry.entryName}': Min rarity > max rarity");

                if (entry.minQuantity > entry.maxQuantity)
                    errors.Add($"Entry '{entry.entryName}': Min quantity > max quantity");

                if (entry.minLevel > entry.maxLevel && entry.maxLevel > 0)
                    errors.Add($"Entry '{entry.entryName}': Min level > max level");
            }

            return errors.Count == 0;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(tableId) && !string.IsNullOrEmpty(tableName))
            {
                tableId = tableName.Replace(" ", "_").ToLower();
            }
        }
#endif
    }

    /// <summary>
    /// Rarity distribution configuration
    /// </summary>
    [Serializable]
    public class RarityDistribution
    {
        [Range(0f, 1f)] public float commonWeight = 0.60f;
        [Range(0f, 1f)] public float uncommonWeight = 0.25f;
        [Range(0f, 1f)] public float rareWeight = 0.10f;
        [Range(0f, 1f)] public float epicWeight = 0.04f;
        [Range(0f, 1f)] public float legendaryWeight = 0.009f;
        [Range(0f, 1f)] public float pearlescentWeight = 0.0009f;
        [Range(0f, 1f)] public float apocalypseWeight = 0.0001f;

        /// <summary>
        /// Normalize weights to sum to 1
        /// </summary>
        public void Normalize()
        {
            float total = commonWeight + uncommonWeight + rareWeight + epicWeight +
                         legendaryWeight + pearlescentWeight + apocalypseWeight;

            if (total > 0)
            {
                commonWeight /= total;
                uncommonWeight /= total;
                rareWeight /= total;
                epicWeight /= total;
                legendaryWeight /= total;
                pearlescentWeight /= total;
                apocalypseWeight /= total;
            }
        }
    }

    /// <summary>
    /// Result of a loot roll
    /// </summary>
    [Serializable]
    public class LootDrop
    {
        public LootItemType itemType;
        public string itemId;
        public string displayName;
        public WeaponRarity rarity;
        public WeaponManufacturer? manufacturer;
        public int quantity;
        public int itemLevel;
    }

    /// <summary>
    /// Statistics about a loot table
    /// </summary>
    public class LootTableStatistics
    {
        public int TotalEntries;
        public Dictionary<LootItemType, int> EntriesByType;
        public Dictionary<WeaponRarity, int> EntriesByRarity;
        public float TotalWeight;
        public int GuaranteedDrops;
        public int ConditionalDrops;
    }
}
