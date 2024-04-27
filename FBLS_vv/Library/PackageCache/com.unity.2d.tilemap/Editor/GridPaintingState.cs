using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// GridPaintingState controls the state of objects for painting with a Tile Palette.
    /// </summary>
    /// <remarks>
    /// Utilize this class to get and set the current painting target and brush for painting
    /// with the Tile Palette.
    /// </remarks>
    public class GridPaintingState : ScriptableSingleton<GridPaintingState>
    {
        [SerializeField] private GameObject m_EditModeScenePaintTarget; // Which GameObject in scene was the last painting target in EditMode
        [SerializeField] private GameObject m_ScenePaintTarget; // Which GameObject in scene is considered as painting target
        [SerializeField] private GridBrushBase m_Brush; // Which brush will handle painting callbacks
        [SerializeField] private GridBrushPickStore m_BrushPickStore; // Stores prior brush selection settings
        [SerializeField] private PaintableGrid m_ActiveGrid; // Grid that has painting focus (can be palette, too)
        [SerializeField] private PaintableGrid m_LastActiveGrid; // Grid that last had painting focus (can be palette, too)
        [SerializeField] private HashSet<Object> m_InterestedPainters = new HashSet<Object>(); // A list of objects that can paint using the GridPaintingState

        private GameObject[] m_CachedPaintTargets;
        private bool m_FlushPaintTargetCache;
        private Editor m_CachedEditor;
        private bool m_SavingPalette;

        /// <summary>
        /// Callback when the Tile Palette's active target has changed
        /// </summary>
        public static event Action<GameObject> scenePaintTargetChanged;
        /// <summary>
        /// Callback when the Tile Palette's active target has been edited
        /// </summary>
        public static event Action<GameObject> scenePaintTargetEdited;
        /// <summary>
        /// Callback when the Tile Palette's active brush has changed.
        /// </summary>
        public static event Action<GridBrushBase> brushChanged;
        /// <summary>
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintingState.cs
        /// Callback when the Tile Palette's active palette GameObject has changed.
        /// </summary>
        public static event Action<GameObject> paletteChanged;
=======
        /// Callback when the Tile Palette's active brush's selection has changed.
        /// </summary>
        public static event Action brushPickChanged;
        /// <summary>
        /// Callback when the Tile Palette's brush selection store has changed.
        /// </summary>
        public static event Action brushPickStoreChanged;
        /// <summary>
        /// Callback when the Tile Palette's active brush tools have changed.
        /// </summary>
        public static event Action brushToolsChanged;
        /// <summary>
        /// Callback before the Tile Palette's active palette GameObject has changed.
        /// </summary>
        public static event Action beforePaletteChanged;
        /// <summary>
        /// Callback when the Tile Palette's active palette GameObject has changed.
        /// </summary>
        public static event Action<GameObject> paletteChanged;
        /// <summary>
        /// Callback when the Tile Palette's list of palettes has changed
        /// </summary>
        public static event Action palettesChanged;
        /// <summary>
        /// Callback when the Tile Palette's valid targets has changed.
        /// </summary>
        public static event Action validTargetsChanged;
        /// <summary>
        /// Callback when Tile Palette edit mode has changed.
        /// </summary>
        public static event Action editModeChanged;

        private static readonly string k_TilemapLastPaletteEditorPref = "TilemapLastPalette";
        private string lastTilemapPalette
        {
            get => EditorPrefs.GetString(k_TilemapLastPaletteEditorPref, "");
            set => EditorPrefs.SetString(k_TilemapLastPaletteEditorPref, value);
        }

        private static readonly string k_GridBrushMousePositionAtZ = "TilemapGridBrushMousePositionAtZ";
        private static bool? m_CachedGridBrushMousePositionAtZ;

        internal static bool gridBrushMousePositionAtZ
        {
            get
            {
                m_CachedGridBrushMousePositionAtZ ??= EditorPrefs.GetBool(k_GridBrushMousePositionAtZ, false);
                return m_CachedGridBrushMousePositionAtZ.Value;
            }
            set
            {
                m_CachedGridBrushMousePositionAtZ = value;
                EditorPrefs.SetBool(k_GridBrushMousePositionAtZ, value);
            }
        }

        readonly TilemapEditorTool.ShortcutContext m_ShortcutContext = new TilemapEditorTool.ShortcutContext { active = true };
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintingState.cs

        private void OnEnable()
        {
            EditorApplication.hierarchyChanged += HierarchyChanged;
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
            Selection.selectionChanged += OnSelectionChange;
            m_FlushPaintTargetCache = true;
        }

        private void OnDisable()
        {
            m_InterestedPainters.Clear();
            EditorApplication.hierarchyChanged -= HierarchyChanged;
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
            Selection.selectionChanged -= OnSelectionChange;
            FlushCache();
        }

<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintingState.cs
=======
        private void OnEditEnable()
        {
            isEditing = true;
            if (palette == null && !String.IsNullOrEmpty(lastTilemapPalette))
            {
                var lastPalette = GridPalettes.palettes
                    .Where((paletteInList, _) => (AssetDatabase.GetAssetPath(paletteInList) == lastTilemapPalette))
                    .FirstOrDefault();
                if (lastPalette != null)
                    palette = lastPalette;
            }
            if (palette == null && GridPalettes.palettes.Count > 0)
            {
                palette = GridPalettes.palettes[0];
            }

            if (m_PaintableSceneViewGrid == null)
            {
                m_PaintableSceneViewGrid = CreateInstance<PaintableSceneViewGrid>();
                m_PaintableSceneViewGrid.hideFlags = HideFlags.HideAndDontSave;
                m_PaintableSceneViewGrid.onEdited += OnEdited;
            }

            m_FlushPaintTargetCache = true;
            GridPaletteBrushes.FlushCache();
            GridPalettes.palettesChanged += PalettesChanged;
            ShortcutIntegration.instance.profileManager.shortcutBindingChanged += UpdateTooltips;

            scenePaintTargetChanged += TilemapFocusModeUtility.OnScenePaintTargetChanged;
            brushChanged += TilemapFocusModeUtility.OnBrushChanged;
            paletteChanged += PaletteChanged;
            SceneView.duringSceneGui += TilemapFocusModeUtility.OnSceneViewGUI;

            ToolManager.activeToolChanged += ActiveToolChanged;
            ToolManager.activeToolChanging += ActiveToolChanging;

            AssetPreview.SetPreviewTextureCacheSize(256, GetInstanceID());

            ShortcutIntegration.instance.contextManager.RegisterToolContext(m_ShortcutContext);
        }

        private void OnEdited(GameObject obj)
        {
            scenePaintTargetEdited?.Invoke(obj);
        }

        private void PaletteChanged(GameObject obj)
        {
            lastTilemapPalette = AssetDatabase.GetAssetPath(palette);
        }

        private void PalettesChanged()
        {
            palettesChanged?.Invoke();
        }

        private void OnEditDisable()
        {
            TilemapFocusModeUtility.SetFocusMode(TilemapFocusModeUtility.TilemapFocusMode.None);

            CallOnToolDeactivated();

            gridBrush = null;

            DestroyImmediate(m_PaintableSceneViewGrid);

            if (PaintableGrid.InGridEditMode())
            {
                // Set Editor Tool to an always available Tool, as Tile Palette Tools are not available any more
                ToolManager.SetActiveTool<ViewModeTool>();
            }

            ShortcutIntegration.instance.profileManager.shortcutBindingChanged -= UpdateTooltips;
            ToolManager.activeToolChanged -= ActiveToolChanged;
            ToolManager.activeToolChanging -= ActiveToolChanging;
            SceneView.duringSceneGui -= TilemapFocusModeUtility.OnSceneViewGUI;
            brushChanged -= TilemapFocusModeUtility.OnBrushChanged;
            paletteChanged -= PaletteChanged;
            GridPalettes.palettesChanged -= PalettesChanged;

            ShortcutIntegration.instance.contextManager.DeregisterToolContext(m_ShortcutContext);

            isEditing = false;
        }

        private void ActiveToolChanged()
        {
            if (gridBrush != null && PaintableGrid.InGridEditMode() && activeBrushEditor != null)
            {
                GridBrushBase.Tool tool = PaintableGrid.EditTypeToBrushTool(ToolManager.activeToolType);
                activeBrushEditor.OnToolActivated(tool);
                m_PreviousToolActivatedEditor = activeBrushEditor;
                m_PreviousToolActivated = tool;

                for (int i = 0; i < TilePaletteMouseCursorUtility.MouseStyles.sceneViewEditModes.Length; ++i)
                {
                    if (TilePaletteMouseCursorUtility.MouseStyles.sceneViewEditModes[i] == tool)
                    {
                        Cursor.SetCursor(TilePaletteMouseCursorUtility.MouseStyles.mouseCursorTextures[i],
                            TilePaletteMouseCursorUtility.MouseStyles.mouseCursorTextures[i] != null ? TilePaletteMouseCursorUtility.MouseStyles.mouseCursorOSHotspot[(int)SystemInfo.operatingSystemFamily] : Vector2.zero,
                            CursorMode.Auto);
                        break;
                    }
                }
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }

        private void ActiveToolChanging()
        {
            if (GridSelection.active
                && !TilemapEditorTool.IsActive(typeof(MoveTool))
                && !TilemapEditorTool.IsActive(typeof(SelectTool))
                && !ToolManager.activeToolType.IsSubclassOf(typeof(GridSelectionTool)))
            {
                GridSelection.Clear();
            }
            CallOnToolDeactivated();
        }

        private void CallOnToolDeactivated()
        {
            if (gridBrush != null && m_PreviousToolActivatedEditor != null)
            {
                m_PreviousToolActivatedEditor.OnToolDeactivated(m_PreviousToolActivated);
                m_PreviousToolActivatedEditor = null;

                if (!PaintableGrid.InGridEditMode())
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }

>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintingState.cs
        private void OnSelectionChange()
        {
            if (hasInterestedPainters && ValidatePaintTarget(Selection.activeGameObject))
            {
                scenePaintTarget = Selection.activeGameObject;
            }
        }

        private void PlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                m_EditModeScenePaintTarget = scenePaintTarget;
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                if (GridPaintActiveTargetsPreferences.restoreEditModeSelection && m_EditModeScenePaintTarget != null)
                {
                    scenePaintTarget = m_EditModeScenePaintTarget;
                }
            }
        }

        private void HierarchyChanged()
        {
            if (hasInterestedPainters)
            {
                m_FlushPaintTargetCache = true;
                if (validTargets == null || validTargets.Length == 0 || !validTargets.Contains(scenePaintTarget))
                {
                    // case 1102618: Try to use current Selection as scene paint target if possible
                    if (Selection.activeGameObject != null && hasInterestedPainters && ValidatePaintTarget(Selection.activeGameObject))
                    {
                        scenePaintTarget = Selection.activeGameObject;
                    }
                    else
                    {
                        AutoSelectPaintTarget();
                    }
                }
            }
        }

        private GameObject[] GetValidTargets()
        {
            if (m_FlushPaintTargetCache)
            {
                m_CachedPaintTargets = null;
                if (activeBrushEditor != null)
                    m_CachedPaintTargets = activeBrushEditor.validTargets;
                if (m_CachedPaintTargets == null || m_CachedPaintTargets.Length == 0)
                    scenePaintTarget = null;
                else
                {
                    var comparer = GridPaintActiveTargetsPreferences.GetTargetComparer();
                    if (comparer != null)
                        Array.Sort(m_CachedPaintTargets, comparer);
                }

                m_FlushPaintTargetCache = false;
            }
            return m_CachedPaintTargets;
        }

        internal static void AutoSelectPaintTarget()
        {
            if (activeBrushEditor != null)
            {
                if (validTargets != null && validTargets.Length > 0)
                {
                    scenePaintTarget = validTargets[0];
                }
            }
        }

        /// <summary>
        /// The currently active target for the Tile Palette
        /// </summary>
        public static GameObject scenePaintTarget
        {
            get => instance.m_ScenePaintTarget;
            set
            {
                if (value != instance.m_ScenePaintTarget)
                {
                    instance.m_ScenePaintTarget = value;
                    if (scenePaintTargetChanged != null)
                        scenePaintTargetChanged(instance.m_ScenePaintTarget);
                    RepaintGridPaintPaletteWindow();
                }
            }
        }

        /// <summary>
        /// The currently active brush for the Tile Palette
        /// </summary>
        public static GridBrushBase gridBrush
        {
            get
            {
                if (instance.m_Brush == null)
                    instance.m_Brush = GridPaletteBrushes.instance.GetLastUsedBrush();
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintingState.cs

=======
                    instance.m_BrushPickStore = GridBrushPickStore.LoadOrCreateLibraryGridBrushPickAsset();
                    UpdateBrushToolbar();
                }
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintingState.cs
                return instance.m_Brush;
            }
            set
            {
                if (instance.m_Brush != value)
                {
                    instance.m_Brush = value;
                    instance.m_FlushPaintTargetCache = true;

                    if (value != null)
                        GridPaletteBrushes.instance.StoreLastUsedBrush(value);
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintingState.cs
=======
                        UpdateBrushToolbar();
                    }
                    else
                    {
                        instance.m_BrushPickStore = null;
                    }
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintingState.cs

                    // Ensure that current scenePaintTarget is still a valid target after a brush change
                    if (scenePaintTarget != null && !ValidatePaintTarget(scenePaintTarget))
                        scenePaintTarget = null;

                    // Use Selection if previous scenePaintTarget was not valid
                    if (scenePaintTarget == null)
                        scenePaintTarget = ValidatePaintTarget(Selection.activeGameObject) ? Selection.activeGameObject : null;

                    // Auto select a valid target if there is still no scenePaintTarget
                    if (scenePaintTarget == null)
                        AutoSelectPaintTarget();

                    if (null != brushChanged)
                        brushChanged(value);

                    RepaintGridPaintPaletteWindow();
                }
            }
        }

        /// <summary>
        /// Returns a store of brush selection data for the current gridBrush
        /// </summary>
        public static GridBrushPickStore brushPickStore
        {
            get
            {
                if (gridBrush != null && (instance.m_BrushPickStore == null
                                          || !instance.m_BrushPickStore.IsValid()))
                {
                    instance.m_BrushPickStore = GridBrushPickStore.LoadOrCreateLibraryGridBrushPickAsset();
                }
                return instance.m_BrushPickStore;
            }

            internal set
            {
                if (instance.m_BrushPickStore == value)
                    return;

                var store = value;
                if (store == null)
                    store = GridBrushPickStore.LoadOrCreateLibraryGridBrushPickAsset();

                instance.m_BrushPickStore = store;
                InvokeBrushPickStoreChanged();
            }
        }

        /// <summary>
        /// Returns all available brushes for the Tile Palette
        /// </summary>
        public static IList<GridBrushBase> brushes => GridPaletteBrushes.brushes;

        internal static GridBrush defaultBrush
        {
            get => gridBrush as GridBrush;
            set => gridBrush = value;
        }

        /// <summary>
        /// The currently active palette GameObject for the Tile Palette
        /// </summary>
        public static GameObject palette
        {
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintingState.cs
            get
            {
                if (GridPaintPaletteWindow.instances.Count > 0)
                    return GridPaintPaletteWindow.instances[0].palette;
                return null;
            }
=======
            get => instance.m_Palette;
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintingState.cs
            set
            {
                if (value == null || !GridPalettes.palettes.Contains(value))
                    throw new ArgumentException(L10n.Tr("Unable to set invalid palette"));
                if (GridPaintPaletteWindow.instances.Count > 0 && GridPaintPaletteWindow.instances[0].palette != value)
                {
                    GridPaintPaletteWindow.instances[0].palette = value;
                }
            }
        }

        /// <summary>
        /// Checks if target GameObject is part of the active Palette.
        /// </summary>
        /// <param name="target">GameObject to check.</param>
        /// <returns>True if the target GameObject is part of the active palette. False if not.</returns>
        public static bool IsPartOfActivePalette(GameObject target)
        {
            if (GridPaintPaletteWindow.instances.Count > 0 && target == GridPaintPaletteWindow.instances[0].paletteInstance)
                return true;
            if (target == palette)
                return true;
            var parent = target.transform.parent;
            return parent != null && IsPartOfActivePalette(parent.gameObject);
        }

        /// <summary>
        /// Returns all available Palette GameObjects for the Tile Palette
        /// </summary>
        public static IList<GameObject> palettes => GridPalettes.palettes;

        /// <summary>
        /// The currently active editor for the active brush for the Tile Palette
        /// </summary>
        public static GridBrushEditorBase activeBrushEditor
        {
            get
            {
                Editor.CreateCachedEditor(gridBrush, null, ref instance.m_CachedEditor);
                GridBrushEditorBase baseEditor = instance.m_CachedEditor as GridBrushEditorBase;
                return baseEditor;
            }
        }

        internal static Editor fallbackEditor
        {
            get
            {
                Editor.CreateCachedEditor(gridBrush, null, ref instance.m_CachedEditor);
                return instance.m_CachedEditor;
            }
        }

        internal static PaintableGrid activeGrid
        {
            get => instance.m_ActiveGrid;
            set
            {
                instance.m_ActiveGrid = value;
                if (instance.m_ActiveGrid != null)
                    instance.m_LastActiveGrid = value;
            }
        }

        internal static PaintableGrid lastActiveGrid => instance.m_LastActiveGrid;

<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintingState.cs
=======
        internal static PaintableSceneViewGrid paintableSceneViewGrid => instance.m_PaintableSceneViewGrid;

        /// <summary>
        /// The last active mouse position on the `SceneView`
        /// when the `GridPaintingState` is active.
        /// </summary>
        public static Vector2 lastSceneViewMousePosition => paintableSceneViewGrid.mousePosition;

        /// <summary>
        /// The last active grid position on the `SceneView`
        /// when the `GridPaintingState` is active.
        /// </summary>
        public static Vector3Int lastSceneViewGridPosition =>
            new Vector3Int(paintableSceneViewGrid.mouseGridPosition.x
                , paintableSceneViewGrid.mouseGridPosition.y
                , paintableSceneViewGrid.zPosition);

        internal static EditorTool[] activeBrushTools
        {
            get => instance.m_BrushTools;
            set
            {
                instance.m_BrushTools = value;
                brushToolsChanged?.Invoke();
            }
        }

        internal static float activeBrushToolbarSize
        {
            get
            {
                if (instance.m_BrushToolbarSize == 0.0f)
                    CalculateToolbarSize();
                return instance.m_BrushToolbarSize;
            }
            set => instance.m_BrushToolbarSize = value;
        }

        internal static bool drawGridGizmo
        {
            get => instance.m_DrawGridGizmo;
            set => instance.m_DrawGridGizmo = value;
        }

        internal static bool drawGizmos
        {
            get => instance.m_DrawGizmos;
            set => instance.m_DrawGizmos = value;
        }

        /// <summary>
        /// Returns whether GridPaintingState is active for editing.
        /// </summary>
        public static bool isEditing
        {
            get => instance.m_IsEditing;
            internal set
            {
                if (value != instance.m_IsEditing)
                {
                    instance.m_IsEditing = value;
                    editModeChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// Retrieves a stored selection from the current Active GridBrushPickStore
        /// and copies it into the Active GridBrush.
        /// </summary>
        /// <param name="user">Use user selection or last selection.</param>
        /// <param name="index">Index of selection from store to use.</param>
        public static void SetPickOnActiveGridBrush(bool user, int index)
        {
            if (gridBrush == null || brushPickStore == null)
                return;

            GridBrushBase selection = null;
            if (user)
            {
                if (0 <= index && index < brushPickStore.userSavedBrushes.Count)
                    selection = brushPickStore.userSavedBrushes[index];
            }
            else
            {
                if (0 <= index && index < brushPickStore.lastSavedBrushes.Count)
                    selection = brushPickStore.lastSavedBrushes[index];
            }
            if (selection == null)
                return;

            var selectedBrushType = selection.GetType();
            if (!GridPaletteBrushes.IsDefaultInstanceVisibleGridBrushType(selectedBrushType)
                && selectedBrushType != GridPaletteBrushes.GetDefaultBrushType())
                return;

            if (gridBrush.GetType() != selectedBrushType)
            {
                foreach (var brush in GridPaletteBrushes.brushes)
                {
                    if (brush.GetType() == selectedBrushType)
                    {
                        gridBrush = brush;
                        break;
                    }
                }
            }

            var originalName = gridBrush.name;
            var originalFlags = gridBrush.hideFlags;
            EditorUtility.CopySerialized(selection, gridBrush);
            gridBrush.name = originalName;
            gridBrush.hideFlags = originalFlags;

            GridPaletteBrushes.GridBrushAssetChanged(gridBrush);
        }

        private static void CalculateToolbarSize()
        {
            GUIStyle toolbarStyle = "Command";
            activeBrushToolbarSize = activeBrushTools.Sum(x => toolbarStyle.CalcSize(x.toolbarIcon).x);
        }

        internal static void SetBrushTools(EditorTool[] editorTools)
        {
            activeBrushTools = editorTools;
            activeBrushToolbarSize = 0.0f;
        }

>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintingState.cs
        private static bool ValidatePaintTarget(GameObject candidate)
        {
            if (candidate == null || candidate.GetComponentInParent<Grid>() == null && candidate.GetComponent<Grid>() == null)
                return false;

            if (validTargets != null && validTargets.Length > 0 && !validTargets.Contains(candidate))
                return false;

            return true;
        }

        internal static void FlushCache()
        {
            if (instance.m_CachedEditor != null)
            {
                DestroyImmediate(instance.m_CachedEditor);
                instance.m_CachedEditor = null;
            }
            instance.m_FlushPaintTargetCache = true;
        }

        /// <summary>
        /// A list of all valid targets that can be set as an active target for the Tile Palette
        /// </summary>
        public static GameObject[] validTargets => instance.GetValidTargets();

        internal static bool savingPalette
        {
            get => instance.m_SavingPalette;
            set => instance.m_SavingPalette = value;
        }

        internal static void OnPaletteChanged(GameObject palette)
        {
            if (null != paletteChanged)
                paletteChanged(palette);
        }

        internal static void ActiveGridBrushAssetChanged()
        {
            if (gridBrush == null)
                return;

            GridPaletteBrushes.GridBrushAssetChanged(gridBrush);

            if (activeBrushEditor != null && activeBrushEditor.shouldSaveBrushForSelection)
                brushPickStore.AddNewLastSavedBrush(gridBrush);

            brushPickChanged?.Invoke();
        }

        internal static void UpdateActiveGridPalette()
        {
            if (GridPaintPaletteWindow.instances.Count > 0)
                GridPaintPaletteWindow.instances[0].DelayedResetPreviewInstance();
        }

        internal static void RepaintGridPaintPaletteWindow()
        {
            if (GridPaintPaletteWindow.instances.Count > 0)
                GridPaintPaletteWindow.instances[0].Repaint();
        }

        internal static void InvokeBrushPickStoreChanged()
        {
            brushPickStoreChanged?.Invoke();
        }

        internal static void UnlockGridPaintPaletteClipboardForEditing()
        {
            if (GridPaintPaletteWindow.instances.Count > 0)
                GridPaintPaletteWindow.instances[0].clipboardView.UnlockAndEdit();
        }

        internal static void RegisterPainterInterest(Object painter)
        {
            instance.m_InterestedPainters.Add(painter);
        }

        internal static void UnregisterPainterInterest(Object painter)
        {
            instance.m_InterestedPainters.Remove(painter);
        }

<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintingState.cs
        private bool hasInterestedPainters
        {
            get { return m_InterestedPainters.Count > 0; }
        }
=======
        internal static bool hasInterestedPainters => instance != null && instance.m_InterestedPainters.Count > 0;
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintingState.cs
    }
}
