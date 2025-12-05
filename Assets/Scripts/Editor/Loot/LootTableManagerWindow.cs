using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using NexusProtocol.Loot;
using NexusProtocol.Weapons;

namespace NexusProtocol.Editor
{
    /// <summary>
    /// Loot Table Manager - Editor tool for configuring drop rates and loot pools
    ///
    /// LOOT SYSTEM DESIGN:
    /// - Generic weapons (Common-Epic): Generated at RUNTIME by DOTS/ECS - NOT in loot tables
    /// - Unique weapons (Legendary+): Defined via UniqueWeaponDefinition, managed in loot tables
    /// - Equipment (all rarities): Managed via loot tables
    /// - Consumables/Cosmetics: Managed via loot tables
    ///
    /// Features per Editor_Tools_Development_Prompt.md:
    /// - Enemy-specific loot tables
    /// - Rarity weight configuration
    /// - Manufacturer bias settings
    /// - Level-based scaling
    /// - Quest reward pools
    /// - Chest tier configuration
    /// - World drop rates
    /// - UNIQUE WEAPON MANAGEMENT (Legendary/Pearlescent/Apocalypse)
    /// </summary>
    public class LootTableManagerWindow : EditorWindow
    {
        private const string TOOL_VERSION = "1.1";
        private const string TOOL_NAME = "Loot Table Manager";
        private const string DATABASE_PATH = "Assets/ScriptableObjects/LootTables/LootTableDatabase.asset";
        private const string TABLES_FOLDER = "Assets/ScriptableObjects/LootTables/Tables";
        private const string UNIQUE_WEAPONS_FOLDER = "Assets/ScriptableObjects/LootTables/UniqueWeapons";

        private enum Tab
        {
            Overview,
            UniqueWeapons,
            TableEditor,
            EnemyTables,
            ChestTiers,
            QuestRewards,
            WorldDrops,
            RarityConfig,
            Simulation
        }

        private Tab _currentTab = Tab.Overview;
        private Vector2 _scrollPosition;
        private Vector2 _tableListScrollPosition;
        private Vector2 _entryListScrollPosition;

        // Data
        private LootTableDatabase _database;
        private LootTable _selectedTable;
        private SerializedObject _selectedTableSO;
        private LootTableEntry _selectedEntry;
        private int _selectedEntryIndex = -1;

        // Filters
        private string _searchFilter = "";
        private LootSourceType? _sourceFilter = null;
        private GameZone? _zoneFilter = null;

        // Simulation settings
        private int _simulationCount = 1000;
        private int _simulationPlayerLevel = 30;
        private float _simulationLuck = 0f;
        private MayhemLevel _simulationMayhem = MayhemLevel.None;
        private Dictionary<WeaponRarity, int> _simulationRarityResults;
        private Dictionary<LootItemType, int> _simulationTypeResults;
        private Dictionary<WeaponManufacturer, int> _simulationManufacturerResults;

        // Unique weapon management
        private List<UniqueWeaponDefinition> _uniqueWeapons = new List<UniqueWeaponDefinition>();
        private UniqueWeaponDefinition _selectedUniqueWeapon;
        private SerializedObject _selectedUniqueWeaponSO;
        private Vector2 _uniqueWeaponListScrollPosition;
        private string _uniqueWeaponSearchFilter = "";

        // Rarity colors (from GDD)
        private static readonly Dictionary<WeaponRarity, Color> RarityColors = new Dictionary<WeaponRarity, Color>
        {
            { WeaponRarity.Common, new Color(0.7f, 0.7f, 0.7f) },
            { WeaponRarity.Uncommon, new Color(0.2f, 0.8f, 0.2f) },
            { WeaponRarity.Rare, new Color(0.3f, 0.5f, 1.0f) },
            { WeaponRarity.Epic, new Color(0.6f, 0.2f, 0.8f) },
            { WeaponRarity.Legendary, new Color(1.0f, 0.6f, 0.0f) },
            { WeaponRarity.Pearlescent, new Color(0.0f, 0.9f, 0.9f) },
            { WeaponRarity.Apocalypse, new Color(0.9f, 0.1f, 0.1f) }
        };

        [MenuItem("Nexus/Loot/Loot Table Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<LootTableManagerWindow>("Loot Table Manager");
            window.minSize = new Vector2(900, 650);
        }

        private void OnEnable()
        {
            LoadDatabase();
            LoadUniqueWeapons();
        }

        private void LoadDatabase()
        {
            _database = AssetDatabase.LoadAssetAtPath<LootTableDatabase>(DATABASE_PATH);

            if (_database == null)
            {
                EnsureFolderExists(Path.GetDirectoryName(DATABASE_PATH));
                _database = CreateInstance<LootTableDatabase>();
                AssetDatabase.CreateAsset(_database, DATABASE_PATH);
                AssetDatabase.SaveAssets();
                Debug.Log($"[{TOOL_NAME}] Created new loot table database at {DATABASE_PATH}");
            }

            _database.InitializeCache();
        }

        private void LoadUniqueWeapons()
        {
            _uniqueWeapons.Clear();

            string[] guids = AssetDatabase.FindAssets("t:UniqueWeaponDefinition");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var weapon = AssetDatabase.LoadAssetAtPath<UniqueWeaponDefinition>(path);
                if (weapon != null)
                {
                    _uniqueWeapons.Add(weapon);
                }
            }

            Debug.Log($"[{TOOL_NAME}] Loaded {_uniqueWeapons.Count} unique weapons");
        }

        private void EnsureFolderExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = Path.GetDirectoryName(path);
                string folder = Path.GetFileName(path);

                if (!AssetDatabase.IsValidFolder(parent))
                {
                    EnsureFolderExists(parent);
                }

                AssetDatabase.CreateFolder(parent, folder);
            }
        }

        private void OnGUI()
        {
            // Header
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField($"{TOOL_NAME} v{TOOL_VERSION}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            if (_database != null)
            {
                EditorGUILayout.LabelField($"Tables: {_database.allTables.Count}", GUILayout.Width(80));
            }
            EditorGUILayout.EndHorizontal();

            // Tab bar
            EditorGUILayout.BeginHorizontal();
            foreach (Tab tab in Enum.GetValues(typeof(Tab)))
            {
                if (GUILayout.Toggle(_currentTab == tab, tab.ToString(), EditorStyles.toolbarButton))
                {
                    _currentTab = tab;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Content
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            switch (_currentTab)
            {
                case Tab.Overview:
                    DrawOverviewTab();
                    break;
                case Tab.UniqueWeapons:
                    DrawUniqueWeaponsTab();
                    break;
                case Tab.TableEditor:
                    DrawTableEditorTab();
                    break;
                case Tab.EnemyTables:
                    DrawEnemyTablesTab();
                    break;
                case Tab.ChestTiers:
                    DrawChestTiersTab();
                    break;
                case Tab.QuestRewards:
                    DrawQuestRewardsTab();
                    break;
                case Tab.WorldDrops:
                    DrawWorldDropsTab();
                    break;
                case Tab.RarityConfig:
                    DrawRarityConfigTab();
                    break;
                case Tab.Simulation:
                    DrawSimulationTab();
                    break;
            }

            EditorGUILayout.EndScrollView();
        }

        #region Overview Tab

        private void DrawOverviewTab()
        {
            // Loot system design explanation
            EditorGUILayout.HelpBox(
                "LOOT SYSTEM DESIGN:\n\n" +
                "WEAPONS:\n" +
                "  - Common/Uncommon/Rare/Epic: Generated at RUNTIME by DOTS/ECS (NOT in loot tables)\n" +
                "  - Legendary/Pearlescent/Apocalypse: UNIQUE named weapons defined in loot tables\n" +
                "    (Fixed unique parts + randomized other parts via ECS)\n\n" +
                "NON-WEAPONS:\n" +
                "  - Equipment (Shields, Grenades, etc.): All rarities via loot tables\n" +
                "  - Consumables & Cosmetics: Via loot tables\n\n" +
                "Use the 'Unique Weapons' tab to create named legendary+ weapons with unique effects.",
                MessageType.Info);

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Features:\n" +
                "- UNIQUE WEAPON MANAGEMENT (Legendary/Pearlescent/Apocalypse)\n" +
                "- Enemy-specific loot tables\n" +
                "- Rarity weight configuration (7 tiers)\n" +
                "- Manufacturer bias settings (7 manufacturers)\n" +
                "- Level-based scaling (1-50)\n" +
                "- Quest reward pools\n" +
                "- Chest tier configuration (White to Red)\n" +
                "- World drop rates by zone\n" +
                "- Drop simulation testing",
                MessageType.None);

            EditorGUILayout.Space();

            // Quick Actions
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create New Table", GUILayout.Height(40)))
            {
                CreateNewTable();
            }

            if (GUILayout.Button("Generate Default Tables", GUILayout.Height(40)))
            {
                GenerateDefaultTables();
            }

            if (GUILayout.Button("Refresh Database", GUILayout.Height(40)))
            {
                RefreshDatabase();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Stats
            if (_database != null)
            {
                var stats = _database.GetStatistics();

                EditorGUILayout.LabelField("Database Overview", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.LabelField($"Total Tables: {stats.TotalTables}");
                EditorGUILayout.LabelField($"Total Entries: {stats.TotalEntries}");
                EditorGUILayout.LabelField($"Total Weight: {stats.TotalWeight:F1}");

                EditorGUILayout.Space();

                // Tables by source category
                EditorGUILayout.LabelField("Tables by Source Category:");
                int enemyTables = stats.TablesBySource
                    .Where(kvp => kvp.Key.ToString().StartsWith("Enemy_"))
                    .Sum(kvp => kvp.Value);
                int chestTables = stats.TablesBySource
                    .Where(kvp => kvp.Key.ToString().StartsWith("Chest_"))
                    .Sum(kvp => kvp.Value);
                int questTables = stats.TablesBySource
                    .Where(kvp => kvp.Key.ToString().StartsWith("Quest_"))
                    .Sum(kvp => kvp.Value);
                int worldTables = stats.TablesBySource
                    .Where(kvp => kvp.Key.ToString().StartsWith("World_"))
                    .Sum(kvp => kvp.Value);

                EditorGUILayout.LabelField($"  Enemy Tables: {enemyTables}");
                EditorGUILayout.LabelField($"  Chest Tables: {chestTables}");
                EditorGUILayout.LabelField($"  Quest Tables: {questTables}");
                EditorGUILayout.LabelField($"  World Tables: {worldTables}");

                EditorGUILayout.EndVertical();

                // Unique weapons summary
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Unique Weapons (Legendary+)", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                int legendaryCount = _uniqueWeapons.Count(w => w.rarity == WeaponRarity.Legendary);
                int pearlescentCount = _uniqueWeapons.Count(w => w.rarity == WeaponRarity.Pearlescent);
                int apocalypseCount = _uniqueWeapons.Count(w => w.rarity == WeaponRarity.Apocalypse);

                GUI.color = RarityColors[WeaponRarity.Legendary];
                EditorGUILayout.LabelField($"  Legendary: {legendaryCount}");
                GUI.color = RarityColors[WeaponRarity.Pearlescent];
                EditorGUILayout.LabelField($"  Pearlescent: {pearlescentCount}");
                GUI.color = RarityColors[WeaponRarity.Apocalypse];
                EditorGUILayout.LabelField($"  Apocalypse: {apocalypseCount}");
                GUI.color = Color.white;
                EditorGUILayout.LabelField($"  Total Unique Weapons: {_uniqueWeapons.Count}");

                EditorGUILayout.EndVertical();

                // Validation
                EditorGUILayout.Space();
                if (GUILayout.Button("Validate All Tables"))
                {
                    var errors = _database.ValidateAll();
                    if (errors.Count == 0)
                    {
                        EditorUtility.DisplayDialog("Validation", "All tables are valid!", "OK");
                    }
                    else
                    {
                        string errorMsg = string.Join("\n", errors.Select(e =>
                            $"{e.table?.tableName ?? "NULL"}: {string.Join(", ", e.errors)}"));
                        EditorUtility.DisplayDialog("Validation Errors", errorMsg, "OK");
                    }
                }
            }
        }

        #endregion

        #region Unique Weapons Tab

        private void DrawUniqueWeaponsTab()
        {
            EditorGUILayout.LabelField("Unique Weapon Management (Legendary+)", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Create and manage unique named weapons (Legendary, Pearlescent, Apocalypse).\n" +
                "Each unique weapon has:\n" +
                "- Fixed parts that define its identity (at least the receiver)\n" +
                "- A unique effect/perk\n" +
                "- Dedicated drop sources (bosses, chests)\n\n" +
                "Non-fixed parts are randomized by the ECS system when the weapon drops.",
                MessageType.Info);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            // Left panel - Unique weapon list
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
            DrawUniqueWeaponList();
            EditorGUILayout.EndVertical();

            // Right panel - Weapon editor
            EditorGUILayout.BeginVertical();
            DrawUniqueWeaponEditor();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawUniqueWeaponList()
        {
            EditorGUILayout.LabelField("Unique Weapons", EditorStyles.boldLabel);

            // Search
            EditorGUILayout.BeginHorizontal();
            _uniqueWeaponSearchFilter = EditorGUILayout.TextField(_uniqueWeaponSearchFilter, EditorStyles.toolbarSearchField);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _uniqueWeaponSearchFilter = "";
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Weapon list
            _uniqueWeaponListScrollPosition = EditorGUILayout.BeginScrollView(_uniqueWeaponListScrollPosition, GUILayout.Height(400));

            var filteredWeapons = _uniqueWeapons
                .Where(w => string.IsNullOrEmpty(_uniqueWeaponSearchFilter) ||
                            w.weaponName.ToLower().Contains(_uniqueWeaponSearchFilter.ToLower()))
                .OrderBy(w => w.rarity)
                .ThenBy(w => w.weaponName);

            foreach (var weapon in filteredWeapons)
            {
                GUI.color = weapon == _selectedUniqueWeapon ? Color.cyan : weapon.GetRarityColor();

                string label = $"[{weapon.rarity.ToString().Substring(0, 1)}] {weapon.weaponName}";
                if (GUILayout.Button(label, EditorStyles.miniButton))
                {
                    _selectedUniqueWeapon = weapon;
                    _selectedUniqueWeaponSO = new SerializedObject(weapon);
                }
            }

            GUI.color = Color.white;

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            if (GUILayout.Button("+ Create New Unique Weapon", GUILayout.Height(30)))
            {
                CreateNewUniqueWeapon();
            }

            if (GUILayout.Button("Refresh List"))
            {
                LoadUniqueWeapons();
            }
        }

        private void DrawUniqueWeaponEditor()
        {
            if (_selectedUniqueWeapon == null)
            {
                EditorGUILayout.HelpBox("Select a unique weapon from the list to edit.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Editing: {_selectedUniqueWeapon.weaponName}", EditorStyles.boldLabel);

            if (_selectedUniqueWeaponSO != null)
            {
                _selectedUniqueWeaponSO.Update();

                // Identity
                EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("weaponId"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("weaponName"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("flavorText"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("loreText"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("rarity"));

                EditorGUILayout.Space();

                // Fixed Parts
                EditorGUILayout.LabelField("Fixed Parts (Define Weapon Identity)", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("Fixed parts are guaranteed on this weapon. At minimum, set the receiver.", MessageType.None);
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("fixedReceiver"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("fixedBarrel"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("fixedMagazine"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("fixedGrip"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("fixedStock"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("fixedSight"));

                // Show part configuration summary
                var config = _selectedUniqueWeapon.GetPartConfiguration();
                EditorGUILayout.LabelField($"Fixed: {config.FixedCount}/6 parts, Randomized: {config.RandomCount}/6 parts");

                EditorGUILayout.Space();

                // Randomization settings
                EditorGUILayout.LabelField("Randomization Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("minRandomPartRarity"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("maxRandomPartRarity"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("preferredManufacturers"), true);

                EditorGUILayout.Space();

                // Unique effect
                EditorGUILayout.LabelField("Unique Effect", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("uniqueEffectName"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("uniqueEffectDescription"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("uniqueEffectPrefab"));

                EditorGUILayout.Space();

                // Drop settings
                EditorGUILayout.LabelField("Drop Configuration", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("baseDropWeight"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("minimumLevel"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("worldDropEnabled"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("dedicatedSources"), true);
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("minimumMayhemLevel"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("mayhemExclusive"));

                EditorGUILayout.Space();

                // Visual
                EditorGUILayout.LabelField("Visual", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("icon"));
                EditorGUILayout.PropertyField(_selectedUniqueWeaponSO.FindProperty("uniqueMaterial"));

                _selectedUniqueWeaponSO.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();

            // Validation
            if (_selectedUniqueWeapon.Validate(out var errors))
            {
                EditorGUILayout.HelpBox("Weapon definition is valid.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox($"Validation errors:\n{string.Join("\n", errors)}", MessageType.Error);
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(_selectedUniqueWeapon);
                AssetDatabase.SaveAssets();
                Debug.Log($"[{TOOL_NAME}] Saved {_selectedUniqueWeapon.weaponName}");
            }

            GUI.color = Color.red;
            if (GUILayout.Button("Delete"))
            {
                if (EditorUtility.DisplayDialog("Delete Unique Weapon",
                    $"Delete '{_selectedUniqueWeapon.weaponName}'?", "Delete", "Cancel"))
                {
                    DeleteUniqueWeapon(_selectedUniqueWeapon);
                }
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private void CreateNewUniqueWeapon()
        {
            EnsureFolderExists(UNIQUE_WEAPONS_FOLDER);

            var weapon = CreateInstance<UniqueWeaponDefinition>();
            weapon.weaponName = "New Unique Weapon";
            weapon.weaponId = $"unique_{DateTime.Now.Ticks}";
            weapon.rarity = WeaponRarity.Legendary;

            string path = EditorUtility.SaveFilePanelInProject(
                "Save New Unique Weapon",
                "NewUniqueWeapon",
                "asset",
                "Create a new unique weapon definition",
                UNIQUE_WEAPONS_FOLDER);

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(weapon, path);
                AssetDatabase.SaveAssets();

                _uniqueWeapons.Add(weapon);
                _selectedUniqueWeapon = weapon;
                _selectedUniqueWeaponSO = new SerializedObject(weapon);

                Debug.Log($"[{TOOL_NAME}] Created new unique weapon at {path}");
            }
        }

        private void DeleteUniqueWeapon(UniqueWeaponDefinition weapon)
        {
            string path = AssetDatabase.GetAssetPath(weapon);
            _uniqueWeapons.Remove(weapon);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();

            _selectedUniqueWeapon = null;
            _selectedUniqueWeaponSO = null;

            Debug.Log($"[{TOOL_NAME}] Deleted unique weapon: {weapon.weaponName}");
        }

        #endregion

        #region Table Editor Tab

        private void DrawTableEditorTab()
        {
            EditorGUILayout.BeginHorizontal();

            // Left panel - Table list
            EditorGUILayout.BeginVertical(GUILayout.Width(280));
            DrawTableList();
            EditorGUILayout.EndVertical();

            // Middle panel - Table properties
            EditorGUILayout.BeginVertical(GUILayout.Width(350));
            DrawTableProperties();
            EditorGUILayout.EndVertical();

            // Right panel - Entry editor
            EditorGUILayout.BeginVertical();
            DrawEntryEditor();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawTableList()
        {
            EditorGUILayout.LabelField("Loot Tables", EditorStyles.boldLabel);

            // Search
            EditorGUILayout.BeginHorizontal();
            _searchFilter = EditorGUILayout.TextField(_searchFilter, EditorStyles.toolbarSearchField);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _searchFilter = "";
            }
            EditorGUILayout.EndHorizontal();

            // Source filter
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Source:", GUILayout.Width(50));
            string[] sourceOptions = new string[] { "All" }
                .Concat(Enum.GetNames(typeof(LootSourceType)))
                .ToArray();
            int sourceIndex = _sourceFilter.HasValue ? (int)_sourceFilter.Value + 1 : 0;
            int newSourceIndex = EditorGUILayout.Popup(sourceIndex, sourceOptions);
            _sourceFilter = newSourceIndex == 0 ? null : (LootSourceType?)(newSourceIndex - 1);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Table list
            _tableListScrollPosition = EditorGUILayout.BeginScrollView(_tableListScrollPosition, GUILayout.Height(400));

            if (_database != null)
            {
                var filteredTables = _database.allTables
                    .Where(t => t != null)
                    .Where(t => string.IsNullOrEmpty(_searchFilter) ||
                                t.tableName.ToLower().Contains(_searchFilter.ToLower()) ||
                                t.tableId.ToLower().Contains(_searchFilter.ToLower()))
                    .Where(t => !_sourceFilter.HasValue || t.sourceType == _sourceFilter.Value)
                    .OrderBy(t => t.sourceType)
                    .ThenBy(t => t.tableName);

                foreach (var table in filteredTables)
                {
                    GUI.color = table == _selectedTable ? Color.cyan : Color.white;

                    string label = $"[{GetSourcePrefix(table.sourceType)}] {table.tableName}";
                    if (GUILayout.Button(label, EditorStyles.miniButton))
                    {
                        SelectTable(table);
                    }
                }

                GUI.color = Color.white;
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            if (GUILayout.Button("+ Create New Table"))
            {
                CreateNewTable();
            }
        }

        private string GetSourcePrefix(LootSourceType source)
        {
            if (source.ToString().StartsWith("Enemy_"))
                return "E";
            if (source.ToString().StartsWith("Chest_"))
                return "C";
            if (source.ToString().StartsWith("Quest_"))
                return "Q";
            if (source.ToString().StartsWith("World_"))
                return "W";
            if (source.ToString().StartsWith("Vendor_"))
                return "V";
            if (source.ToString().StartsWith("Endgame_"))
                return "X";
            return "?";
        }

        private void DrawTableProperties()
        {
            if (_selectedTable == null)
            {
                EditorGUILayout.HelpBox("Select a table from the list to edit.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Table: {_selectedTable.tableName}", EditorStyles.boldLabel);

            if (_selectedTableSO != null)
            {
                _selectedTableSO.Update();

                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("tableId"));
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("tableName"));
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("description"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Source Configuration", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("sourceType"));
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("specificSourceId"));
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("applicableZones"), true);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Drop Count", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("minDrops"));
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("maxDrops"));
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("bonusDropChance"));
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("guaranteedDrop"));
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("allowDuplicates"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Level & Mayhem", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("minimumPlayerLevel"));
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("levelRange"));
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("minimumMayhemLevel"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Rarity Distribution", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("rarityDistribution"), true);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Manufacturer Bias", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("useGlobalManufacturerBias"));
                if (_selectedTable.useGlobalManufacturerBias)
                {
                    EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("globalManufacturerWeights"), true);
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_selectedTableSO.FindProperty("parentTable"));

                _selectedTableSO.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(_selectedTable);
                AssetDatabase.SaveAssets();
                Debug.Log($"[{TOOL_NAME}] Saved {_selectedTable.tableName}");
            }

            GUI.color = Color.red;
            if (GUILayout.Button("Delete"))
            {
                if (EditorUtility.DisplayDialog("Delete Table",
                    $"Delete '{_selectedTable.tableName}'?", "Delete", "Cancel"))
                {
                    DeleteTable(_selectedTable);
                }
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawEntryEditor()
        {
            if (_selectedTable == null)
            {
                return;
            }

            EditorGUILayout.LabelField($"Entries ({_selectedTable.entries.Count})", EditorStyles.boldLabel);

            // Entry list
            _entryListScrollPosition = EditorGUILayout.BeginScrollView(_entryListScrollPosition, GUILayout.Height(200));

            for (int i = 0; i < _selectedTable.entries.Count; i++)
            {
                var entry = _selectedTable.entries[i];
                GUI.color = i == _selectedEntryIndex ? Color.cyan : Color.white;

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button($"{entry.entryName ?? entry.itemType.ToString()}", EditorStyles.miniButton))
                {
                    _selectedEntryIndex = i;
                    _selectedEntry = entry;
                }

                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _selectedTable.entries.RemoveAt(i);
                    EditorUtility.SetDirty(_selectedTable);
                    if (_selectedEntryIndex >= _selectedTable.entries.Count)
                    {
                        _selectedEntryIndex = _selectedTable.entries.Count - 1;
                    }
                    _selectedEntry = _selectedEntryIndex >= 0 ? _selectedTable.entries[_selectedEntryIndex] : null;
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("+ Add Entry"))
            {
                var newEntry = new LootTableEntry
                {
                    entryName = "New Entry",
                    itemType = LootItemType.Weapon_AssaultRifle,
                    weight = 10f,
                    baseDropChance = 0.1f
                };
                _selectedTable.entries.Add(newEntry);
                _selectedEntryIndex = _selectedTable.entries.Count - 1;
                _selectedEntry = newEntry;
                EditorUtility.SetDirty(_selectedTable);
            }

            EditorGUILayout.Space();

            // Selected entry details
            if (_selectedEntry != null && _selectedEntryIndex >= 0)
            {
                EditorGUILayout.LabelField("Entry Details", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                _selectedEntry.entryName = EditorGUILayout.TextField("Name", _selectedEntry.entryName);
                _selectedEntry.itemType = (LootItemType)EditorGUILayout.EnumPopup("Item Type", _selectedEntry.itemType);
                _selectedEntry.specificItemId = EditorGUILayout.TextField("Specific Item ID", _selectedEntry.specificItemId);

                EditorGUILayout.Space();
                _selectedEntry.weight = EditorGUILayout.Slider("Weight", _selectedEntry.weight, 0f, 100f);
                _selectedEntry.baseDropChance = EditorGUILayout.Slider("Base Drop Chance", _selectedEntry.baseDropChance, 0f, 1f);
                _selectedEntry.guaranteed = EditorGUILayout.Toggle("Guaranteed", _selectedEntry.guaranteed);

                EditorGUILayout.Space();
                _selectedEntry.minRarity = (WeaponRarity)EditorGUILayout.EnumPopup("Min Rarity", _selectedEntry.minRarity);
                _selectedEntry.maxRarity = (WeaponRarity)EditorGUILayout.EnumPopup("Max Rarity", _selectedEntry.maxRarity);

                EditorGUILayout.Space();
                _selectedEntry.minLevel = EditorGUILayout.IntSlider("Min Level", _selectedEntry.minLevel, 1, 50);
                _selectedEntry.maxLevel = EditorGUILayout.IntSlider("Max Level", _selectedEntry.maxLevel, 0, 50);

                EditorGUILayout.Space();
                _selectedEntry.minQuantity = EditorGUILayout.IntSlider("Min Quantity", _selectedEntry.minQuantity, 1, 100);
                _selectedEntry.maxQuantity = EditorGUILayout.IntSlider("Max Quantity", _selectedEntry.maxQuantity, 1, 100);

                EditorGUILayout.Space();
                _selectedEntry.conditions = (DropCondition)EditorGUILayout.EnumFlagsField("Conditions", _selectedEntry.conditions);

                if (_selectedEntry.conditions.HasFlag(DropCondition.QuestActive) ||
                    _selectedEntry.conditions.HasFlag(DropCondition.QuestComplete))
                {
                    _selectedEntry.requiredQuestId = EditorGUILayout.TextField("Required Quest ID", _selectedEntry.requiredQuestId);
                }

                EditorGUILayout.EndVertical();

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(_selectedTable);
                }
            }
        }

        private void SelectTable(LootTable table)
        {
            _selectedTable = table;
            _selectedTableSO = new SerializedObject(table);
            _selectedEntryIndex = -1;
            _selectedEntry = null;
        }

        #endregion

        #region Enemy Tables Tab

        private void DrawEnemyTablesTab()
        {
            EditorGUILayout.LabelField("Enemy-Specific Loot Tables", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Configure loot tables for each enemy type. Per GDD:\n" +
                "- Scrappers: Basic enemies, common drops\n" +
                "- Badass variants: Better rarity chances\n" +
                "- Bosses: Guaranteed rare+ drops\n" +
                "- Raid Bosses: Apocalypse tier possible",
                MessageType.Info);

            EditorGUILayout.Space();

            // Show enemy types grouped
            var enemySources = Enum.GetValues(typeof(LootSourceType))
                .Cast<LootSourceType>()
                .Where(s => s.ToString().StartsWith("Enemy_"))
                .ToList();

            foreach (var source in enemySources)
            {
                var tables = _database?.GetTablesBySource(source) ?? new List<LootTable>();

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(source.ToString().Replace("Enemy_", ""), GUILayout.Width(120));
                EditorGUILayout.LabelField($"{tables.Count} table(s)", GUILayout.Width(80));

                if (tables.Count > 0)
                {
                    if (GUILayout.Button("Edit", GUILayout.Width(50)))
                    {
                        SelectTable(tables[0]);
                        _currentTab = Tab.TableEditor;
                    }
                }

                if (GUILayout.Button("Create", GUILayout.Width(50)))
                {
                    CreateTableForSource(source);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate All Enemy Tables", GUILayout.Height(30)))
            {
                GenerateEnemyTables();
            }
        }

        #endregion

        #region Chest Tiers Tab

        private void DrawChestTiersTab()
        {
            EditorGUILayout.LabelField("Chest Tier Configuration", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Configure loot for each chest tier per GDD:\n" +
                "- White: Common items only\n" +
                "- Green: Up to Uncommon\n" +
                "- Blue: Up to Rare\n" +
                "- Purple: Up to Epic\n" +
                "- Orange (Red Chest): Up to Legendary\n" +
                "- Cyan (Vault): Up to Pearlescent\n" +
                "- Red: Apocalypse possible",
                MessageType.Info);

            EditorGUILayout.Space();

            foreach (ChestTier tier in Enum.GetValues(typeof(ChestTier)))
            {
                var tables = _database?.GetTablesBySource(GetChestSourceType(tier)) ?? new List<LootTable>();

                Color tierColor = GetChestTierColor(tier);
                GUI.color = tierColor;

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUI.color = Color.white;

                EditorGUILayout.LabelField($"{tier} Chest", GUILayout.Width(100));
                EditorGUILayout.LabelField($"Max: {GetMaxRarityForTier(tier)}", GUILayout.Width(120));
                EditorGUILayout.LabelField($"{tables.Count} table(s)", GUILayout.Width(80));

                if (tables.Count > 0)
                {
                    if (GUILayout.Button("Edit", GUILayout.Width(50)))
                    {
                        SelectTable(tables[0]);
                        _currentTab = Tab.TableEditor;
                    }
                }

                if (GUILayout.Button("Create", GUILayout.Width(50)))
                {
                    CreateChestTable(tier);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate All Chest Tables", GUILayout.Height(30)))
            {
                GenerateChestTables();
            }
        }

        private LootSourceType GetChestSourceType(ChestTier tier)
        {
            return tier switch
            {
                ChestTier.White => LootSourceType.Chest_Common,
                ChestTier.Green => LootSourceType.Chest_Uncommon,
                ChestTier.Blue => LootSourceType.Chest_Rare,
                ChestTier.Purple => LootSourceType.Chest_Epic,
                ChestTier.Orange => LootSourceType.Chest_Legendary,
                ChestTier.Cyan => LootSourceType.Chest_Vault,
                ChestTier.Red => LootSourceType.Chest_Vault,
                _ => LootSourceType.Chest_Common
            };
        }

        private Color GetChestTierColor(ChestTier tier)
        {
            return tier switch
            {
                ChestTier.White => Color.white,
                ChestTier.Green => RarityColors[WeaponRarity.Uncommon],
                ChestTier.Blue => RarityColors[WeaponRarity.Rare],
                ChestTier.Purple => RarityColors[WeaponRarity.Epic],
                ChestTier.Orange => RarityColors[WeaponRarity.Legendary],
                ChestTier.Cyan => RarityColors[WeaponRarity.Pearlescent],
                ChestTier.Red => RarityColors[WeaponRarity.Apocalypse],
                _ => Color.white
            };
        }

        private WeaponRarity GetMaxRarityForTier(ChestTier tier)
        {
            return tier switch
            {
                ChestTier.White => WeaponRarity.Common,
                ChestTier.Green => WeaponRarity.Uncommon,
                ChestTier.Blue => WeaponRarity.Rare,
                ChestTier.Purple => WeaponRarity.Epic,
                ChestTier.Orange => WeaponRarity.Legendary,
                ChestTier.Cyan => WeaponRarity.Pearlescent,
                ChestTier.Red => WeaponRarity.Apocalypse,
                _ => WeaponRarity.Common
            };
        }

        #endregion

        #region Quest Rewards Tab

        private void DrawQuestRewardsTab()
        {
            EditorGUILayout.LabelField("Quest Reward Pools", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Configure reward pools for different quest types:\n" +
                "- Side Quests: Standard rewards, level-scaled\n" +
                "- Main Quests: Better rewards, story-relevant items\n" +
                "- Daily Challenges: Currency and consumables\n" +
                "- Weekly Challenges: Rare+ guaranteed",
                MessageType.Info);

            EditorGUILayout.Space();

            var questSources = new[]
            {
                LootSourceType.Quest_Side,
                LootSourceType.Quest_Main,
                LootSourceType.Quest_Daily,
                LootSourceType.Quest_Weekly
            };

            foreach (var source in questSources)
            {
                var tables = _database?.GetTablesBySource(source) ?? new List<LootTable>();

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(source.ToString().Replace("Quest_", "") + " Quest", GUILayout.Width(120));
                EditorGUILayout.LabelField($"{tables.Count} table(s)", GUILayout.Width(80));

                if (tables.Count > 0)
                {
                    if (GUILayout.Button("Edit", GUILayout.Width(50)))
                    {
                        SelectTable(tables[0]);
                        _currentTab = Tab.TableEditor;
                    }
                }

                if (GUILayout.Button("Create", GUILayout.Width(50)))
                {
                    CreateTableForSource(source);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate All Quest Tables", GUILayout.Height(30)))
            {
                GenerateQuestTables();
            }
        }

        #endregion

        #region World Drops Tab

        private void DrawWorldDropsTab()
        {
            EditorGUILayout.LabelField("World Drop Configuration", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Configure world drop rates by zone per GDD Level Design:\n" +
                "- Fractured Coast (1-10): Common focus\n" +
                "- Scorched Plateau (10-20): Uncommon focus\n" +
                "- Crystal Caverns (20-30): Rare focus\n" +
                "- Nexus Wasteland (30-40): Epic possible\n" +
                "- Core Facility (40-50): Legendary possible",
                MessageType.Info);

            EditorGUILayout.Space();

            // Global settings
            EditorGUILayout.LabelField("Global Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (_database != null)
            {
                var so = new SerializedObject(_database);
                EditorGUILayout.PropertyField(so.FindProperty("globalLuckModifier"));
                EditorGUILayout.PropertyField(so.FindProperty("globalRarityBoost"));
                EditorGUILayout.PropertyField(so.FindProperty("defaultWorldDropTable"));
                so.ApplyModifiedProperties();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Zone-specific tables
            EditorGUILayout.LabelField("Zone-Specific Tables", EditorStyles.boldLabel);

            foreach (GameZone zone in Enum.GetValues(typeof(GameZone)))
            {
                if (zone.ToString().StartsWith("ProvingGround_") ||
                    zone.ToString().StartsWith("Raid_") ||
                    zone == GameZone.CircleOfSlaughter ||
                    zone == GameZone.TheSpiral)
                    continue;

                var tables = _database?.GetTablesByZone(zone) ?? new List<LootTable>();
                var levelRange = GetZoneLevelRange(zone);

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(zone.ToString(), GUILayout.Width(140));
                EditorGUILayout.LabelField($"Lvl {levelRange.min}-{levelRange.max}", GUILayout.Width(70));
                EditorGUILayout.LabelField($"{tables.Count} table(s)", GUILayout.Width(80));

                if (GUILayout.Button("Create Zone Table", GUILayout.Width(120)))
                {
                    CreateZoneTable(zone);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private (int min, int max) GetZoneLevelRange(GameZone zone)
        {
            return zone switch
            {
                GameZone.HavenCity => (1, 5),
                GameZone.FracturedCoast => (1, 10),
                GameZone.ScorchedPlateau => (10, 20),
                GameZone.CrystalCaverns => (20, 30),
                GameZone.NexusWasteland => (30, 40),
                GameZone.CoreFacility => (40, 50),
                _ => (1, 50)
            };
        }

        #endregion

        #region Rarity Config Tab

        private void DrawRarityConfigTab()
        {
            EditorGUILayout.LabelField("Rarity Weight Configuration", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "GDD-defined drop rates:\n" +
                "- Common: 60%\n" +
                "- Uncommon: 25%\n" +
                "- Rare: 10%\n" +
                "- Epic: 4%\n" +
                "- Legendary: 0.9%\n" +
                "- Pearlescent: 0.09%\n" +
                "- Apocalypse: 0.01%",
                MessageType.Info);

            EditorGUILayout.Space();

            // Visual bar chart
            EditorGUILayout.LabelField("Rarity Distribution Visualization", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            float[] rates = { 0.60f, 0.25f, 0.10f, 0.04f, 0.009f, 0.0009f, 0.0001f };
            int i = 0;

            foreach (WeaponRarity rarity in Enum.GetValues(typeof(WeaponRarity)))
            {
                float rate = rates[i];

                EditorGUILayout.BeginHorizontal();
                GUI.color = RarityColors[rarity];
                EditorGUILayout.LabelField(rarity.ToString(), GUILayout.Width(100));
                GUI.color = Color.white;

                // Draw bar (log scale for visibility)
                float logRate = Mathf.Log10(rate * 1000 + 1) / Mathf.Log10(1001);
                Rect barRect = GUILayoutUtility.GetRect(200, 20);
                EditorGUI.DrawRect(barRect, new Color(0.2f, 0.2f, 0.2f));

                Rect fillRect = new Rect(barRect.x, barRect.y, barRect.width * logRate, barRect.height);
                EditorGUI.DrawRect(fillRect, RarityColors[rarity]);

                EditorGUILayout.LabelField($"{rate * 100:F4}%", GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();

                i++;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Manufacturer distribution
            EditorGUILayout.LabelField("Manufacturer Distribution (7 manufacturers)", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            foreach (WeaponManufacturer manufacturer in Enum.GetValues(typeof(WeaponManufacturer)))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(manufacturer.ToString(), GUILayout.Width(150));

                Rect barRect = GUILayoutUtility.GetRect(200, 20);
                EditorGUI.DrawRect(barRect, new Color(0.2f, 0.2f, 0.2f));

                Rect fillRect = new Rect(barRect.x, barRect.y, barRect.width / 7f, barRect.height);
                EditorGUI.DrawRect(fillRect, new Color(0.4f, 0.6f, 0.9f));

                EditorGUILayout.LabelField($"{100f / 7f:F1}%", GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Simulation Tab

        private void DrawSimulationTab()
        {
            EditorGUILayout.LabelField("Drop Simulation", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Test loot tables by simulating drops. Adjust parameters and run simulations to verify drop rates.",
                MessageType.Info);

            EditorGUILayout.Space();

            // Simulation parameters
            EditorGUILayout.LabelField("Simulation Parameters", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            _simulationCount = EditorGUILayout.IntSlider("Simulation Count", _simulationCount, 100, 100000);
            _simulationPlayerLevel = EditorGUILayout.IntSlider("Player Level", _simulationPlayerLevel, 1, 50);
            _simulationLuck = EditorGUILayout.Slider("Luck Modifier", _simulationLuck, -0.5f, 2f);
            _simulationMayhem = (MayhemLevel)EditorGUILayout.EnumPopup("Mayhem Level", _simulationMayhem);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Select Table to Simulate:");
            _selectedTable = (LootTable)EditorGUILayout.ObjectField(_selectedTable, typeof(LootTable), false);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            if (GUILayout.Button("Run Simulation", GUILayout.Height(30)))
            {
                RunSimulation();
            }

            // Results
            if (_simulationRarityResults != null && _simulationRarityResults.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Results ({_simulationCount} drops)", EditorStyles.boldLabel);

                // Rarity distribution
                EditorGUILayout.LabelField("Rarity Distribution:");
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                int maxRarityCount = _simulationRarityResults.Values.Max();
                if (maxRarityCount == 0) maxRarityCount = 1;

                foreach (var kvp in _simulationRarityResults.OrderByDescending(k => (int)k.Key))
                {
                    float ratio = (float)kvp.Value / _simulationCount;

                    EditorGUILayout.BeginHorizontal();
                    GUI.color = RarityColors[kvp.Key];
                    EditorGUILayout.LabelField(kvp.Key.ToString(), GUILayout.Width(100));
                    GUI.color = Color.white;

                    Rect barRect = GUILayoutUtility.GetRect(200, 18);
                    EditorGUI.DrawRect(barRect, new Color(0.2f, 0.2f, 0.2f));

                    float displayRatio = (float)kvp.Value / maxRarityCount;
                    Rect fillRect = new Rect(barRect.x, barRect.y, barRect.width * displayRatio, barRect.height);
                    EditorGUI.DrawRect(fillRect, RarityColors[kvp.Key]);

                    EditorGUILayout.LabelField($"{kvp.Value} ({ratio * 100:F2}%)", GUILayout.Width(120));
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

                // Item type distribution
                if (_simulationTypeResults != null && _simulationTypeResults.Count > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Item Type Distribution:");
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    foreach (var kvp in _simulationTypeResults.OrderByDescending(k => k.Value).Take(10))
                    {
                        float ratio = (float)kvp.Value / _simulationCount;
                        EditorGUILayout.LabelField($"{kvp.Key}: {kvp.Value} ({ratio * 100:F2}%)");
                    }

                    EditorGUILayout.EndVertical();
                }

                // Manufacturer distribution
                if (_simulationManufacturerResults != null && _simulationManufacturerResults.Count > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Manufacturer Distribution:");
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    int totalWeaponDrops = _simulationManufacturerResults.Values.Sum();

                    foreach (var kvp in _simulationManufacturerResults.OrderByDescending(k => k.Value))
                    {
                        float ratio = totalWeaponDrops > 0 ? (float)kvp.Value / totalWeaponDrops : 0;
                        EditorGUILayout.LabelField($"{kvp.Key}: {kvp.Value} ({ratio * 100:F1}%)");
                    }

                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void RunSimulation()
        {
            if (_selectedTable == null)
            {
                EditorUtility.DisplayDialog("No Table", "Please select a loot table to simulate.", "OK");
                return;
            }

            _simulationRarityResults = new Dictionary<WeaponRarity, int>();
            _simulationTypeResults = new Dictionary<LootItemType, int>();
            _simulationManufacturerResults = new Dictionary<WeaponManufacturer, int>();

            foreach (WeaponRarity rarity in Enum.GetValues(typeof(WeaponRarity)))
            {
                _simulationRarityResults[rarity] = 0;
            }

            foreach (WeaponManufacturer manufacturer in Enum.GetValues(typeof(WeaponManufacturer)))
            {
                _simulationManufacturerResults[manufacturer] = 0;
            }

            System.Random random = new System.Random();

            try
            {
                EditorUtility.DisplayProgressBar("Simulating", "Running drop simulation...", 0f);

                for (int i = 0; i < _simulationCount; i++)
                {
                    if (i % 1000 == 0)
                    {
                        EditorUtility.DisplayProgressBar("Simulating",
                            $"Running drop simulation... {i}/{_simulationCount}",
                            (float)i / _simulationCount);
                    }

                    var drops = _selectedTable.RollDrops(
                        _simulationPlayerLevel,
                        _simulationLuck,
                        _simulationMayhem,
                        DropCondition.None,
                        random);

                    foreach (var drop in drops)
                    {
                        _simulationRarityResults[drop.rarity]++;

                        if (!_simulationTypeResults.ContainsKey(drop.itemType))
                            _simulationTypeResults[drop.itemType] = 0;
                        _simulationTypeResults[drop.itemType]++;

                        if (drop.manufacturer.HasValue)
                        {
                            _simulationManufacturerResults[drop.manufacturer.Value]++;
                        }
                    }
                }

                Debug.Log($"[{TOOL_NAME}] Simulation complete: {_simulationCount} drops");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        #endregion

        #region Utility Methods

        private void CreateNewTable()
        {
            EnsureFolderExists(TABLES_FOLDER);

            var table = CreateInstance<LootTable>();
            table.tableName = "New Loot Table";
            table.tableId = $"table_{DateTime.Now.Ticks}";

            string path = EditorUtility.SaveFilePanelInProject(
                "Save New Loot Table",
                "NewLootTable",
                "asset",
                "Create a new loot table",
                TABLES_FOLDER);

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(table, path);
                _database.AddTable(table);
                EditorUtility.SetDirty(_database);
                AssetDatabase.SaveAssets();

                SelectTable(table);
                _currentTab = Tab.TableEditor;

                Debug.Log($"[{TOOL_NAME}] Created new table at {path}");
            }
        }

        private void CreateTableForSource(LootSourceType source)
        {
            EnsureFolderExists(TABLES_FOLDER);

            var table = CreateInstance<LootTable>();
            table.tableName = source.ToString().Replace("_", " ");
            table.tableId = source.ToString().ToLower();
            table.sourceType = source;

            // Apply defaults based on source type
            ApplySourceDefaults(table, source);

            string filename = $"{source}.asset";
            string path = $"{TABLES_FOLDER}/{filename}";

            AssetDatabase.CreateAsset(table, path);
            _database.AddTable(table);
            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();

            SelectTable(table);
            _currentTab = Tab.TableEditor;

            Debug.Log($"[{TOOL_NAME}] Created table for {source}");
        }

        private void CreateChestTable(ChestTier tier)
        {
            var source = GetChestSourceType(tier);
            var table = CreateInstance<LootTable>();
            table.tableName = $"{tier} Chest";
            table.tableId = $"chest_{tier}".ToLower();
            table.sourceType = source;

            // Set max rarity based on tier
            var maxRarity = GetMaxRarityForTier(tier);
            table.rarityDistribution = CreateDistributionForMaxRarity(maxRarity);

            // Set drop counts
            table.minDrops = (int)tier + 1;
            table.maxDrops = (int)tier + 3;
            table.guaranteedDrop = true;

            // Add default weapon entry
            table.entries.Add(new LootTableEntry
            {
                entryName = "Weapon Drop",
                itemType = LootItemType.Weapon_AssaultRifle,
                weight = 50f,
                baseDropChance = 0.5f,
                minRarity = WeaponRarity.Common,
                maxRarity = maxRarity
            });

            EnsureFolderExists($"{TABLES_FOLDER}/Chests");
            string path = $"{TABLES_FOLDER}/Chests/{table.tableId}.asset";

            AssetDatabase.CreateAsset(table, path);
            _database.AddTable(table);
            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();

            SelectTable(table);
            _currentTab = Tab.TableEditor;
        }

        private void CreateZoneTable(GameZone zone)
        {
            var table = CreateInstance<LootTable>();
            table.tableName = $"{zone} World Drops";
            table.tableId = $"world_{zone}".ToLower();
            table.sourceType = LootSourceType.World_Zone;
            table.applicableZones = new[] { zone };

            var levelRange = GetZoneLevelRange(zone);
            table.minimumPlayerLevel = levelRange.min;

            EnsureFolderExists($"{TABLES_FOLDER}/Zones");
            string path = $"{TABLES_FOLDER}/Zones/{table.tableId}.asset";

            AssetDatabase.CreateAsset(table, path);
            _database.AddTable(table);
            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();

            SelectTable(table);
            _currentTab = Tab.TableEditor;
        }

        private void ApplySourceDefaults(LootTable table, LootSourceType source)
        {
            if (source.ToString().StartsWith("Enemy_"))
            {
                table.minDrops = 1;
                table.maxDrops = 2;
                table.guaranteedDrop = false;

                if (source == LootSourceType.Enemy_Boss)
                {
                    table.minDrops = 3;
                    table.maxDrops = 5;
                    table.guaranteedDrop = true;
                    table.rarityDistribution.rareWeight = 0.3f;
                    table.rarityDistribution.epicWeight = 0.15f;
                }
                else if (source == LootSourceType.Enemy_RaidBoss)
                {
                    table.minDrops = 5;
                    table.maxDrops = 10;
                    table.guaranteedDrop = true;
                    table.rarityDistribution.legendaryWeight = 0.1f;
                    table.rarityDistribution.pearlescentWeight = 0.01f;
                }
                else if (source == LootSourceType.Enemy_Badass)
                {
                    table.minDrops = 2;
                    table.maxDrops = 3;
                    table.rarityDistribution.rareWeight = 0.2f;
                }
            }
            else if (source.ToString().StartsWith("Quest_"))
            {
                table.guaranteedDrop = true;
                table.minDrops = 1;
                table.maxDrops = 1;

                if (source == LootSourceType.Quest_Main)
                {
                    table.minDrops = 2;
                    table.maxDrops = 3;
                    table.rarityDistribution.rareWeight = 0.3f;
                }
            }
        }

        private RarityDistribution CreateDistributionForMaxRarity(WeaponRarity maxRarity)
        {
            var dist = new RarityDistribution();

            // Zero out rarities above max
            if (maxRarity < WeaponRarity.Apocalypse) dist.apocalypseWeight = 0;
            if (maxRarity < WeaponRarity.Pearlescent) dist.pearlescentWeight = 0;
            if (maxRarity < WeaponRarity.Legendary) dist.legendaryWeight = 0;
            if (maxRarity < WeaponRarity.Epic) dist.epicWeight = 0;
            if (maxRarity < WeaponRarity.Rare) dist.rareWeight = 0;
            if (maxRarity < WeaponRarity.Uncommon) dist.uncommonWeight = 0;

            dist.Normalize();
            return dist;
        }

        private void DeleteTable(LootTable table)
        {
            string path = AssetDatabase.GetAssetPath(table);
            _database.RemoveTable(table);
            EditorUtility.SetDirty(_database);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();

            _selectedTable = null;
            _selectedTableSO = null;
            _selectedEntry = null;
            _selectedEntryIndex = -1;

            Debug.Log($"[{TOOL_NAME}] Deleted table: {table.tableName}");
        }

        private void RefreshDatabase()
        {
            string[] guids = AssetDatabase.FindAssets("t:LootTable");
            List<LootTable> foundTables = new List<LootTable>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var table = AssetDatabase.LoadAssetAtPath<LootTable>(path);
                if (table != null)
                {
                    foundTables.Add(table);
                }
            }

            _database.allTables = foundTables;
            _database.ClearCache();
            _database.InitializeCache();

            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();

            Debug.Log($"[{TOOL_NAME}] Database refreshed. Found {foundTables.Count} tables.");
        }

        private void GenerateDefaultTables()
        {
            if (!EditorUtility.DisplayDialog("Generate Default Tables",
                "This will create default loot tables for all source types. Continue?",
                "Generate", "Cancel"))
            {
                return;
            }

            GenerateEnemyTables();
            GenerateChestTables();
            GenerateQuestTables();

            EditorUtility.DisplayDialog("Complete",
                "Default tables generated successfully.", "OK");
        }

        private void GenerateEnemyTables()
        {
            var enemySources = Enum.GetValues(typeof(LootSourceType))
                .Cast<LootSourceType>()
                .Where(s => s.ToString().StartsWith("Enemy_"))
                .ToList();

            foreach (var source in enemySources)
            {
                if (_database.GetTablesBySource(source).Count == 0)
                {
                    CreateTableForSource(source);
                }
            }

            Debug.Log($"[{TOOL_NAME}] Enemy tables generated.");
        }

        private void GenerateChestTables()
        {
            foreach (ChestTier tier in Enum.GetValues(typeof(ChestTier)))
            {
                var source = GetChestSourceType(tier);
                if (_database.GetTablesBySource(source).Count == 0)
                {
                    CreateChestTable(tier);
                }
            }

            Debug.Log($"[{TOOL_NAME}] Chest tables generated.");
        }

        private void GenerateQuestTables()
        {
            var questSources = new[]
            {
                LootSourceType.Quest_Side,
                LootSourceType.Quest_Main,
                LootSourceType.Quest_Daily,
                LootSourceType.Quest_Weekly
            };

            foreach (var source in questSources)
            {
                if (_database.GetTablesBySource(source).Count == 0)
                {
                    CreateTableForSource(source);
                }
            }

            Debug.Log($"[{TOOL_NAME}] Quest tables generated.");
        }

        #endregion
    }
}
