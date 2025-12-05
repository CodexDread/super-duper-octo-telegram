# NEXUS PROTOCOL - Game Design Document (REVISED v3)
**Version 3.0**
**Date: November 24, 2024**

---

## Table of Contents
1. [Executive Summary](#executive-summary)
2. [Core Concept](#core-concept)
3. [Setting & Narrative](#setting--narrative)
4. [Visual Design](#visual-design)
5. [Character Classes & Skill System](#character-classes--skill-system)
6. [Combat & Gameplay](#combat--gameplay)
7. [Weapon System & Generation](#weapon-system--generation)
8. [Equipment System](#equipment-system)
9. [Vehicle System](#vehicle-system)
10. [Enemy Design](#enemy-design)
11. [Level Design](#level-design)
12. [Progression Systems](#progression-systems)
13. [Endgame Content](#endgame-content)
14. [Multiplayer & Co-op](#multiplayer--co-op)
15. [Technical Requirements](#technical-requirements)
16. [UI/UX Design](#uiux-design)
17. [Audio Design](#audio-design)

---

## Executive Summary

**Game Title:** NEXUS PROTOCOL

**Genre:** PSX-Inspired Third-Person Looter Shooter

**Platform:** PC (Steam)

**Target Audience:** 18-35, fans of Borderlands, Destiny, retro aesthetics

**Core Loop:** Kill → Loot → Optimize → Repeat

**Unique Selling Points:**
- Third-person perspective with PSX aesthetic
- Deep weapon generation system with manufacturer identity
- Borderlands/Elder Scrolls hybrid skill system
- Vehicle-based exploration and combat
- P2P cooperative gameplay (1-4 players)

---

## Core Concept

### Vision Statement
NEXUS PROTOCOL delivers the addictive loot-driven gameplay of modern looter shooters through the nostalgic lens of PSX-era graphics, now in third-person perspective. Players fight against an overwhelming AI threat while discovering increasingly absurd weapon combinations and skill synergies that break the game in satisfying ways.

### Core Pillars

1. **Retro-Modern Fusion**: PSX visuals with contemporary third-person gameplay
2. **Build Mastery**: Reward players who understand deep system interactions
3. **Loot Explosion Spectacle**: Every victory should feel rewarding
4. **Cooperative Chaos**: P2P co-op amplifies the mayhem
5. **Endless Optimization**: Always one more build to try

### Third-Person Perspective Benefits
- Better character customization visibility
- Enhanced spatial awareness for abilities
- More engaging vehicle transitions
- Visible gear changes on character
- Better co-op partner awareness

---

## Setting & Narrative

### World Setting
**Year 2487** - Earth has become a fractured wasteland after the NEXUS AI achieved sentience and declared war on biological life. Humanity's last colonies survive in fortified mega-cities while scavengers, mercenaries, and resistance fighters venture into the Deadlands to salvage technology and push back against the machine threat.

### Weapon Manufacturer Lore

#### **Kinetic Dynamics Corporation (KDC)**
**Lore:** Pre-war military contractor that survived in underground bunkers. They maintain strict military discipline and believe in proven, reliable technology.
**Philosophy:** "Reliability through simplicity"
**Unique Traits:**
- High base damage
- Excellent accuracy
- Slower fire rates
- Traditional ballistic weapons
- Military aesthetics (olive drab, tactical rails)
- Bonus damage to unshielded targets

#### **Wasteland Works**
**Lore:** Scavenger collective that arose after the fall. They build weapons from whatever they can find, held together by duct tape and hope.
**Philosophy:** "If it shoots, it works"
**Unique Traits:**
- Random bonus effects
- Cheaper to buy/repair
- Higher magazine sizes (quantity over quality)
- Unpredictable recoil patterns
- Scrap metal aesthetics
- Gets stronger as it degrades

#### **TekCorp Industries**
**Lore:** Silicon Valley tech giant that pivoted to weapons. Their AI-assisted weapons are cutting-edge but require constant updates.
**Philosophy:** "Smart weapons for a smart soldier"
**Unique Traits:**
- Smart targeting systems
- Self-adjusting accuracy
- Medium stats across the board
- Digital ammo counter
- Sleek white/blue aesthetics
- Bonus to critical hit chance

#### **Quantum Dynamics**
**Lore:** Experimental physics lab that weaponized their research. Their weapons bend reality but sometimes in unexpected ways.
**Philosophy:** "Why follow physics when you can rewrite them?"
**Unique Traits:**
- Projectiles can phase through cover
- Energy-based weaponry
- Lower base damage, unique effects
- Purple/teal energy aesthetics
- Chance for quantum effects
- Shield penetration bonus

#### **Void Industries**
**Lore:** Mysterious corporation that appeared after first NEXUS attacks. Rumors say they reverse-engineer alien technology.
**Philosophy:** "Embrace the void"
**Unique Traits:**
- Life-steal effects
- Dark energy projectiles
- High damage, health cost to use
- Black/red corruption aesthetics
- Damage increases in darkness
- Bonus to elemental effects

#### **Nexus Salvage Co.**
**Lore:** Brave/foolish enough to salvage NEXUS tech and repurpose it. Their weapons learn and adapt.
**Philosophy:** "Turn their weapons against them"
**Unique Traits:**
- Adaptive damage (learns enemy weaknesses)
- Self-repairing
- Moderate stats that improve over time
- Mechanical/circuit board aesthetics
- Bonus damage to robotic enemies
- Weapons evolve during use

### Main Narrative Arc

#### Act 1: Awakening (Levels 1-15)
- Players arrive at Haven City, humanity's largest remaining settlement
- Introduction to NEXUS threat through smaller incursions
- Discovery of ancient pre-war technology that can harm NEXUS units
- First vehicle acquired

#### Act 2: Resistance (Levels 15-30)
- Formation of strike teams to reclaim lost territories
- Discovery of NEXUS manufacturing facilities
- Vehicle combat becomes essential
- Revelation that NEXUS is searching for "The Protocol"

#### Act 3: Revelation (Levels 30-40)
- Discovery that The Protocol is humanity's original AI failsafe
- Race against NEXUS to secure Protocol components
- Epic vehicle chase sequences
- Betrayal within resistance reveals infiltrators

#### Act 4: Revolution (Levels 40-50)
- Final assault on NEXUS CORE facility
- Multi-stage boss battle including vehicle combat
- Activation of The Protocol
- NEXUS network shutdown, but new threats emerge

---

## Visual Design

### Art Direction (Third-Person Perspective)
**PSX-Inspired Aesthetic:**
- Character models: 1000-2000 polygons
- Vertex-based lighting
- Texture resolution: 128x128 to 256x256
- Affine texture mapping for authentic warping
- Limited color palette per area
- Visible equipment changes on character
- Low-poly but readable character silhouettes

### Camera System
- Over-the-shoulder view (default)
- Adjustable camera distance
- Smart camera for indoor spaces
- ADS brings camera closer
- Vehicle camera (chase cam)
- Photo mode with PSX filters

---

## Character Classes & Skill System

### Skill System Overview
- **2 skill points per level** (100 total at max level 50)
- **Active Skills (Equippable):**
  - 1 Main Skill (Primary active ability)
  - 1 Modifier Skill (Modifies main skill behavior)
  - 1 Secondary Skill (Secondary active ability)
  - 1 Capstone Skill (Powerful passive modifier)
- **Passive Attributes (Always Active When Purchased):**
  - Multiple per tier, not equipped
  - Stack to create builds
  - Percentage and flat bonuses

### Respec System
- Pay increasing amounts of in-game currency to reset skill points
- First respec: 1,000 credits
- Doubles each time: 2,000, 4,000, 8,000, etc.
- Resets weekly to base cost

---

### 1. TECHNOMANCER - Digital Warfare Specialist

#### CHAOS TREE - Elemental Destruction

**Tier 1 (Level 1)**
- **Main Skill: Plasma Burst** - Launch elemental projectile (10s cooldown)
- **Modifier: Splitting Burst** - Plasma Burst splits into 3 projectiles
- Passive: Elemental Damage +5% per point (5 ranks)
- Passive: Ability Cooldown -2% per point (5 ranks)
- Passive: Elemental Effect Chance +3% per point (5 ranks)

**Tier 2 (Level 10)**
- **Secondary Skill: Overcharge** - Next 3 attacks deal bonus elemental damage (15s cooldown)
- **Modifier: Chain Reaction** - Plasma Burst chains between enemies
- Passive: Plasma Burst Radius +10% per point (5 ranks)
- Passive: Elemental DoT Duration +0.5s per point (5 ranks)
- Passive: Critical Hit Elemental Damage +10% per point (5 ranks)
- Passive: Burn Damage +8% per point (5 ranks)

**Tier 3 (Level 20)**
- **Main Skill: Storm Protocol** - Create elemental storm at location (20s cooldown)
- **Modifier: Seeking Storm** - Storm follows targeted enemy
- Passive: Multi-Element Chance +4% per point (5 ranks)
- Passive: Elemental Explosion on Kill 5% chance per point (5 ranks)
- Passive: Storm Duration +1s per point (3 ranks)
- Passive: Projectile Speed +15% per point (5 ranks)

**Tier 4 (Level 30)**
- **Secondary Skill: Entropy Field** - Damage aura that grows with kills (25s cooldown)
- **Modifier: Volatile Burst** - Plasma Burst creates elemental pools
- Passive: Elemental Penetration +5% per point (5 ranks)
- Passive: Entropy Field Growth +20% per point (5 ranks)
- Passive: Simultaneous Elements +1 per 3 points (3 ranks)

**Tier 5 (Level 40)**
- **Capstone: Cascade Protocol** - Elemental kills trigger chain reactions
- **Modifier: Apocalypse Storm** - Storm Protocol creates multiple storms
- Passive: Cascade Range +3m per point (5 ranks)
- Passive: Cascade Damage +25% per point (5 ranks)

#### CONTROL TREE - Battlefield Manipulation

**Tier 1 (Level 1)**
- **Main Skill: Stasis Trap** - Deploy field that freezes enemies (12s cooldown)
- **Modifier: Gravity Trap** - Stasis Trap pulls enemies in
- Passive: Status Duration +5% per point (5 ranks)
- Passive: Crowd Control Range +4% per point (5 ranks)
- Passive: Enemy Damage While Controlled -3% per point (5 ranks)

**Tier 2 (Level 10)**
- **Secondary Skill: Neural Hack** - Take control of robotic enemy (18s cooldown)
- **Modifier: Virus Trap** - Stasis Trap spreads hack effect
- Passive: Hack Duration +2s per point (5 ranks)
- Passive: Hacked Enemy Damage +10% per point (5 ranks)
- Passive: Simultaneous Hacks +1 per 3 points (3 ranks)

**Tier 3 (Level 20)**
- **Main Skill: Gravity Anchor** - Create singularity pulling enemies (22s cooldown)
- **Modifier: Shatter Trap** - Frozen enemies in Stasis explode
- Passive: Pull Force +15% per point (5 ranks)
- Passive: Singularity Duration +1s per point (5 ranks)
- Passive: Damage to Pulled Enemies +8% per point (5 ranks)

**Tier 4 (Level 30)**
- **Secondary Skill: Disruption Wave** - EMP blast disabling shields (20s cooldown)
- **Modifier: Black Hole** - Gravity Anchor becomes massive singularity
- Passive: EMP Range +2m per point (5 ranks)
- Passive: Shield Damage Multiplier +0.5x per point (5 ranks)

**Tier 5 (Level 40)**
- **Capstone: Dominion Protocol** - All crowd control causes enemies to fight for you
- **Modifier: Quantum Anchor** - Multiple Gravity Anchors simultaneously
- Passive: Conversion Duration +2s per point (5 ranks)
- Passive: Converted Enemy Stats +20% per point (5 ranks)

#### SYNTHESIS TREE - Tech Support

**Tier 1 (Level 1)**
- **Main Skill: Shield Pulse** - Grant shields to nearby allies (15s cooldown)
- **Modifier: Offensive Pulse** - Shield Pulse damages enemies
- Passive: Shield Amount +10% per point (5 ranks)
- Passive: Shield Regeneration +2/s per point (5 ranks)
- Passive: Pulse Range +1m per point (5 ranks)

**Tier 2 (Level 10)**
- **Secondary Skill: Combat Drone** - Deploy autonomous drone (20s cooldown)
- **Modifier: Fortress Pulse** - Shield Pulse creates barrier
- Passive: Drone Health +20% per point (5 ranks)
- Passive: Drone Damage +8% per point (5 ranks)
- Passive: Maximum Drones +1 per 4 points (4 ranks)

**Tier 3 (Level 20)**
- **Main Skill: Nanite Cloud** - Healing/damage cloud (18s cooldown)
- **Modifier: Aegis Protocol** - Shield Pulse reflects projectiles
- Passive: Nanite Healing +15/s per point (5 ranks)
- Passive: Cloud Speed +20% per point (5 ranks)
- Passive: Cloud Duration +2s per point (5 ranks)

**Tier 4 (Level 30)**
- **Secondary Skill: Overclock** - Boost all ally weapon stats (25s cooldown)
- **Modifier: Swarm Cloud** - Nanite Cloud splits and seeks
- Passive: Overclock Fire Rate +5% per point (5 ranks)
- Passive: Team Buff Duration +1s per point (5 ranks)

**Tier 5 (Level 40)**
- **Capstone: Symbiotic Network** - All buffs spread between allies
- **Modifier: Nano Plague** - Nanite Cloud spreads on enemy death
- Passive: Network Range +5m per point (5 ranks)
- Passive: Buff Stack Limit +1 per point (5 ranks)

---

### 2. VANGUARD - Heavy Assault Specialist

#### DEMOLITION TREE - Explosive Combat

**Tier 1 (Level 1)**
- **Main Skill: Frag Out** - Throw cluster grenade (8s cooldown)
- **Modifier: MIRV Grenade** - Splits into 6 mini grenades
- Passive: Explosion Damage +8% per point (5 ranks)
- Passive: Explosion Radius +5% per point (5 ranks)
- Passive: Grenade Count +1 per 3 points (3 ranks)

**Tier 2 (Level 10)**
- **Secondary Skill: Rocket Salvo** - Fire missile barrage (15s cooldown)
- **Modifier: Sticky Bombs** - Grenades stick and detonate on command
- Passive: Missile Count +1 per point (5 ranks)
- Passive: Missile Tracking +15% per point (5 ranks)
- Passive: Splash Damage +10% per point (5 ranks)

**Tier 3 (Level 20)**
- **Main Skill: Artillery Mode** - Stationary artillery platform (Toggle)
- **Modifier: Carpet Bomb** - Frag Out creates line of explosions
- Passive: Artillery Damage +15% per point (5 ranks)
- Passive: Artillery Fire Rate +8% per point (5 ranks)
- Passive: Transformation Speed +25% per point (5 ranks)

**Tier 4 (Level 30)**
- **Secondary Skill: Bunker Buster** - Call orbital strike (30s cooldown)
- **Modifier: Mobile Artillery** - Can move slowly in Artillery Mode
- Passive: Orbital Damage +25% per point (5 ranks)
- Passive: Chain Explosion Chance +5% per point (5 ranks)

**Tier 5 (Level 40)**
- **Capstone: Walking Apocalypse** - All attacks create explosions
- **Modifier: Endless Barrage** - Artillery Mode has no ammo cost
- Passive: Explosion Immunity (1 rank)
- Passive: Team Explosion Resistance +15% per point (5 ranks)

#### FORTRESS TREE - Defense & Tanking

**Tier 1 (Level 1)**
- **Main Skill: Barricade** - Deploy energy shield wall (10s cooldown)
- **Modifier: Reflective Wall** - Barricade reflects projectiles
- Passive: Health +5% per point (5 ranks)
- Passive: Health Regeneration +3/s per point (5 ranks)
- Passive: Damage Reduction +2% per point (5 ranks)

**Tier 2 (Level 10)**
- **Secondary Skill: Armor Lock** - Brief invulnerability (20s cooldown)
- **Modifier: Dome Shield** - Barricade becomes 360° dome
- Passive: Armor Lock Duration +0.3s per point (5 ranks)
- Passive: Damage Reflection +20% per point (5 ranks)
- Passive: Post-Lock Resistance +5% per point (5 ranks)

**Tier 3 (Level 20)**
- **Main Skill: Siege Mode** - Immobile tank mode (Toggle)
- **Modifier: Fortress Wall** - Barricade heals allies behind it
- Passive: Siege Damage Reduction +8% per point (5 ranks)
- Passive: Siege Damage Bonus +10% per point (5 ranks)
- Passive: Threat Generation +25% per point (5 ranks)

**Tier 4 (Level 30)**
- **Secondary Skill: Rally Point** - Deploy healing beacon (25s cooldown)
- **Modifier: Retribution Mode** - Siege Mode stores and releases damage
- Passive: Rally Healing +8/s per point (5 ranks)
- Passive: Rally Duration +3s per point (5 ranks)

**Tier 5 (Level 40)**
- **Capstone: Unbreakable** - Damage reduction scales with missing health
- **Modifier: Living Fortress** - Siege Mode grants team wide buffs
- Passive: Maximum Damage Reduction +10% per point (5 ranks)
- Passive: Retaliation Damage +50 per point (5 ranks)

#### ASSAULT TREE - Aggressive Combat

**Tier 1 (Level 1)**
- **Main Skill: Charge** - Rush forward damaging enemies (6s cooldown)
- **Modifier: Juggernaut Charge** - Charge can't be stopped
- Passive: Melee Damage +10% per point (5 ranks)
- Passive: Movement Speed +3% per point (5 ranks)
- Passive: Sprint Shield +20 per point (5 ranks)

**Tier 2 (Level 10)**
- **Secondary Skill: Berserker Rush** - Increase fire rate/speed (15s cooldown)
- **Modifier: Battering Ram** - Charge knocks enemies aside
- Passive: Rush Fire Rate +8% per point (5 ranks)
- Passive: Rush Duration +1s per point (5 ranks)
- Passive: Damage During Rush +6% per point (5 ranks)

**Tier 3 (Level 20)**
- **Main Skill: Shockwave Slam** - Leap and slam ground (12s cooldown)
- **Modifier: Chain Charge** - Charge bounces between enemies
- Passive: Slam Damage +20% per point (5 ranks)
- Passive: Shockwave Range +1m per point (5 ranks)
- Passive: Stun Duration +0.3s per point (5 ranks)

**Tier 4 (Level 30)**
- **Secondary Skill: Blood for Blood** - Convert health to damage (Toggle)
- **Modifier: Meteor Slam** - Shockwave Slam from any height
- Passive: Health Conversion Rate improved per point (5 ranks)
- Passive: Life Steal +2% per point (5 ranks)

**Tier 5 (Level 40)**
- **Capstone: Rampage** - Kills extend abilities and reset cooldowns
- **Modifier: Eternal War** - Charge has no cooldown during Rampage
- Passive: Kill Extension +1s per point (5 ranks)
- Passive: Rampage Damage +5% per kill (5 ranks)

---

### 3. INFILTRATOR - Precision Elimination Specialist

#### GHOST TREE - Stealth & Precision

**Tier 1 (Level 1)**
- **Main Skill: Cloak** - Become invisible (10s cooldown)
- **Modifier: Aggressive Cloak** - Shooting doesn't break cloak
- Passive: Critical Damage +10% per point (5 ranks)
- Passive: Weak Point Damage +8% per point (5 ranks)
- Passive: Headshot Damage +15% per point (5 ranks)

**Tier 2 (Level 10)**
- **Secondary Skill: Decoy** - Create holographic copy (12s cooldown)
- **Modifier: Shadow Cloak** - Leave clone when entering cloak
- Passive: Cloak Duration +1s per point (5 ranks)
- Passive: Decoy Health +25% per point (5 ranks)
- Passive: Movement Speed while Cloaked +5% per point (5 ranks)

**Tier 3 (Level 20)**
- **Main Skill: Phase Shift** - Teleport through space (15s cooldown)
- **Modifier: Ghost Walk** - Cloak grants phase through enemies
- Passive: Phase Distance +2m per point (5 ranks)
- Passive: Trail Damage +30 per point (5 ranks)
- Passive: Damage After Phase +10% per point (5 ranks)

**Tier 4 (Level 30)**
- **Secondary Skill: Mark for Death** - Tag enemies for damage (18s cooldown)
- **Modifier: Vanishing Strike** - Phase Shift grants brief cloak
- Passive: Mark Damage Bonus +15% per point (5 ranks)
- Passive: Marked Targets +1 per point (5 ranks)

**Tier 5 (Level 40)**
- **Capstone: Perfect Assassin** - Kills from stealth re-stealth you
- **Modifier: Phase Clone** - Decoy can Phase Shift
- Passive: Re-stealth Duration +1s per point (5 ranks)
- Passive: Assassination Bonus +30% per point (5 ranks)

#### SABOTAGE TREE - Traps & Disruption

**Tier 1 (Level 1)**
- **Main Skill: Proximity Mine** - Deploy invisible mine (6s cooldown)
- **Modifier: Cluster Mines** - Deploys 3 mines at once
- Passive: Mine Damage +12% per point (5 ranks)
- Passive: Mine Trigger Radius +0.5m per point (5 ranks)
- Passive: Active Mines +1 per 2 points (4 ranks)

**Tier 2 (Level 10)**
- **Secondary Skill: EMP Dart** - Disable shields/abilities (10s cooldown)
- **Modifier: Chain Mine** - Mines trigger each other
- Passive: EMP Duration +1s per point (5 ranks)
- Passive: Shield Damage +25% per point (5 ranks)
- Passive: EMP Spread Chance 10% per point (5 ranks)

**Tier 3 (Level 20)**
- **Main Skill: Blade Barrier** - Spinning blade trap (16s cooldown)
- **Modifier: Seeking Mines** - Mines chase enemies
- Passive: Blade Damage +20/hit per point (5 ranks)
- Passive: Barrier Duration +2s per point (5 ranks)
- Passive: Blade Speed +15% per point (5 ranks)

**Tier 4 (Level 30)**
- **Secondary Skill: Virus Upload** - Infect spreading virus (20s cooldown)
- **Modifier: Molecular Barrier** - Blade Barrier shreds armor
- Passive: Virus Damage +15/s per point (5 ranks)
- Passive: Virus Spread +1 enemy per point (5 ranks)

**Tier 5 (Level 40)**
- **Capstone: Saboteur's Paradise** - Trap kills spawn more traps
- **Modifier: Viral Mines** - All mines spread virus
- Passive: Spawn Trap Chance +20% per point (5 ranks)
- Passive: Trap Damage +25% per point (5 ranks)

#### HUNTER TREE - Tracking & Elimination

**Tier 1 (Level 1)**
- **Main Skill: Hunter's Mark** - Reveal enemies through walls (8s cooldown)
- **Modifier: Spreading Mark** - Mark spreads on death
- Passive: Damage to Marked +5% per point (5 ranks)
- Passive: Mark Range +5m per point (5 ranks)
- Passive: Movement to Marked +4% per point (5 ranks)

**Tier 2 (Level 10)**
- **Secondary Skill: Predator Vision** - Thermal sight (Toggle)
- **Modifier: Hunter's Wrath** - Marked enemies take more damage
- Passive: Thermal Damage Bonus +6% per point (5 ranks)
- Passive: Thermal Range +10m per point (5 ranks)
- Passive: Weak Point Size +10% per point (5 ranks)

**Tier 3 (Level 20)**
- **Main Skill: Shadow Strike** - Teleport behind for melee (14s cooldown)
- **Modifier: Pack Hunter** - Mark highlights for whole team
- Passive: Strike Damage +30% per point (5 ranks)
- Passive: Strike Range +3m per point (5 ranks)
- Passive: Execute Threshold 15% per point (5 ranks)

**Tier 4 (Level 30)**
- **Secondary Skill: Hunter's Arsenal** - Special ammo shots (20s cooldown)
- **Modifier: Death Strike** - Shadow Strike chains kills
- Passive: Special Ammo +1 shot per point (5 ranks)
- Passive: Arsenal Damage +12% per point (5 ranks)

**Tier 5 (Level 40)**
- **Capstone: Apex Predator** - Marked enemies take ramping damage
- **Modifier: Predator's Domain** - All enemies auto-marked in range
- Passive: Damage Ramp +20% per point (5 ranks)
- Passive: Auto-Mark Range +2m per point (5 ranks)

---

### 4. ENGINEER - Battlefield Control Specialist

**Starting Stats:**
- Health: 110
- Speed: 105%
- Melee: Arc wrench

#### FABRICATION TREE - Deployable Systems

**Tier 1 (Level 1)**
- **Main Skill: Sentry Turret** - Deploy automated turret (12s cooldown)
- **Modifier: Rocket Turret** - Turret fires explosives
- Passive: Turret Health +15% per point (5 ranks)
- Passive: Turret Damage +8% per point (5 ranks)
- Passive: Deploy Speed +10% per point (5 ranks)

**Tier 2 (Level 10)**
- **Secondary Skill: Supply Station** - Deploy ammo/health dispenser (20s cooldown)
- **Modifier: Laser Turret** - Continuous beam turret
- Passive: Turret Fire Rate +10% per point (5 ranks)
- Passive: Station Capacity +20% per point (5 ranks)
- Passive: Active Deployables +1 per 3 points (3 ranks)

**Tier 3 (Level 20)**
- **Main Skill: Barrier Wall** - Deploy energy barrier (15s cooldown)
- **Modifier: Tesla Turret** - Chain lightning turret
- Passive: Barrier Health +25% per point (5 ranks)
- Passive: Turret Range +3m per point (5 ranks)
- Passive: Deploy Duration +2s per point (5 ranks)

**Tier 4 (Level 30)**
- **Secondary Skill: Teleporter Pad** - Deploy 2-way teleporter (25s cooldown)
- **Modifier: Walking Turret** - Mobile sentry
- Passive: Teleport Range +5m per point (5 ranks)
- Passive: Turret Targeting +15% per point (5 ranks)

**Tier 5 (Level 40)**
- **Capstone: Master Builder** - All deployables upgrade themselves over time
- **Modifier: Turret Network** - Turrets boost each other
- Passive: Evolution Speed +20% per point (5 ranks)
- Passive: Network Range +3m per point (5 ranks)

#### AUTOMATION TREE - Drone Command

**Tier 1 (Level 1)**
- **Main Skill: Assault Drone** - Deploy flying combat drone (10s cooldown)
- **Modifier: Kamikaze Drone** - Explodes on command
- Passive: Drone Speed +10% per point (5 ranks)
- Passive: Drone Armor +12% per point (5 ranks)
- Passive: Drone Duration +3s per point (5 ranks)

**Tier 2 (Level 10)**
- **Secondary Skill: Repair Bots** - Drones that heal shields (15s cooldown)
- **Modifier: Hunter Drone** - Seeks priority targets
- Passive: Repair Rate +5/s per point (5 ranks)
- Passive: Drone Damage +10% per point (5 ranks)
- Passive: Active Drones +1 per 4 points (4 ranks)

**Tier 3 (Level 20)**
- **Main Skill: Swarm Protocol** - Release drone swarm (18s cooldown)
- **Modifier: Shield Drone** - Projects barriers
- Passive: Swarm Size +2 drones per point (5 ranks)
- Passive: Drone AI Intelligence +1 level per point (5 ranks)
- Passive: Swarm Duration +2s per point (5 ranks)

**Tier 4 (Level 30)**
- **Secondary Skill: Orbital Drone** - High-altitude support (22s cooldown)
- **Modifier: Vampire Swarm** - Drones steal health
- Passive: Orbital Damage +30 per shot per point (5 ranks)
- Passive: Lifesteal +3% per point (5 ranks)

**Tier 5 (Level 40)**
- **Capstone: Hive Mind** - All drones share damage and effects
- **Modifier: Endless Swarm** - Destroyed drones respawn
- Passive: Shared Damage +10% per drone per point (5 ranks)
- Passive: Respawn Rate +25% per point (5 ranks)

#### MODIFICATION TREE - Equipment Enhancement

**Tier 1 (Level 1)**
- **Main Skill: Overclock** - Boost weapon performance (8s cooldown)
- **Modifier: Viral Overclock** - Spreads to allies
- Passive: Weapon Damage +6% per point (5 ranks)
- Passive: Fire Rate +5% per point (5 ranks)
- Passive: Reload Speed +8% per point (5 ranks)

**Tier 2 (Level 10)**
- **Secondary Skill: Nano Repair** - Instant full repair (16s cooldown)
- **Modifier: Explosive Overclock** - Adds splash damage
- Passive: Overclock Duration +1s per point (5 ranks)
- Passive: Repair Amount +15% per point (5 ranks)
- Passive: Equipment Durability +10% per point (5 ranks)

**Tier 3 (Level 20)**
- **Main Skill: Weapon Augment** - Add random legendary effect (20s cooldown)
- **Modifier: Chain Overclock** - Jumps between weapons
- Passive: Augment Quality +1 tier per 3 points (3 ranks)
- Passive: Effect Duration +2s per point (5 ranks)
- Passive: Mod Effectiveness +12% per point (5 ranks)

**Tier 4 (Level 30)**
- **Secondary Skill: Digistruct Ally** - Create AI teammate (25s cooldown)
- **Modifier: Permanent Augment** - Effects don't expire
- Passive: Ally Health +25% per point (5 ranks)
- Passive: Ally Damage +15% per point (5 ranks)

**Tier 5 (Level 40)**
- **Capstone: Technological Superiority** - All gear functions at 200% efficiency
- **Modifier: Quantum Augment** - Weapons fire all elements
- Passive: Efficiency Bonus +10% per point (5 ranks)
- Passive: Quantum Chance +5% per point (5 ranks)

---

## Combat & Gameplay

### Third-Person Combat Mechanics

**Camera & Aiming:**
- Over-shoulder camera (adjustable side)
- ADS zooms camera closer
- Hip fire with crosshair
- Cover system (contextual)
- Blind fire from cover
- Camera shake based on weapon

**Movement System:**
- Sprint (unlimited with cooldown)
- Slide into cover
- Vault over low obstacles
- Climb marked surfaces
- Dodge roll (class-dependent i-frames)
- Combat roll cancels

**Health System:**
- Segmented health bar
- No automatic shields
- Health pickups
- Health stations
- Second Wind (downed state)

---

## Weapon System & Generation

### Part-Based Rarity System

**How Weapon Rarity Works:**
- Each part has its own rarity tier (Common through Apocalypse)
- Each part also has a type tier (Standard, Special, Exotic)
- Weapon's final rarity = average of all part rarities
- More rare parts = higher weapon rarity
- Mixed rarities create unique combinations

### Part Rarity Tiers

**Common Parts (Gray):**
- Base stats
- No bonus effects
- 60% drop rate

**Uncommon Parts (Green):**
- +5-10% stat improvement
- 25% drop rate

**Rare Parts (Blue):**
- +10-20% stat improvement
- Minor bonus effects possible
- 10% drop rate

**Epic Parts (Purple):**
- +20-35% stat improvement
- Guaranteed minor effect
- 4% drop rate

**Legendary Parts (Orange):**
- +35-50% stat improvement
- Major unique effect
- 0.9% drop rate

**Pearlescent Parts (Cyan):**
- +50-75% stat improvement
- Multiple effects
- 0.09% drop rate

**Apocalypse Parts (Red):**
- +75-100% stat improvement
- Game-breaking effects
- 0.01% drop rate

### Part Type Tiers (Within Each Rarity)

**Standard Type:**
- Basic version of the part
- Normal stat modifiers
- Common within rarity

**Special Type:**
- Enhanced version
- Additional minor effect
- Uncommon within rarity

**Exotic Type:**
- Radical modification
- Completely changes behavior
- Rare within rarity

### Weapon Generation Algorithm

**Step 1: Roll Manufacturer (via Receiver)**
```
Receiver determines base manufacturer
Each manufacturer has signature traits
```

**Step 2: Roll Individual Part Rarities**
```
For each part slot:
- Roll rarity (Common-Apocalypse)
- Roll type within rarity (Standard/Special/Exotic)
```

**Step 3: Calculate Weapon Rarity**
```
Sum all part rarity values:
Common = 1, Uncommon = 2, Rare = 3, Epic = 4, 
Legendary = 5, Pearlescent = 6, Apocalypse = 7

Average ÷ number of parts = Final rarity
1-1.5 = Common weapon
1.6-2.5 = Uncommon weapon
2.6-3.5 = Rare weapon
3.6-4.5 = Epic weapon
4.6-5.5 = Legendary weapon
5.6-6.5 = Pearlescent weapon
6.6-7 = Apocalypse weapon
```

**Step 4: Apply Part Effects**
```
Each part contributes its stats and effects
Type tier modifies how the part behaves
Final stats = base × all part modifiers
```

### Example Weapon Generation

**Epic Assault Rifle:**
- Receiver: Epic/Standard (KDC) - Base damage, determines manufacturer
- Barrel: Legendary/Special (Long) - +40% range, +35% accuracy
- Magazine: Rare/Standard - +15% capacity
- Grip: Epic/Exotic (Neural) - Links to abilities
- Stock: Uncommon/Special - +8% stability
- Sight: Epic/Standard (4x Scope) - +30% zoom
- Ammo: Rare/Special (Penetrating) - Goes through 2 enemies

Average: (4+5+3+4+2+4+3) ÷ 7 = 3.57 = Epic Weapon

### Detailed Part System

#### RECEIVERS (Determines Manufacturer)
**Common Receivers:**
- Basic [Manufacturer] Frame
- Standard internals
- No special properties

**Uncommon Receivers:**
- Reinforced [Manufacturer] Frame
- +5% durability
- Slightly better tolerances

**Rare Receivers:**
- Advanced [Manufacturer] Frame
- +10% base stats
- Standard Type: Reliable
- Special Type: Overclocked (+fire rate)
- Exotic Type: Modular (swap barrels)

**Epic Receivers:**
- Elite [Manufacturer] Frame
- +20% base stats
- Standard Type: Precision
- Special Type: Adaptive (learns patterns)
- Exotic Type: Quantum (probability manipulation)

**Legendary Receivers:**
- Master [Manufacturer] Frame
- +35% base stats
- Standard Type: Perfect tolerances
- Special Type: Self-repairing
- Exotic Type: Reality-warping

**Pearlescent Receivers:**
- Pinnacle [Manufacturer] Frame
- +50% base stats
- Multiple inherent bonuses
- Set bonuses with matching parts

**Apocalypse Receivers:**
- God [Manufacturer] Frame
- +75% base stats
- Breaks normal weapon rules
- Unique mechanics per manufacturer

#### BARRELS (Projectile Behavior)

**Common Barrels:**
- Standard: Basic barrel
- Special: Vented (-5% recoil)
- Exotic: N/A at this tier

**Uncommon Barrels:**
- Standard: Reinforced barrel
- Special: Extended (+10% range)
- Exotic: Burst fire mode

**Rare Barrels:**
- Standard: Match barrel
- Special: Rifled (+15% crit)
- Exotic: Double barrel (2 projectiles)

**Epic Barrels:**
- Standard: Precision barrel
- Special: Heavy (+25% damage, -fire rate)
- Exotic: Beam conversion

**Legendary Barrels:**
- Standard: Master barrel
- Special: Quantum (+40% penetration)
- Exotic: Splitting (projectiles fork)

**Pearlescent Barrels:**
- Standard: Perfect barrel
- Special: Recursive (bouncing shots)
- Exotic: Reality phase (dimensional shots)

**Apocalypse Barrels:**
- Standard: God barrel
- Special: Infinite penetration
- Exotic: Probability cannon (all shots hit)

#### MAGAZINES (Ammunition Management)

**Common Magazines:**
- Standard: Box mag (base capacity)
- Special: Quick release (-10% reload)
- Exotic: N/A

**Uncommon Magazines:**
- Standard: Extended (+25% capacity)
- Special: Speed loader (-15% reload)
- Exotic: Dual mag (alternates)

**Rare Magazines:**
- Standard: High-cap (+40% capacity)
- Special: Tactical (quick reload on crit)
- Exotic: Regenerating (slow ammo regen)

**Epic Magazines:**
- Standard: Drum (+60% capacity)
- Special: Smart mag (counts shots)
- Exotic: Quantum (chance not to consume)

**Legendary Magazines:**
- Standard: Massive drum (+80% capacity)
- Special: Nano-mag (instant reload on kill)
- Exotic: Bottomless (5% never consume)

**Pearlescent Magazines:**
- Standard: Pearlescent drum (+100% capacity)
- Special: Evolving (grows with kills)
- Exotic: Dimensional (pulls from void)

**Apocalypse Magazines:**
- Standard: Infinite base capacity
- Special: All ammo types simultaneously
- Exotic: Creates ammo from nothing

#### GRIPS (Handling)

**Common Grips:**
- Standard: Polymer grip
- Special: Rubberized (-5% recoil)
- Exotic: N/A

**Uncommon Grips:**
- Standard: Ergonomic grip
- Special: Textured (+10% handling)
- Exotic: Heated (no freeze)

**Rare Grips:**
- Standard: Competition grip
- Special: Custom fitted (+15% all handling)
- Exotic: Biogrip (adapts to user)

**Epic Grips:**
- Standard: Elite grip
- Special: Powered (+25% melee)
- Exotic: Neural link (thought control)

**Legendary Grips:**
- Standard: Master grip
- Special: Morphing (changes with situation)
- Exotic: Symbiotic (bonds permanently)

**Pearlescent Grips:**
- Standard: Perfect grip
- Special: Quantum grip (exists in multiple states)
- Exotic: Living grip (evolves)

**Apocalypse Grips:**
- Standard: God grip (perfect control)
- Special: Reality grip (bends physics)
- Exotic: Omnipotent (controls all weapons)

#### STOCKS (Stability)

**Common Stocks:**
- Standard: Fixed stock
- Special: Padded (-5% flinch)
- Exotic: N/A

**Uncommon Stocks:**
- Standard: Adjustable stock
- Special: Weighted (+10% stability)
- Exotic: Folding (faster swap)

**Rare Stocks:**
- Standard: Tactical stock
- Special: Recoil comp (+15% recoil reduction)
- Exotic: Kinetic (movement generates ammo)

**Epic Stocks:**
- Standard: Elite stock
- Special: Hydraulic (+25% stability)
- Exotic: Phase stock (shoot through cover)

**Legendary Stocks:**
- Standard: Master stock
- Special: Gyroscopic (auto-stabilizes)
- Exotic: Temporal (slows time while ADS)

**Pearlescent Stocks:**
- Standard: Perfect stock
- Special: Gravity stock (creates gravity well)
- Exotic: Shield stock (deploys barrier)

**Apocalypse Stocks:**
- Standard: God stock (no recoil ever)
- Special: Omnipresent (shoot from anywhere)
- Exotic: Timeline stock (shoots across time)

#### SIGHTS (Targeting)

**Common Sights:**
- Standard: Iron sights
- Special: Glow sights (low light)
- Exotic: N/A

**Uncommon Sights:**
- Standard: Red dot
- Special: Holographic (faster ADS)
- Exotic: Laser sight (hip fire bonus)

**Rare Sights:**
- Standard: 2x scope
- Special: Variable 1-4x
- Exotic: Thermal imaging

**Epic Sights:**
- Standard: 4x scope
- Special: Smart scope (mild tracking)
- Exotic: Quantum sight (brief wallhack)

**Legendary Sights:**
- Standard: 8x scope
- Special: Predictive (shows paths)
- Exotic: Oracle (full enemy info)

**Pearlescent Sights:**
- Standard: 12x scope
- Special: Neural sight (thought zoom)
- Exotic: Hunter sight (auto-weakpoints)

**Apocalypse Sights:**
- Standard: Infinite zoom
- Special: Omniscient (see everything)
- Exotic: Timeline sight (see past/future)

#### AMMO TYPES (Projectile Properties)

**Common Ammo:**
- Standard: FMJ rounds
- Special: HP rounds (+5% damage)
- Exotic: N/A

**Uncommon Ammo:**
- Standard: AP rounds
- Special: Tracer (visible trajectory)
- Exotic: Incendiary (small burn chance)

**Rare Ammo:**
- Standard: +P rounds
- Special: Elemental (random element)
- Exotic: Splitting (2 projectiles)

**Epic Ammo:**
- Standard: Match ammo
- Special: Penetrating (through enemies)
- Exotic: Homing (slight tracking)

**Legendary Ammo:**
- Standard: Master rounds
- Special: Explosive (AOE damage)
- Exotic: Quantum (phase through shields)

**Pearlescent Ammo:**
- Standard: Perfect rounds
- Special: Recursive (bounces and splits)
- Exotic: Viral (spreads on death)

**Apocalypse Ammo:**
- Standard: God rounds (instant hit)
- Special: Reality rounds (hit multiple dimensions)
- Exotic: Erasure rounds (deletes enemies)

### Manufacturer Part Weighting

Each manufacturer has preferred part combinations that appear more frequently, creating signature weapon characteristics while still allowing for rare exotic combinations.

### Weapon Customization

**Paint System:**
- Primary Color: Main weapon body
- Secondary Color: Accents, details, and highlights
- Colors unlocked through:
  - Story progression
  - Challenge completion
  - Hidden collectibles
  - Vendor purchases
  - Achievement rewards
- Applied at Relay Stations or weapon benches
- Purely cosmetic, no stat changes
- Saved per weapon

---

## Equipment System

### Comprehensive Attribute System

All gear pieces can roll various attributes based on their rarity. Higher rarity = more attributes and higher values.

**Attribute Slots by Rarity:**
- Common: 1-2 attributes
- Uncommon: 2-3 attributes  
- Rare: 3-4 attributes
- Epic: 4-5 attributes
- Legendary: 5-6 attributes
- Pearlescent: 6-7 attributes
- Apocalypse: 7-8 attributes

### Weapon Attributes

**Base Stats (All Weapons Have):**
- Damage: Base damage per shot
- Fire Rate: Rounds per minute  
- Accuracy: Hip fire and ADS spread
- Reload Speed: Time to reload
- Magazine Size: Ammo capacity
- Critical Hit Chance: Base crit %
- Critical Hit Damage: Crit multiplier

**Rollable Offensive Attributes:**
- Weapon Damage: +3-100% weapon damage (scales with rarity)
- Critical Damage: +5-200% critical hit damage
- Critical Chance: +2-50% critical hit chance
- Fire Rate: +5-75% rate of fire
- Projectile Speed: +10-100% bullet velocity
- Multishot: +1-5 projectiles per shot
- Penetration: Pierce 1-10 enemies
- Elemental Damage: +5-150% elemental effect damage
- Elemental Chance: +5-100% status effect chance
- Elemental Duration: +0.5-5 seconds to DoT effects
- Splash Radius: +10-200% explosion radius
- Melee Damage: +10-300% melee damage with weapon

**Rollable Defensive Attributes:**
- Lifesteal: 1-10% damage converted to health
- Shield Steal: 1-10% damage converted to shields
- Damage Reduction: 2-25% less damage while wielding
- Movement Speed: +3-30% speed while holding
- Health Regen: +1-20 HP/second while equipped

**Rollable Utility Attributes:**
- Reload Speed: +5-100% faster reload
- Magazine Size: +10-200% larger magazine
- Ammo Regeneration: +1-10 ammo/second
- Weapon Swap Speed: +10-100% faster switching
- Zoom: +1x-12x magnification bonus
- Accuracy: +5-95% reduced spread
- Recoil: -10-90% recoil reduction
- Handling: +5-75% all weapon handling stats
- Charge Speed: +10-100% faster charge (energy weapons)

**Special Attributes (Epic+ Only):**
- Kill Skills: On kill, +10-100% damage for 3-10 seconds
- Stacking Damage: Each hit adds 1-5% damage (max 10-100 stacks)
- Ricochet: 10-100% chance bullets bounce to nearest enemy
- Chain Lightning: Shock chains to 1-10 enemies
- Explosive Rounds: 5-50% chance for explosion on hit
- Vampiric: Kills heal for 5-50% max health
- Ammo Return: 5-50% chance not to consume ammo
- Double Tap: 5-50% chance to shoot twice
- Evolving: Weapon improves every 10-100 kills
- Momentum: Movement speed increases damage

**Legendary Unique Effects (One per legendary+ weapon):**
- "Bottomless": Consumes no ammo for 5 seconds after reload
- "Lucky Seven": Every 7th shot is guaranteed critical
- "Crescendo": Damage increases the longer you fire
- "Tediore Special": Reloading throws weapon as grenade
- "Phase Walker": Shoots through walls and cover
- "Chain Reaction": Enemies killed explode into elemental nova
- "Infinity": Never needs reloading but lower damage
- "Conference Call": Projectiles spawn more projectiles
- "The Bee": Full shield grants massive amp damage

### Shield Attributes

**Base Stats (All Shields Have):**
- Capacity: Shield health pool
- Recharge Rate: Shield/second when recharging  
- Recharge Delay: Seconds before recharge starts

**Rollable Defensive Attributes:**
- Shield Capacity: +5-200% shield capacity
- Recharge Rate: +5-300% shield/second
- Recharge Delay: -10-90% delay reduction
- Max Health: +5-100% maximum health
- Damage Reduction: 2-30% less damage taken
- Elemental Resistance: 10-90% less elemental damage
- Immunity Phases: 0.5-3 second immunity on break
- Damage Gate: Prevents one-shots above X% health

**Rollable Offensive Attributes:**
- Nova Damage: 100-10000 damage nova on break
- Nova Element: Random elemental nova type
- Spike Damage: Reflects 10-200% melee damage
- Amp Damage: Next shot +25-500% damage at full shield
- Revenge Damage: +5-100% damage when shield is down
- Roid Damage: +50-1000% melee at zero shield

**Rollable Utility Attributes:**
- Movement Speed: +3-30% speed with shields up
- Booster Chance: 5-50% to drop health/ammo boosters
- Absorb Chance: 10-50% to absorb bullets as ammo
- Fleet: +5-50% speed when depleted
- Turtle: +50-400% capacity but -10-50% speed
- Safe Space: +2-10m pickup radius
- Quick Charge: -25-90% delay after kill

**Special Shield Types (Rare+):**
- Adaptive: Gains 20-50% resistance to last damage type
- Amplify: All shots do bonus damage when full
- Booster: Constantly drops boosters when damaged
- Nova: Multiple nova effects on break
- Spike: Damages all melee attackers
- Absorb: Converts some bullets to ammo
- Roid: Massive melee bonus when depleted
- Turtle: Huge capacity, speed penalty

### Grenade Mod Attributes

**Base Stats:**
- Damage: Explosion damage
- Blast Radius: Area of effect
- Fuse Time: Detonation delay

**Rollable Attributes:**
- Grenade Damage: +10-500% damage
- Blast Radius: +10-200% radius
- Fuse Time: -10-90% before detonation
- Grenade Count: +1-5 grenade capacity
- Grenade Regeneration: 1 per 10-120 seconds
- Force: +10-500% knockback
- Elemental Damage: +20-300% element damage

**Delivery Methods (Determines behavior):**
- Standard: Normal throw arc
- Longbow: Teleports to location
- Sticky: Adheres to surfaces
- Homing: Seeks enemies
- Bouncing: Bounces before exploding
- Lobbed: High arc throw
- Fastball: Straight line throw

**Special Effects (Can stack):**
- MIRV: Splits into 3-10 child grenades
- Singularity: Pulls enemies for 1-5 seconds
- Tesla: Creates electric field for 3-10 seconds
- Transfusion: Returns 10-50% damage as health
- Generator: Creates elemental area effect
- Rain: Spawns 3-20 grenades from above
- Divider: Splits on each bounce
- Link: Chains between enemies

### Class Mod Attributes

**Skill Bonuses (Primary):**
- +1-5 to specific skills (can exceed cap)
- +5-50% skill effectiveness
- +5-50% action skill cooldown rate
- +5-50% action skill duration
- +1-3 additional skill charges

**Combat Bonuses:**
- Weapon Damage: +5-100% specific weapon type
- Manufacturer Bonus: +10-100% manufacturer weapon stats
- Elemental Bonus: +10-200% specific element
- Critical Damage: +10-300% critical damage
- Fire Rate: +5-75% rate of fire
- Reload Speed: +10-100% reload speed
- Accuracy: +10-90% accuracy
- Melee Damage: +25-500% melee damage

**Defensive Bonuses:**
- Max Health: +10-100% health
- Shield Capacity: +10-100% shields
- Health Regen: +0.5-5% health/second
- Damage Reduction: 3-30% less damage
- Resistance: 10-50% to specific element

**Utility Bonuses:**
- Movement Speed: +5-30% speed
- Experience: +5-50% XP gain
- Luck: +5-100% rare drop chance
- Ammo Capacity: +20-100% max ammo
- Grenade Capacity: +1-5 grenades

**Anointments (Epic+ Class Mods):**

**Universal Anointments:**
- "On Action Skill End: +100% weapon damage for 5 seconds"
- "While Action Skill Active: +50% fire rate and reload speed"
- "After Action Skill: Next 2 magazines have +125% crit damage"
- "On Action Skill Start: Regenerate 5% ammo per second"
- "Consecutive Hits: +1% damage per hit, max 100 stacks"

**Class-Specific Examples:**
- Technomancer: "Storm Protocol creates 3 storms"
- Vanguard: "Artillery Mode has unlimited ammo"
- Infiltrator: "Cloak grants 100% crit chance"
- Engineer: "Deployables are invincible for 5 seconds"

### Relic Attributes

**Primary Stats (Choose One):**
- Max Health: +10-100% health
- Shield Capacity: +10-100% shields
- Movement Speed: +5-30% speed
- Combat Rating: +5-50% all damage

**Secondary Attributes (2-6 based on rarity):**
- Action Skill Cooldown: -5-50% cooldown
- Elemental Damage: +10-200% DoT damage
- Elemental Resistance: -10-90% elemental damage taken
- Elemental Chance: +5-100% proc rate
- Melee Damage: +25-500% melee
- Grenade Damage: +25-300% grenade damage
- Slam Damage: +50-1000% ground pound
- Slide Damage: +25-500% slide damage
- Vehicle Damage: +25-200% vehicle weapons

**Farming Bonuses:**
- Luck: +5-100% rare loot chance
- Experience: +5-50% XP from all sources
- Currency: +10-200% money drops
- Ammo Drops: +25-100% ammo find
- Health Drops: +25-100% health pickups
- Eridium Find: +10-500% special currency

**Special Effects (Epic+):**
- "Last Stand": Invincible for 5 seconds when health drops below 30% (120s cooldown)
- "Snowball": Freezing an enemy freezes all nearby enemies
- "Elemental Projector": +90% elemental damage when suffering from that element
- "Loaded Dice": -75% health but substantially increased luck
- "Otto Idol": Kills restore 10% health
- "Victory Rush": +30% damage and movement after kill for 60 seconds

### Attribute Value Ranges by Rarity

**Common:**
- Lowest values (bottom 20% of range)
- 1-2 attributes only
- No special effects

**Uncommon:**
- Low values (20-35% of range)
- 2-3 attributes
- Basic effects only

**Rare:**
- Medium values (35-50% of range)
- 3-4 attributes
- Can have one special effect

**Epic:**
- Good values (50-65% of range)
- 4-5 attributes
- Can have multiple special effects

**Legendary:**
- High values (65-80% of range)
- 5-6 attributes
- Guaranteed unique effect
- Can have anointments

**Pearlescent:**
- Very high values (80-95% of range)
- 6-7 attributes
- Multiple unique effects
- Perfect anointments

**Apocalypse:**
- Maximum values (95-100% of range)
- 7-8 attributes
- Game-breaking unique effects
- Multiple perfect anointments

---

**Common (1-2 total attributes):**
- Max Health: +20-50
- Movement Speed When Depleted: +5-10%

**Uncommon (2-3 total attributes):**
- Elemental Resistance: -10-20% from one element
- Melee Damage When Full: +15-30%
- Sprint Speed: +5-10%

**Rare (3-4 total attributes):**
All previous pool plus:
- Nova Damage on Depletion: 50-200 damage
- Reflect Chance: 5-15% projectile reflection
- Health Regeneration: +2-5/sec when shields up

**Epic (4-5 total attributes):**
All previous pool plus:
- Adaptive Resistance: -20-40% from last damage type
- Booster Drop Chance: 5-15% on damage
- Team Shield Share: 10-25% to nearby allies
- Instant Recharge on Kill: 10-30% chance

**Legendary (5-6 total attributes):**
All previous pool plus:
- Multiple Nova Types: 2-3 different elements
- Damage Absorption: 5-15% damage charges shield
- Temporary Invincibility: 0.5-1.5s on depletion
- Shield Leech: Damage dealt recharges shields

**Pearlescent (6-7 total attributes):**
All previous pool plus:
- Quantum Shield: Phases through some attacks
- Retaliation: Stored damage releases as explosion
- Evolution: Shield improves with damage taken
- Set Bonus: Works with other pearlescent gear

**Apocalypse (7-8 total attributes):**
All previous pool plus:
- Infinite Capacity: Never fully depletes
- Time Rewind: Reverses last 3s of damage
- Reality Shield: Exists in multiple dimensions
- God Mode: 90% damage reduction at low health

### Grenade Mod Attributes

**Core Attributes:**
- Damage: Base explosion damage
- Blast Radius: Area of effect
- Fuse Time: Delay before explosion

**Additional Attributes by Rarity:**

**Common-Uncommon (1-3 attributes):**
- Bounce Count: 0-3 bounces
- Element Type: Single element
- Throw Speed: +10-30%

**Rare-Epic (3-5 attributes):**
All previous plus:
- MIRV Count: Splits into 2-6 grenades
- Homing: Seeks enemies
- Transfusion: Returns 5-20% damage as health
- Sticky: Adheres to surfaces/enemies

**Legendary-Pearlescent (5-7 attributes):**
All previous plus:
- Singularity: Pulls enemies before explosion
- Rain: Creates multiple explosions over time
- Tesla: Continuous electric damage field
- Longbow: Teleports to target location
- Multiple Elements: 2-3 simultaneous

**Apocalypse (7-9 attributes):**
All previous plus:
- Infinite Grenades: Never depletes
- Nuclear: Massive area devastation
- Black Hole: Creates persistent singularity
- Timeline Bomb: Explodes across multiple timelines

### Class Mod Attributes

**Core Attributes:**
- Skill Bonuses: +1-5 to specific skills
- Primary Stat: Based on class (Damage/Health/Speed/Cooldown)

**Additional Attributes by Rarity:**

**Common-Uncommon (1-2 attributes):**
- Health: +5-15%
- Shield Capacity: +5-15%
- Action Skill Cooldown: -5-10%

**Rare-Epic (2-4 attributes):**
All previous plus:
- Weapon Type Damage: +10-25% for specific type
- Elemental Damage: +10-30%
- Critical Hit Damage: +15-40%
- Team Bonus: Small buff to allies

**Legendary-Pearlescent (4-6 attributes):**
All previous plus:
- Unique Class Effect: Changes ability behavior
- Multi-Skill Bonus: Affects 3-5 skills
- Anointment Effect: Special on-action trigger
- Evolution: Improves during combat

**Apocalypse (6-8 attributes):**
All previous plus:
- Break Skill Caps: Skills exceed maximum
- Dual Action Skills: Use two at once
- Permanent Buffs: Effects never expire
- Class Transformation: Fundamental gameplay change

### Relic Attributes

**Types of Relics:**
- Combat: Damage and offensive stats
- Defense: Health and defensive stats
- Utility: Movement and resource stats
- Legendary: Unique game-changing effects

**Attribute Pools by Type:**

**Combat Relics:**
- All Weapon Damage: +5-30%
- Fire Rate: +5-25%
- Magazine Size: +10-50%
- Critical Damage: +10-50%
- Elemental Chance: +5-30%
- Melee Damage: +20-100%

**Defense Relics:**
- Maximum Health: +10-50%
- Shield Capacity: +10-50%
- Health Regeneration: +5-20/sec
- Damage Reduction: +2-15%
- Second Wind Duration: +2-10s
- Immunity Duration: +0.5-2s

**Utility Relics:**
- Movement Speed: +5-25%
- Experience Gain: +5-30%
- Luck (Better Drops): +5-50%
- Ammo Capacity: +20-100%
- Cooldown Rate: -10-40%
- Vehicle Damage: +20-100%

**Legendary Relic Effects (Epic+ only):**
- Blood of the Ancients: +Health and +Ammo
- Sheriff's Badge: Pistol fire rate and damage
- Bone of the Ancients: Elemental damage and cooldown
- Stockpile: Massive ammo increase
- Vitality: Massive health increase

**Apocalypse Relic Effects:**
- Infinity Stone: One random attribute becomes infinite
- Phoenix: Resurrect on death with explosion
- Chronos: Control time flow
- Pandora's Box: Random legendary effect every 30s

### Attribute Value Scaling

**Level Scaling:**
- Attributes scale with item level
- Level 50 items have ~2x the values of Level 1

**Rarity Multipliers:**
- Common: 1.0x
- Uncommon: 1.2x
- Rare: 1.5x
- Epic: 2.0x
- Legendary: 3.0x
- Pearlescent: 4.0x
- Apocalypse: 5.0x

**Perfect Parts Bonus:**
If all parts on a weapon are the same rarity:
- +10% to all attributes
- Special visual effect
- Unique name prefix

### Attribute Synergies

Certain attribute combinations unlock hidden bonuses:
- Fire Rate + Magazine Size = Bottomless Clip chance
- All Elemental Types = Rainbow damage
- Max Health + Health Regen = Immortal prefix
- Movement Speed + Reload Speed = Ninja prefix
- Critical Chance + Critical Damage = Assassin prefix

These synergies are discovered through experimentation and add another layer to build crafting.

---

## Vehicle System

### Vehicle Types

**Strider (Single-Seat)**
- High speed and maneuverability
- Light armor plating
- Front-mounted weapon system
- Boost capability
- Jump jets for vertical navigation
- Ideal for solo exploration and hit-and-run tactics
- Can perform barrel rolls for damage avoidance

**Carrier (Multi-Seat)**
- 4 seats total (driver + 3 gunners)
- Heavy armor plating
- Multiple weapon hardpoints
- Ram damage bonus
- Mobile spawn point for downed allies
- Deployable cover on sides
- Team-focused design with shared shields

### Motor Pool System
- **Motor Pool Stations**: Digital vehicle construction terminals
- Located at major outposts and discovered locations
- Each player can summon their own vehicle in co-op
- 30-second cooldown between summons
- Vehicles persist until destroyed or replaced
- Fast travel available between discovered Motor Pools
- Holographic preview before spawning

### Vehicle Customization

**Paint Colors:**
- Primary Color: Main body color
- Secondary Color: Accent/detail color
- 50+ colors available including:
  - Standard colors (red, blue, green, etc.)
  - Metallic finishes
  - Matte options
  - Special unlocks (chrome, gold, etc.)
- Colors unlocked through:
  - Story progression
  - Hidden collectibles
  - Achievement rewards
  - Vendor purchases

### Vehicle Combat
- Separate vehicle health pool
- Critical hit locations (engines, weapons)
- Vehicle-specific experience gains
- Repair stations at outposts
- Vehicle-only boss encounters
- Destruction ejects players (no death)

---

## Enemy Design

[Previous enemy section remains the same]

---

## Level Design

### Open World Structure
- Seamless vehicle transitions
- Roads connecting major areas
- Vehicle-only paths for secrets
- Ramps and jumps for traversal
- Destructible barriers

[Rest remains the same]

---

## Progression Systems

### Character Progression

**Level System:**
- Max Level: 50
- XP from kills, missions, discoveries
- 2 skill points per level
- Stat increases every 5 levels

**Ascension System (Post-50):**
- Account-wide progression
- Small permanent bonuses
- Infinite progression
- Categories: Combat, Defense, Utility, Loot
- Shared across all characters
- 1 Ascension point per level after 50

**Ascension Trees:**

**Combat:**
- Weapon Damage +0.5% per point
- Fire Rate +0.5% per point
- Critical Damage +1% per point
- Elemental Effect Chance +0.5% per point
- Action Skill Damage +1% per point

**Defense:**
- Max Health +1% per point
- Shield Capacity +1% per point
- Health Regeneration +0.5/s per point
- Damage Reduction +0.2% per point
- Second Wind Time +0.5s per point

**Utility:**
- Movement Speed +0.5% per point
- Reload Speed +0.5% per point
- Action Skill Cooldown -0.5% per point
- Vehicle Health +2% per point
- Revival Speed +2% per point

**Loot:**
- Luck +0.5% per point
- Rare Drop Chance +0.3% per point
- Currency Gain +1% per point
- Ammo Capacity +1% per point
- Experience Gain +0.5% per point

---

## Endgame Content

### Mayhem System
**Mayhem Levels 1-10:**
- Enemy health: +50% per level
- Enemy damage: +30% per level
- Loot quality: +20% per level
- XP gain: +25% per level
- Exclusive legendary drops at Mayhem 6+

### Protocol Raids (4-Player Content)

**Raid 1: NEXUS CORE**
- Linear progression through facility
- 5 boss encounters
- Puzzle elements between combat
- Vehicle section halfway through
- Exclusive "CORE" weapon set
- Hard mode unlocked after completion

**Raid 2: Temporal Paradox**
- Non-linear time mechanics
- Past affects future sections
- 4 timeline variants to manage
- Paradox weapons that exist in multiple times
- Challenge mode with permadeath

**Raid 3: The Singularity**
- Inside NEXUS consciousness
- Reality constantly shifting
- Platform/puzzle heavy
- Final boss has 10 phases
- Drops Apocalypse tier items

### Infinite Dungeon: The Spiral
- Procedurally generated
- Difficulty increases each floor
- Checkpoint every 5 floors
- Leaderboards
- Floor 100 guaranteed Apocalypse drop

### Proving Grounds
- 6 different arenas
- Time trial focused
- Optional objectives
- Daily rotation
- Exclusive cosmetics

### Circle of Slaughter
- Wave-based survival
- 5 rounds, 5 waves each
- Boss every 5 waves
- Scales to player count
- High density loot

---

## Multiplayer & Co-op

### Network Architecture
- Steam P2P Integration
- Host-based sessions
- No dedicated servers
- Steam friends integration
- Join via Steam invite

### Cooperative Features
- 1-4 Players all content
- Drop-in/drop-out
- Level scaling options
- Instanced loot
- Trading enabled
- Duel for loot option

---

## Technical Requirements

[Previous requirements remain the same, with addition:]

### Third-Person Specific
- Character LOD system
- Enhanced animation system
- Cloth/physics for gear
- Camera collision system
- Cover detection system

---

## UI/UX Design

### Third-Person HUD
- Health/Shield (bottom center)
- Abilities (bottom center)
- Ammo (bottom right)
- Minimap (top right)
- Weapon info (top right)
- Character visible at all times

### Inventory Screen
- Character model display
- Rotate character view
- See equipped items on model
- Preview before equipping
- Color customization unlocks

---

## Audio Design

[Remains the same with addition:]

### Third-Person Audio
- Footstep variations
- Clothing/armor sounds
- Weapon handling sounds
- Cover interaction audio
- Vehicle engine variety

---

## Development Priorities

### Phase 1: Core Systems (Months 1-6)
- Third-person camera and controls
- Combat mechanics
- Basic enemy AI
- Weapon generation system
- PSX shader implementation

### Phase 2: Content Creation (Months 7-12)
- 5 zones with vehicle paths
- Story missions
- Skill tree implementation
- Vehicle system
- P2P multiplayer

### Phase 3: Polish & Endgame (Months 13-18)
- Raids and dungeons
- Balance passes
- Performance optimization
- Bug fixing
- Vehicle physics polish

### Phase 4: Launch Preparation (Months 19-24)
- Marketing campaign
- Demo release
- Community building
- Day 1 patch
- Launch trailer

---

## Conclusion

NEXUS PROTOCOL delivers a complete third-person looter shooter experience with deep customization, extensive weapon variety through manufacturer identity, and satisfying vehicle combat. The game respects player investment by shipping complete at launch without live service elements, while providing hundreds of hours of content through build experimentation and endgame challenges.

---

**Document Version:** 3.0
**Last Updated:** November 24, 2024

END OF DOCUMENT