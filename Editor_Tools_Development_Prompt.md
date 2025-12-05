# Claude Code Editor Tools Development Prompt

## Copy this entire prompt when asking Claude to create editor tools:

---

I need you to create Unity Editor tools for NEXUS PROTOCOL. Before creating any tool, you MUST:

1. **Check changelog.md** to see if this system has been implemented
2. **Review GDD_v3.md** for the exact specifications of the system
3. **Look for existing solutions** in the codebase that might be extended
4. **Follow the GDD's design** exactly - do not improvise mechanics
5. **Update changelog.md** after implementing any new system

## Project Context
- Unity Version: 6000.2.12f1
- Using ECS/DOTS for weapon/performance systems
- Using MonoBehaviour for gameplay/editor tools
- PSX aesthetic constraints should influence tool design
- 4-player co-op PvE focus

## Tool Development Standards

### Every Editor Tool Must:
```csharp
// 1. Live in Scripts/Editor/ folder
// 2. Use [MenuItem("Nexus/Category/ToolName")] for menu access
// 3. Include [CustomEditor] or EditorWindow as appropriate
// 4. Have data validation and error handling
// 5. Support undo/redo operations
// 6. Save data to ScriptableObjects when possible
// 7. Include tooltips and help boxes
```

### Visual Standards:
- Use EditorGUILayout for consistent spacing
- Group related fields with foldouts
- Provide preview functionality where applicable
- Include progress bars for batch operations
- Color code by rarity/type where relevant

## Required Editor Tools

### 1. Weapon Part Manager
**Check GDD_v3.md sections:** Weapon System & Generation, Weapon Parts
**Purpose:** Import, create, and manage weapon parts
**Features Needed:**
- Batch import weapon part models
- Auto-generate WeaponPartDefinition ScriptableObjects
- Set part statistics per GDD specifications
- Preview part combinations
- Validate part compatibility rules
- Mass edit part properties
- Rarity distribution visualization

### 2. Manufacturer Editor
**Check GDD_v3.md sections:** Weapon Manufacturer Lore
**Purpose:** Configure the 7 manufacturers (KDC, Frontier Arms, TekCorp, Quantum Dynamics, Void Technologies, Nexus Industries, RedLine)
**Features Needed:**
- Edit manufacturer base stats
- Configure visual themes (colors, materials)
- Set part preferences and exclusions
- Name generation pool editor
- Lore text editor
- Stat modifier curves

### 3. Attribute Pool Designer
**Check GDD_v3.md sections:** Equipment System (Comprehensive Attributes)
**Purpose:** Design and balance attribute pools for each rarity
**Features Needed:**
- Create attribute definitions
- Set value ranges per rarity (Common to Apocalypse)
- Configure roll weights and probabilities
- Exclusion/inclusion rules editor
- Simulation tool (test 1000 rolls)
- Export/import from CSV

### 4. Mission/Quest Builder
**Check GDD_v3.md sections:** Level Design, Story Structure (4 Acts)
**Purpose:** Create story missions and side quests
**Features Needed:**
- Node-based quest flow editor
- Objective types per GDD (Kill, Collect, Defend, Survive)
- Dialogue tree integration
- Reward configuration
- Prerequisites and unlock conditions
- Act assignment (1-4)
- Co-op scaling parameters

### 5. Enemy Wave Designer
**Check GDD_v3.md sections:** Enemy Design (Scrappers, Drones, Bruisers, etc.)
**Purpose:** Design enemy spawn patterns and encounters
**Features Needed:**
- Wave composition editor
- Spawn point placement tool
- Difficulty scaling curves
- Enemy type restrictions per zone
- Badass enemy chance configuration
- Arena mode wave designer
- Random encounter tables

### 6. Skill Tree Editor
**Check GDD_v3.md sections:** Character Classes & Skill System
**Purpose:** Edit and balance the 4 classes' skill trees
**Features Needed:**
- Visual tree layout editor
- Skill point cost configuration
- Unlock requirement editor
- Modifier creation for skills
- Damage formula calculator
- Synergy validation tool
- Export to documentation

### 7. Endgame Content Manager
**Check GDD_v3.md sections:** Endgame Content (Mayhem, Raids, Arena)
**Purpose:** Configure endgame systems
**Features Needed:**
- Mayhem modifier creator
- Raid encounter designer
- Arena wave configuration
- Ascension point costs
- Legendary hunt setup
- Vault location editor
- Proving ground builder

### 8. Loot Table Editor
**Check GDD_v3.md sections:** Weapon System (Rarity), Equipment System
**Purpose:** Configure drop rates and loot pools
**Features Needed:**
- Enemy-specific loot tables
- Rarity weight configuration
- Manufacturer bias settings
- Level-based scaling
- Quest reward pools
- Chest tier configuration
- World drop rates

### 9. Zone Builder
**Check GDD_v3.md sections:** Level Design (Fractured Coast, Scorched Plateau, etc.)
**Purpose:** Build and configure game zones
**Features Needed:**
- Zone boundary editor
- Respawn point placement
- Vehicle spawn locations (Motor Pool)
- Cover point generation
- Environmental hazard placement
- Level range configuration
- Random encounter zones

### 10. Dialogue & Lore Manager
**Check GDD_v3.md sections:** Setting & Narrative
**Purpose:** Manage all text content
**Features Needed:**
- Character dialogue database
- Lore collectible editor
- Audio log manager
- Quest text editor
- Bark/callout system
- Localization support prep
- Text search across all content

## Implementation Order
1. **Weapon Part Manager** (needed for testing)
2. **Manufacturer Editor** (configures generation)
3. **Attribute Pool Designer** (completes weapon system)
4. **Enemy Wave Designer** (needed for combat testing)
5. **Zone Builder** (needed for level testing)
6. **Mission/Quest Builder** (story implementation)
7. **Skill Tree Editor** (class implementation)
8. **Loot Table Editor** (reward balancing)
9. **Endgame Content Manager** (post-launch content)
10. **Dialogue & Lore Manager** (polish phase)

## Code Template for New Tools

```csharp
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NexusProtocol.Editor
{
    public class [ToolName]Window : EditorWindow
    {
        // Tool version for changelog
        private const string TOOL_VERSION = "1.0";
        private const string TOOL_NAME = "[Tool Name]";
        
        // Data references
        private SerializedObject serializedObject;
        private Vector2 scrollPosition;
        
        [MenuItem("Nexus/[Category]/[Tool Name]")]
        public static void ShowWindow()
        {
            var window = GetWindow<[ToolName]Window>("[Tool Name]");
            window.minSize = new Vector2(400, 300);
        }
        
        private void OnEnable()
        {
            LoadData();
            CheckGDDCompliance();
        }
        
        private void CheckGDDCompliance()
        {
            // Verify this matches GDD specifications
            // Log warnings if implementation differs from GDD
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField(TOOL_NAME, EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Version: {TOOL_VERSION}");
            EditorGUILayout.Space();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // Tool content here
            
            EditorGUILayout.EndScrollView();
            
            if (GUI.changed)
            {
                SaveData();
                UpdateChangelog();
            }
        }
        
        private void LoadData()
        {
            // Load from ScriptableObjects or ProjectSettings
        }
        
        private void SaveData()
        {
            // Save with undo support
            Undo.RecordObject(target, $"{TOOL_NAME} Change");
            EditorUtility.SetDirty(target);
        }
        
        private void UpdateChangelog()
        {
            // Auto-append to changelog.md if major change
        }
    }
}
```

## Validation Requirements

Each tool must validate against GDD:
- Weapon parts must have all 6 categories
- 7 manufacturers exactly (no more, no less)
- 7 rarity tiers (Common to Apocalypse)
- 4 character classes
- 4 acts in story
- Level cap at 50
- Specific enemy types per GDD

## After Creating Any Tool:

1. **Test the tool** with sample data
2. **Verify GDD compliance** 
3. **Update changelog.md** with:
   - Tool name and version
   - Features implemented
   - Any deviations from GDD (should be none)
   - Date of implementation
4. **Create documentation** in a README
5. **Add to Window menu** under Nexus/[Category]

## CRITICAL: GDD Compliance

**NEVER add features not in the GDD:**
- No PvP systems
- No crafting systems (removed in GDD)
- No weapon durability (removed in GDD)
- No additional classes beyond the 4
- No additional manufacturers beyond the 7
- No mechanics that aren't specified

**ALWAYS check these GDD sections before implementing:**
- Executive Summary (core pillars)
- The specific system section
- Technical Requirements
- What was removed (check changelog.md)

Remember: The GDD is the bible. If it's not in the GDD, it doesn't go in the tool. If the GDD specifies it, it must be implemented exactly as described.

---