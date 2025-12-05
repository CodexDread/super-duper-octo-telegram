using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using NexusProtocol.Weapons;

namespace NexusProtocol.Editor
{
    /// <summary>
    /// Weapon Part Manager - Editor tool for managing weapon parts
    /// Features:
    /// - Batch import weapon part models
    /// - Auto-generate WeaponPartDefinition ScriptableObjects
    /// - Set part statistics per GDD specifications
    /// - Preview part combinations
    /// - Validate part compatibility rules
    /// - Mass edit part properties
    /// - Rarity distribution visualization
    /// </summary>
    public class WeaponPartManagerWindow : EditorWindow
    {
        // Tool version for changelog
        private const string TOOL_VERSION = "1.0";
        private const string TOOL_NAME = "Weapon Part Manager";
        private const string DATABASE_PATH = "Assets/ScriptableObjects/WeaponParts/WeaponPartDatabase.asset";
        private const string PARTS_FOLDER = "Assets/ScriptableObjects/WeaponParts/Parts";

        // Tab system
        private enum Tab
        {
            Overview,
            PartEditor,
            BatchImport,
            MassEdit,
            Preview,
            Validation,
            Statistics
        }

        private Tab _currentTab = Tab.Overview;
        private Vector2 _scrollPosition;
        private Vector2 _partListScrollPosition;

        // Data references
        private WeaponPartDatabase _database;
        private WeaponPartDefinition _selectedPart;
        private SerializedObject _selectedPartSerializedObject;

        // Batch import settings
        private string _importFolderPath = "";
        private WeaponPartCategory _importCategory = WeaponPartCategory.Receiver;
        private WeaponRarity _importRarity = WeaponRarity.Common;
        private WeaponPartType _importPartType = WeaponPartType.Standard;
        private WeaponManufacturer _importManufacturer = WeaponManufacturer.KDC;
        private List<GameObject> _pendingImports = new List<GameObject>();

        // Mass edit settings
        private WeaponPartCategory _massEditCategory;
        private bool _massEditCategoryFilter = false;
        private WeaponRarity _massEditRarity;
        private bool _massEditRarityFilter = false;
        private WeaponManufacturer _massEditManufacturer;
        private bool _massEditManufacturerFilter = false;

        // Mass edit operations
        private float _massEditDropWeightMultiplier = 1f;
        private int _massEditMinLevelOffset = 0;
        private bool _massEditWorldDropEnabled = true;

        // Preview settings
        private WeaponPartDefinition[] _previewParts = new WeaponPartDefinition[6];
        private float _previewCalculatedRarity;

        // Filter settings
        private string _searchFilter = "";
        private WeaponPartCategory? _categoryFilter = null;
        private WeaponRarity? _rarityFilter = null;

        // GDD-defined values
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

        private static readonly Dictionary<WeaponRarity, float> RarityDropRates = new Dictionary<WeaponRarity, float>
        {
            { WeaponRarity.Common, 0.60f },
            { WeaponRarity.Uncommon, 0.25f },
            { WeaponRarity.Rare, 0.10f },
            { WeaponRarity.Epic, 0.04f },
            { WeaponRarity.Legendary, 0.009f },
            { WeaponRarity.Pearlescent, 0.0009f },
            { WeaponRarity.Apocalypse, 0.0001f }
        };

        [MenuItem("Nexus/Weapons/Weapon Part Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<WeaponPartManagerWindow>("Weapon Part Manager");
            window.minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            _database = AssetDatabase.LoadAssetAtPath<WeaponPartDatabase>(DATABASE_PATH);

            if (_database == null)
            {
                // Create database if it doesn't exist
                EnsureFolderExists(Path.GetDirectoryName(DATABASE_PATH));
                _database = CreateInstance<WeaponPartDatabase>();
                AssetDatabase.CreateAsset(_database, DATABASE_PATH);
                AssetDatabase.SaveAssets();
                Debug.Log($"[{TOOL_NAME}] Created new weapon part database at {DATABASE_PATH}");
            }

            _database.InitializeCache();
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
                EditorGUILayout.LabelField($"Parts: {_database.allParts.Count}", GUILayout.Width(80));
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
                case Tab.PartEditor:
                    DrawPartEditorTab();
                    break;
                case Tab.BatchImport:
                    DrawBatchImportTab();
                    break;
                case Tab.MassEdit:
                    DrawMassEditTab();
                    break;
                case Tab.Preview:
                    DrawPreviewTab();
                    break;
                case Tab.Validation:
                    DrawValidationTab();
                    break;
                case Tab.Statistics:
                    DrawStatisticsTab();
                    break;
            }

            EditorGUILayout.EndScrollView();
        }

        #region Overview Tab

        private void DrawOverviewTab()
        {
            EditorGUILayout.HelpBox(
                "Weapon Part Manager allows you to create, import, and manage weapon parts for NEXUS PROTOCOL.\n\n" +
                "Part Categories (6): Receiver, Barrel, Magazine, Grip, Stock, Sight\n" +
                "Rarity Tiers (7): Common, Uncommon, Rare, Epic, Legendary, Pearlescent, Apocalypse\n" +
                "Manufacturers (7): KDC, Frontier Arms, TekCorp, Quantum Dynamics, Void Industries, Nexus Salvage, RedLine",
                MessageType.Info);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create New Part", GUILayout.Height(40)))
            {
                CreateNewPart();
            }

            if (GUILayout.Button("Generate Sample Parts", GUILayout.Height(40)))
            {
                GenerateSampleParts();
            }

            if (GUILayout.Button("Refresh Database", GUILayout.Height(40)))
            {
                RefreshDatabase();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Quick stats
            if (_database != null)
            {
                var stats = _database.GetStatistics();

                EditorGUILayout.LabelField("Database Overview", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.LabelField($"Total Parts: {stats.TotalParts}");

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Parts by Category:");
                foreach (WeaponPartCategory category in Enum.GetValues(typeof(WeaponPartCategory)))
                {
                    int count = stats.PartsByCategory.TryGetValue(category, out int c) ? c : 0;
                    EditorGUILayout.LabelField($"  {category}: {count}");
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Parts by Rarity:");
                foreach (WeaponRarity rarity in Enum.GetValues(typeof(WeaponRarity)))
                {
                    int count = stats.PartsByRarity.TryGetValue(rarity, out int c) ? c : 0;
                    GUI.color = RarityColors[rarity];
                    EditorGUILayout.LabelField($"  {rarity}: {count}");
                }
                GUI.color = Color.white;

                EditorGUILayout.EndVertical();
            }
        }

        #endregion

        #region Part Editor Tab

        private void DrawPartEditorTab()
        {
            EditorGUILayout.BeginHorizontal();

            // Left panel - Part list
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            DrawPartList();
            EditorGUILayout.EndVertical();

            // Right panel - Part editor
            EditorGUILayout.BeginVertical();
            DrawPartDetails();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawPartList()
        {
            EditorGUILayout.LabelField("Parts", EditorStyles.boldLabel);

            // Search filter
            EditorGUILayout.BeginHorizontal();
            _searchFilter = EditorGUILayout.TextField(_searchFilter, EditorStyles.toolbarSearchField);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _searchFilter = "";
            }
            EditorGUILayout.EndHorizontal();

            // Category filter
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Category:", GUILayout.Width(60));
            int categoryIndex = _categoryFilter.HasValue ? (int)_categoryFilter.Value + 1 : 0;
            string[] categoryOptions = new string[] { "All" }.Concat(Enum.GetNames(typeof(WeaponPartCategory))).ToArray();
            int newCategoryIndex = EditorGUILayout.Popup(categoryIndex, categoryOptions);
            _categoryFilter = newCategoryIndex == 0 ? null : (WeaponPartCategory?)(newCategoryIndex - 1);
            EditorGUILayout.EndHorizontal();

            // Rarity filter
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Rarity:", GUILayout.Width(60));
            int rarityIndex = _rarityFilter.HasValue ? (int)_rarityFilter.Value : 0;
            string[] rarityOptions = new string[] { "All" }.Concat(Enum.GetNames(typeof(WeaponRarity))).ToArray();
            int newRarityIndex = EditorGUILayout.Popup(rarityIndex, rarityOptions);
            _rarityFilter = newRarityIndex == 0 ? null : (WeaponRarity?)(newRarityIndex - 1);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Part list
            _partListScrollPosition = EditorGUILayout.BeginScrollView(_partListScrollPosition, GUILayout.Height(400));

            if (_database != null)
            {
                var filteredParts = _database.allParts
                    .Where(p => p != null)
                    .Where(p => string.IsNullOrEmpty(_searchFilter) ||
                                p.partName.ToLower().Contains(_searchFilter.ToLower()) ||
                                p.partID.ToLower().Contains(_searchFilter.ToLower()))
                    .Where(p => !_categoryFilter.HasValue || p.category == _categoryFilter.Value)
                    .Where(p => !_rarityFilter.HasValue || p.rarity == _rarityFilter.Value)
                    .OrderBy(p => p.category)
                    .ThenBy(p => p.rarity)
                    .ThenBy(p => p.partName);

                foreach (var part in filteredParts)
                {
                    GUI.color = part == _selectedPart ? Color.cyan : RarityColors[part.rarity];

                    if (GUILayout.Button($"[{part.category}] {part.partName}", EditorStyles.miniButton))
                    {
                        SelectPart(part);
                    }
                }

                GUI.color = Color.white;
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            if (GUILayout.Button("+ Create New Part"))
            {
                CreateNewPart();
            }
        }

        private void DrawPartDetails()
        {
            if (_selectedPart == null)
            {
                EditorGUILayout.HelpBox("Select a part from the list to edit its properties.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Editing: {_selectedPart.partName}", EditorStyles.boldLabel);

            if (_selectedPartSerializedObject != null)
            {
                _selectedPartSerializedObject.Update();

                // Draw all properties
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("partID"));
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("partName"));
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("description"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Classification", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("category"));
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("rarity"));
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("partType"));

                if (_selectedPart.category == WeaponPartCategory.Receiver)
                {
                    EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("manufacturer"));
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Visual Assets", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("modelPrefab"));
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("icon"));
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("attachmentOffset"));
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("attachmentRotation"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("statModifiers"), true);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Special Effects", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("specialEffects"), true);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Compatibility", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("compatibleWeaponTypes"), true);
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("incompatiblePartIDs"), true);
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("requiredPartIDs"), true);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Generation Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("dropWeight"));
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("minimumLevel"));
                EditorGUILayout.PropertyField(_selectedPartSerializedObject.FindProperty("worldDropEnabled"));

                _selectedPartSerializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();

            // Validation
            if (_selectedPart.Validate(out var errors))
            {
                EditorGUILayout.HelpBox("Part is valid.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox($"Validation errors:\n{string.Join("\n", errors)}", MessageType.Error);
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Changes"))
            {
                EditorUtility.SetDirty(_selectedPart);
                AssetDatabase.SaveAssets();
                Debug.Log($"[{TOOL_NAME}] Saved changes to {_selectedPart.partName}");
            }

            GUI.color = Color.red;
            if (GUILayout.Button("Delete Part"))
            {
                if (EditorUtility.DisplayDialog("Delete Part", $"Are you sure you want to delete {_selectedPart.partName}?", "Delete", "Cancel"))
                {
                    DeletePart(_selectedPart);
                }
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private void SelectPart(WeaponPartDefinition part)
        {
            _selectedPart = part;
            _selectedPartSerializedObject = new SerializedObject(part);
        }

        #endregion

        #region Batch Import Tab

        private void DrawBatchImportTab()
        {
            EditorGUILayout.LabelField("Batch Import Weapon Parts", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Import multiple weapon part models at once. Drag a folder containing FBX/OBJ models " +
                "or select individual models to import.",
                MessageType.Info);

            EditorGUILayout.Space();

            // Import settings
            EditorGUILayout.LabelField("Import Settings", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            _importCategory = (WeaponPartCategory)EditorGUILayout.EnumPopup("Category", _importCategory);
            _importRarity = (WeaponRarity)EditorGUILayout.EnumPopup("Default Rarity", _importRarity);
            _importPartType = (WeaponPartType)EditorGUILayout.EnumPopup("Default Part Type", _importPartType);

            if (_importCategory == WeaponPartCategory.Receiver)
            {
                _importManufacturer = (WeaponManufacturer)EditorGUILayout.EnumPopup("Manufacturer", _importManufacturer);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Folder selection
            EditorGUILayout.BeginHorizontal();
            _importFolderPath = EditorGUILayout.TextField("Import Folder", _importFolderPath);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Folder with Models", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    _importFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Scan Folder for Models"))
            {
                ScanFolderForModels();
            }

            EditorGUILayout.Space();

            // Pending imports
            EditorGUILayout.LabelField($"Pending Imports ({_pendingImports.Count})", EditorStyles.boldLabel);

            if (_pendingImports.Count > 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                foreach (var model in _pendingImports.ToList())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(model, typeof(GameObject), false);
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        _pendingImports.Remove(model);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Import All", GUILayout.Height(30)))
                {
                    ImportAllPending();
                }
                if (GUILayout.Button("Clear List", GUILayout.Height(30)))
                {
                    _pendingImports.Clear();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            // Drag & drop area
            EditorGUILayout.LabelField("Or drag models here:", EditorStyles.boldLabel);

            Rect dropArea = GUILayoutUtility.GetRect(0, 100, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drop Models Here");

            Event evt = Event.current;
            if (dropArea.Contains(evt.mousePosition))
            {
                if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            if (obj is GameObject go && !_pendingImports.Contains(go))
                            {
                                _pendingImports.Add(go);
                            }
                        }
                    }

                    evt.Use();
                }
            }
        }

        private void ScanFolderForModels()
        {
            if (string.IsNullOrEmpty(_importFolderPath) || !AssetDatabase.IsValidFolder(_importFolderPath))
            {
                EditorUtility.DisplayDialog("Invalid Folder", "Please select a valid folder.", "OK");
                return;
            }

            _pendingImports.Clear();

            string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { _importFolderPath });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (model != null)
                {
                    _pendingImports.Add(model);
                }
            }

            Debug.Log($"[{TOOL_NAME}] Found {_pendingImports.Count} models in {_importFolderPath}");
        }

        private void ImportAllPending()
        {
            if (_pendingImports.Count == 0)
            {
                EditorUtility.DisplayDialog("No Models", "No models to import.", "OK");
                return;
            }

            EnsureFolderExists(PARTS_FOLDER);
            string categoryFolder = $"{PARTS_FOLDER}/{_importCategory}";
            EnsureFolderExists(categoryFolder);

            int importCount = 0;

            try
            {
                EditorUtility.DisplayProgressBar("Importing Parts", "Starting import...", 0f);

                for (int i = 0; i < _pendingImports.Count; i++)
                {
                    var model = _pendingImports[i];
                    float progress = (float)i / _pendingImports.Count;
                    EditorUtility.DisplayProgressBar("Importing Parts", $"Importing {model.name}...", progress);

                    // Create part definition
                    var part = CreateInstance<WeaponPartDefinition>();
                    part.partName = model.name;
                    part.partID = $"{_importCategory}_{model.name}_{_importRarity}_{_importPartType}".ToLower().Replace(" ", "_");
                    part.category = _importCategory;
                    part.rarity = _importRarity;
                    part.partType = _importPartType;
                    part.modelPrefab = model;

                    if (_importCategory == WeaponPartCategory.Receiver)
                    {
                        part.manufacturer = _importManufacturer;
                    }

                    // Apply default stats based on rarity
                    ApplyDefaultStats(part);

                    // Save asset
                    string assetPath = $"{categoryFolder}/{part.partID}.asset";
                    AssetDatabase.CreateAsset(part, assetPath);

                    // Add to database
                    _database.AddPart(part);
                    importCount++;
                }

                EditorUtility.SetDirty(_database);
                AssetDatabase.SaveAssets();

                _pendingImports.Clear();
                _database.InitializeCache();

                Debug.Log($"[{TOOL_NAME}] Successfully imported {importCount} parts");
                EditorUtility.DisplayDialog("Import Complete", $"Successfully imported {importCount} parts.", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void ApplyDefaultStats(WeaponPartDefinition part)
        {
            var (minBonus, maxBonus) = part.GetStatImprovementRange();
            float avgBonus = (minBonus + maxBonus) / 2f;

            // Apply category-specific default stats
            switch (part.category)
            {
                case WeaponPartCategory.Receiver:
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Damage, 0, avgBonus));
                    break;

                case WeaponPartCategory.Barrel:
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Range, 0, avgBonus));
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Accuracy, 0, avgBonus * 0.5f));
                    break;

                case WeaponPartCategory.Magazine:
                    part.statModifiers.Add(new StatModifier(WeaponStatType.MagazineSize, 0, avgBonus));
                    part.statModifiers.Add(new StatModifier(WeaponStatType.ReloadSpeed, 0, avgBonus * 0.3f));
                    break;

                case WeaponPartCategory.Grip:
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Handling, 0, avgBonus));
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Recoil, 0, -avgBonus * 0.5f));
                    break;

                case WeaponPartCategory.Stock:
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Recoil, 0, -avgBonus));
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Accuracy, 0, avgBonus * 0.3f));
                    break;

                case WeaponPartCategory.Sight:
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Accuracy, 0, avgBonus));
                    part.statModifiers.Add(new StatModifier(WeaponStatType.CriticalChance, 0, avgBonus * 0.2f));
                    break;
            }
        }

        #endregion

        #region Mass Edit Tab

        private void DrawMassEditTab()
        {
            EditorGUILayout.LabelField("Mass Edit Parts", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Apply changes to multiple parts at once based on filters.",
                MessageType.Info);

            EditorGUILayout.Space();

            // Filters
            EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            _massEditCategoryFilter = EditorGUILayout.Toggle(_massEditCategoryFilter, GUILayout.Width(20));
            EditorGUI.BeginDisabledGroup(!_massEditCategoryFilter);
            _massEditCategory = (WeaponPartCategory)EditorGUILayout.EnumPopup("Category", _massEditCategory);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _massEditRarityFilter = EditorGUILayout.Toggle(_massEditRarityFilter, GUILayout.Width(20));
            EditorGUI.BeginDisabledGroup(!_massEditRarityFilter);
            _massEditRarity = (WeaponRarity)EditorGUILayout.EnumPopup("Rarity", _massEditRarity);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _massEditManufacturerFilter = EditorGUILayout.Toggle(_massEditManufacturerFilter, GUILayout.Width(20));
            EditorGUI.BeginDisabledGroup(!_massEditManufacturerFilter);
            _massEditManufacturer = (WeaponManufacturer)EditorGUILayout.EnumPopup("Manufacturer", _massEditManufacturer);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            // Calculate affected parts
            int affectedCount = GetAffectedParts().Count;
            EditorGUILayout.LabelField($"Affected Parts: {affectedCount}");

            EditorGUILayout.Space();

            // Operations
            EditorGUILayout.LabelField("Operations", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            _massEditDropWeightMultiplier = EditorGUILayout.FloatField("Drop Weight Multiplier", _massEditDropWeightMultiplier);
            if (GUILayout.Button($"Apply Drop Weight x{_massEditDropWeightMultiplier}"))
            {
                ApplyMassEdit(p => p.dropWeight *= _massEditDropWeightMultiplier);
            }

            EditorGUILayout.Space();

            _massEditMinLevelOffset = EditorGUILayout.IntField("Min Level Offset", _massEditMinLevelOffset);
            if (GUILayout.Button($"Apply Min Level +{_massEditMinLevelOffset}"))
            {
                ApplyMassEdit(p => p.minimumLevel = Mathf.Clamp(p.minimumLevel + _massEditMinLevelOffset, 1, 50));
            }

            EditorGUILayout.Space();

            _massEditWorldDropEnabled = EditorGUILayout.Toggle("World Drop Enabled", _massEditWorldDropEnabled);
            if (GUILayout.Button($"Set World Drop = {_massEditWorldDropEnabled}"))
            {
                ApplyMassEdit(p => p.worldDropEnabled = _massEditWorldDropEnabled);
            }

            EditorGUILayout.EndVertical();
        }

        private List<WeaponPartDefinition> GetAffectedParts()
        {
            if (_database == null) return new List<WeaponPartDefinition>();

            return _database.allParts
                .Where(p => p != null)
                .Where(p => !_massEditCategoryFilter || p.category == _massEditCategory)
                .Where(p => !_massEditRarityFilter || p.rarity == _massEditRarity)
                .Where(p => !_massEditManufacturerFilter || (p.category == WeaponPartCategory.Receiver && p.manufacturer == _massEditManufacturer))
                .ToList();
        }

        private void ApplyMassEdit(Action<WeaponPartDefinition> action)
        {
            var parts = GetAffectedParts();

            if (parts.Count == 0)
            {
                EditorUtility.DisplayDialog("No Parts", "No parts match the current filters.", "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog("Confirm Mass Edit",
                $"This will modify {parts.Count} parts. Continue?", "Apply", "Cancel"))
            {
                return;
            }

            Undo.RecordObjects(parts.ToArray(), "Mass Edit Parts");

            foreach (var part in parts)
            {
                action(part);
                EditorUtility.SetDirty(part);
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[{TOOL_NAME}] Mass edit applied to {parts.Count} parts");
        }

        #endregion

        #region Preview Tab

        private void DrawPreviewTab()
        {
            EditorGUILayout.LabelField("Preview Part Combinations", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Select parts for each category to preview a complete weapon configuration and calculate its rarity.",
                MessageType.Info);

            EditorGUILayout.Space();

            // Part selection
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            foreach (WeaponPartCategory category in Enum.GetValues(typeof(WeaponPartCategory)))
            {
                int categoryIndex = (int)category;
                var currentPart = _previewParts[categoryIndex];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(category.ToString(), GUILayout.Width(80));

                if (currentPart != null)
                {
                    GUI.color = RarityColors[currentPart.rarity];
                }

                var newPart = (WeaponPartDefinition)EditorGUILayout.ObjectField(
                    currentPart,
                    typeof(WeaponPartDefinition),
                    false);

                GUI.color = Color.white;

                if (newPart != currentPart)
                {
                    if (newPart == null || newPart.category == category)
                    {
                        _previewParts[categoryIndex] = newPart;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Invalid Part",
                            $"Please select a {category} part.", "OK");
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Calculate rarity
            if (GUILayout.Button("Calculate Weapon Rarity", GUILayout.Height(30)))
            {
                CalculatePreviewRarity();
            }

            EditorGUILayout.Space();

            // Results
            EditorGUILayout.LabelField("Preview Results", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Part summary
            int partCount = 0;
            float raritySum = 0;
            List<string> partSummary = new List<string>();

            foreach (WeaponPartCategory category in Enum.GetValues(typeof(WeaponPartCategory)))
            {
                var part = _previewParts[(int)category];
                if (part != null)
                {
                    partCount++;
                    raritySum += part.GetRarityValue();
                    partSummary.Add($"{category}: {part.partName} ({part.rarity})");
                }
                else
                {
                    partSummary.Add($"{category}: Not selected");
                }
            }

            foreach (var summary in partSummary)
            {
                EditorGUILayout.LabelField(summary);
            }

            EditorGUILayout.Space();

            if (partCount > 0)
            {
                _previewCalculatedRarity = raritySum / partCount;

                // Determine final rarity per GDD
                WeaponRarity finalRarity;
                if (_previewCalculatedRarity >= 6.6f) finalRarity = WeaponRarity.Apocalypse;
                else if (_previewCalculatedRarity >= 5.6f) finalRarity = WeaponRarity.Pearlescent;
                else if (_previewCalculatedRarity >= 4.6f) finalRarity = WeaponRarity.Legendary;
                else if (_previewCalculatedRarity >= 3.6f) finalRarity = WeaponRarity.Epic;
                else if (_previewCalculatedRarity >= 2.6f) finalRarity = WeaponRarity.Rare;
                else if (_previewCalculatedRarity >= 1.6f) finalRarity = WeaponRarity.Uncommon;
                else finalRarity = WeaponRarity.Common;

                GUI.color = RarityColors[finalRarity];
                EditorGUILayout.LabelField($"Average Rarity Value: {_previewCalculatedRarity:F2}");
                EditorGUILayout.LabelField($"Final Weapon Rarity: {finalRarity}", EditorStyles.boldLabel);
                GUI.color = Color.white;

                // Compatibility check
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Compatibility Check", EditorStyles.boldLabel);

                bool hasIncompatibilities = false;
                for (int i = 0; i < _previewParts.Length; i++)
                {
                    var part = _previewParts[i];
                    if (part == null) continue;

                    foreach (var incompatibleId in part.incompatiblePartIDs)
                    {
                        for (int j = 0; j < _previewParts.Length; j++)
                        {
                            if (i == j) continue;
                            var otherPart = _previewParts[j];
                            if (otherPart != null && otherPart.partID == incompatibleId)
                            {
                                GUI.color = Color.red;
                                EditorGUILayout.LabelField($"Incompatible: {part.partName} <-> {otherPart.partName}");
                                GUI.color = Color.white;
                                hasIncompatibilities = true;
                            }
                        }
                    }
                }

                if (!hasIncompatibilities)
                {
                    EditorGUILayout.LabelField("All parts are compatible!", EditorStyles.boldLabel);
                }
            }
            else
            {
                EditorGUILayout.LabelField("Select parts to calculate rarity");
            }

            EditorGUILayout.EndVertical();
        }

        private void CalculatePreviewRarity()
        {
            // Already calculated in DrawPreviewTab
        }

        #endregion

        #region Validation Tab

        private void DrawValidationTab()
        {
            EditorGUILayout.LabelField("Part Validation", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Validate all parts in the database against GDD specifications.",
                MessageType.Info);

            EditorGUILayout.Space();

            if (GUILayout.Button("Validate All Parts", GUILayout.Height(30)))
            {
                ValidateAllParts();
            }

            EditorGUILayout.Space();

            if (_database == null) return;

            var validationResults = _database.ValidateAll();

            if (validationResults.Count == 0)
            {
                EditorGUILayout.HelpBox("All parts are valid!", MessageType.Info);
            }
            else
            {
                EditorGUILayout.LabelField($"Found {validationResults.Count} parts with issues:", EditorStyles.boldLabel);

                foreach (var (part, errors) in validationResults)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    if (part != null)
                    {
                        EditorGUILayout.ObjectField(part, typeof(WeaponPartDefinition), false);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Null Part Reference");
                    }

                    GUI.color = Color.red;
                    foreach (var error in errors)
                    {
                        EditorGUILayout.LabelField($"  - {error}");
                    }
                    GUI.color = Color.white;

                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.Space();

            // GDD Compliance Check
            EditorGUILayout.LabelField("GDD Compliance Check", EditorStyles.boldLabel);
            DrawGDDComplianceCheck();
        }

        private void ValidateAllParts()
        {
            if (_database != null)
            {
                _database.ValidateAll();
            }
        }

        private void DrawGDDComplianceCheck()
        {
            if (_database == null) return;

            var stats = _database.GetStatistics();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Check all 6 categories have parts
            EditorGUILayout.LabelField("Part Categories (Required: 6)");
            int categoriesWithParts = 0;
            foreach (WeaponPartCategory category in Enum.GetValues(typeof(WeaponPartCategory)))
            {
                int count = stats.PartsByCategory.TryGetValue(category, out int c) ? c : 0;
                bool hasParts = count > 0;
                if (hasParts) categoriesWithParts++;

                GUI.color = hasParts ? Color.green : Color.red;
                EditorGUILayout.LabelField($"  {category}: {(hasParts ? "OK" : "MISSING")} ({count} parts)");
            }
            GUI.color = Color.white;

            EditorGUILayout.Space();

            // Check all 7 rarities are represented
            EditorGUILayout.LabelField("Rarity Tiers (Required: 7)");
            int raritiesWithParts = 0;
            foreach (WeaponRarity rarity in Enum.GetValues(typeof(WeaponRarity)))
            {
                int count = stats.PartsByRarity.TryGetValue(rarity, out int c) ? c : 0;
                bool hasParts = count > 0;
                if (hasParts) raritiesWithParts++;

                GUI.color = hasParts ? Color.green : Color.yellow;
                EditorGUILayout.LabelField($"  {rarity}: {count} parts");
            }
            GUI.color = Color.white;

            EditorGUILayout.Space();

            // Check all 7 manufacturers have receivers
            EditorGUILayout.LabelField("Manufacturers (Required: 7)");
            int manufacturersWithReceivers = 0;
            foreach (WeaponManufacturer manufacturer in Enum.GetValues(typeof(WeaponManufacturer)))
            {
                int count = stats.PartsByManufacturer.TryGetValue(manufacturer, out int c) ? c : 0;
                bool hasReceivers = count > 0;
                if (hasReceivers) manufacturersWithReceivers++;

                GUI.color = hasReceivers ? Color.green : Color.red;
                EditorGUILayout.LabelField($"  {manufacturer}: {(hasReceivers ? "OK" : "MISSING")} ({count} receivers)");
            }
            GUI.color = Color.white;

            EditorGUILayout.Space();

            // Summary
            bool fullyCompliant = categoriesWithParts == 6 && manufacturersWithReceivers == 7;
            GUI.color = fullyCompliant ? Color.green : Color.yellow;
            EditorGUILayout.LabelField(
                fullyCompliant
                    ? "Database is GDD compliant!"
                    : "Database needs more parts to be GDD compliant",
                EditorStyles.boldLabel);
            GUI.color = Color.white;

            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Statistics Tab

        private void DrawStatisticsTab()
        {
            EditorGUILayout.LabelField("Rarity Distribution Visualization", EditorStyles.boldLabel);

            if (_database == null) return;

            var stats = _database.GetStatistics();

            // Rarity distribution bar chart
            EditorGUILayout.LabelField("Parts by Rarity", EditorStyles.boldLabel);
            DrawRarityDistributionChart(stats);

            EditorGUILayout.Space();

            // Category distribution
            EditorGUILayout.LabelField("Parts by Category", EditorStyles.boldLabel);
            DrawCategoryDistributionChart(stats);

            EditorGUILayout.Space();

            // Drop rate analysis
            EditorGUILayout.LabelField("Expected Drop Rate Analysis", EditorStyles.boldLabel);
            DrawDropRateAnalysis();

            EditorGUILayout.Space();

            // Simulation
            EditorGUILayout.LabelField("Drop Simulation", EditorStyles.boldLabel);
            DrawDropSimulation();
        }

        private void DrawRarityDistributionChart(DatabaseStatistics stats)
        {
            int maxCount = stats.PartsByRarity.Values.Max();
            if (maxCount == 0) maxCount = 1;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            foreach (WeaponRarity rarity in Enum.GetValues(typeof(WeaponRarity)))
            {
                int count = stats.PartsByRarity.TryGetValue(rarity, out int c) ? c : 0;
                float ratio = (float)count / maxCount;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(rarity.ToString(), GUILayout.Width(100));

                Rect barRect = GUILayoutUtility.GetRect(200, 20);
                EditorGUI.DrawRect(barRect, new Color(0.2f, 0.2f, 0.2f));

                Rect fillRect = new Rect(barRect.x, barRect.y, barRect.width * ratio, barRect.height);
                EditorGUI.DrawRect(fillRect, RarityColors[rarity]);

                EditorGUILayout.LabelField($"{count} ({ratio * 100:F1}%)", GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawCategoryDistributionChart(DatabaseStatistics stats)
        {
            int maxCount = stats.PartsByCategory.Values.Max();
            if (maxCount == 0) maxCount = 1;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            foreach (WeaponPartCategory category in Enum.GetValues(typeof(WeaponPartCategory)))
            {
                int count = stats.PartsByCategory.TryGetValue(category, out int c) ? c : 0;
                float ratio = (float)count / maxCount;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(category.ToString(), GUILayout.Width(100));

                Rect barRect = GUILayoutUtility.GetRect(200, 20);
                EditorGUI.DrawRect(barRect, new Color(0.2f, 0.2f, 0.2f));

                Rect fillRect = new Rect(barRect.x, barRect.y, barRect.width * ratio, barRect.height);
                EditorGUI.DrawRect(fillRect, new Color(0.3f, 0.6f, 0.9f));

                EditorGUILayout.LabelField($"{count}", GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawDropRateAnalysis()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("GDD-Defined Drop Rates:");

            foreach (WeaponRarity rarity in Enum.GetValues(typeof(WeaponRarity)))
            {
                float rate = RarityDropRates[rarity];
                GUI.color = RarityColors[rarity];
                EditorGUILayout.LabelField($"  {rarity}: {rate * 100:F2}%");
            }

            GUI.color = Color.white;
            EditorGUILayout.EndVertical();
        }

        private int _simulationCount = 1000;
        private Dictionary<WeaponRarity, int> _simulationResults;

        private void DrawDropSimulation()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            _simulationCount = EditorGUILayout.IntField("Simulation Count", _simulationCount);
            _simulationCount = Mathf.Clamp(_simulationCount, 100, 100000);

            if (GUILayout.Button("Run Simulation"))
            {
                RunDropSimulation();
            }

            if (_simulationResults != null && _simulationResults.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Results ({_simulationCount} rolls):");

                foreach (WeaponRarity rarity in Enum.GetValues(typeof(WeaponRarity)))
                {
                    int count = _simulationResults.TryGetValue(rarity, out int c) ? c : 0;
                    float actualRate = (float)count / _simulationCount;
                    float expectedRate = RarityDropRates[rarity];
                    float deviation = Mathf.Abs(actualRate - expectedRate) / expectedRate * 100;

                    GUI.color = RarityColors[rarity];
                    EditorGUILayout.LabelField(
                        $"  {rarity}: {count} ({actualRate * 100:F2}%) - Expected: {expectedRate * 100:F2}% - Deviation: {deviation:F1}%");
                }

                GUI.color = Color.white;
            }

            EditorGUILayout.EndVertical();
        }

        private void RunDropSimulation()
        {
            _simulationResults = new Dictionary<WeaponRarity, int>();

            foreach (WeaponRarity rarity in Enum.GetValues(typeof(WeaponRarity)))
            {
                _simulationResults[rarity] = 0;
            }

            System.Random random = new System.Random();

            for (int i = 0; i < _simulationCount; i++)
            {
                float roll = (float)random.NextDouble();
                WeaponRarity rarity;

                if (roll < 0.0001f)
                    rarity = WeaponRarity.Apocalypse;
                else if (roll < 0.001f)
                    rarity = WeaponRarity.Pearlescent;
                else if (roll < 0.01f)
                    rarity = WeaponRarity.Legendary;
                else if (roll < 0.05f)
                    rarity = WeaponRarity.Epic;
                else if (roll < 0.15f)
                    rarity = WeaponRarity.Rare;
                else if (roll < 0.40f)
                    rarity = WeaponRarity.Uncommon;
                else
                    rarity = WeaponRarity.Common;

                _simulationResults[rarity]++;
            }

            Debug.Log($"[{TOOL_NAME}] Simulation complete: {_simulationCount} rolls");
        }

        #endregion

        #region Utility Methods

        private void CreateNewPart()
        {
            EnsureFolderExists(PARTS_FOLDER);

            var part = CreateInstance<WeaponPartDefinition>();
            part.partName = "New Part";
            part.partID = $"new_part_{DateTime.Now.Ticks}";

            string path = EditorUtility.SaveFilePanelInProject(
                "Save New Part",
                "NewWeaponPart",
                "asset",
                "Create a new weapon part definition",
                PARTS_FOLDER);

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(part, path);
                _database.AddPart(part);
                EditorUtility.SetDirty(_database);
                AssetDatabase.SaveAssets();

                SelectPart(part);
                _currentTab = Tab.PartEditor;

                Debug.Log($"[{TOOL_NAME}] Created new part at {path}");
            }
        }

        private void DeletePart(WeaponPartDefinition part)
        {
            string path = AssetDatabase.GetAssetPath(part);

            _database.RemovePart(part);
            EditorUtility.SetDirty(_database);

            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();

            _selectedPart = null;
            _selectedPartSerializedObject = null;

            Debug.Log($"[{TOOL_NAME}] Deleted part: {part.partName}");
        }

        private void GenerateSampleParts()
        {
            if (!EditorUtility.DisplayDialog("Generate Sample Parts",
                "This will create sample parts for each category and manufacturer. Continue?",
                "Generate", "Cancel"))
            {
                return;
            }

            EnsureFolderExists(PARTS_FOLDER);

            int createdCount = 0;

            try
            {
                EditorUtility.DisplayProgressBar("Generating Parts", "Creating sample parts...", 0f);

                // Create parts for each category
                foreach (WeaponPartCategory category in Enum.GetValues(typeof(WeaponPartCategory)))
                {
                    string categoryFolder = $"{PARTS_FOLDER}/{category}";
                    EnsureFolderExists(categoryFolder);

                    // Create one part per rarity
                    foreach (WeaponRarity rarity in Enum.GetValues(typeof(WeaponRarity)))
                    {
                        var part = CreateInstance<WeaponPartDefinition>();
                        part.category = category;
                        part.rarity = rarity;
                        part.partType = WeaponPartType.Standard;
                        part.partName = $"{category} {rarity}";
                        part.partID = $"{category}_{rarity}_standard".ToLower();
                        part.description = $"A {rarity.ToString().ToLower()} quality {category.ToString().ToLower()}.";

                        // For receivers, assign to different manufacturers
                        if (category == WeaponPartCategory.Receiver)
                        {
                            part.manufacturer = (WeaponManufacturer)((int)rarity % 7);
                        }

                        ApplyDefaultStats(part);

                        string assetPath = $"{categoryFolder}/{part.partID}.asset";
                        AssetDatabase.CreateAsset(part, assetPath);
                        _database.AddPart(part);
                        createdCount++;
                    }
                }

                // Create manufacturer-specific receivers
                foreach (WeaponManufacturer manufacturer in Enum.GetValues(typeof(WeaponManufacturer)))
                {
                    string categoryFolder = $"{PARTS_FOLDER}/Receiver";
                    EnsureFolderExists(categoryFolder);

                    var part = CreateInstance<WeaponPartDefinition>();
                    part.category = WeaponPartCategory.Receiver;
                    part.rarity = WeaponRarity.Rare;
                    part.partType = WeaponPartType.Standard;
                    part.manufacturer = manufacturer;
                    part.partName = $"{manufacturer} Receiver";
                    part.partID = $"receiver_{manufacturer}_rare_standard".ToLower();
                    part.description = GetManufacturerDescription(manufacturer);

                    ApplyDefaultStats(part);
                    ApplyManufacturerBonuses(part, manufacturer);

                    string assetPath = $"{categoryFolder}/{part.partID}.asset";
                    AssetDatabase.CreateAsset(part, assetPath);
                    _database.AddPart(part);
                    createdCount++;
                }

                EditorUtility.SetDirty(_database);
                AssetDatabase.SaveAssets();
                _database.InitializeCache();

                Debug.Log($"[{TOOL_NAME}] Generated {createdCount} sample parts");
                EditorUtility.DisplayDialog("Generation Complete",
                    $"Successfully created {createdCount} sample parts.", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private string GetManufacturerDescription(WeaponManufacturer manufacturer)
        {
            return manufacturer switch
            {
                WeaponManufacturer.KDC => "Kinetic Dynamics Corporation - Reliability through simplicity. High damage, excellent accuracy.",
                WeaponManufacturer.FrontierArms => "Frontier Arms - Rugged frontier engineering. Balanced stats with durability focus.",
                WeaponManufacturer.TekCorp => "TekCorp Industries - Smart weapons for smart soldiers. Tech-enhanced targeting.",
                WeaponManufacturer.QuantumDynamics => "Quantum Dynamics - Reality-bending weapons. Phase through cover, unique effects.",
                WeaponManufacturer.VoidIndustries => "Void Industries - Embrace the void. Life-steal, dark energy projectiles.",
                WeaponManufacturer.NexusSalvage => "Nexus Salvage Co. - Turn their weapons against them. Adaptive damage, self-repairing.",
                WeaponManufacturer.RedLine => "RedLine - Push the limits. Maximum fire rate, aggressive combat style.",
                _ => "Unknown manufacturer."
            };
        }

        private void ApplyManufacturerBonuses(WeaponPartDefinition part, WeaponManufacturer manufacturer)
        {
            switch (manufacturer)
            {
                case WeaponManufacturer.KDC:
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Damage, 0, 0.15f));
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Accuracy, 0, 0.10f));
                    break;

                case WeaponManufacturer.FrontierArms:
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Handling, 0, 0.10f));
                    part.statModifiers.Add(new StatModifier(WeaponStatType.ReloadSpeed, 0, 0.05f));
                    break;

                case WeaponManufacturer.TekCorp:
                    part.statModifiers.Add(new StatModifier(WeaponStatType.CriticalChance, 0, 0.10f));
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Accuracy, 0, 0.08f));
                    break;

                case WeaponManufacturer.QuantumDynamics:
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Penetration, 0, 0.20f));
                    part.statModifiers.Add(new StatModifier(WeaponStatType.ElementalChance, 0, 0.10f));
                    break;

                case WeaponManufacturer.VoidIndustries:
                    part.statModifiers.Add(new StatModifier(WeaponStatType.ElementalDamage, 0, 0.15f));
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Damage, 0, 0.10f));
                    break;

                case WeaponManufacturer.NexusSalvage:
                    // Adaptive - starts lower but can improve
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Damage, 0, 0.05f));
                    break;

                case WeaponManufacturer.RedLine:
                    part.statModifiers.Add(new StatModifier(WeaponStatType.FireRate, 0, 0.20f));
                    part.statModifiers.Add(new StatModifier(WeaponStatType.Recoil, 0, 0.10f)); // More recoil
                    break;
            }
        }

        private void RefreshDatabase()
        {
            // Scan for all WeaponPartDefinition assets
            string[] guids = AssetDatabase.FindAssets("t:WeaponPartDefinition");
            List<WeaponPartDefinition> foundParts = new List<WeaponPartDefinition>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var part = AssetDatabase.LoadAssetAtPath<WeaponPartDefinition>(path);
                if (part != null)
                {
                    foundParts.Add(part);
                }
            }

            _database.allParts = foundParts;
            _database.ClearCache();
            _database.InitializeCache();

            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();

            Debug.Log($"[{TOOL_NAME}] Database refreshed. Found {foundParts.Count} parts.");
        }

        #endregion
    }
}
