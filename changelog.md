# Project Changelog - NEXUS PROTOCOL

## [Editor Tools v1.1] - 2024-12-05 - Loot Table Manager

### Added - Loot Table Manager Tool (v1.0)
**Location:** `Nexus/Loot/Loot Table Manager` menu

#### Core Features Implemented:
- **Overview Tab**: Database statistics, quick actions, validation
- **Table Editor Tab**: Full loot table editing with entry management
- **Enemy Tables Tab**: Enemy-specific loot configuration (10 enemy types)
- **Chest Tiers Tab**: Chest tier configuration (7 tiers: White to Red)
- **Quest Rewards Tab**: Quest reward pool configuration (Side, Main, Daily, Weekly)
- **World Drops Tab**: Zone-specific world drop rates (6 zones + endgame)
- **Rarity Config Tab**: Visualize rarity weights and manufacturer distribution
- **Simulation Tab**: Drop simulation testing (up to 100,000 rolls)

#### GDD Compliance:
- **7 Rarity Tiers**: Proper drop rates per GDD (Common 60% to Apocalypse 0.01%)
- **7 Manufacturers**: Manufacturer bias settings for weapon drops
- **10 Mayhem Levels**: Mayhem scaling (+20% loot quality per level, +50% enemy health)
- **6 Game Zones**: Level-based scaling (Fractured Coast to Core Facility)
- **Chest Tiers**: White (Common only) to Red (Apocalypse possible)

#### Files Created:
- `Assets/Scripts/Runtime/Loot/LootEnums.cs` - Source types, item types, chest tiers, zones
- `Assets/Scripts/Runtime/Loot/LootTable.cs` - LootTableEntry and LootTable ScriptableObjects
- `Assets/Scripts/Runtime/Loot/LootTableDatabase.cs` - Database with cached lookups
- `Assets/Scripts/Editor/Loot/LootTableManagerWindow.cs` - Main editor window (1100+ lines)

#### Technical Features:
- Enemy-specific loot tables with rarity/level scaling
- Rarity weight configuration with visual charts
- Manufacturer bias settings (per-table and per-entry)
- Level-based scaling with AnimationCurve support
- Quest reward pools with quest ID conditions
- Chest tier configuration matching GDD specifications
- World drop rates by zone
- Drop condition flags (FirstKill, Coop, Mayhem6+, etc.)
- Drop simulation with rarity/type/manufacturer breakdown
- Mayhem-exclusive entries (Mayhem 6+ only drops)
- Parent table inheritance system

#### Tool Standards Followed:
- Lives in Scripts/Editor/ folder
- Uses [MenuItem("Nexus/Loot/Loot Table Manager")]
- Data stored in ScriptableObjects
- Includes tooltips and help boxes
- Supports batch table generation
- Progress bars for simulations

---

## [Editor Tools v1.0] - 2024-12-05 - Weapon Part Manager

### Added - Weapon Part Manager Tool (v1.0)
**Location:** `Nexus/Weapons/Weapon Part Manager` menu

#### Core Features Implemented:
- **Part Editor Tab**: Create, edit, and delete individual weapon parts with full property editing
- **Batch Import Tab**: Import multiple weapon part models at once with auto-generation of ScriptableObjects
- **Mass Edit Tab**: Apply changes to multiple parts simultaneously based on filters
- **Preview Tab**: Preview part combinations and calculate final weapon rarity per GDD algorithm
- **Validation Tab**: Validate all parts against GDD specifications and check compliance
- **Statistics Tab**: Visualize rarity distribution with charts and run drop simulations

#### GDD Compliance:
- **6 Part Categories**: Receiver, Barrel, Magazine, Grip, Stock, Sight
- **7 Rarity Tiers**: Common (60%), Uncommon (25%), Rare (10%), Epic (4%), Legendary (0.9%), Pearlescent (0.09%), Apocalypse (0.01%)
- **7 Manufacturers**: KDC, Frontier Arms, TekCorp, Quantum Dynamics, Void Industries, Nexus Salvage, RedLine
- **3 Part Types**: Standard, Special, Exotic (within each rarity)

#### Files Created:
- `Assets/Scripts/Runtime/Weapons/WeaponEnums.cs` - All weapon-related enumerations
- `Assets/Scripts/Runtime/Weapons/WeaponPartDefinition.cs` - ScriptableObject for individual parts
- `Assets/Scripts/Runtime/Weapons/WeaponPartDatabase.cs` - Database for managing all parts
- `Assets/Scripts/Editor/Weapons/WeaponPartManagerWindow.cs` - Main editor window (1000+ lines)

#### Technical Features:
- Undo/Redo support for all operations
- Progress bars for batch operations
- Color-coded rarity display
- Cached lookups for performance
- Drag & drop model import
- Drop rate simulation (up to 100,000 rolls)
- Automatic stat application based on rarity ranges
- Manufacturer-specific bonus application
- Compatibility validation between parts

#### Tool Standards Followed:
- Lives in Scripts/Editor/ folder
- Uses [MenuItem("Nexus/Weapons/Weapon Part Manager")]
- Data stored in ScriptableObjects
- Includes tooltips and help boxes
- Supports batch operations with progress bars

---

## [Revision 3.4 FINAL] - 2024-11-24

### Changed
- **Skill Tree Capstones**: Now require 50 points to reach (was 30)
- This means reaching a capstone requires level 25 minimum if focusing entirely on one tree
- Creates more meaningful build decisions and progression

## [Revision 3.3] - 2024-11-24

### Changed
- **Removed Weapon Durability**: No repair or durability mechanics
- **Replaced Wasteland Works**: New manufacturer "Frontier Arms" with cohesive theme
- **Manufacturer Design Philosophy**: Added achievable visual design guidelines within PSX constraints
- **Skill Tree Progression**: Now based on points spent in tree, not character level
- **Skill Tree Structure**: Standardized to Main → Passives → Secondary → Passives → 3 Modifier Branches → Capstone
- **Combat Changes**: Removed dodge roll, kept combat roll only
- **Health System**: Single health bar with passive out-of-combat regeneration
- **Ammo Types**: Integrated into weapon parts system instead of separate section
- **Critical Chance**: Lowered all values to make 100% crit require dedicated builds
- **Removed**: All direct Borderlands references and item names

### Removed
- Vehicle weapon customization (too complex)
- Wasteland Works manufacturer (didn't fit theme)
- Weapon repair/durability system
- Separate ammo types section

## [Revision 3.2] - 2024-11-24

### Changed
- **Vehicle Spawning System**: Renamed from "Catch-a-Ride" to "Motor Pool"
- **Vehicle Customization Simplified**: Only primary and secondary paint colors
- **Comprehensive Attribute System Added**: Detailed attribute pools for all gear types
  - Weapons: 30+ possible attributes
  - Shields: 20+ possible attributes  
  - Class Mods: 25+ possible attributes
  - Grenades: 15+ possible attributes
  - Relics: 20+ possible attributes

### Technical Improvements
- Attribute system creates more build diversity
- Clearer progression through attribute tiers
- Simplified vehicle customization for easier implementation

## [Revision 3.1] - 2024-11-24

### Changed
- **Vehicle System Simplified**: Two vehicles only - Strider (single seat) and Carrier (multi-seat)
- **Weapon Rarity Rework**: Rarity now determined by part rarity, not weapon rarity
  - Parts have both rarity tier (Common-Apocalypse) and type tier (Standard-Exotic)
  - Weapon rarity is sum of part rarities
- **Scavenger Class Replaced**: Complete redesign as "ENGINEER" class
  - Focus on deployables, constructs, and battlefield control
  - Three new trees: Fabrication, Automation, Modification

### Technical Changes
- Each co-op player can summon individual vehicles
- Catch-a-Ride stations are only summon method
- Part rarity system creates more granular loot progression

## [Revision 2.1] - 2024-11-24

### Changed
- **Third-Person Perspective**: Changed from FPS to TPS
- **Simplified Shield System**: Shields now use attribute pools instead of parts
- **Removed Trigger Parts**: Simplified weapon system
- **Ammo Types Reworked**: Now modify damage type, penetration, projectile count
- **Renamed Paragon to Ascension System**: More unique naming
- **Skill System Adjustment**: 
  - Modifier skills (1 per main skill) are equipable
  - Attributes are passive non-equipable upgrades
- **Manufacturer System**: Receiver determines manufacturer, added lore
- **Standard Parts**: No longer modify stats (only special/exotic do)
- **Removed Infusion System**: Too complex for initial release
- **Combined Anointments with Class Mods**: Single unified system
- **Added Vehicle System**: Similar to Borderlands 2 implementation

### Added
- Detailed weapon generation algorithm explanation
- Manufacturer lore and unique characteristics
- Vehicle types and customization system

### Removed
- Infusion system (too complex)
- Trigger weapon parts
- Separate anointment system

## [Major Rework] - 2024-11-24

### Changed
- **Complete Class System Overhaul**: Redesigned to Borderlands/Bethesda hybrid system
  - Players equip 1 main skill, 1 secondary skill (both active)
  - 1 attribute skill and 1 capstone skill (both passive modifiers)
  - Massively expanded attribute skills throughout trees
  - 2 skill points per level, capstone reachable by level 25
- **Shields as Gear**: Shields now separate equipment pieces with attribute pools
- **Linear Story Ending**: Removed branching choice system from Act 4
- **Special Weapons → Special Parts**: Converted to weapon modification system
- **5x Expansion of Weapon Parts**: Massively expanded parts variety and combinations
- **Renamed Guardian Ranks**: Now called "Paragon System"
- **P2P Networking**: Single-player base with 1-4 co-op, P2P host architecture
- **Removed Crafting/Rerolling**: Focus on drops only
- **Expanded Infusion/Anointment**: More detailed progression systems
- **Traditional Respec System**: Pay to respec or create new characters
- **Removed MMO Features**: No world events, seasonal content, or live service elements

### Removed
- Server-based matchmaking for raids (now 4-player co-op)
- Gear crafting and rerolling systems
- Build loadout quick-swap system
- All post-launch content plans
- World events and seasonal systems

### Technical Decisions
- Steam P2P networking for all multiplayer
- Host-based sessions, no dedicated servers
- Raids scaled to 4-player content
- Focus on complete game at launch, no live service

## [Initial] - 2024-11-24

### Added
- Created comprehensive Game Design Document (GDD.md) for PSX-inspired looter shooter
- Established core game concept: "NEXUS PROTOCOL"
- Defined 4 character classes with complete skill trees
- Designed weapon rarity and parts system
- Created enemy faction hierarchy (NEXUS AI)
- Outlined story structure and world lore
- Detailed endgame content systems
- Specified technical requirements and art direction

### Design Decisions
- Chose retro-futuristic aesthetic combining PSX graphics with modern gameplay
- Implemented modular weapon system for maximum build variety
- Created synergy-focused skill trees to encourage experimentation
- Designed endgame around repeatable, scalable content

### Notes for Future Developers
- Weapon part pools are designed to be easily expandable
- Skill trees use a node-based system for flexibility
- Enemy AI should prioritize spectacle over complexity (PSX limitations)
- UI/UX should balance nostalgia with modern convenience

## [Version 4.0] - 2024-11-24 - Unity Implementation Phase

### Major Systems Implemented

#### Unity ECS Weapon Generation System (Unity_ECS_Weapon_System.md)
- Complete ECS architecture with Burst compilation
- 7 weapon manufacturers with unique traits and philosophies
- 7 rarity tiers (Common to Apocalypse) with special effects
- 300+ weapon part combinations
- Dynamic attribute rolling system
- Performance: 10,000+ weapons/second generation
- Custom editor tools for testing and validation
- Zero GC allocation design

#### Third-Person Controller System (Unity_ThirdPerson_Controller.md)
- CharacterController-based movement system
- Complete movement states: Idle, Walk, Run, Sprint, Crouch, Slide, Jump, Cover
- Division-style magnetic cover system with edge detection
- Three-directional peeking mechanics (left, right, up)
- Blind fire when in cover without ADS
- Dynamic height adjustment for crouch/slide
- Momentum-based sliding with cooldown system
- Event-driven architecture for system decoupling

#### Hybrid Animation System (Unity_Hybrid_Animation_System.md)
- Leverages Unity's Animator for prebaked animations
- Procedural weapon IK for randomly generated weapons
- Automatic grip point detection from weapon geometry
- Dynamic recoil patterns per weapon
- Sway, breathing, and movement bob simulation
- Animation Rigging package integration
- Simplified from initial full-custom approach

#### Weapon Spawning System (Unity_Weapon_Spawning_System.md)
- Development testing spawners (1, 10, 100, 1000 weapons)
- Performance-optimized batch spawning with Job System
- Object pooling for weapon pickups
- Visual effects based on rarity and manufacturer
- Cleanup interactable for spawned weapons management
- Magnetic pickup with range detection
- Auto-despawn timers for performance

#### Input System Integration
- Unity Input System with seamless device switching
- Full KB/M and controller support
- Steam Input API ready
- Context-sensitive control prompts
- Action maps for all player interactions

#### Camera System
- Cinemachine integration with state-based cameras
- Multiple virtual cameras (Default, Aim, Sprint, Cover)
- Dynamic FOV adjustments
- Movement-based camera shake
- Smooth blending between camera modes

### Technical Decisions
- Hybrid approach: Unity's systems for standard features, custom for unique needs
- ECS for weapon generation (performance critical)
- MonoBehaviour for gameplay systems (easier iteration)
- Event-driven communication between systems
- ScriptableObject configs for designer-friendly tweaking

### Performance Optimizations
- Burst compilation for weapon generation
- Job System for parallel processing
- Object pooling for frequently spawned objects
- Batch processing for large spawn counts
- Struct-based components for zero GC

### Development Tools Created
- WeaponGeneratorWindow: Editor tool for testing generation
- Spawner interactables for various weapon counts
- Performance benchmarking tools
- Visual debugging for cover detection and IK

### Architecture Changes from GDD
- Removed full custom animation system (overkill)
- Simplified to hybrid Unity/procedural approach
- Spawners marked as dev-only testing tools
- Actual loot system to be implemented separately

### Next Implementation Steps
1. Basic enemy AI with state machines
2. UI system for inventory and character stats
3. Integration testing of all systems
4. Performance profiling and optimization
5. Networking foundation for co-op
