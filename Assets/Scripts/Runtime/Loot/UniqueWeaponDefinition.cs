using System;
using System.Collections.Generic;
using UnityEngine;
using NexusProtocol.Weapons;

namespace NexusProtocol.Loot
{
    /// <summary>
    /// Defines a unique/named weapon (Legendary, Pearlescent, or Apocalypse tier).
    /// These weapons have specific fixed parts that define their identity,
    /// while other parts are generated using the ECS weapon generation system.
    ///
    /// IMPORTANT: Common through Epic weapons do NOT use loot tables.
    /// They are generated purely at runtime via the DOTS/ECS system.
    /// </summary>
    [CreateAssetMenu(fileName = "NewUniqueWeapon", menuName = "Nexus Protocol/Loot/Unique Weapon Definition")]
    public class UniqueWeaponDefinition : ScriptableObject
    {
        [Header("Weapon Identity")]
        [Tooltip("Unique identifier for this weapon")]
        public string weaponId;

        [Tooltip("Display name of the unique weapon (e.g., 'The Infinity', 'Pandemic')")]
        public string weaponName;

        [Tooltip("Red flavor text describing the weapon's special effect")]
        [TextArea(2, 4)]
        public string flavorText;

        [Tooltip("Lore/backstory for the weapon")]
        [TextArea(3, 6)]
        public string loreText;

        [Header("Rarity")]
        [Tooltip("Rarity tier (must be Legendary, Pearlescent, or Apocalypse)")]
        public WeaponRarity rarity = WeaponRarity.Legendary;

        [Header("Fixed Parts - These define the weapon's identity")]
        [Tooltip("The unique receiver that defines this weapon (REQUIRED - determines manufacturer)")]
        public WeaponPartDefinition fixedReceiver;

        [Tooltip("Fixed barrel (leave empty to randomize)")]
        public WeaponPartDefinition fixedBarrel;

        [Tooltip("Fixed magazine (leave empty to randomize)")]
        public WeaponPartDefinition fixedMagazine;

        [Tooltip("Fixed grip (leave empty to randomize)")]
        public WeaponPartDefinition fixedGrip;

        [Tooltip("Fixed stock (leave empty to randomize)")]
        public WeaponPartDefinition fixedStock;

        [Tooltip("Fixed sight (leave empty to randomize)")]
        public WeaponPartDefinition fixedSight;

        [Header("Randomization Settings")]
        [Tooltip("Minimum rarity for randomized parts")]
        public WeaponRarity minRandomPartRarity = WeaponRarity.Rare;

        [Tooltip("Maximum rarity for randomized parts")]
        public WeaponRarity maxRandomPartRarity = WeaponRarity.Legendary;

        [Tooltip("Manufacturer bias for randomized parts (empty = no bias)")]
        public WeaponManufacturer[] preferredManufacturers;

        [Header("Unique Effect")]
        [Tooltip("Name of the unique effect/perk")]
        public string uniqueEffectName;

        [Tooltip("Description of the unique effect")]
        [TextArea(2, 4)]
        public string uniqueEffectDescription;

        [Tooltip("Visual effect prefab for the unique ability")]
        public GameObject uniqueEffectPrefab;

        [Header("Drop Configuration")]
        [Tooltip("Base drop weight (higher = more common among unique weapons)")]
        [Range(1f, 100f)]
        public float baseDropWeight = 10f;

        [Tooltip("Minimum player level to drop")]
        [Range(1, 50)]
        public int minimumLevel = 1;

        [Tooltip("Can drop from world sources (not just dedicated drops)")]
        public bool worldDropEnabled = false;

        [Tooltip("Dedicated drop sources (bosses, chests that always have a chance)")]
        public DedicatedDropSource[] dedicatedSources;

        [Header("Mayhem Requirements")]
        [Tooltip("Minimum Mayhem level required to drop")]
        public MayhemLevel minimumMayhemLevel = MayhemLevel.None;

        [Tooltip("Is this a Mayhem 6+ exclusive?")]
        public bool mayhemExclusive = false;

        [Header("Visual")]
        [Tooltip("Unique weapon icon")]
        public Sprite icon;

        [Tooltip("Unique skin/material override")]
        public Material uniqueMaterial;

        /// <summary>
        /// Get which parts are fixed vs randomized
        /// </summary>
        public PartConfiguration GetPartConfiguration()
        {
            return new PartConfiguration
            {
                receiverFixed = fixedReceiver != null,
                barrelFixed = fixedBarrel != null,
                magazineFixed = fixedMagazine != null,
                gripFixed = fixedGrip != null,
                stockFixed = fixedStock != null,
                sightFixed = fixedSight != null
            };
        }

        /// <summary>
        /// Get the number of fixed parts
        /// </summary>
        public int GetFixedPartCount()
        {
            int count = 0;
            if (fixedReceiver != null) count++;
            if (fixedBarrel != null) count++;
            if (fixedMagazine != null) count++;
            if (fixedGrip != null) count++;
            if (fixedStock != null) count++;
            if (fixedSight != null) count++;
            return count;
        }

        /// <summary>
        /// Validate the unique weapon definition
        /// </summary>
        public bool Validate(out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrEmpty(weaponId))
                errors.Add("Weapon ID is required");

            if (string.IsNullOrEmpty(weaponName))
                errors.Add("Weapon name is required");

            // Must be Legendary or higher
            if (rarity < WeaponRarity.Legendary)
                errors.Add($"Unique weapons must be Legendary or higher (current: {rarity})");

            // Must have at least a receiver
            if (fixedReceiver == null)
                errors.Add("Fixed receiver is required (defines manufacturer and weapon identity)");

            // Validate fixed parts match expected categories
            if (fixedReceiver != null && fixedReceiver.category != WeaponPartCategory.Receiver)
                errors.Add("Fixed receiver must be a Receiver part");
            if (fixedBarrel != null && fixedBarrel.category != WeaponPartCategory.Barrel)
                errors.Add("Fixed barrel must be a Barrel part");
            if (fixedMagazine != null && fixedMagazine.category != WeaponPartCategory.Magazine)
                errors.Add("Fixed magazine must be a Magazine part");
            if (fixedGrip != null && fixedGrip.category != WeaponPartCategory.Grip)
                errors.Add("Fixed grip must be a Grip part");
            if (fixedStock != null && fixedStock.category != WeaponPartCategory.Stock)
                errors.Add("Fixed stock must be a Stock part");
            if (fixedSight != null && fixedSight.category != WeaponPartCategory.Sight)
                errors.Add("Fixed sight must be a Sight part");

            // Unique effect should be defined for legendary+
            if (string.IsNullOrEmpty(uniqueEffectName))
                errors.Add("Unique effect name is recommended for legendary weapons");

            if (string.IsNullOrEmpty(flavorText))
                errors.Add("Flavor text is recommended for legendary weapons");

            return errors.Count == 0;
        }

        /// <summary>
        /// Get the manufacturer from the fixed receiver
        /// </summary>
        public WeaponManufacturer GetManufacturer()
        {
            return fixedReceiver != null ? fixedReceiver.manufacturer : WeaponManufacturer.KDC;
        }

        /// <summary>
        /// Get color based on rarity
        /// </summary>
        public Color GetRarityColor()
        {
            return rarity switch
            {
                WeaponRarity.Legendary => new Color(1.0f, 0.6f, 0.0f),   // Orange
                WeaponRarity.Pearlescent => new Color(0.0f, 0.9f, 0.9f), // Cyan
                WeaponRarity.Apocalypse => new Color(0.9f, 0.1f, 0.1f),  // Red
                _ => Color.white
            };
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Auto-generate ID if empty
            if (string.IsNullOrEmpty(weaponId) && !string.IsNullOrEmpty(weaponName))
            {
                weaponId = weaponName.Replace(" ", "_").Replace("'", "").ToLower();
            }

            // Enforce minimum rarity
            if (rarity < WeaponRarity.Legendary)
            {
                rarity = WeaponRarity.Legendary;
            }
        }
#endif
    }

    /// <summary>
    /// Configuration showing which parts are fixed vs randomized
    /// </summary>
    [Serializable]
    public struct PartConfiguration
    {
        public bool receiverFixed;
        public bool barrelFixed;
        public bool magazineFixed;
        public bool gripFixed;
        public bool stockFixed;
        public bool sightFixed;

        public int FixedCount =>
            (receiverFixed ? 1 : 0) +
            (barrelFixed ? 1 : 0) +
            (magazineFixed ? 1 : 0) +
            (gripFixed ? 1 : 0) +
            (stockFixed ? 1 : 0) +
            (sightFixed ? 1 : 0);

        public int RandomCount => 6 - FixedCount;
    }

    /// <summary>
    /// Defines a dedicated drop source for a unique weapon
    /// </summary>
    [Serializable]
    public class DedicatedDropSource
    {
        [Tooltip("Type of source")]
        public LootSourceType sourceType;

        [Tooltip("Specific source ID (e.g., boss name, chest ID)")]
        public string sourceId;

        [Tooltip("Drop chance from this source (0-1)")]
        [Range(0f, 1f)]
        public float dropChance = 0.05f;

        [Tooltip("Increased chance on Mayhem (per level)")]
        [Range(0f, 0.1f)]
        public float mayhemBonusPerLevel = 0.01f;
    }
}
