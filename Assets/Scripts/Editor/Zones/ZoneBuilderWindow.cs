using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using NexusProtocol.Zones;

namespace NexusProtocol.Editor.Zones
{
    /// <summary>
    /// Zone Builder editor window for creating and configuring game zones.
    /// Provides tools for zone boundaries, spawn points, Motor Pools, cover points,
    /// environmental hazards, and random encounter zones.
    /// </summary>
    public class ZoneBuilderWindow : EditorWindow
    {
        private const string TOOL_VERSION = "1.0";
        private const string TOOL_NAME = "Zone Builder";

        // Data references
        private ZoneDatabase _database;
        private ZoneDefinition _selectedZone;
        private SerializedObject _serializedDatabase;
        private SerializedObject _serializedZone;

        // UI state
        private Vector2 _scrollPosition;
        private int _selectedTab;
        private string[] _tabNames = new string[]
        {
            "Overview", "Zone Editor", "Spawn Points", "Motor Pools",
            "Cover Points", "Hazards", "Encounters", "Connections", "Validation"
        };

        // Zone Editor state
        private bool _showZoneIdentity = true;
        private bool _showLevelConfig = true;
        private bool _showFeatures = true;

        // Spawn point editor state
        private Vector2 _spawnScrollPosition;
        private int _selectedSpawnIndex = -1;

        // Motor pool editor state
        private Vector2 _motorPoolScrollPosition;
        private int _selectedMotorPoolIndex = -1;

        // Cover point editor state
        private Vector2 _coverScrollPosition;
        private int _selectedCoverIndex = -1;
        private bool _showCoverGenerationSettings;
        private float _coverGenerationRadius = 50f;
        private float _coverMinSpacing = 3f;
        private LayerMask _coverLayerMask = ~0;

        // Hazard editor state
        private Vector2 _hazardScrollPosition;
        private int _selectedHazardIndex = -1;

        // Encounter editor state
        private Vector2 _encounterScrollPosition;
        private int _selectedEncounterIndex = -1;

        // Connection editor state
        private Vector2 _connectionScrollPosition;
        private int _selectedConnectionIndex = -1;

        // Validation state
        private List<(ZoneDefinition zone, List<string> errors)> _validationResults;

        // Colors
        private static readonly Color HeaderColor = new Color(0.3f, 0.3f, 0.3f);
        private static readonly Color SelectedColor = new Color(0.4f, 0.6f, 0.9f, 0.3f);

        [MenuItem("Nexus/Zones/Zone Builder")]
        public static void ShowWindow()
        {
            var window = GetWindow<ZoneBuilderWindow>("Zone Builder");
            window.minSize = new Vector2(600, 500);
        }

        private void OnEnable()
        {
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            // Try to find existing database
            string[] guids = AssetDatabase.FindAssets("t:ZoneDatabase");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _database = AssetDatabase.LoadAssetAtPath<ZoneDatabase>(path);
            }

            if (_database != null)
            {
                _serializedDatabase = new SerializedObject(_database);
            }
        }

        private void OnGUI()
        {
            DrawHeader();

            if (_database == null)
            {
                DrawNoDatabase();
                return;
            }

            // Tab selection
            EditorGUILayout.Space(5);
            _selectedTab = GUILayout.Toolbar(_selectedTab, _tabNames);
            EditorGUILayout.Space(10);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            switch (_selectedTab)
            {
                case 0: DrawOverviewTab(); break;
                case 1: DrawZoneEditorTab(); break;
                case 2: DrawSpawnPointsTab(); break;
                case 3: DrawMotorPoolsTab(); break;
                case 4: DrawCoverPointsTab(); break;
                case 5: DrawHazardsTab(); break;
                case 6: DrawEncountersTab(); break;
                case 7: DrawConnectionsTab(); break;
                case 8: DrawValidationTab(); break;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField($"{TOOL_NAME} v{TOOL_VERSION}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            if (_database != null)
            {
                EditorGUILayout.LabelField($"Zones: {_database.allZones.Count}", GUILayout.Width(80));
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawNoDatabase()
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.HelpBox("No Zone Database found. Create one to begin.", MessageType.Info);
            EditorGUILayout.Space(10);

            if (GUILayout.Button("Create Zone Database", GUILayout.Height(30)))
            {
                CreateDatabase();
            }
        }

        private void CreateDatabase()
        {
            string path = "Assets/ScriptableObjects/Zones";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Zones");
            }

            _database = CreateInstance<ZoneDatabase>();
            AssetDatabase.CreateAsset(_database, $"{path}/ZoneDatabase.asset");
            AssetDatabase.SaveAssets();
            _serializedDatabase = new SerializedObject(_database);
        }

        #region Overview Tab

        private void DrawOverviewTab()
        {
            EditorGUILayout.LabelField("Zone System Overview", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // Design explanation
            EditorGUILayout.HelpBox(
                "ZONE SYSTEM DESIGN:\n\n" +
                "Story Zones: Linear progression from Haven City through Core Facility (Level 1-50)\n" +
                "Endgame Zones: Proving Grounds, Circle of Slaughter, Raids, The Spiral\n\n" +
                "Each zone contains:\n" +
                "- Level range configuration\n" +
                "- Player spawn/respawn points\n" +
                "- Motor Pool vehicle stations\n" +
                "- Tactical cover points\n" +
                "- Environmental hazards\n" +
                "- Random encounter zones",
                MessageType.Info);

            EditorGUILayout.Space(10);

            // Statistics
            var stats = _database.GetStatistics();
            DrawStatisticsSection(stats);

            EditorGUILayout.Space(10);

            // Quick Actions
            DrawQuickActionsSection();

            EditorGUILayout.Space(10);

            // Zone list overview
            DrawZoneListOverview();
        }

        private void DrawStatisticsSection(ZoneDatabaseStats stats)
        {
            EditorGUILayout.LabelField("Database Statistics", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            DrawStatBox("Total Zones", stats.TotalZones.ToString(), Color.white);
            DrawStatBox("Story Zones", stats.StoryZones.ToString(), new Color(0.4f, 0.8f, 0.4f));
            DrawStatBox("Endgame Zones", stats.EndgameZones.ToString(), new Color(0.9f, 0.4f, 0.4f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            DrawStatBox("Spawn Points", stats.TotalSpawnPoints.ToString(), new Color(0.4f, 0.6f, 0.9f));
            DrawStatBox("Motor Pools", stats.TotalMotorPools.ToString(), new Color(0.9f, 0.6f, 0.2f));
            DrawStatBox("Cover Points", stats.TotalCoverPoints.ToString(), new Color(0.6f, 0.6f, 0.6f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            DrawStatBox("Hazard Areas", stats.TotalHazardAreas.ToString(), new Color(0.9f, 0.2f, 0.2f));
            DrawStatBox("Encounters", stats.TotalEncounterZones.ToString(), new Color(0.8f, 0.4f, 0.8f));
            DrawStatBox("Connections", stats.TotalConnections.ToString(), new Color(0.4f, 0.8f, 0.8f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawStatBox(string label, string value, Color color)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MinWidth(100));
            var oldColor = GUI.color;
            GUI.color = color;
            EditorGUILayout.LabelField(value, new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 18 });
            GUI.color = oldColor;
            EditorGUILayout.LabelField(label, new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter });
            EditorGUILayout.EndVertical();
        }

        private void DrawQuickActionsSection()
        {
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate Story Zones", GUILayout.Height(25)))
            {
                GenerateStoryZones();
            }

            if (GUILayout.Button("Generate Endgame Zones", GUILayout.Height(25)))
            {
                GenerateEndgameZones();
            }

            if (GUILayout.Button("Validate All", GUILayout.Height(25)))
            {
                _validationResults = _database.ValidateAll();
                _selectedTab = 8; // Switch to validation tab
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawZoneListOverview()
        {
            EditorGUILayout.LabelField("Zone Progression", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Story zones
            EditorGUILayout.LabelField("Story Zones:", EditorStyles.miniBoldLabel);
            var storyZones = _database.GetStoryZonesInOrder();
            foreach (var zone in storyZones)
            {
                EditorGUILayout.BeginHorizontal();
                GUI.color = zone.GetZoneTypeColor();
                EditorGUILayout.LabelField($"  {zone.zoneName}", GUILayout.Width(150));
                GUI.color = Color.white;
                EditorGUILayout.LabelField(zone.GetLevelRangeString(), GUILayout.Width(100));
                EditorGUILayout.LabelField($"Spawns: {zone.spawnPoints?.Length ?? 0}", GUILayout.Width(80));
                EditorGUILayout.LabelField($"Encounters: {zone.encounterZones?.Length ?? 0}", GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(5);

            // Endgame zones
            EditorGUILayout.LabelField("Endgame Zones:", EditorStyles.miniBoldLabel);
            var endgameZones = _database.GetEndgameZones();
            foreach (var zone in endgameZones)
            {
                EditorGUILayout.BeginHorizontal();
                GUI.color = zone.GetZoneTypeColor();
                EditorGUILayout.LabelField($"  {zone.zoneName}", GUILayout.Width(150));
                GUI.color = Color.white;
                EditorGUILayout.LabelField(zone.zoneType.ToString(), GUILayout.Width(150));
                EditorGUILayout.EndHorizontal();
            }

            GUI.color = Color.white;
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Zone Editor Tab

        private void DrawZoneEditorTab()
        {
            EditorGUILayout.BeginHorizontal();

            // Left panel - zone list
            EditorGUILayout.BeginVertical(GUILayout.Width(200));
            DrawZoneList();
            EditorGUILayout.EndVertical();

            // Right panel - zone details
            EditorGUILayout.BeginVertical();
            DrawZoneDetails();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawZoneList()
        {
            EditorGUILayout.LabelField("Zones", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                CreateNewZone();
            }
            if (GUILayout.Button("-", GUILayout.Width(25)) && _selectedZone != null)
            {
                DeleteSelectedZone();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            var scrollRect = EditorGUILayout.BeginScrollView(Vector2.zero, GUILayout.Height(400));

            foreach (var zone in _database.allZones)
            {
                if (zone == null) continue;

                bool isSelected = zone == _selectedZone;
                var style = new GUIStyle(EditorStyles.label)
                {
                    normal = { background = isSelected ? MakeTexture(2, 2, SelectedColor) : null }
                };

                EditorGUILayout.BeginHorizontal(style);
                GUI.color = zone.GetZoneTypeColor();
                if (GUILayout.Button(zone.zoneName, EditorStyles.label))
                {
                    SelectZone(zone);
                }
                GUI.color = Color.white;
                EditorGUILayout.LabelField($"L{zone.minLevel}-{zone.maxLevel}", GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void SelectZone(ZoneDefinition zone)
        {
            _selectedZone = zone;
            _serializedZone = zone != null ? new SerializedObject(zone) : null;
            _selectedSpawnIndex = -1;
            _selectedMotorPoolIndex = -1;
            _selectedCoverIndex = -1;
            _selectedHazardIndex = -1;
            _selectedEncounterIndex = -1;
            _selectedConnectionIndex = -1;
        }

        private void DrawZoneDetails()
        {
            if (_selectedZone == null)
            {
                EditorGUILayout.HelpBox("Select a zone to edit or create a new one.", MessageType.Info);
                return;
            }

            _serializedZone.Update();

            // Identity section
            _showZoneIdentity = EditorGUILayout.Foldout(_showZoneIdentity, "Zone Identity", true);
            if (_showZoneIdentity)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_serializedZone.FindProperty("zoneId"));
                EditorGUILayout.PropertyField(_serializedZone.FindProperty("zoneName"));
                EditorGUILayout.PropertyField(_serializedZone.FindProperty("zoneType"));
                EditorGUILayout.PropertyField(_serializedZone.FindProperty("description"));
                EditorGUILayout.PropertyField(_serializedZone.FindProperty("zoneIcon"));
                EditorGUILayout.PropertyField(_serializedZone.FindProperty("zoneColor"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // Level configuration
            _showLevelConfig = EditorGUILayout.Foldout(_showLevelConfig, "Level Configuration", true);
            if (_showLevelConfig)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_serializedZone.FindProperty("minLevel"));
                EditorGUILayout.PropertyField(_serializedZone.FindProperty("maxLevel"));
                EditorGUILayout.PropertyField(_serializedZone.FindProperty("recommendedLevel"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // Features
            _showFeatures = EditorGUILayout.Foldout(_showFeatures, "Zone Features", true);
            if (_showFeatures)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_serializedZone.FindProperty("features"));
                EditorGUILayout.PropertyField(_serializedZone.FindProperty("atmosphere"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            // Quick stats
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"Spawns: {_selectedZone.spawnPoints?.Length ?? 0}", GUILayout.Width(80));
            EditorGUILayout.LabelField($"Motor Pools: {_selectedZone.motorPools?.Length ?? 0}", GUILayout.Width(100));
            EditorGUILayout.LabelField($"Cover: {_selectedZone.coverPoints?.Length ?? 0}", GUILayout.Width(80));
            EditorGUILayout.LabelField($"Hazards: {_selectedZone.hazardAreas?.Length ?? 0}", GUILayout.Width(90));
            EditorGUILayout.LabelField($"Encounters: {_selectedZone.encounterZones?.Length ?? 0}", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            _serializedZone.ApplyModifiedProperties();
        }

        private void CreateNewZone()
        {
            string path = "Assets/ScriptableObjects/Zones";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Zones");
            }

            var zone = CreateInstance<ZoneDefinition>();
            zone.zoneId = $"zone_{_database.allZones.Count + 1}";
            zone.zoneName = $"New Zone {_database.allZones.Count + 1}";

            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/{zone.zoneName}.asset");
            AssetDatabase.CreateAsset(zone, assetPath);

            Undo.RecordObject(_database, "Add Zone");
            _database.AddZone(zone);
            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();

            SelectZone(zone);
        }

        private void DeleteSelectedZone()
        {
            if (_selectedZone == null) return;

            if (EditorUtility.DisplayDialog("Delete Zone",
                $"Are you sure you want to delete '{_selectedZone.zoneName}'?", "Delete", "Cancel"))
            {
                Undo.RecordObject(_database, "Delete Zone");
                _database.RemoveZone(_selectedZone);
                EditorUtility.SetDirty(_database);

                string path = AssetDatabase.GetAssetPath(_selectedZone);
                AssetDatabase.DeleteAsset(path);

                SelectZone(null);
            }
        }

        #endregion

        #region Spawn Points Tab

        private void DrawSpawnPointsTab()
        {
            if (_selectedZone == null)
            {
                EditorGUILayout.HelpBox("Select a zone in the Zone Editor tab first.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Spawn Points - {_selectedZone.zoneName}", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            // Left panel - spawn point list
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            DrawSpawnPointList();
            EditorGUILayout.EndVertical();

            // Right panel - spawn point details
            EditorGUILayout.BeginVertical();
            DrawSpawnPointDetails();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSpawnPointList()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Spawn Points", EditorStyles.boldLabel);
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                AddSpawnPoint();
            }
            if (GUILayout.Button("-", GUILayout.Width(25)) && _selectedSpawnIndex >= 0)
            {
                RemoveSpawnPoint(_selectedSpawnIndex);
            }
            EditorGUILayout.EndHorizontal();

            _spawnScrollPosition = EditorGUILayout.BeginScrollView(_spawnScrollPosition, GUILayout.Height(350));

            if (_selectedZone.spawnPoints != null)
            {
                for (int i = 0; i < _selectedZone.spawnPoints.Length; i++)
                {
                    var point = _selectedZone.spawnPoints[i];
                    bool isSelected = i == _selectedSpawnIndex;

                    var style = new GUIStyle(EditorStyles.label)
                    {
                        normal = { background = isSelected ? MakeTexture(2, 2, SelectedColor) : null }
                    };

                    EditorGUILayout.BeginHorizontal(style);
                    GUI.color = GetSpawnPointColor(point.spawnType);
                    if (GUILayout.Button(point.pointName ?? point.pointId, EditorStyles.label))
                    {
                        _selectedSpawnIndex = i;
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.LabelField(point.spawnType.ToString(), GUILayout.Width(80));
                    if (point.isDefault)
                    {
                        EditorGUILayout.LabelField("*", GUILayout.Width(15));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawSpawnPointDetails()
        {
            if (_selectedZone.spawnPoints == null || _selectedSpawnIndex < 0 ||
                _selectedSpawnIndex >= _selectedZone.spawnPoints.Length)
            {
                EditorGUILayout.HelpBox("Select a spawn point to edit.", MessageType.Info);
                return;
            }

            var point = _selectedZone.spawnPoints[_selectedSpawnIndex];

            EditorGUILayout.LabelField("Spawn Point Details", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();

            point.pointId = EditorGUILayout.TextField("Point ID", point.pointId);
            point.pointName = EditorGUILayout.TextField("Display Name", point.pointName);
            point.spawnType = (SpawnPointType)EditorGUILayout.EnumPopup("Spawn Type", point.spawnType);
            point.position = EditorGUILayout.Vector3Field("Position", point.position);
            point.rotation = EditorGUILayout.Vector3Field("Rotation", point.rotation);

            EditorGUILayout.Space(5);

            point.isDefault = EditorGUILayout.Toggle("Is Default Spawn", point.isDefault);
            point.fastTravelEnabled = EditorGUILayout.Toggle("Fast Travel Enabled", point.fastTravelEnabled);
            point.requiresDiscovery = EditorGUILayout.Toggle("Requires Discovery", point.requiresDiscovery);

            if (EditorGUI.EndChangeCheck())
            {
                _selectedZone.spawnPoints[_selectedSpawnIndex] = point;
                EditorUtility.SetDirty(_selectedZone);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Focus Scene View on Point", GUILayout.Height(25)))
            {
                SceneView.lastActiveSceneView?.LookAt(point.position);
            }
        }

        private void AddSpawnPoint()
        {
            Undo.RecordObject(_selectedZone, "Add Spawn Point");

            var newPoint = new SpawnPointData
            {
                pointId = $"spawn_{(_selectedZone.spawnPoints?.Length ?? 0) + 1}",
                pointName = "New Spawn Point",
                spawnType = SpawnPointType.Checkpoint,
                position = Vector3.zero,
                rotation = Vector3.zero
            };

            var list = _selectedZone.spawnPoints?.ToList() ?? new List<SpawnPointData>();
            list.Add(newPoint);
            _selectedZone.spawnPoints = list.ToArray();

            EditorUtility.SetDirty(_selectedZone);
            _selectedSpawnIndex = _selectedZone.spawnPoints.Length - 1;
        }

        private void RemoveSpawnPoint(int index)
        {
            Undo.RecordObject(_selectedZone, "Remove Spawn Point");

            var list = _selectedZone.spawnPoints.ToList();
            list.RemoveAt(index);
            _selectedZone.spawnPoints = list.ToArray();

            EditorUtility.SetDirty(_selectedZone);
            _selectedSpawnIndex = -1;
        }

        private Color GetSpawnPointColor(SpawnPointType type)
        {
            return type switch
            {
                SpawnPointType.ZoneEntry => new Color(0.2f, 0.9f, 0.2f),
                SpawnPointType.Checkpoint => new Color(0.4f, 0.6f, 0.9f),
                SpawnPointType.SafeRoom => new Color(0.9f, 0.9f, 0.2f),
                SpawnPointType.FastTravel => new Color(0.9f, 0.4f, 0.9f),
                SpawnPointType.BossCheckpoint => new Color(0.9f, 0.2f, 0.2f),
                SpawnPointType.CoopJoin => new Color(0.4f, 0.9f, 0.9f),
                SpawnPointType.RespawnAnchor => new Color(0.9f, 0.6f, 0.2f),
                _ => Color.white
            };
        }

        #endregion

        #region Motor Pools Tab

        private void DrawMotorPoolsTab()
        {
            if (_selectedZone == null)
            {
                EditorGUILayout.HelpBox("Select a zone in the Zone Editor tab first.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Motor Pool Stations - {_selectedZone.zoneName}", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Motor Pool stations are vehicle spawn points per GDD:\n" +
                "- Each player can summon their own vehicle\n" +
                "- 30-second cooldown between summons\n" +
                "- Fast travel available between discovered Motor Pools",
                MessageType.Info);

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            // Left panel - motor pool list
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            DrawMotorPoolList();
            EditorGUILayout.EndVertical();

            // Right panel - motor pool details
            EditorGUILayout.BeginVertical();
            DrawMotorPoolDetails();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawMotorPoolList()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Motor Pool Stations", EditorStyles.boldLabel);
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                AddMotorPool();
            }
            if (GUILayout.Button("-", GUILayout.Width(25)) && _selectedMotorPoolIndex >= 0)
            {
                RemoveMotorPool(_selectedMotorPoolIndex);
            }
            EditorGUILayout.EndHorizontal();

            _motorPoolScrollPosition = EditorGUILayout.BeginScrollView(_motorPoolScrollPosition, GUILayout.Height(350));

            if (_selectedZone.motorPools != null)
            {
                for (int i = 0; i < _selectedZone.motorPools.Length; i++)
                {
                    var pool = _selectedZone.motorPools[i];
                    bool isSelected = i == _selectedMotorPoolIndex;

                    var style = new GUIStyle(EditorStyles.label)
                    {
                        normal = { background = isSelected ? MakeTexture(2, 2, SelectedColor) : null }
                    };

                    EditorGUILayout.BeginHorizontal(style);
                    GUI.color = new Color(0.9f, 0.6f, 0.2f);
                    if (GUILayout.Button(pool.stationName ?? pool.stationId, EditorStyles.label))
                    {
                        _selectedMotorPoolIndex = i;
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.LabelField(pool.motorPoolType.ToString(), GUILayout.Width(80));
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawMotorPoolDetails()
        {
            if (_selectedZone.motorPools == null || _selectedMotorPoolIndex < 0 ||
                _selectedMotorPoolIndex >= _selectedZone.motorPools.Length)
            {
                EditorGUILayout.HelpBox("Select a Motor Pool station to edit.", MessageType.Info);
                return;
            }

            var pool = _selectedZone.motorPools[_selectedMotorPoolIndex];

            EditorGUILayout.LabelField("Motor Pool Details", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();

            pool.stationId = EditorGUILayout.TextField("Station ID", pool.stationId);
            pool.stationName = EditorGUILayout.TextField("Display Name", pool.stationName);
            pool.motorPoolType = (MotorPoolType)EditorGUILayout.EnumPopup("Motor Pool Type", pool.motorPoolType);
            pool.position = EditorGUILayout.Vector3Field("Position", pool.position);
            pool.spawnDirection = EditorGUILayout.Vector3Field("Spawn Direction", pool.spawnDirection);

            EditorGUILayout.Space(5);

            pool.requiresDiscovery = EditorGUILayout.Toggle("Requires Discovery", pool.requiresDiscovery);
            pool.hasFastTravel = EditorGUILayout.Toggle("Has Fast Travel", pool.hasFastTravel);

            EditorGUILayout.Space(5);

            // Available vehicles
            EditorGUILayout.LabelField("Available Vehicles", EditorStyles.miniBoldLabel);
            int vehicleCount = pool.availableVehicles?.Length ?? 0;
            int newCount = EditorGUILayout.IntField("Count", vehicleCount);
            if (newCount != vehicleCount)
            {
                Array.Resize(ref pool.availableVehicles, newCount);
            }

            if (pool.availableVehicles != null)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < pool.availableVehicles.Length; i++)
                {
                    pool.availableVehicles[i] = EditorGUILayout.TextField($"Vehicle {i + 1}", pool.availableVehicles[i]);
                }
                EditorGUI.indentLevel--;
            }

            if (EditorGUI.EndChangeCheck())
            {
                _selectedZone.motorPools[_selectedMotorPoolIndex] = pool;
                EditorUtility.SetDirty(_selectedZone);
            }

            EditorGUILayout.EndVertical();
        }

        private void AddMotorPool()
        {
            Undo.RecordObject(_selectedZone, "Add Motor Pool");

            var newPool = new MotorPoolData
            {
                stationId = $"motorpool_{(_selectedZone.motorPools?.Length ?? 0) + 1}",
                stationName = "New Motor Pool",
                motorPoolType = MotorPoolType.Standard,
                position = Vector3.zero,
                spawnDirection = Vector3.forward,
                availableVehicles = new[] { "Strider", "Carrier" }
            };

            var list = _selectedZone.motorPools?.ToList() ?? new List<MotorPoolData>();
            list.Add(newPool);
            _selectedZone.motorPools = list.ToArray();

            // Enable motor pool feature
            _selectedZone.features |= ZoneFeatures.HasMotorPool;

            EditorUtility.SetDirty(_selectedZone);
            _selectedMotorPoolIndex = _selectedZone.motorPools.Length - 1;
        }

        private void RemoveMotorPool(int index)
        {
            Undo.RecordObject(_selectedZone, "Remove Motor Pool");

            var list = _selectedZone.motorPools.ToList();
            list.RemoveAt(index);
            _selectedZone.motorPools = list.ToArray();

            if (_selectedZone.motorPools.Length == 0)
            {
                _selectedZone.features &= ~ZoneFeatures.HasMotorPool;
            }

            EditorUtility.SetDirty(_selectedZone);
            _selectedMotorPoolIndex = -1;
        }

        #endregion

        #region Cover Points Tab

        private void DrawCoverPointsTab()
        {
            if (_selectedZone == null)
            {
                EditorGUILayout.HelpBox("Select a zone in the Zone Editor tab first.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Cover Points - {_selectedZone.zoneName}", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Cover points define tactical positions for the Division-style cover system.\n" +
                "Players can snap to cover, peek, and blind fire from these positions.",
                MessageType.Info);

            EditorGUILayout.Space(5);

            // Cover generation settings
            _showCoverGenerationSettings = EditorGUILayout.Foldout(_showCoverGenerationSettings, "Auto-Generation Settings", true);
            if (_showCoverGenerationSettings)
            {
                EditorGUI.indentLevel++;
                _coverGenerationRadius = EditorGUILayout.FloatField("Search Radius", _coverGenerationRadius);
                _coverMinSpacing = EditorGUILayout.FloatField("Min Spacing", _coverMinSpacing);
                _coverLayerMask = EditorGUILayout.MaskField("Layer Mask",
                    _coverLayerMask, UnityEditorInternal.InternalEditorUtility.layers);

                if (GUILayout.Button("Generate Cover Points (Scene Scan)"))
                {
                    GenerateCoverPoints();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            // Left panel - cover point list
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            DrawCoverPointList();
            EditorGUILayout.EndVertical();

            // Right panel - cover point details
            EditorGUILayout.BeginVertical();
            DrawCoverPointDetails();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawCoverPointList()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Cover Points ({_selectedZone.coverPoints?.Length ?? 0})", EditorStyles.boldLabel);
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                AddCoverPoint();
            }
            if (GUILayout.Button("-", GUILayout.Width(25)) && _selectedCoverIndex >= 0)
            {
                RemoveCoverPoint(_selectedCoverIndex);
            }
            if (GUILayout.Button("Clear", GUILayout.Width(40)))
            {
                ClearCoverPoints();
            }
            EditorGUILayout.EndHorizontal();

            _coverScrollPosition = EditorGUILayout.BeginScrollView(_coverScrollPosition, GUILayout.Height(300));

            if (_selectedZone.coverPoints != null)
            {
                for (int i = 0; i < _selectedZone.coverPoints.Length; i++)
                {
                    var cover = _selectedZone.coverPoints[i];
                    bool isSelected = i == _selectedCoverIndex;

                    var style = new GUIStyle(EditorStyles.label)
                    {
                        normal = { background = isSelected ? MakeTexture(2, 2, SelectedColor) : null }
                    };

                    EditorGUILayout.BeginHorizontal(style);
                    GUI.color = GetCoverTypeColor(cover.coverType);
                    if (GUILayout.Button($"Cover {i + 1}", EditorStyles.label))
                    {
                        _selectedCoverIndex = i;
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.LabelField(cover.coverType.ToString(), GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawCoverPointDetails()
        {
            if (_selectedZone.coverPoints == null || _selectedCoverIndex < 0 ||
                _selectedCoverIndex >= _selectedZone.coverPoints.Length)
            {
                EditorGUILayout.HelpBox("Select a cover point to edit.", MessageType.Info);
                return;
            }

            var cover = _selectedZone.coverPoints[_selectedCoverIndex];

            EditorGUILayout.LabelField("Cover Point Details", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();

            cover.coverType = (CoverType)EditorGUILayout.EnumPopup("Cover Type", cover.coverType);
            cover.position = EditorGUILayout.Vector3Field("Position", cover.position);
            cover.forward = EditorGUILayout.Vector3Field("Forward Direction", cover.forward);
            cover.width = EditorGUILayout.FloatField("Width", cover.width);
            cover.height = EditorGUILayout.FloatField("Height", cover.height);

            EditorGUILayout.Space(5);

            cover.isDestructible = EditorGUILayout.Toggle("Is Destructible", cover.isDestructible);
            if (cover.isDestructible)
            {
                cover.health = EditorGUILayout.FloatField("Health", cover.health);
            }

            if (EditorGUI.EndChangeCheck())
            {
                _selectedZone.coverPoints[_selectedCoverIndex] = cover;
                EditorUtility.SetDirty(_selectedZone);
            }

            EditorGUILayout.EndVertical();
        }

        private void AddCoverPoint()
        {
            Undo.RecordObject(_selectedZone, "Add Cover Point");

            var newCover = new CoverPointData
            {
                coverType = CoverType.LowCover,
                position = Vector3.zero,
                forward = Vector3.forward,
                width = 2f,
                height = 1f
            };

            var list = _selectedZone.coverPoints?.ToList() ?? new List<CoverPointData>();
            list.Add(newCover);
            _selectedZone.coverPoints = list.ToArray();

            EditorUtility.SetDirty(_selectedZone);
            _selectedCoverIndex = _selectedZone.coverPoints.Length - 1;
        }

        private void RemoveCoverPoint(int index)
        {
            Undo.RecordObject(_selectedZone, "Remove Cover Point");

            var list = _selectedZone.coverPoints.ToList();
            list.RemoveAt(index);
            _selectedZone.coverPoints = list.ToArray();

            EditorUtility.SetDirty(_selectedZone);
            _selectedCoverIndex = -1;
        }

        private void ClearCoverPoints()
        {
            if (EditorUtility.DisplayDialog("Clear Cover Points",
                "Are you sure you want to remove all cover points?", "Clear", "Cancel"))
            {
                Undo.RecordObject(_selectedZone, "Clear Cover Points");
                _selectedZone.coverPoints = new CoverPointData[0];
                EditorUtility.SetDirty(_selectedZone);
                _selectedCoverIndex = -1;
            }
        }

        private void GenerateCoverPoints()
        {
            EditorUtility.DisplayDialog("Cover Generation",
                "Cover point auto-generation requires scene geometry.\n\n" +
                "This feature scans the scene for objects that can provide cover " +
                "based on height and width criteria.\n\n" +
                "For now, please manually place cover points or implement " +
                "runtime cover detection in gameplay code.",
                "OK");
        }

        private Color GetCoverTypeColor(CoverType type)
        {
            return type switch
            {
                CoverType.LowCover => new Color(0.4f, 0.6f, 0.9f),
                CoverType.HighCover => new Color(0.2f, 0.4f, 0.8f),
                CoverType.DestructibleLow => new Color(0.9f, 0.6f, 0.4f),
                CoverType.DestructibleHigh => new Color(0.8f, 0.4f, 0.2f),
                CoverType.VaultableLow => new Color(0.4f, 0.9f, 0.4f),
                CoverType.CornerCover => new Color(0.9f, 0.9f, 0.4f),
                CoverType.VehicleCover => new Color(0.6f, 0.6f, 0.6f),
                _ => Color.white
            };
        }

        #endregion

        #region Hazards Tab

        private void DrawHazardsTab()
        {
            if (_selectedZone == null)
            {
                EditorGUILayout.HelpBox("Select a zone in the Zone Editor tab first.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Environmental Hazards - {_selectedZone.zoneName}", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Environmental hazards create danger zones that damage players.\n" +
                "Types: Radiation, Toxic, Fire, Electric, Cryo, Void, Explosive, Falling, Crushing",
                MessageType.Info);

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            // Left panel
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            DrawHazardList();
            EditorGUILayout.EndVertical();

            // Right panel
            EditorGUILayout.BeginVertical();
            DrawHazardDetails();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawHazardList()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Hazard Areas", EditorStyles.boldLabel);
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                AddHazard();
            }
            if (GUILayout.Button("-", GUILayout.Width(25)) && _selectedHazardIndex >= 0)
            {
                RemoveHazard(_selectedHazardIndex);
            }
            EditorGUILayout.EndHorizontal();

            _hazardScrollPosition = EditorGUILayout.BeginScrollView(_hazardScrollPosition, GUILayout.Height(350));

            if (_selectedZone.hazardAreas != null)
            {
                for (int i = 0; i < _selectedZone.hazardAreas.Length; i++)
                {
                    var hazard = _selectedZone.hazardAreas[i];
                    bool isSelected = i == _selectedHazardIndex;

                    var style = new GUIStyle(EditorStyles.label)
                    {
                        normal = { background = isSelected ? MakeTexture(2, 2, SelectedColor) : null }
                    };

                    EditorGUILayout.BeginHorizontal(style);
                    GUI.color = GetHazardColor(hazard.hazardType);
                    if (GUILayout.Button(hazard.hazardId ?? $"Hazard {i + 1}", EditorStyles.label))
                    {
                        _selectedHazardIndex = i;
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.LabelField(hazard.hazardType.ToString(), GUILayout.Width(80));
                    EditorGUILayout.LabelField($"{hazard.damagePerSecond}/s", GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawHazardDetails()
        {
            if (_selectedZone.hazardAreas == null || _selectedHazardIndex < 0 ||
                _selectedHazardIndex >= _selectedZone.hazardAreas.Length)
            {
                EditorGUILayout.HelpBox("Select a hazard area to edit.", MessageType.Info);
                return;
            }

            var hazard = _selectedZone.hazardAreas[_selectedHazardIndex];

            EditorGUILayout.LabelField("Hazard Details", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();

            hazard.hazardId = EditorGUILayout.TextField("Hazard ID", hazard.hazardId);
            hazard.hazardType = (HazardType)EditorGUILayout.EnumPopup("Hazard Type", hazard.hazardType);
            hazard.position = EditorGUILayout.Vector3Field("Center Position", hazard.position);
            hazard.radius = EditorGUILayout.FloatField("Radius", hazard.radius);
            hazard.height = EditorGUILayout.FloatField("Height", hazard.height);
            hazard.damagePerSecond = EditorGUILayout.FloatField("Damage/Second", hazard.damagePerSecond);

            EditorGUILayout.Space(5);

            hazard.alwaysActive = EditorGUILayout.Toggle("Always Active", hazard.alwaysActive);
            if (!hazard.alwaysActive)
            {
                hazard.triggerCondition = EditorGUILayout.TextField("Trigger Condition", hazard.triggerCondition);
            }
            hazard.canBeDisabled = EditorGUILayout.Toggle("Can Be Disabled", hazard.canBeDisabled);
            hazard.warningTime = EditorGUILayout.FloatField("Warning Time", hazard.warningTime);

            if (EditorGUI.EndChangeCheck())
            {
                _selectedZone.hazardAreas[_selectedHazardIndex] = hazard;
                EditorUtility.SetDirty(_selectedZone);
            }

            EditorGUILayout.EndVertical();
        }

        private void AddHazard()
        {
            Undo.RecordObject(_selectedZone, "Add Hazard");

            var newHazard = new HazardAreaData
            {
                hazardId = $"hazard_{(_selectedZone.hazardAreas?.Length ?? 0) + 1}",
                hazardType = HazardType.Radiation,
                position = Vector3.zero,
                radius = 5f,
                height = 3f,
                damagePerSecond = 10f,
                alwaysActive = true,
                warningTime = 1f
            };

            var list = _selectedZone.hazardAreas?.ToList() ?? new List<HazardAreaData>();
            list.Add(newHazard);
            _selectedZone.hazardAreas = list.ToArray();

            EditorUtility.SetDirty(_selectedZone);
            _selectedHazardIndex = _selectedZone.hazardAreas.Length - 1;
        }

        private void RemoveHazard(int index)
        {
            Undo.RecordObject(_selectedZone, "Remove Hazard");

            var list = _selectedZone.hazardAreas.ToList();
            list.RemoveAt(index);
            _selectedZone.hazardAreas = list.ToArray();

            EditorUtility.SetDirty(_selectedZone);
            _selectedHazardIndex = -1;
        }

        private Color GetHazardColor(HazardType type)
        {
            return type switch
            {
                HazardType.Radiation => new Color(0.2f, 0.9f, 0.2f),
                HazardType.Toxic => new Color(0.4f, 0.8f, 0.2f),
                HazardType.Fire => new Color(0.9f, 0.4f, 0.1f),
                HazardType.Electric => new Color(0.9f, 0.9f, 0.2f),
                HazardType.Cryo => new Color(0.4f, 0.8f, 0.9f),
                HazardType.Void => new Color(0.5f, 0.2f, 0.8f),
                HazardType.Explosive => new Color(0.9f, 0.2f, 0.2f),
                HazardType.Falling => new Color(0.2f, 0.2f, 0.2f),
                HazardType.Crushing => new Color(0.6f, 0.4f, 0.2f),
                _ => Color.white
            };
        }

        #endregion

        #region Encounters Tab

        private void DrawEncountersTab()
        {
            if (_selectedZone == null)
            {
                EditorGUILayout.HelpBox("Select a zone in the Zone Editor tab first.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Random Encounters - {_selectedZone.zoneName}", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Random encounter zones trigger enemy spawns when players enter.\n" +
                "Types: Patrol, Ambush, Defend Point, Assault, Convoy, Boss, Treasure, Rescue, Hunt Target",
                MessageType.Info);

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            // Left panel
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            DrawEncounterList();
            EditorGUILayout.EndVertical();

            // Right panel
            EditorGUILayout.BeginVertical();
            DrawEncounterDetails();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawEncounterList()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Encounter Zones", EditorStyles.boldLabel);
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                AddEncounter();
            }
            if (GUILayout.Button("-", GUILayout.Width(25)) && _selectedEncounterIndex >= 0)
            {
                RemoveEncounter(_selectedEncounterIndex);
            }
            EditorGUILayout.EndHorizontal();

            _encounterScrollPosition = EditorGUILayout.BeginScrollView(_encounterScrollPosition, GUILayout.Height(350));

            if (_selectedZone.encounterZones != null)
            {
                for (int i = 0; i < _selectedZone.encounterZones.Length; i++)
                {
                    var encounter = _selectedZone.encounterZones[i];
                    bool isSelected = i == _selectedEncounterIndex;

                    var style = new GUIStyle(EditorStyles.label)
                    {
                        normal = { background = isSelected ? MakeTexture(2, 2, SelectedColor) : null }
                    };

                    EditorGUILayout.BeginHorizontal(style);
                    GUI.color = GetEncounterColor(encounter.encounterType);
                    if (GUILayout.Button(encounter.encounterId ?? $"Encounter {i + 1}", EditorStyles.label))
                    {
                        _selectedEncounterIndex = i;
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.LabelField(encounter.encounterType.ToString(), GUILayout.Width(80));
                    EditorGUILayout.LabelField(encounter.difficulty.ToString(), GUILayout.Width(60));
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawEncounterDetails()
        {
            if (_selectedZone.encounterZones == null || _selectedEncounterIndex < 0 ||
                _selectedEncounterIndex >= _selectedZone.encounterZones.Length)
            {
                EditorGUILayout.HelpBox("Select an encounter zone to edit.", MessageType.Info);
                return;
            }

            var encounter = _selectedZone.encounterZones[_selectedEncounterIndex];

            EditorGUILayout.LabelField("Encounter Details", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();

            encounter.encounterId = EditorGUILayout.TextField("Encounter ID", encounter.encounterId);
            encounter.encounterType = (EncounterType)EditorGUILayout.EnumPopup("Encounter Type", encounter.encounterType);
            encounter.difficulty = (EncounterDifficulty)EditorGUILayout.EnumPopup("Difficulty", encounter.difficulty);
            encounter.position = EditorGUILayout.Vector3Field("Center Position", encounter.position);
            encounter.triggerRadius = EditorGUILayout.FloatField("Trigger Radius", encounter.triggerRadius);

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Enemy Configuration", EditorStyles.miniBoldLabel);
            encounter.minEnemies = EditorGUILayout.IntField("Min Enemies", encounter.minEnemies);
            encounter.maxEnemies = EditorGUILayout.IntField("Max Enemies", encounter.maxEnemies);
            encounter.triggerChance = EditorGUILayout.Slider("Trigger Chance", encounter.triggerChance, 0f, 1f);
            encounter.badassChance = EditorGUILayout.Slider("Badass Chance", encounter.badassChance, 0f, 1f);
            encounter.cooldownTime = EditorGUILayout.FloatField("Cooldown (sec)", encounter.cooldownTime);
            encounter.oneTimeOnly = EditorGUILayout.Toggle("One-Time Only", encounter.oneTimeOnly);

            EditorGUILayout.Space(5);

            // Spawn positions
            EditorGUILayout.LabelField("Spawn Positions", EditorStyles.miniBoldLabel);
            int spawnCount = encounter.spawnPositions?.Length ?? 0;
            int newSpawnCount = EditorGUILayout.IntField("Spawn Point Count", spawnCount);
            if (newSpawnCount != spawnCount)
            {
                Array.Resize(ref encounter.spawnPositions, newSpawnCount);
            }

            if (encounter.spawnPositions != null && encounter.spawnPositions.Length > 0)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < Mathf.Min(encounter.spawnPositions.Length, 5); i++)
                {
                    encounter.spawnPositions[i] = EditorGUILayout.Vector3Field($"Position {i + 1}", encounter.spawnPositions[i]);
                }
                if (encounter.spawnPositions.Length > 5)
                {
                    EditorGUILayout.LabelField($"... and {encounter.spawnPositions.Length - 5} more");
                }
                EditorGUI.indentLevel--;
            }

            // Allowed enemy types
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Allowed Enemy Types", EditorStyles.miniBoldLabel);
            int typeCount = encounter.allowedEnemyTypes?.Length ?? 0;
            int newTypeCount = EditorGUILayout.IntField("Enemy Type Count", typeCount);
            if (newTypeCount != typeCount)
            {
                Array.Resize(ref encounter.allowedEnemyTypes, newTypeCount);
            }

            if (encounter.allowedEnemyTypes != null)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < encounter.allowedEnemyTypes.Length; i++)
                {
                    encounter.allowedEnemyTypes[i] = EditorGUILayout.TextField($"Type {i + 1}", encounter.allowedEnemyTypes[i]);
                }
                EditorGUI.indentLevel--;
            }

            if (EditorGUI.EndChangeCheck())
            {
                _selectedZone.encounterZones[_selectedEncounterIndex] = encounter;
                EditorUtility.SetDirty(_selectedZone);
            }

            EditorGUILayout.EndVertical();
        }

        private void AddEncounter()
        {
            Undo.RecordObject(_selectedZone, "Add Encounter");

            var newEncounter = new EncounterZoneData
            {
                encounterId = $"encounter_{(_selectedZone.encounterZones?.Length ?? 0) + 1}",
                encounterType = EncounterType.Patrol,
                difficulty = EncounterDifficulty.Normal,
                position = Vector3.zero,
                triggerRadius = 15f,
                minEnemies = 3,
                maxEnemies = 6,
                triggerChance = 0.5f,
                badassChance = 0.1f,
                cooldownTime = 300f,
                spawnPositions = new Vector3[3],
                allowedEnemyTypes = new[] { "Scrapper", "Drone" }
            };

            var list = _selectedZone.encounterZones?.ToList() ?? new List<EncounterZoneData>();
            list.Add(newEncounter);
            _selectedZone.encounterZones = list.ToArray();

            EditorUtility.SetDirty(_selectedZone);
            _selectedEncounterIndex = _selectedZone.encounterZones.Length - 1;
        }

        private void RemoveEncounter(int index)
        {
            Undo.RecordObject(_selectedZone, "Remove Encounter");

            var list = _selectedZone.encounterZones.ToList();
            list.RemoveAt(index);
            _selectedZone.encounterZones = list.ToArray();

            EditorUtility.SetDirty(_selectedZone);
            _selectedEncounterIndex = -1;
        }

        private Color GetEncounterColor(EncounterType type)
        {
            return type switch
            {
                EncounterType.Patrol => new Color(0.4f, 0.6f, 0.9f),
                EncounterType.Ambush => new Color(0.9f, 0.4f, 0.2f),
                EncounterType.DefendPoint => new Color(0.2f, 0.8f, 0.4f),
                EncounterType.Assault => new Color(0.9f, 0.2f, 0.2f),
                EncounterType.Convoy => new Color(0.9f, 0.9f, 0.4f),
                EncounterType.Boss => new Color(0.9f, 0.2f, 0.9f),
                EncounterType.Treasure => new Color(0.9f, 0.7f, 0.2f),
                EncounterType.Rescue => new Color(0.4f, 0.9f, 0.9f),
                EncounterType.HuntTarget => new Color(0.9f, 0.5f, 0.0f),
                _ => Color.white
            };
        }

        #endregion

        #region Connections Tab

        private void DrawConnectionsTab()
        {
            if (_selectedZone == null)
            {
                EditorGUILayout.HelpBox("Select a zone in the Zone Editor tab first.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Zone Connections - {_selectedZone.zoneName}", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Zone connections define how players travel between zones.\n" +
                "Types: Door, Gate, Tunnel, Elevator, Teleporter, Vehicle Ramp, Climbing Path, Secret Passage",
                MessageType.Info);

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            // Left panel
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            DrawConnectionList();
            EditorGUILayout.EndVertical();

            // Right panel
            EditorGUILayout.BeginVertical();
            DrawConnectionDetails();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawConnectionList()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Connections", EditorStyles.boldLabel);
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                AddConnection();
            }
            if (GUILayout.Button("-", GUILayout.Width(25)) && _selectedConnectionIndex >= 0)
            {
                RemoveConnection(_selectedConnectionIndex);
            }
            EditorGUILayout.EndHorizontal();

            _connectionScrollPosition = EditorGUILayout.BeginScrollView(_connectionScrollPosition, GUILayout.Height(350));

            if (_selectedZone.connections != null)
            {
                for (int i = 0; i < _selectedZone.connections.Length; i++)
                {
                    var connection = _selectedZone.connections[i];
                    bool isSelected = i == _selectedConnectionIndex;

                    var style = new GUIStyle(EditorStyles.label)
                    {
                        normal = { background = isSelected ? MakeTexture(2, 2, SelectedColor) : null }
                    };

                    EditorGUILayout.BeginHorizontal(style);
                    GUI.color = new Color(0.4f, 0.8f, 0.8f);
                    string targetName = connection.targetZone != null ? connection.targetZone.zoneName : "(None)";
                    if (GUILayout.Button($"-> {targetName}", EditorStyles.label))
                    {
                        _selectedConnectionIndex = i;
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.LabelField(connection.connectionType.ToString(), GUILayout.Width(80));
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawConnectionDetails()
        {
            if (_selectedZone.connections == null || _selectedConnectionIndex < 0 ||
                _selectedConnectionIndex >= _selectedZone.connections.Length)
            {
                EditorGUILayout.HelpBox("Select a connection to edit.", MessageType.Info);
                return;
            }

            var connection = _selectedZone.connections[_selectedConnectionIndex];

            EditorGUILayout.LabelField("Connection Details", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();

            connection.connectionId = EditorGUILayout.TextField("Connection ID", connection.connectionId);
            connection.connectionType = (ConnectionType)EditorGUILayout.EnumPopup("Connection Type", connection.connectionType);

            EditorGUILayout.Space(5);

            connection.targetZone = (ZoneDefinition)EditorGUILayout.ObjectField(
                "Target Zone", connection.targetZone, typeof(ZoneDefinition), false);
            connection.targetSpawnPointId = EditorGUILayout.TextField("Target Spawn Point ID", connection.targetSpawnPointId);

            EditorGUILayout.Space(5);

            connection.position = EditorGUILayout.Vector3Field("Trigger Position", connection.position);
            connection.triggerSize = EditorGUILayout.Vector3Field("Trigger Size", connection.triggerSize);

            EditorGUILayout.Space(5);

            connection.requiresConfirmation = EditorGUILayout.Toggle("Requires Confirmation", connection.requiresConfirmation);
            connection.vehicleOnly = EditorGUILayout.Toggle("Vehicle Only", connection.vehicleOnly);
            connection.minLevelRequired = EditorGUILayout.IntField("Min Level Required", connection.minLevelRequired);
            connection.requiredQuestId = EditorGUILayout.TextField("Required Quest ID", connection.requiredQuestId);

            if (EditorGUI.EndChangeCheck())
            {
                _selectedZone.connections[_selectedConnectionIndex] = connection;
                EditorUtility.SetDirty(_selectedZone);
            }

            EditorGUILayout.EndVertical();

            // Show connected zones
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("All Connected Zones", EditorStyles.boldLabel);
            var connectedZones = _database.GetConnectedZones(_selectedZone);
            foreach (var zone in connectedZones)
            {
                GUI.color = zone.GetZoneTypeColor();
                EditorGUILayout.LabelField($"  - {zone.zoneName}");
            }
            GUI.color = Color.white;
        }

        private void AddConnection()
        {
            Undo.RecordObject(_selectedZone, "Add Connection");

            var newConnection = new ZoneConnectionData
            {
                connectionId = $"connection_{(_selectedZone.connections?.Length ?? 0) + 1}",
                connectionType = ConnectionType.Door,
                position = Vector3.zero,
                triggerSize = new Vector3(3f, 3f, 1f)
            };

            var list = _selectedZone.connections?.ToList() ?? new List<ZoneConnectionData>();
            list.Add(newConnection);
            _selectedZone.connections = list.ToArray();

            EditorUtility.SetDirty(_selectedZone);
            _selectedConnectionIndex = _selectedZone.connections.Length - 1;
        }

        private void RemoveConnection(int index)
        {
            Undo.RecordObject(_selectedZone, "Remove Connection");

            var list = _selectedZone.connections.ToList();
            list.RemoveAt(index);
            _selectedZone.connections = list.ToArray();

            EditorUtility.SetDirty(_selectedZone);
            _selectedConnectionIndex = -1;
        }

        #endregion

        #region Validation Tab

        private void DrawValidationTab()
        {
            EditorGUILayout.LabelField("Zone Validation", EditorStyles.boldLabel);

            if (GUILayout.Button("Validate All Zones", GUILayout.Height(30)))
            {
                _validationResults = _database.ValidateAll();
            }

            EditorGUILayout.Space(10);

            if (_validationResults == null)
            {
                EditorGUILayout.HelpBox("Click 'Validate All Zones' to check for issues.", MessageType.Info);
                return;
            }

            if (_validationResults.Count == 0)
            {
                EditorGUILayout.HelpBox("All zones passed validation!", MessageType.Info);
                return;
            }

            EditorGUILayout.HelpBox($"Found {_validationResults.Count} zone(s) with issues.", MessageType.Warning);

            EditorGUILayout.Space(5);

            foreach (var (zone, errors) in _validationResults)
            {
                string zoneName = zone != null ? zone.zoneName : "Unknown Zone";
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(zoneName, EditorStyles.boldLabel);

                foreach (var error in errors)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUI.color = Color.yellow;
                    EditorGUILayout.LabelField("!", GUILayout.Width(15));
                    GUI.color = Color.white;
                    EditorGUILayout.LabelField(error);
                    EditorGUILayout.EndHorizontal();
                }

                if (zone != null)
                {
                    if (GUILayout.Button("Select Zone", GUILayout.Width(100)))
                    {
                        SelectZone(zone);
                        _selectedTab = 1; // Switch to zone editor
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        #endregion

        #region Zone Generation

        private void GenerateStoryZones()
        {
            if (!EditorUtility.DisplayDialog("Generate Story Zones",
                "This will create the 6 story zones from the GDD:\n" +
                "- Haven City (Hub)\n" +
                "- Fractured Coast (1-10)\n" +
                "- Scorched Plateau (10-20)\n" +
                "- Crystal Caverns (20-30)\n" +
                "- Nexus Wasteland (30-40)\n" +
                "- Core Facility (40-50)\n\n" +
                "Continue?", "Generate", "Cancel"))
            {
                return;
            }

            string path = "Assets/ScriptableObjects/Zones";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Zones");
            }

            var storyZones = new[]
            {
                (ZoneType.HavenCity, "Haven City", 1, 5, 3, ZoneFeatures.IsSafeZone | ZoneFeatures.HasVendor | ZoneFeatures.HasFastTravel | ZoneFeatures.HasMotorPool),
                (ZoneType.FracturedCoast, "Fractured Coast", 1, 10, 5, ZoneFeatures.HasVehicles | ZoneFeatures.HasMotorPool | ZoneFeatures.HasFastTravel),
                (ZoneType.ScorchedPlateau, "Scorched Plateau", 10, 20, 15, ZoneFeatures.HasVehicles | ZoneFeatures.HasMotorPool | ZoneFeatures.HasFastTravel | ZoneFeatures.HasBoss),
                (ZoneType.CrystalCaverns, "Crystal Caverns", 20, 30, 25, ZoneFeatures.HasVehicles | ZoneFeatures.HasMotorPool | ZoneFeatures.HasFastTravel | ZoneFeatures.HasSecrets),
                (ZoneType.NexusWasteland, "Nexus Wasteland", 30, 40, 35, ZoneFeatures.HasVehicles | ZoneFeatures.HasMotorPool | ZoneFeatures.HasFastTravel | ZoneFeatures.HasBoss),
                (ZoneType.CoreFacility, "Core Facility", 40, 50, 45, ZoneFeatures.HasVehicles | ZoneFeatures.HasMotorPool | ZoneFeatures.HasFastTravel | ZoneFeatures.HasBoss | ZoneFeatures.IsMayhemEnabled)
            };

            foreach (var (zoneType, name, minLvl, maxLvl, recLvl, features) in storyZones)
            {
                // Check if already exists
                if (_database.allZones.Any(z => z != null && z.zoneType == zoneType))
                    continue;

                var zone = CreateInstance<ZoneDefinition>();
                zone.zoneId = name.Replace(" ", "_").ToLower();
                zone.zoneName = name;
                zone.zoneType = zoneType;
                zone.minLevel = minLvl;
                zone.maxLevel = maxLvl;
                zone.recommendedLevel = recLvl;
                zone.features = features;

                // Add default spawn point
                zone.spawnPoints = new[]
                {
                    new SpawnPointData
                    {
                        pointId = $"{zone.zoneId}_entry",
                        pointName = $"{name} Entry",
                        spawnType = SpawnPointType.ZoneEntry,
                        isDefault = true,
                        fastTravelEnabled = true
                    }
                };

                // Add motor pool if feature enabled
                if (features.HasFlag(ZoneFeatures.HasMotorPool))
                {
                    zone.motorPools = new[]
                    {
                        new MotorPoolData
                        {
                            stationId = $"{zone.zoneId}_motorpool",
                            stationName = $"{name} Motor Pool",
                            motorPoolType = MotorPoolType.Outpost,
                            availableVehicles = new[] { "Strider", "Carrier" },
                            hasFastTravel = true
                        }
                    };
                }

                string assetPath = $"{path}/{zone.zoneName.Replace(" ", "")}.asset";
                AssetDatabase.CreateAsset(zone, assetPath);

                _database.AddZone(zone);

                // Set as hub/starting zone if appropriate
                if (zoneType == ZoneType.HavenCity)
                {
                    _database.hubZone = zone;
                    _database.startingZone = zone;
                }
            }

            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Generation Complete",
                $"Generated {storyZones.Length} story zones.", "OK");
        }

        private void GenerateEndgameZones()
        {
            if (!EditorUtility.DisplayDialog("Generate Endgame Zones",
                "This will create endgame zones from the GDD:\n" +
                "- Proving Ground Alpha/Beta/Gamma\n" +
                "- Circle of Slaughter\n" +
                "- Raid: NEXUS Core\n" +
                "- Raid: Temporal Paradox\n" +
                "- Raid: The Singularity\n" +
                "- The Spiral (Infinite Dungeon)\n\n" +
                "Continue?", "Generate", "Cancel"))
            {
                return;
            }

            string path = "Assets/ScriptableObjects/Zones";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Zones");
            }

            var endgameZones = new[]
            {
                (ZoneType.ProvingGround_Alpha, "Proving Ground Alpha", ZoneFeatures.IsEndgame | ZoneFeatures.IsMayhemEnabled),
                (ZoneType.ProvingGround_Beta, "Proving Ground Beta", ZoneFeatures.IsEndgame | ZoneFeatures.IsMayhemEnabled),
                (ZoneType.ProvingGround_Gamma, "Proving Ground Gamma", ZoneFeatures.IsEndgame | ZoneFeatures.IsMayhemEnabled),
                (ZoneType.CircleOfSlaughter, "Circle of Slaughter", ZoneFeatures.IsEndgame | ZoneFeatures.IsMayhemEnabled),
                (ZoneType.Raid_NexusCore, "Raid: NEXUS Core", ZoneFeatures.IsEndgame | ZoneFeatures.HasRaid | ZoneFeatures.HasBoss),
                (ZoneType.Raid_TemporalParadox, "Raid: Temporal Paradox", ZoneFeatures.IsEndgame | ZoneFeatures.HasRaid | ZoneFeatures.HasBoss),
                (ZoneType.Raid_Singularity, "Raid: The Singularity", ZoneFeatures.IsEndgame | ZoneFeatures.HasRaid | ZoneFeatures.HasBoss),
                (ZoneType.TheSpiral, "The Spiral", ZoneFeatures.IsEndgame | ZoneFeatures.IsProcedurallyGenerated | ZoneFeatures.IsMayhemEnabled)
            };

            foreach (var (zoneType, name, features) in endgameZones)
            {
                // Check if already exists
                if (_database.allZones.Any(z => z != null && z.zoneType == zoneType))
                    continue;

                var zone = CreateInstance<ZoneDefinition>();
                zone.zoneId = name.Replace(" ", "_").Replace(":", "").ToLower();
                zone.zoneName = name;
                zone.zoneType = zoneType;
                zone.minLevel = 50;
                zone.maxLevel = 50;
                zone.recommendedLevel = 50;
                zone.features = features;

                // Add default spawn point
                zone.spawnPoints = new[]
                {
                    new SpawnPointData
                    {
                        pointId = $"{zone.zoneId}_entry",
                        pointName = $"{name} Entry",
                        spawnType = SpawnPointType.ZoneEntry,
                        isDefault = true,
                        fastTravelEnabled = false
                    }
                };

                string assetPath = $"{path}/{zone.zoneId}.asset";
                AssetDatabase.CreateAsset(zone, assetPath);

                _database.AddZone(zone);
            }

            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Generation Complete",
                $"Generated {endgameZones.Length} endgame zones.", "OK");
        }

        #endregion

        #region Utility

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            var pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            var texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        #endregion
    }
}
