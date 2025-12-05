using System;
using System.Collections.Generic;
using UnityEngine;

namespace NexusProtocol.Weapons
{
    /// <summary>
    /// Defines a stat modifier applied by a weapon part
    /// </summary>
    [Serializable]
    public struct StatModifier
    {
        [Tooltip("The stat type being modified")]
        public WeaponStatType statType;

        [Tooltip("Flat value added to the stat")]
        public float flatBonus;

        [Tooltip("Percentage multiplier (1.0 = 100%, 0.1 = 10% bonus)")]
        public float percentBonus;

        public StatModifier(WeaponStatType type, float flat = 0f, float percent = 0f)
        {
            statType = type;
            flatBonus = flat;
            percentBonus = percent;
        }
    }

    /// <summary>
    /// Defines a special effect that can be applied by a weapon part
    /// </summary>
    [Serializable]
    public class PartSpecialEffect
    {
        [Tooltip("Name of the effect")]
        public string effectName;

        [Tooltip("Description of what the effect does")]
        [TextArea(2, 4)]
        public string description;

        [Tooltip("Effect trigger chance (0-1)")]
        [Range(0f, 1f)]
        public float triggerChance = 1f;

        [Tooltip("Effect magnitude/strength")]
        public float magnitude = 1f;

        [Tooltip("Effect duration in seconds (0 for instant)")]
        public float duration = 0f;

        [Tooltip("Optional elemental type for the effect")]
        public ElementType elementType = ElementType.None;
    }

    /// <summary>
    /// ScriptableObject defining a single weapon part
    /// Used by the Weapon Part Manager tool to create and configure parts
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponPart", menuName = "Nexus Protocol/Weapons/Weapon Part Definition")]
    public class WeaponPartDefinition : ScriptableObject
    {
        [Header("Part Identification")]
        [Tooltip("Unique identifier for this part")]
        public string partID;

        [Tooltip("Display name of the part")]
        public string partName;

        [Tooltip("Part description")]
        [TextArea(2, 4)]
        public string description;

        [Header("Part Classification")]
        [Tooltip("Category of this part (Receiver, Barrel, etc.)")]
        public WeaponPartCategory category;

        [Tooltip("Rarity tier of this part")]
        public WeaponRarity rarity = WeaponRarity.Common;

        [Tooltip("Type tier within rarity")]
        public WeaponPartType partType = WeaponPartType.Standard;

        [Tooltip("Manufacturer (only applicable to Receivers)")]
        public WeaponManufacturer manufacturer;

        [Header("Visual Assets")]
        [Tooltip("3D model prefab for this part")]
        public GameObject modelPrefab;

        [Tooltip("Icon for UI display")]
        public Sprite icon;

        [Tooltip("Attachment point offset")]
        public Vector3 attachmentOffset;

        [Tooltip("Attachment point rotation")]
        public Vector3 attachmentRotation;

        [Header("Base Statistics")]
        [Tooltip("List of stat modifiers applied by this part")]
        public List<StatModifier> statModifiers = new List<StatModifier>();

        [Header("Special Effects")]
        [Tooltip("Special effects granted by this part (typically for Epic+ rarity)")]
        public List<PartSpecialEffect> specialEffects = new List<PartSpecialEffect>();

        [Header("Compatibility")]
        [Tooltip("Compatible weapon types (empty = all)")]
        public List<string> compatibleWeaponTypes = new List<string>();

        [Tooltip("Incompatible part IDs (cannot be used together)")]
        public List<string> incompatiblePartIDs = new List<string>();

        [Tooltip("Required part IDs (must have these parts)")]
        public List<string> requiredPartIDs = new List<string>();

        [Header("Generation Settings")]
        [Tooltip("Weight for random generation (higher = more common)")]
        [Range(0f, 100f)]
        public float dropWeight = 10f;

        [Tooltip("Minimum player level required")]
        [Range(1, 50)]
        public int minimumLevel = 1;

        [Tooltip("Can this part appear in world drops")]
        public bool worldDropEnabled = true;

        /// <summary>
        /// Get the rarity color for UI display
        /// </summary>
        public Color GetRarityColor()
        {
            return rarity switch
            {
                WeaponRarity.Common => new Color(0.7f, 0.7f, 0.7f),      // Gray
                WeaponRarity.Uncommon => new Color(0.2f, 0.8f, 0.2f),    // Green
                WeaponRarity.Rare => new Color(0.3f, 0.5f, 1.0f),        // Blue
                WeaponRarity.Epic => new Color(0.6f, 0.2f, 0.8f),        // Purple
                WeaponRarity.Legendary => new Color(1.0f, 0.6f, 0.0f),   // Orange
                WeaponRarity.Pearlescent => new Color(0.0f, 0.9f, 0.9f), // Cyan
                WeaponRarity.Apocalypse => new Color(0.9f, 0.1f, 0.1f),  // Red
                _ => Color.white
            };
        }

        /// <summary>
        /// Get the rarity value for weapon rarity calculation
        /// </summary>
        public int GetRarityValue()
        {
            return (int)rarity;
        }

        /// <summary>
        /// Get stat improvement range based on rarity
        /// </summary>
        public (float min, float max) GetStatImprovementRange()
        {
            return rarity switch
            {
                WeaponRarity.Common => (0f, 0f),          // Base stats
                WeaponRarity.Uncommon => (0.05f, 0.10f),  // +5-10%
                WeaponRarity.Rare => (0.10f, 0.20f),      // +10-20%
                WeaponRarity.Epic => (0.20f, 0.35f),      // +20-35%
                WeaponRarity.Legendary => (0.35f, 0.50f), // +35-50%
                WeaponRarity.Pearlescent => (0.50f, 0.75f), // +50-75%
                WeaponRarity.Apocalypse => (0.75f, 1.00f),  // +75-100%
                _ => (0f, 0f)
            };
        }

        /// <summary>
        /// Get the drop rate for this rarity tier
        /// </summary>
        public float GetDropRate()
        {
            return rarity switch
            {
                WeaponRarity.Common => 0.60f,       // 60%
                WeaponRarity.Uncommon => 0.25f,     // 25%
                WeaponRarity.Rare => 0.10f,         // 10%
                WeaponRarity.Epic => 0.04f,         // 4%
                WeaponRarity.Legendary => 0.009f,   // 0.9%
                WeaponRarity.Pearlescent => 0.0009f,// 0.09%
                WeaponRarity.Apocalypse => 0.0001f, // 0.01%
                _ => 0f
            };
        }

        /// <summary>
        /// Validate the part definition
        /// </summary>
        public bool Validate(out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrEmpty(partID))
                errors.Add("Part ID is required");

            if (string.IsNullOrEmpty(partName))
                errors.Add("Part Name is required");

            // Check for exotic type at low rarities (not allowed per GDD)
            if (partType == WeaponPartType.Exotic && rarity < WeaponRarity.Rare)
                errors.Add("Exotic parts are only available at Rare rarity or higher");

            // Check special effects at low rarities
            if (specialEffects.Count > 0 && rarity < WeaponRarity.Rare)
                errors.Add("Special effects should only be on Rare or higher rarity parts");

            // Receivers must have manufacturer
            if (category == WeaponPartCategory.Receiver && manufacturer == 0)
            {
                // This is fine, KDC is manufacturer 0
            }

            return errors.Count == 0;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Auto-generate part ID if empty
            if (string.IsNullOrEmpty(partID) && !string.IsNullOrEmpty(partName))
            {
                partID = $"{category}_{partName}_{rarity}_{partType}".Replace(" ", "_").ToLower();
            }
        }
#endif
    }
}
