<<<<<<< Updated upstream
using System;
using System.Collections.Generic;
using System.Linq;
=======
using System.Collections.Generic;
>>>>>>> Stashed changes
using UnityEditor.EditorTools;
using UnityEditor.Overlays;
using UnityEditor.ShortcutManagement;
using UnityEngine;
<<<<<<< Updated upstream
using UnityEngine.Tilemaps;
using Event = UnityEngine.Event;
using Object = UnityEngine.Object;

using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
=======
using UnityEngine.UIElements;

using Event = UnityEngine.Event;
>>>>>>> Stashed changes

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// EditorWindow containing the Tile Palette
    /// </summary>
    public class GridPaintPaletteWindow : EditorWindow, ISupportsOverlays
    {
<<<<<<< Updated upstream
        internal enum TilemapFocusMode
        {
            None = 0,
            Tilemap = 1,
            Grid = 2
        }
        private static readonly string k_TilemapFocusModeEditorPref = "TilemapFocusMode";
        private TilemapFocusMode focusMode
        {
            get
            {
                return (TilemapFocusMode)EditorPrefs.GetInt(k_TilemapFocusModeEditorPref, (int)TilemapFocusMode.None);
            }
            set
            {
                EditorPrefs.SetInt(k_TilemapFocusModeEditorPref, (int)value);
            }
        }

        private static readonly string k_TilemapLastPaletteEditorPref = "TilemapLastPalette";
        private string lastTilemapPalette
        {
            get
            {
                return EditorPrefs.GetString(k_TilemapLastPaletteEditorPref, "");
            }
            set
            {
                EditorPrefs.SetString(k_TilemapLastPaletteEditorPref, value);
            }
        }

        private static class MouseStyles
        {
            // The following paths match the enums in OperatingSystemFamily
            public static readonly string[] mouseCursorOSPath =
            {
                "", // Other OS
                "Cursors/macOS",
                "Cursors/Windows",
                "Cursors/Linux",
            };
            // The following paths match the enums in OperatingSystemFamily
            public static readonly Vector2[] mouseCursorOSHotspot =
            {
                Vector2.zero, // Other OS
                new Vector2(6f, 4f),
                new Vector2(6f, 4f),
                new Vector2(6f, 4f),
            };
            // The following paths match the enums in sceneViewEditModes above
            public static readonly string[] mouseCursorTexturePaths =
            {
                "",
                "Grid.MoveTool.png",
                "Grid.PaintTool.png",
                "Grid.BoxTool.png",
                "Grid.PickingTool.png",
                "Grid.EraserTool.png",
                "Grid.FillTool.png",
            };
            public static readonly Texture2D[] mouseCursorTextures;
            static MouseStyles()
            {
                mouseCursorTextures = new Texture2D[mouseCursorTexturePaths.Length];
                int osIndex = (int)SystemInfo.operatingSystemFamily;
                for (int i = 0; i < mouseCursorTexturePaths.Length; ++i)
                {
                    if ((mouseCursorOSPath[osIndex] != null && mouseCursorOSPath[osIndex].Length > 0)
                        && (mouseCursorTexturePaths[i] != null && mouseCursorTexturePaths[i].Length > 0))
                    {
                        string cursorPath = Utils.Paths.Combine(mouseCursorOSPath[osIndex], mouseCursorTexturePaths[i]);
                        mouseCursorTextures[i] = EditorGUIUtility.LoadRequired(cursorPath) as Texture2D;
                    }
                    else
                        mouseCursorTextures[i] = null;
                }
            }
        }

        private static class Styles
        {
            public static readonly GUIContent emptyProjectInfo = EditorGUIUtility.TrTextContent("Create a new palette in the dropdown above.");
            public static readonly GUIContent emptyPaletteInfo = EditorGUIUtility.TrTextContent("Drag Tile, Sprite or Sprite Texture assets here.");
            public static readonly GUIContent invalidPaletteInfo = EditorGUIUtility.TrTextContent("This is an invalid palette. Did you delete the palette asset?");
            public static readonly GUIContent invalidGridInfo = EditorGUIUtility.TrTextContent("The palette has an invalid Grid. Did you add a Grid to the palette asset?");
=======
        private static class Styles
        {
>>>>>>> Stashed changes
            public static readonly GUIContent selectPaintTarget = EditorGUIUtility.TrTextContent("Select Paint Target");
            public static readonly GUIContent selectPalettePrefab = EditorGUIUtility.TrTextContent("Select Palette Prefab");
            public static readonly GUIContent selectTileAsset = EditorGUIUtility.TrTextContent("Select Tile Asset");
            public static readonly GUIContent unlockPaletteEditing = EditorGUIUtility.TrTextContent("Unlock Palette Editing");
            public static readonly GUIContent lockPaletteEditing = EditorGUIUtility.TrTextContent("Lock Palette Editing");
<<<<<<< Updated upstream
            public static readonly GUIContent openTilePalettePreferences = EditorGUIUtility.TrTextContent("Open Tile Palette Preferences");
            public static readonly GUIContent createNewPalette = EditorGUIUtility.TrTextContent("Create New Palette");
            public static readonly GUIContent focusLabel = EditorGUIUtility.TrTextContent("Focus On");
            public static readonly GUIContent rendererOverlayTitleLabel = EditorGUIUtility.TrTextContent("Tilemap");
            public static readonly GUIContent activeTargetLabel = EditorGUIUtility.TrTextContent("Active Tilemap", "Specifies the currently active Tilemap used for painting in the Scene View.");
            public static readonly GUIContent prefabWarningIcon = EditorGUIUtility.TrIconContent("console.warnicon.sml", "Editing Tilemaps in Prefabs will have better performance if edited in Prefab Mode.");

            public static readonly GUIContent tilePalette = EditorGUIUtility.TrTextContent("Tile Palette");
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
            public static readonly GUIContent edit = EditorGUIUtility.TrTextContent("Edit", "Toggle to edit current Tile Palette");
            public static readonly GUIContent editModified = EditorGUIUtility.TrTextContent("Edit*", "Toggle to save edits for current Tile Palette");
            public static readonly GUIContent gizmos = EditorGUIUtility.TrTextContent("Gizmos", "Toggle visibility of Gizmos in the Tile Palette");
            public static readonly GUIContent lockZPosition = EditorGUIUtility.TrTextContent("Lock Z Position", "Toggle editing of Z position");
            public static readonly GUIContent zPosition = EditorGUIUtility.TrTextContent("Z Position", "Set a Z position for the active Brush for painting");
            public static readonly GUIContent resetZPosition = EditorGUIUtility.TrTextContent("Reset", "Reset Z position for the active Brush");
            public static readonly GUIStyle ToolbarTitleStyle = "Toolbar";
            public static readonly GUIStyle dragHandle = "RL DragHandle";
            public static readonly float dragPadding = 3f;

            public static readonly GUILayoutOption[] dropdownOptions = { GUILayout.Width(k_DropdownWidth) };
=======

            public static readonly GUIContent mouseGridPositionAtZ = EditorGUIUtility.TrTextContent("Mouse Grid Position At Z", "Shows the Mouse Grid Position marquee at the Brush's Z Position.");
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
        }

        private class TilePaletteSaveScope : IDisposable
        {
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
            private GameObject m_GameObject;
=======
=======
            public static readonly GUIContent verticalBrushSplit = EditorGUIUtility.TrTextContent("Vertical Split for Brush Inspector");
            public static readonly GUIContent horizontalBrushSplit = EditorGUIUtility.TrTextContent("Horizontal Split for Brush Inspector");
            public static readonly GUIContent openTilePalettePreferences = EditorGUIUtility.TrTextContent("Open Tile Palette Preferences");
            public static readonly GUIContent openAsFloatingWindow = EditorGUIUtility.TrTextContent("Open Window as/Floating");
            public static readonly GUIContent openAsDockableWindow = EditorGUIUtility.TrTextContent("Open Window as/Dockable");

            public static readonly GUIContent tilePalette = EditorGUIUtility.TrTextContent("Tile Palette");

            public static readonly GUIContent mouseGridPositionAtZ = EditorGUIUtility.TrTextContent("Mouse Grid Position At Z", "Shows the Mouse Grid Position marquee at the Brush's Z Position.");
        }

        private static class UIStyles
        {
>>>>>>> Stashed changes
            public static readonly string styleSheetPath = "Packages/com.unity.2d.tilemap/Editor/UI/GridPaintPaletteWindow.uss";
            public static readonly string darkStyleSheetPath = "Packages/com.unity.2d.tilemap/Editor/UI/GridPaintPaletteWindowDark.uss";
            public static readonly string lightStyleSheetPath = "Packages/com.unity.2d.tilemap/Editor/UI/GridPaintPaletteWindowLight.uss";
            public static readonly string ussClassName = "unity-grid-paint-palette-window";
        }
<<<<<<< Updated upstream
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs

            public TilePaletteSaveScope(GameObject paletteInstance)
            {
                m_GameObject = paletteInstance;
                if (m_GameObject != null)
                {
                    GridPaintingState.savingPalette = true;
                    SetHideFlagsRecursively(paletteInstance, HideFlags.HideInHierarchy);
                    foreach (var renderer in paletteInstance.GetComponentsInChildren<Renderer>())
                        renderer.gameObject.layer = 0;
                }
            }

            public void Dispose()
            {
                if (m_GameObject != null)
                {
                    SetHideFlagsRecursively(m_GameObject, HideFlags.HideAndDontSave);
                    GridPaintingState.savingPalette = false;
                }
            }

            private void SetHideFlagsRecursively(GameObject root, HideFlags flags)
            {
                root.hideFlags = flags;
                for (int i = 0; i < root.transform.childCount; i++)
                    SetHideFlagsRecursively(root.transform.GetChild(i).gameObject, flags);
            }
        }

        internal class TilePaletteProperties
        {
            public enum PrefabEditModeSettings
            {
                EnableDialog = 0,
                EditInPrefabMode = 1,
                EditInScene = 2
            }

            public static readonly string targetEditModeDialogTitle = L10n.Tr("Open in Prefab Mode");
            public static readonly string targetEditModeDialogMessage = L10n.Tr("Editing Tilemaps in Prefabs will have better performance if edited in Prefab Mode. Do you want to open it in Prefab Mode or edit it in the Scene?");
            public static readonly string targetEditModeDialogYes = L10n.Tr("Prefab Mode");
            public static readonly string targetEditModeDialogChange = L10n.Tr("Preferences");
            public static readonly string targetEditModeDialogNo = L10n.Tr("Scene");

            public static readonly string targetEditModeEditorPref = "TilePalette.TargetEditMode";
            public static readonly string targetEditModeLookup = "Target Edit Mode";
            public static readonly string tilePalettePreferencesLookup = "Tile Palette";

            public static readonly GUIContent targetEditModeDialogLabel = EditorGUIUtility.TrTextContent(targetEditModeLookup, "Controls the behaviour of editing a Prefab Instance when one is selected as the Active Target in the Tile Palette");
        }

        private static readonly GridBrushBase.Tool[] k_SceneViewEditModes =
        {
            GridBrushBase.Tool.Select,
            GridBrushBase.Tool.Move,
            GridBrushBase.Tool.Paint,
            GridBrushBase.Tool.Box,
            GridBrushBase.Tool.Pick,
            GridBrushBase.Tool.Erase,
            GridBrushBase.Tool.FloodFill
        };

        private const float k_DropdownWidth = 200f;
        private const float k_ActiveTargetLabelWidth = 90f;
        private const float k_ActiveTargetDropdownWidth = 130f;
        private const float k_ActiveTargetWarningSize = 20f;
        private const float k_TopAreaHeight = 104f;
        private const float k_MinBrushInspectorHeight = 50f;
        private const float k_MinClipboardHeight = 200f;
        private const float k_ToolbarHeight = 17f;
        private const float k_ResizerDragRectPadding = 10f;
        private static readonly Vector2 k_MinWindowSize = new Vector2(k_ActiveTargetLabelWidth + k_ActiveTargetDropdownWidth + k_ActiveTargetWarningSize, 200f);

        private PaintableSceneViewGrid m_PaintableSceneViewGrid;

        class ShortcutContext : IShortcutToolContext
        {
            public bool active { get; set; }
        }

        readonly ShortcutContext m_ShortcutContext = new ShortcutContext { active = true };

<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [FormerlyPrefKeyAs("Grid Painting/Select", "s")]
        [Shortcut("Grid Painting/Select", typeof(ShortcutContext), KeyCode.S)]
=======
=======

        private static readonly string k_TilePaletteVerticalBrushSplitPref = "TilePaletteVerticalBrushSplit";
        internal static bool tilePaletteVerticalBrushSplit
        {
            get
            {
                return EditorPrefs.GetBool(k_TilePaletteVerticalBrushSplitPref, true);
            }
            set
            {
                EditorPrefs.SetBool(k_TilePaletteVerticalBrushSplitPref, value);
            }
        }

        private const float k_ActiveTargetLabelWidth = 90f;
        private const float k_ActiveTargetDropdownWidth = 130f;
        private const float k_ActiveTargetWarningSize = 20f;
        private const float k_MinClipboardHeight = 200f;
        private static readonly Vector2 k_MinWindowSize = new Vector2(k_ActiveTargetLabelWidth + k_ActiveTargetDropdownWidth + k_ActiveTargetWarningSize, k_MinClipboardHeight);

>>>>>>> Stashed changes
        internal static class ShortcutIds
        {
            public const string k_Select = "Grid Painting/Select";
            public const string k_Move = "Grid Painting/Move";
            public const string k_Brush = "Grid Painting/Brush";
            public const string k_Rectangle = "Grid Painting/Rectangle";
            public const string k_Picker = "Grid Painting/Picker";
            public const string k_Erase = "Grid Painting/Erase";
            public const string k_Fill = "Grid Painting/Fill";
            public const string k_RotateClockwise = "Grid Painting/Rotate Clockwise";
            public const string k_RotateAntiClockwise = "Grid Painting/Rotate Anti-Clockwise";
            public const string k_FlipX = "Grid Painting/Flip X";
            public const string k_FlipY = "Grid Painting/Flip Y";
            public const string k_IncreaseZ = "Grid Painting/Increase Z";
            public const string k_DecreaseZ = "Grid Painting/Decrease Z";
            public const string k_SwitchToNextBrush = "Grid Painting/Switch To Next Brush";
            public const string k_SwitchToPreviousBrush = "Grid Painting/Switch To Previous Brush";
            public const string k_ToggleSceneViewPalette = "Grid Painting/Toggle SceneView Palette";
            public const string k_ToggleSceneViewBrushPick = "Grid Painting/Toggle SceneView BrushPick";
        }
        
        [FormerlyPrefKeyAs(ShortcutIds.k_Select, "s")]
        [Shortcut(ShortcutIds.k_Select, typeof(TilemapEditorTool.ShortcutContext), KeyCode.S)]
<<<<<<< Updated upstream
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
>>>>>>> Stashed changes
        static void GridSelectKey()
        {
            TilemapEditorTool.ToggleActiveEditorTool(typeof(SelectTool));
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [FormerlyPrefKeyAs("Grid Painting/Move", "m")]
        [Shortcut("Grid Painting/Move", typeof(ShortcutContext), KeyCode.M)]
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_Move, "m")]
        [Shortcut(ShortcutIds.k_Move, typeof(TilemapEditorTool.ShortcutContext), KeyCode.M)]
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_Move, "m")]
        [Shortcut(ShortcutIds.k_Move, typeof(TilemapEditorTool.ShortcutContext), KeyCode.M)]
>>>>>>> Stashed changes
        static void GridMoveKey()
        {
            TilemapEditorTool.ToggleActiveEditorTool(typeof(MoveTool));
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [FormerlyPrefKeyAs("Grid Painting/Brush", "b")]
        [Shortcut("Grid Painting/Brush", typeof(ShortcutContext), KeyCode.B)]
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_Brush, "b")]
        [Shortcut(ShortcutIds.k_Brush, typeof(TilemapEditorTool.ShortcutContext), KeyCode.B)]
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_Brush, "b")]
        [Shortcut(ShortcutIds.k_Brush, typeof(TilemapEditorTool.ShortcutContext), KeyCode.B)]
>>>>>>> Stashed changes
        static void GridBrushKey()
        {
            TilemapEditorTool.ToggleActiveEditorTool(typeof(PaintTool));
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [FormerlyPrefKeyAs("Grid Painting/Rectangle", "u")]
        [Shortcut("Grid Painting/Rectangle", typeof(ShortcutContext), KeyCode.U)]
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_Rectangle, "u")]
        [Shortcut(ShortcutIds.k_Rectangle, typeof(TilemapEditorTool.ShortcutContext), KeyCode.U)]
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_Rectangle, "u")]
        [Shortcut(ShortcutIds.k_Rectangle, typeof(TilemapEditorTool.ShortcutContext), KeyCode.U)]
>>>>>>> Stashed changes
        static void GridRectangleKey()
        {
            TilemapEditorTool.ToggleActiveEditorTool(typeof(BoxTool));
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [FormerlyPrefKeyAs("Grid Painting/Picker", "i")]
        [Shortcut("Grid Painting/Picker", typeof(ShortcutContext), KeyCode.I)]
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_Picker, "i")]
        [Shortcut(ShortcutIds.k_Picker, typeof(TilemapEditorTool.ShortcutContext), KeyCode.I)]
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_Picker, "i")]
        [Shortcut(ShortcutIds.k_Picker, typeof(TilemapEditorTool.ShortcutContext), KeyCode.I)]
>>>>>>> Stashed changes
        static void GridPickerKey()
        {
            TilemapEditorTool.ToggleActiveEditorTool(typeof(PickingTool));
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [FormerlyPrefKeyAs("Grid Painting/Erase", "d")]
        [Shortcut("Grid Painting/Erase", typeof(ShortcutContext), KeyCode.D)]
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_Erase, "d")]
        [Shortcut(ShortcutIds.k_Erase, typeof(TilemapEditorTool.ShortcutContext), KeyCode.D)]
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_Erase, "d")]
        [Shortcut(ShortcutIds.k_Erase, typeof(TilemapEditorTool.ShortcutContext), KeyCode.D)]
>>>>>>> Stashed changes
        static void GridEraseKey()
        {
            TilemapEditorTool.ToggleActiveEditorTool(typeof(EraseTool));
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [FormerlyPrefKeyAs("Grid Painting/Fill", "g")]
        [Shortcut("Grid Painting/Fill", typeof(ShortcutContext), KeyCode.G)]
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_Fill, "g")]
        [Shortcut(ShortcutIds.k_Fill, typeof(TilemapEditorTool.ShortcutContext), KeyCode.G)]
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_Fill, "g")]
        [Shortcut(ShortcutIds.k_Fill, typeof(TilemapEditorTool.ShortcutContext), KeyCode.G)]
>>>>>>> Stashed changes
        static void GridFillKey()
        {
            TilemapEditorTool.ToggleActiveEditorTool(typeof(FillTool));
        }

        static void RotateBrush(GridBrushBase.RotationDirection direction)
        {
            GridPaintingState.gridBrush.Rotate(direction, GridPaintingState.activeGrid.cellLayout);
            GridPaintingState.activeGrid.Repaint();
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [FormerlyPrefKeyAs("Grid Painting/Rotate Clockwise", "[")]
        [Shortcut("Grid Painting/Rotate Clockwise", typeof(ShortcutContext), KeyCode.LeftBracket)]
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_RotateClockwise, "]")]
        [Shortcut(ShortcutIds.k_RotateClockwise, typeof(TilemapEditorTool.ShortcutContext), KeyCode.RightBracket)]
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_RotateClockwise, "]")]
        [Shortcut(ShortcutIds.k_RotateClockwise, typeof(TilemapEditorTool.ShortcutContext), KeyCode.RightBracket)]
>>>>>>> Stashed changes
        static void RotateBrushClockwise()
        {
            if (GridPaintingState.gridBrush != null && GridPaintingState.activeGrid != null)
                RotateBrush(GridBrushBase.RotationDirection.Clockwise);
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [FormerlyPrefKeyAs("Grid Painting/Rotate Anti-Clockwise", "]")]
        [Shortcut("Grid Painting/Rotate Anti-Clockwise", typeof(ShortcutContext), KeyCode.RightBracket)]
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_RotateAntiClockwise, "[")]
        [Shortcut(ShortcutIds.k_RotateAntiClockwise, typeof(TilemapEditorTool.ShortcutContext), KeyCode.LeftBracket)]
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_RotateAntiClockwise, "[")]
        [Shortcut(ShortcutIds.k_RotateAntiClockwise, typeof(TilemapEditorTool.ShortcutContext), KeyCode.LeftBracket)]
>>>>>>> Stashed changes
        static void RotateBrushAntiClockwise()
        {
            if (GridPaintingState.gridBrush != null && GridPaintingState.activeGrid != null)
                RotateBrush(GridBrushBase.RotationDirection.CounterClockwise);
        }

        static void FlipBrush(GridBrushBase.FlipAxis axis)
        {
            GridPaintingState.gridBrush.Flip(axis, GridPaintingState.activeGrid.cellLayout);
            GridPaintingState.activeGrid.Repaint();
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [FormerlyPrefKeyAs("Grid Painting/Flip X", "#[")]
        [Shortcut("Grid Painting/Flip X", typeof(ShortcutContext), KeyCode.LeftBracket, ShortcutModifiers.Shift)]
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_FlipX, "#[")]
        [Shortcut(ShortcutIds.k_FlipX, typeof(TilemapEditorTool.ShortcutContext), KeyCode.LeftBracket, ShortcutModifiers.Shift)]
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_FlipX, "#[")]
        [Shortcut(ShortcutIds.k_FlipX, typeof(TilemapEditorTool.ShortcutContext), KeyCode.LeftBracket, ShortcutModifiers.Shift)]
>>>>>>> Stashed changes
        static void FlipBrushX()
        {
            if (GridPaintingState.gridBrush != null && GridPaintingState.activeGrid != null)
                FlipBrush(GridBrushBase.FlipAxis.X);
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [FormerlyPrefKeyAs("Grid Painting/Flip Y", "#]")]
        [Shortcut("Grid Painting/Flip Y", typeof(ShortcutContext), KeyCode.RightBracket, ShortcutModifiers.Shift)]
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_FlipY, "#]")]
        [Shortcut(ShortcutIds.k_FlipY, typeof(TilemapEditorTool.ShortcutContext), KeyCode.RightBracket, ShortcutModifiers.Shift)]
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
        [FormerlyPrefKeyAs(ShortcutIds.k_FlipY, "#]")]
        [Shortcut(ShortcutIds.k_FlipY, typeof(TilemapEditorTool.ShortcutContext), KeyCode.RightBracket, ShortcutModifiers.Shift)]
>>>>>>> Stashed changes
        static void FlipBrushY()
        {
            if (GridPaintingState.gridBrush != null && GridPaintingState.activeGrid != null)
                FlipBrush(GridBrushBase.FlipAxis.Y);
        }

        static void ChangeBrushZ(int change)
        {
            GridPaintingState.gridBrush.ChangeZPosition(change);
            GridPaintingState.activeGrid.ChangeZPosition(change);
            GridPaintingState.activeGrid.Repaint();
<<<<<<< Updated upstream
            foreach (var window in GridPaintPaletteWindow.instances)
=======

            foreach (var window in instances)
>>>>>>> Stashed changes
            {
                window.Repaint();
            }
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [Shortcut("Grid Painting/Increase Z", typeof(ShortcutContext), KeyCode.Minus)]
=======
        [Shortcut(ShortcutIds.k_IncreaseZ, typeof(TilemapEditorTool.ShortcutContext), KeyCode.Minus)]
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
        [Shortcut(ShortcutIds.k_IncreaseZ, typeof(TilemapEditorTool.ShortcutContext), KeyCode.Minus)]
>>>>>>> Stashed changes
        static void IncreaseBrushZ()
        {
            if (GridPaintingState.gridBrush != null
                && GridPaintingState.activeGrid != null
                && GridPaintingState.activeBrushEditor != null
                && GridPaintingState.activeBrushEditor.canChangeZPosition)
                ChangeBrushZ(1);
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [Shortcut("Grid Painting/Decrease Z", typeof(ShortcutContext), KeyCode.Equals)]
=======
        [Shortcut(ShortcutIds.k_DecreaseZ, typeof(TilemapEditorTool.ShortcutContext), KeyCode.Equals)]
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
        [Shortcut(ShortcutIds.k_DecreaseZ, typeof(TilemapEditorTool.ShortcutContext), KeyCode.Equals)]
>>>>>>> Stashed changes
        static void DecreaseBrushZ()
        {
            if (GridPaintingState.gridBrush != null
                && GridPaintingState.activeGrid != null
                && GridPaintingState.activeBrushEditor != null
                && GridPaintingState.activeBrushEditor.canChangeZPosition)
                ChangeBrushZ(-1);
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
=======
=======
>>>>>>> Stashed changes
        [Shortcut(ShortcutIds.k_SwitchToNextBrush, typeof(TilemapEditorTool.ShortcutContext), KeyCode.B, ShortcutModifiers.Shift)]
        static void SwitchToNextBrush()
        {
            SwitchBrush(1);
        }

        [Shortcut(ShortcutIds.k_SwitchToPreviousBrush, typeof(TilemapEditorTool.ShortcutContext), KeyCode.B, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        static void SwitchToPreviousBrush()
        {
            SwitchBrush(-1);
        }

        static void SwitchBrush(int change)
        {
            var count = GridPaintingState.brushes.Count;
            var index = GridPaintingState.brushes.IndexOf(GridPaintingState.gridBrush);
            var newIndex = (index + change + count) % count;
            if (index != newIndex)
                GridPaintingState.gridBrush = GridPaintingState.brushes[newIndex];
        }

        [Shortcut(ShortcutIds.k_ToggleSceneViewPalette, typeof(TilemapEditorTool.ShortcutContext), KeyCode.Semicolon)]
        static void ToggleSceneViewPalette()
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null)
                return;

            if (!sceneView.TryGetOverlay(TilePaletteClipboardOverlay.k_OverlayId, out Overlay overlay))
                return;

            var tilePaletteClipboardOverlay = overlay as TilePaletteClipboardOverlay;
            if (tilePaletteClipboardOverlay == null)
                return;

            tilePaletteClipboardOverlay.TogglePopup(GridPaintingState.lastSceneViewMousePosition);
        }

        [Shortcut(ShortcutIds.k_ToggleSceneViewBrushPick, typeof(TilemapEditorTool.ShortcutContext), KeyCode.Quote)]
        static void ToggleSceneViewBrushPick()
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null)
                return;

            if (!sceneView.TryGetOverlay(TilePaletteBrushPickOverlay.k_OverlayId, out Overlay overlay))
                return;

            var tilePaletteBrushPickOverlay = overlay as TilePaletteBrushPickOverlay;
            if (tilePaletteBrushPickOverlay == null)
                return;

            tilePaletteBrushPickOverlay.TogglePopup(GridPaintingState.lastSceneViewMousePosition);
        }

<<<<<<< Updated upstream
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
>>>>>>> Stashed changes
        internal static void PreferencesGUI()
        {
            using (new SettingsWindow.GUIScope())
            {
                EditorGUI.BeginChangeCheck();
<<<<<<< Updated upstream
                var val = (TilePaletteProperties.PrefabEditModeSettings)EditorGUILayout.EnumPopup(TilePaletteProperties.targetEditModeDialogLabel, (TilePaletteProperties.PrefabEditModeSettings)EditorPrefs.GetInt(TilePaletteProperties.targetEditModeEditorPref, 0));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(TilePaletteProperties.targetEditModeEditorPref, (int)val);
=======
                var val = (TilePaletteActiveTargetsProperties.PrefabEditModeSettings)EditorGUILayout.EnumPopup(TilePaletteActiveTargetsProperties.targetEditModeDialogLabel
                    , (TilePaletteActiveTargetsProperties.PrefabEditModeSettings)EditorPrefs.GetInt(TilePaletteActiveTargetsProperties.targetEditModeEditorPref
                        , 0));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(TilePaletteActiveTargetsProperties.targetEditModeEditorPref, (int)val);
>>>>>>> Stashed changes
                }
                EditorGUI.BeginChangeCheck();
                var val2 = EditorGUILayout.Toggle(Styles.mouseGridPositionAtZ, GridPaintingState.gridBrushMousePositionAtZ);
                if (EditorGUI.EndChangeCheck())
                {
                    GridPaintingState.gridBrushMousePositionAtZ = val2;
                }
            }
        }

        private static List<GridPaintPaletteWindow> s_Instances;

        private static List<GridPaintPaletteWindow> instances
        {
            get
            {
                if (s_Instances == null)
                    s_Instances = new List<GridPaintPaletteWindow>();
                return s_Instances;
            }
        }

        /// <summary>
        /// Whether the GridPaintPaletteWindow is active in the Editor
        /// </summary>
        public static bool isActive
        {
            get
            {
                return s_Instances != null && s_Instances.Count > 0;
            }
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        [SerializeField]
        private PreviewResizer m_PreviewResizer;

        private GridPalettesDropdown m_PaletteDropdown;

        [SerializeField]
        private GameObject m_Palette;

        [SerializeField]
        private bool m_DrawGizmos;

        internal bool drawGizmos
        {
            get { return m_DrawGizmos; }
        }

        public GameObject palette
=======
        internal GameObject palette
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
        {
            get
            {
                return m_Palette;
            }
            set
            {
                if (m_Palette != value)
                {
                    clipboardView.OnBeforePaletteSelectionChanged();
                    m_Palette = value;
                    clipboardView.OnAfterPaletteSelectionChanged();
                    lastTilemapPalette = AssetDatabase.GetAssetPath(m_Palette);
                    GridPaintingState.OnPaletteChanged(m_Palette);
                    Repaint();
                }
            }
        }

<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        private GameObject m_PaletteInstance;
        public GameObject paletteInstance
=======
        internal GameObject paletteInstance => clipboardView.paletteInstance;

        internal GridPaintPaletteClipboard clipboardView
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
        {
            get
            {
                return m_PaletteInstance;
            }
        }

        private bool m_DelayedResetPaletteInstance;
        private bool m_Enabled;

        public GridPaintPaletteClipboard clipboardView { get; private set; }

        private Vector2 m_BrushScroll;
        private GridBrushEditorBase m_PreviousToolActivatedEditor;
        private GridBrushBase.Tool m_PreviousToolActivated;

        private PreviewRenderUtility m_PreviewUtility;
        public PreviewRenderUtility previewUtility
        {
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
            get
            {
                if (m_Enabled && m_PreviewUtility == null)
                    InitPreviewUtility();

                return m_PreviewUtility;
            }
        }

        private void OnSelectionChange()
        {
            // Update active palette if user has selected a palette prefab
            var selectedObject = Selection.activeGameObject;
            if (selectedObject != null)
            {
                bool isPrefab = EditorUtility.IsPersistent(selectedObject) || (selectedObject.hideFlags & HideFlags.NotEditable) != 0;
                if (isPrefab)
                {
                    var assetPath = AssetDatabase.GetAssetPath(selectedObject);
                    var allAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                    foreach (var asset in allAssets)
                    {
                        if (asset != null && asset.GetType() == typeof(GridPalette))
                        {
                            var targetPalette = (GameObject)AssetDatabase.LoadMainAssetAtPath(assetPath);
                            if (targetPalette != palette)
                            {
                                palette = targetPalette;
                                Repaint();
                            }
                            break;
                        }
                    }
                }
            }
=======
=======
        internal GameObject palette
        {
            get => GridPaintingState.palette;
            set => GridPaintingState.palette = value;
        }

        internal GameObject paletteInstance => clipboardView.paletteInstance;

        internal GridPaintPaletteClipboard clipboardView
        {
            get => m_ClipboardSplitView.paletteElement.clipboardView;
        }

        private Vector2 m_BrushScroll;
        private bool m_IsUtilityWindow;

        private VisualElement m_ToolbarVisualElement;
        private VisualElement m_ActiveTargetsVisualElement;
        private GridPaintPaletteWindowSplitView m_ClipboardSplitView;

        private void CreateGUI()
        {
>>>>>>> Stashed changes
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(UIStyles.styleSheetPath);
            var skinStyleSheet = EditorGUIUtility.isProSkin
                ? AssetDatabase.LoadAssetAtPath<StyleSheet>(UIStyles.darkStyleSheetPath)
                : AssetDatabase.LoadAssetAtPath<StyleSheet>(UIStyles.lightStyleSheetPath);
            if (styleSheet == null || skinStyleSheet == null)
                return;

            m_ToolbarVisualElement = new GridPaintingToolbar(this);
            m_ActiveTargetsVisualElement = new GridPaintPaletteWindowActiveTargets()
            {
                name = "activeTargetsDropdown",
            };
            m_ClipboardSplitView = new GridPaintPaletteWindowSplitView(tilePaletteVerticalBrushSplit);

            var root = rootVisualElement;
            root.Add(m_ToolbarVisualElement);
            root.Add(m_ActiveTargetsVisualElement);
            root.Add(m_ClipboardSplitView);

            root.styleSheetList.Add(styleSheet);
            root.styleSheetList.Add(skinStyleSheet);
            root.AddToClassList(UIStyles.ussClassName);
            root.style.minHeight = k_MinClipboardHeight;

            root.AddManipulator(new TilePaletteContextMenuHandler(DoContextMenu));
            m_ToolbarVisualElement.AddManipulator(new TilePaletteContextMenuHandler(DoContextMenu));
            m_ActiveTargetsVisualElement.AddManipulator(new TilePaletteContextMenuHandler(DoContextMenu));

            m_ClipboardSplitView.AddManipulator(new TilePaletteDragHandler(DragUpdatedForConvertGridPrefabToPalette, DragPerformedForConvertGridPrefabToPalette));
<<<<<<< Updated upstream
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
=======
>>>>>>> Stashed changes
        }

        private void OnGUI()
        {
<<<<<<< Updated upstream
            HandleContextMenu();

            EditorGUILayout.BeginVertical();
            GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal();
            float leftMargin = (Screen.width / EditorGUIUtility.pixelsPerPoint - TilemapEditorTool.tilemapEditorToolsToolbarSize) * 0.5f;
            GUILayout.Space(leftMargin);
            DoTilemapToolbar();
            GUILayout.Space(leftMargin);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(leftMargin);
            DoActiveTargetsGUI();
            GUILayout.Space(leftMargin);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(6f);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            Rect clipboardToolbarRect = EditorGUILayout.BeginHorizontal(GUIContent.none, Styles.ToolbarTitleStyle);
            DoClipboardHeader();
            EditorGUILayout.EndHorizontal();
            ConvertGridPrefabToPalette(clipboardToolbarRect);
            Rect dragRect = new Rect(k_DropdownWidth + k_ResizerDragRectPadding, 0, position.width - k_DropdownWidth - k_ResizerDragRectPadding, k_ToolbarHeight);
            float brushInspectorSize = m_PreviewResizer.ResizeHandle(position, k_MinBrushInspectorHeight, k_MinClipboardHeight, k_ToolbarHeight, dragRect);
            float clipboardHeight = position.height - brushInspectorSize - k_TopAreaHeight;
            Rect clipboardRect = new Rect(0f, clipboardToolbarRect.yMax, position.width, clipboardHeight);
            OnClipboardGUI(clipboardRect);
            EditorGUILayout.EndVertical();

            GUILayout.Space(clipboardRect.height);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal(GUIContent.none, Styles.ToolbarTitleStyle);
            DoBrushesDropdownToolbar();
            EditorGUILayout.EndHorizontal();
            m_BrushScroll = GUILayout.BeginScrollView(m_BrushScroll, false, false);
            GUILayout.Space(4f);
            OnBrushInspectorGUI();
            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            Color oldColor = Handles.color;
            Handles.color = Color.black;
            Handles.DrawLine(new Vector3(0, clipboardRect.yMax + 0.5f, 0), new Vector3(Screen.width, clipboardRect.yMax + 0.5f, 0));
            Handles.color = Color.black.AlphaMultiplied(0.33f);
            Handles.DrawLine(new Vector3(0, GUILayoutUtility.GetLastRect().yMax + 0.5f, 0), new Vector3(Screen.width, GUILayoutUtility.GetLastRect().yMax + 0.5f, 0));
            Handles.color = oldColor;

            EditorGUILayout.BeginVertical();

            GUILayout.Space(2f);

            EditorGUILayout.EndVertical();

=======
>>>>>>> Stashed changes
            // Keep repainting until all previews are loaded
            if (AssetPreview.IsLoadingAssetPreviews(GetInstanceID()))
                Repaint();

            // Release keyboard focus on click to empty space
            if (Event.current.type == EventType.MouseDown)
                GUIUtility.keyboardControl = 0;
        }

<<<<<<< Updated upstream
        static void DoTilemapToolbar()
        {
            EditorTool active = EditorToolManager.activeTool;
            EditorTool selected;

            if (EditorGUILayout.EditorToolbar(active, TilemapEditorTool.tilemapEditorTools, out selected))
            {
                if (active == selected)
                    ToolManager.SetActiveTool(EditorToolManager.GetLastTool(x => !TilemapEditorTool.tilemapEditorTools.Contains(x)));
                else
                    ToolManager.SetActiveTool(selected);
            }
        }

        public void DelayedResetPreviewInstance()
        {
            m_DelayedResetPaletteInstance = true;
        }

        public void ResetPreviewInstance()
        {
            if (m_PreviewUtility == null)
                InitPreviewUtility();

            m_DelayedResetPaletteInstance = false;
            DestroyPreviewInstance();
            if (palette != null)
            {
                m_PaletteInstance = previewUtility.InstantiatePrefabInScene(palette);

                // Disconnecting prefabs is no longer possible.
                // If performance of overrides on palette palette instance turns out to be a problem.
                // unpack the prefab instance here, and overwrite the prefab later instead of reconnecting.
                PrefabUtility.UnpackPrefabInstance(m_PaletteInstance, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

                EditorUtility.InitInstantiatedPreviewRecursive(m_PaletteInstance);
                m_PaletteInstance.transform.position = new Vector3(0, 0, 0);
                m_PaletteInstance.transform.rotation = Quaternion.identity;
                m_PaletteInstance.transform.localScale = Vector3.one;

                GridPalette paletteAsset = GridPaletteUtility.GetGridPaletteFromPaletteAsset(palette);
                if (paletteAsset != null)
                {
                    if (paletteAsset.cellSizing == GridPalette.CellSizing.Automatic)
                    {
                        Grid grid = m_PaletteInstance.GetComponent<Grid>();
                        if (grid != null)
                        {
                            grid.cellSize = GridPaletteUtility.CalculateAutoCellSize(grid, grid.cellSize);
                        }
                        else
                        {
                            Debug.LogWarning("Grid component not found from: " + palette.name);
                        }
                    }

                    previewUtility.camera.transparencySortMode = paletteAsset.transparencySortMode;
                    previewUtility.camera.transparencySortAxis = paletteAsset.transparencySortAxis;
                }
                else
                {
                    Debug.LogWarning("GridPalette subasset not found from: " + palette.name);
                    previewUtility.camera.transparencySortMode = TransparencySortMode.Default;
                    previewUtility.camera.transparencySortAxis = new Vector3(0f, 0f, 1f);
                }

                foreach (var transform in m_PaletteInstance.GetComponentsInChildren<Transform>())
                    transform.gameObject.hideFlags = HideFlags.HideAndDontSave;

                // Show all renderers from Palettes from previous versions
                PreviewRenderUtility.SetEnabledRecursive(m_PaletteInstance, true);

                clipboardView.ResetPreviewMesh();
            }
        }

        public void DestroyPreviewInstance()
        {
            if (m_PaletteInstance != null)
            {
                Undo.ClearUndo(m_PaletteInstance);
                DestroyImmediate(m_PaletteInstance);
            }
        }

        public void InitPreviewUtility()
        {
            int previewCullingLayer = Camera.PreviewCullingLayer;

            m_PreviewUtility = new PreviewRenderUtility(true, true);
            m_PreviewUtility.camera.cullingMask = 1 << previewCullingLayer;
            m_PreviewUtility.camera.gameObject.layer = previewCullingLayer;
            m_PreviewUtility.lights[0].gameObject.layer = previewCullingLayer;
            m_PreviewUtility.camera.orthographic = true;
            m_PreviewUtility.camera.orthographicSize = 5f;
            m_PreviewUtility.camera.transform.position = new Vector3(0f, 0f, -10f);
            m_PreviewUtility.ambientColor = new Color(1f, 1f, 1f, 0);

            ResetPreviewInstance();
            clipboardView.SetupPreviewCameraOnInit();
        }

        private void HandleContextMenu()
        {
            if (Event.current.type == EventType.ContextClick)
            {
                DoContextMenu();
                Event.current.Use();
            }
        }

        public void SavePalette()
        {
            if (paletteInstance != null && palette != null)
            {
                using (new TilePaletteSaveScope(paletteInstance))
                {
                    string path = AssetDatabase.GetAssetPath(palette);
                    PrefabUtility.SaveAsPrefabAssetAndConnect(paletteInstance, path, InteractionMode.AutomatedAction);
                }

                ResetPreviewInstance();
                Repaint();
            }
        }

=======
>>>>>>> Stashed changes
        private void DoContextMenu()
        {
            GenericMenu pm = new GenericMenu();
            if (GridPaintingState.scenePaintTarget != null)
                pm.AddItem(Styles.selectPaintTarget, false, SelectPaintTarget);
            else
                pm.AddDisabledItem(Styles.selectPaintTarget);

            if (palette != null)
                pm.AddItem(Styles.selectPalettePrefab, false, SelectPaletteAsset);
            else
                pm.AddDisabledItem(Styles.selectPalettePrefab);

            if (clipboardView.activeTile != null)
                pm.AddItem(Styles.selectTileAsset, false, SelectTileAsset);
            else
                pm.AddDisabledItem(Styles.selectTileAsset);

            pm.AddSeparator("");

            if (clipboardView.unlocked)
                pm.AddItem(Styles.lockPaletteEditing, false, FlipLocked);
            else
                pm.AddItem(Styles.unlockPaletteEditing, false, FlipLocked);

<<<<<<< Updated upstream
            pm.AddItem(Styles.openTilePalettePreferences, false, OpenTilePalettePreferences);

            pm.ShowAsContext();
        }

        private void OpenTilePalettePreferences()
        {
            var settingsWindow = SettingsWindow.Show(SettingsScope.User);
            settingsWindow.FilterProviders(TilePaletteProperties.tilePalettePreferencesLookup);
=======
            if (tilePaletteVerticalBrushSplit)
                pm.AddItem(Styles.horizontalBrushSplit, false, FlipShowToolbarInSceneView);
            else
                pm.AddItem(Styles.verticalBrushSplit, false, FlipShowToolbarInSceneView);

            pm.AddItem(Styles.openTilePalettePreferences, false, OpenTilePalettePreferences);

            pm.AddItem(Styles.openAsDockableWindow, !m_IsUtilityWindow, () => OpenWindow(false));
            pm.AddItem(Styles.openAsFloatingWindow, m_IsUtilityWindow, () => OpenWindow(true));

            pm.ShowAsContext();
        }

        private void OpenWindow(bool utility)
        {
            Close();
            GridPaintPaletteWindow w = GetWindow<GridPaintPaletteWindow>(utility, Styles.tilePalette.text, true);
            w.m_IsUtilityWindow = utility;
        }

        private void OpenTilePalettePreferences()
        {
            var settingsWindow = SettingsWindow.Show(SettingsScope.User);
            settingsWindow.FilterProviders(TilePaletteActiveTargetsProperties.tilePalettePreferencesLookup);
>>>>>>> Stashed changes
        }

        private void FlipLocked()
        {
<<<<<<< Updated upstream
            clipboardView.unlocked = !clipboardView.unlocked;
=======
            m_ClipboardSplitView.paletteElement.clipboardUnlocked = !m_ClipboardSplitView.paletteElement.clipboardUnlocked;
        }

        private void FlipShowToolbarInSceneView()
        {
            var state = !m_ClipboardSplitView.isVerticalOrientation;
            tilePaletteVerticalBrushSplit = state;
            m_ClipboardSplitView.isVerticalOrientation = state;

            SceneView.RepaintAll();
>>>>>>> Stashed changes
        }

        private void SelectPaintTarget()
        {
            Selection.activeObject = GridPaintingState.scenePaintTarget;
        }

        private void SelectPaletteAsset()
        {
            Selection.activeObject = palette;
        }

        private void SelectTileAsset()
        {
            Selection.activeObject = clipboardView.activeTile;
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPaintPaletteWindow.cs
        private bool NotOverridingColor(GridBrush defaultGridBrush)
        {
            foreach (var cell in defaultGridBrush.cells)
            {
                TileBase tile = cell.tile;
                if (tile is Tile && ((tile as Tile).flags & TileFlags.LockColor) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void DoBrushesDropdownToolbar()
        {
            GUIContent content = GUIContent.Temp(GridPaintingState.gridBrush.name);
            if (EditorGUILayout.DropdownButton(content, FocusType.Passive, EditorStyles.toolbarPopup, Styles.dropdownOptions))
            {
                var menuData = new GridBrushesDropdown.MenuItemProvider();
                var flexibleMenu = new GridBrushesDropdown(menuData, GridPaletteBrushes.brushes.IndexOf(GridPaintingState.gridBrush), null, SelectBrush, k_DropdownWidth);
                PopupWindow.Show(GUILayoutUtility.topLevel.GetLast(), flexibleMenu);
            }
            if (Event.current.type == EventType.Repaint)
            {
                var dragRect = GUILayoutUtility.GetLastRect();
                var dragIconRect = new Rect();
                dragIconRect.x = dragRect.x + dragRect.width + Styles.dragPadding;
                dragIconRect.y = dragRect.y + (dragRect.height - Styles.dragHandle.fixedHeight) / 2 + 1;
                dragIconRect.width = position.width - (dragIconRect.x) - Styles.dragPadding;
                dragIconRect.height = Styles.dragHandle.fixedHeight;
                Styles.dragHandle.Draw(dragIconRect, GUIContent.none, false, false, false, false);
            }
            GUILayout.FlexibleSpace();
        }

        private void SelectBrush(int i, object o)
        {
            GridPaintingState.gridBrush = GridPaletteBrushes.brushes[i];
        }

        public void OnEnable()
=======
        internal void OnEnable()
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaintPaletteWindow.cs
        {
            m_Enabled = true;
            instances.Add(this);
            if (clipboardView == null)
            {
                clipboardView = CreateInstance<GridPaintPaletteClipboard>();
                clipboardView.owner = this;
                clipboardView.hideFlags = HideFlags.HideAndDontSave;
                clipboardView.unlocked = false;
            }

            if (m_PaintableSceneViewGrid == null)
            {
                m_PaintableSceneViewGrid = CreateInstance<PaintableSceneViewGrid>();
                m_PaintableSceneViewGrid.hideFlags = HideFlags.HideAndDontSave;
            }

            GridPaletteBrushes.FlushCache();
            ShortcutIntegration.instance.profileManager.shortcutBindingChanged += UpdateTooltips;
            GridSelection.gridSelectionChanged += OnGridSelectionChanged;
            GridPaintingState.RegisterPainterInterest(this);
            GridPaintingState.scenePaintTargetChanged += OnScenePaintTargetChanged;
            GridPaintingState.brushChanged += OnBrushChanged;
            SceneView.duringSceneGui += OnSceneViewGUI;
            PrefabUtility.prefabInstanceUpdated += PrefabInstanceUpdated;
            EditorApplication.projectWasLoaded += OnProjectLoaded;

            AssetPreview.SetPreviewTextureCacheSize(256, GetInstanceID());
            wantsMouseMove = true;
            wantsMouseEnterLeaveWindow = true;

            if (m_PreviewResizer == null)
            {
                m_PreviewResizer = new PreviewResizer();
                m_PreviewResizer.Init("TilemapBrushInspector");
            }

            minSize = k_MinWindowSize;

            if (palette == null && !String.IsNullOrEmpty(lastTilemapPalette))
            {
                palette = GridPalettes.palettes
                    .Where((palette, index) => (AssetDatabase.GetAssetPath(palette) == lastTilemapPalette))
                    .FirstOrDefault();
            }
            if (palette == null && GridPalettes.palettes.Count > 0)
            {
                palette = GridPalettes.palettes[0];
            }

            ToolManager.activeToolChanged += ActiveToolChanged;
            ToolManager.activeToolChanging += ActiveToolChanging;

            ShortcutIntegration.instance.contextManager.RegisterToolContext(m_ShortcutContext);
        }

        private static void UpdateTooltips(IShortcutProfileManager obj, Identifier identifier, ShortcutBinding oldBinding, ShortcutBinding newBinding)
        {
            TilemapEditorTool.UpdateTooltips();
        }

        private void PrefabInstanceUpdated(GameObject updatedPrefab)
        {
            // case 947462: Reset the palette instance after its prefab has been updated as it could have been changed
            if (m_PaletteInstance != null && PrefabUtility.GetCorrespondingObjectFromSource(updatedPrefab) == m_Palette && !GridPaintingState.savingPalette)
            {
                ResetPreviewInstance();
                Repaint();
            }
=======
        internal void OnEnable()
        {
            instances.Add(this);

            GridSelection.gridSelectionChanged += OnGridSelectionChanged;
            EditorApplication.projectWasLoaded += OnProjectLoaded;
            ToolManager.activeToolChanged += ActiveToolChanged;

            wantsMouseMove = true;
            wantsMouseEnterLeaveWindow = true;
            minSize = k_MinWindowSize;

            GridPaintingState.RegisterPainterInterest(this);
>>>>>>> Stashed changes
        }

        private void OnProjectLoaded()
        {
<<<<<<< Updated upstream
            // ShortcutIntegration instance is recreated after LoadLayout which wipes the OnEnable registration
            ShortcutIntegration.instance.contextManager.RegisterToolContext(m_ShortcutContext);
        }

        private void OnBrushChanged(GridBrushBase brush)
        {
            DisableFocus();
            if (brush is GridBrush)
                EnableFocus();
            SceneView.RepaintAll();
=======
            GridPaintingState.RegisterShortcutContext();
>>>>>>> Stashed changes
        }

        private void OnGridSelectionChanged()
        {
            Repaint();
        }

        internal void OnDisable()
        {
<<<<<<< Updated upstream
            m_Enabled = false;
            DisableFocus();
            focusMode = TilemapFocusMode.None;

            CallOnToolDeactivated();
            instances.Remove(this);
            if (instances.Count <= 1)
                GridPaintingState.gridBrush = null;
            DestroyPreviewInstance();
            DestroyImmediate(clipboardView);
            DestroyImmediate(m_PaintableSceneViewGrid);

            if (m_PreviewUtility != null)
                m_PreviewUtility.Cleanup();
            m_PreviewUtility = null;

            if (PaintableGrid.InGridEditMode())
            {
                // Set Editor Tool to an always available Tool, as Tile Palette Tools are not available any more
                ToolManager.SetActiveTool<UnityEditor.RectTool>();
            }

            ShortcutIntegration.instance.profileManager.shortcutBindingChanged -= UpdateTooltips;
            ToolManager.activeToolChanged -= ActiveToolChanged;
            ToolManager.activeToolChanging -= ActiveToolChanging;
            GridSelection.gridSelectionChanged -= OnGridSelectionChanged;
            SceneView.duringSceneGui -= OnSceneViewGUI;
            GridPaintingState.scenePaintTargetChanged -= OnScenePaintTargetChanged;
            GridPaintingState.brushChanged -= OnBrushChanged;
            GridPaintingState.UnregisterPainterInterest(this);
            PrefabUtility.prefabInstanceUpdated -= PrefabInstanceUpdated;
            EditorApplication.projectWasLoaded -= OnProjectLoaded;

            ShortcutIntegration.instance.contextManager.DeregisterToolContext(m_ShortcutContext);
        }

        private void OnScenePaintTargetChanged(GameObject scenePaintTarget)
        {
            DisableFocus();
            EnableFocus();
            Repaint();
=======
            GridPaintingState.UnregisterPainterInterest(this);

            ToolManager.activeToolChanged -= ActiveToolChanged;
            GridSelection.gridSelectionChanged -= OnGridSelectionChanged;
            EditorApplication.projectWasLoaded -= OnProjectLoaded;

            instances.Remove(this);
>>>>>>> Stashed changes
        }

        private void ActiveToolChanged()
        {
<<<<<<< Updated upstream
            if (GridPaintingState.gridBrush != null && PaintableGrid.InGridEditMode() && GridPaintingState.activeBrushEditor != null)
            {
                GridBrushBase.Tool tool = PaintableGrid.EditTypeToBrushTool(ToolManager.activeToolType);
                GridPaintingState.activeBrushEditor.OnToolActivated(tool);
                m_PreviousToolActivatedEditor = GridPaintingState.activeBrushEditor;
                m_PreviousToolActivated = tool;

                for (int i = 0; i < k_SceneViewEditModes.Length; ++i)
                {
                    if (k_SceneViewEditModes[i] == tool)
                    {
                        Cursor.SetCursor(MouseStyles.mouseCursorTextures[i],
                            MouseStyles.mouseCursorTextures[i] != null ? MouseStyles.mouseCursorOSHotspot[(int)SystemInfo.operatingSystemFamily] : Vector2.zero,
                            CursorMode.Auto);
                        break;
                    }
                }
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            Repaint();
        }

        private void ActiveToolChanging()
        {
            if (!TilemapEditorTool.IsActive(typeof(MoveTool)) && !TilemapEditorTool.IsActive(typeof(SelectTool)))
            {
                GridSelection.Clear();
            }
            CallOnToolDeactivated();
        }

        private void CallOnToolDeactivated()
        {
            if (GridPaintingState.gridBrush != null && m_PreviousToolActivatedEditor != null)
            {
                m_PreviousToolActivatedEditor.OnToolDeactivated(m_PreviousToolActivated);
                m_PreviousToolActivatedEditor = null;

                if (!PaintableGrid.InGridEditMode())
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
=======
            Repaint();
        }

        private bool ValidateDragAndDrop()
        {
            if (DragAndDrop.objectReferences.Length != 1)
                return false;

            var draggedObject = DragAndDrop.objectReferences[0];
            if (!PrefabUtility.IsPartOfRegularPrefab(draggedObject))
                return false;

            return true;
        }

        private void DragUpdatedForConvertGridPrefabToPalette()
        {
            if (!ValidateDragAndDrop())
                return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }

        private void DragPerformedForConvertGridPrefabToPalette()
        {
            if (!ValidateDragAndDrop())
                return;

            var draggedObject = DragAndDrop.objectReferences[0];
            var path = AssetDatabase.GetAssetPath(draggedObject);
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            bool hasNewPaletteAsset = false;
            Grid gridPrefab = null;
            foreach (var asset in assets)
            {
                var gridPalette = asset as GridPalette;
                hasNewPaletteAsset |= gridPalette != null;
                GameObject go = asset as GameObject;
                if (go != null)
                {
                    var grid = go.GetComponent<Grid>();
                    if (grid != null)
                        gridPrefab = grid;
                }
            }
            if (!hasNewPaletteAsset && gridPrefab != null)
            {
                var cellLayout = gridPrefab.cellLayout;
                var cellSizing = (cellLayout == GridLayout.CellLayout.Rectangle
                    || cellLayout == GridLayout.CellLayout.Hexagon)
                    ? GridPalette.CellSizing.Automatic
                    : GridPalette.CellSizing.Manual;
                var newPalette = GridPaletteUtility.CreateGridPalette(cellSizing);
                AssetDatabase.AddObjectToAsset(newPalette, path);
                AssetDatabase.ForceReserializeAssets(new[] {path});
                AssetDatabase.SaveAssets();
                Event.current.Use();
                GUIUtility.ExitGUI();
>>>>>>> Stashed changes
            }
        }

        internal void ResetZPosition()
        {
            GridPaintingState.gridBrush.ResetZPosition();
            GridPaintingState.lastActiveGrid.ResetZPosition();
        }

<<<<<<< Updated upstream
        private void OnBrushInspectorGUI()
        {
            if (GridPaintingState.gridBrush == null)
                return;

            // Brush Inspector GUI
            EditorGUI.BeginChangeCheck();
            if (GridPaintingState.activeBrushEditor != null)
                GridPaintingState.activeBrushEditor.OnPaintInspectorGUI();
            else if (GridPaintingState.fallbackEditor != null)
                GridPaintingState.fallbackEditor.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                GridPaletteBrushes.ActiveGridBrushAssetChanged();
            }

            // Z Position Inspector
            var hasLastActiveGrid = GridPaintingState.lastActiveGrid != null;
            using (new EditorGUI.DisabledScope(!hasLastActiveGrid))
            {
                var lockZPosition = false;
                if (GridPaintingState.activeBrushEditor != null)
                {
                    EditorGUI.BeginChangeCheck();
                    lockZPosition = EditorGUILayout.Toggle(Styles.lockZPosition, !GridPaintingState.activeBrushEditor.canChangeZPosition);
                    if (EditorGUI.EndChangeCheck())
                        GridPaintingState.activeBrushEditor.canChangeZPosition = !lockZPosition;
                }
                using (new EditorGUI.DisabledScope(lockZPosition))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    var zPosition = EditorGUILayout.DelayedIntField(Styles.zPosition, hasLastActiveGrid ? GridPaintingState.lastActiveGrid.zPosition : 0);
                    if (EditorGUI.EndChangeCheck())
                    {
                        GridPaintingState.gridBrush.ChangeZPosition(zPosition - GridPaintingState.lastActiveGrid.zPosition);
                        GridPaintingState.lastActiveGrid.zPosition = zPosition;
                    }
                    if (GUILayout.Button(Styles.resetZPosition))
                    {
                        ResetZPosition();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private bool IsObjectPrefabInstance(Object target)
        {
            return target != null && PrefabUtility.IsPartOfRegularPrefab(target);
        }

        private GameObject FindPrefabInstanceEquivalent(GameObject prefabInstance, GameObject prefabTarget)
        {
            var prefabRoot = prefabTarget.transform.root.gameObject;
            var currentTransform = prefabTarget.transform;
            var reverseTransformOrder = new Stack<int>();
            while (currentTransform != prefabRoot.transform && currentTransform.parent != null)
            {
                var parentTransform = currentTransform.parent;
                for (int i = 0; i < parentTransform.childCount; ++i)
                {
                    if (currentTransform == parentTransform.GetChild(i))
                    {
                        reverseTransformOrder.Push(i);
                        break;
                    }
                }
                currentTransform = currentTransform.parent;
            }

            currentTransform = prefabInstance.transform;
            while (reverseTransformOrder.Count > 0)
            {
                var childIndex = reverseTransformOrder.Pop();
                if (childIndex >= currentTransform.childCount)
                    return null;
                currentTransform = currentTransform.GetChild(childIndex);
            }
            return currentTransform.gameObject;
        }

        private void GoToPrefabMode(GameObject target)
        {
            var prefabObject = PrefabUtility.GetCorrespondingObjectFromSource(target);
            var assetPath = AssetDatabase.GetAssetPath(prefabObject);
            var stage = PrefabStageUtility.OpenPrefab(assetPath);
            var prefabInstance = stage.prefabContentsRoot;
            var prefabTarget = FindPrefabInstanceEquivalent(prefabInstance, prefabObject);
            if (prefabTarget != null)
            {
                GridPaintingState.scenePaintTarget = prefabTarget;
            }
        }

        private void DoActiveTargetsGUI()
        {
            using (new EditorGUI.DisabledScope(GridPaintingState.validTargets == null || GridPaintingState.scenePaintTarget == null))
            {
                bool hasPaintTarget = GridPaintingState.scenePaintTarget != null;
                bool needWarning = IsObjectPrefabInstance(GridPaintingState.scenePaintTarget);

                GUILayout.Label(Styles.activeTargetLabel, GUILayout.Width(k_ActiveTargetLabelWidth), GUILayout.Height(k_ActiveTargetWarningSize));
                GUIContent content = GUIContent.Temp(hasPaintTarget ? GridPaintingState.scenePaintTarget.name : "Nothing");
                if (EditorGUILayout.DropdownButton(content, FocusType.Passive, EditorStyles.popup, GUILayout.Width(k_ActiveTargetDropdownWidth - (needWarning ? k_ActiveTargetWarningSize : 0f)), GUILayout.Height(k_ActiveTargetWarningSize)))
                {
                    int index = hasPaintTarget ? Array.IndexOf(GridPaintingState.validTargets, GridPaintingState.scenePaintTarget) : 0;
                    var menuData = new GridPaintTargetsDropdown.MenuItemProvider();
                    var flexibleMenu = new GridPaintTargetsDropdown(menuData, index, null, SelectTarget, k_ActiveTargetDropdownWidth);
                    PopupWindow.Show(GUILayoutUtility.topLevel.GetLast(), flexibleMenu);
                }
                if (needWarning)
                    GUILayout.Label(Styles.prefabWarningIcon, GUILayout.Width(k_ActiveTargetWarningSize), GUILayout.Height(k_ActiveTargetWarningSize));
            }
        }

        private void SelectTarget(int i, object o)
        {
            var obj = o as GameObject;
            var isPrefabInstance = IsObjectPrefabInstance(obj);
            if (isPrefabInstance)
            {
                var editMode = (TilePaletteProperties.PrefabEditModeSettings)EditorPrefs.GetInt(TilePaletteProperties.targetEditModeEditorPref, 0);
                switch (editMode)
                {
                    case TilePaletteProperties.PrefabEditModeSettings.EnableDialog:
                    {
                        var option = EditorUtility.DisplayDialogComplex(TilePaletteProperties.targetEditModeDialogTitle
                            , TilePaletteProperties.targetEditModeDialogMessage
                            , TilePaletteProperties.targetEditModeDialogYes
                            , TilePaletteProperties.targetEditModeDialogNo
                            , TilePaletteProperties.targetEditModeDialogChange);
                        switch (option)
                        {
                            case 0:
                                GoToPrefabMode(obj);
                                return;
                            case 1:
                                // Do nothing here for "No"
                                break;
                            case 2:
                                var settingsWindow = SettingsWindow.Show(SettingsScope.User);
                                settingsWindow.FilterProviders(TilePaletteProperties.targetEditModeLookup);
                                break;
                        }
                    }
                    break;
                    case TilePaletteProperties.PrefabEditModeSettings.EditInPrefabMode:
                        GoToPrefabMode(obj);
                        return;
                    case TilePaletteProperties.PrefabEditModeSettings.EditInScene:
                    default:
                        break;
                }
            }

            GridPaintingState.scenePaintTarget = obj;
            if (GridPaintingState.scenePaintTarget != null)
                EditorGUIUtility.PingObject(GridPaintingState.scenePaintTarget);
        }

        private void DoClipboardHeader()
        {
            if (!GridPalettes.palettes.Contains(palette) || palette == null) // Palette not in list means it was deleted
            {
                GridPalettes.CleanCache();
                if (GridPalettes.palettes.Count > 0)
                {
                    palette = GridPalettes.palettes.LastOrDefault();
                }
            }

            EditorGUILayout.BeginHorizontal();
            DoPalettesDropdown();
            using (new EditorGUI.DisabledScope(palette == null))
            {
                clipboardView.unlocked = GUILayout.Toggle(clipboardView.unlocked,
                    clipboardView.isModified ? Styles.editModified : Styles.edit,
                    EditorStyles.toolbarButton);
            }
            GUILayout.FlexibleSpace();
            using (new EditorGUI.DisabledScope(palette == null))
            {
                EditorGUI.BeginChangeCheck();
                m_DrawGizmos = GUILayout.Toggle(m_DrawGizmos, Styles.gizmos, EditorStyles.toolbarButton);
                if (EditorGUI.EndChangeCheck())
                {
                    if (m_DrawGizmos)
                    {
                        clipboardView.SavePaletteIfNecessary();
                        ResetPreviewInstance();
                    }
                    Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DoPalettesDropdown()
        {
            string name = palette != null ? palette.name : Styles.createNewPalette.text;
            Rect rect = GUILayoutUtility.GetRect(GUIContent.Temp(name), EditorStyles.toolbarDropDown, Styles.dropdownOptions);
            if (GridPalettes.palettes.Count == 0)
            {
                if (EditorGUI.DropdownButton(rect, GUIContent.Temp(name), FocusType.Passive, EditorStyles.toolbarDropDown))
                {
                    OpenAddPalettePopup(rect);
                }
            }
            else
            {
                GUIContent content = GUIContent.Temp(GridPalettes.palettes.Count > 0 && palette != null ? palette.name : Styles.createNewPalette.text);
                if (EditorGUI.DropdownButton(rect, content, FocusType.Passive, EditorStyles.toolbarPopup))
                {
                    var menuData = new GridPalettesDropdown.MenuItemProvider();
                    m_PaletteDropdown = new GridPalettesDropdown(menuData, GridPalettes.palettes.IndexOf(palette), null, SelectPalette, k_DropdownWidth);
                    PopupWindow.Show(GUILayoutUtility.topLevel.GetLast(), m_PaletteDropdown);
                }
            }
        }

        private void SelectPalette(int i, object o)
        {
            if (i < GridPalettes.palettes.Count)
            {
                palette = GridPalettes.palettes[i];
            }
            else
            {
                m_PaletteDropdown.editorWindow.Close();
                OpenAddPalettePopup(new Rect(0, 0, 0, 0));
            }
        }

        private void OpenAddPalettePopup(Rect rect)
        {
            bool popupOpened = GridPaletteAddPopup.ShowAtPosition(rect, this);
            if (popupOpened)
                GUIUtility.ExitGUI();
        }

        private void DisplayClipboardText(GUIContent clipboardText, Rect position)
        {
            Color old = GUI.color;
            GUI.color = Color.gray;
            var infoSize = GUI.skin.label.CalcSize(clipboardText);
            Rect rect = new Rect(position.center.x - infoSize.x * .5f, position.center.y - infoSize.y, 500, 100);
            GUI.Label(rect, clipboardText);
            GUI.color = old;
        }

        private void OnClipboardGUI(Rect position)
        {
            if (Event.current.type != EventType.Layout && position.Contains(Event.current.mousePosition) && GridPaintingState.activeGrid != clipboardView && clipboardView.unlocked)
            {
                GridPaintingState.activeGrid = clipboardView;
                SceneView.RepaintAll();
            }

            // Validate palette (case 1017965)
            GUIContent paletteError = null;
            if (palette == null)
            {
                if (GridPalettes.palettes.Count == 0)
                    paletteError = Styles.emptyProjectInfo;
                else
                    paletteError = Styles.invalidPaletteInfo;
            }
            else if (palette.GetComponent<Grid>() == null)
            {
                paletteError = Styles.invalidGridInfo;
            }

            if (paletteError != null)
            {
                DisplayClipboardText(paletteError, position);
                return;
            }

            bool oldEnabled = GUI.enabled;
            GUI.enabled = !clipboardView.showNewEmptyClipboardInfo || DragAndDrop.objectReferences.Length > 0;

            if (Event.current.type == EventType.Repaint)
                clipboardView.guiRect = position;

            if (m_DelayedResetPaletteInstance)
                ResetPreviewInstance();

            EditorGUI.BeginChangeCheck();
            clipboardView.OnGUI();
            if (EditorGUI.EndChangeCheck())
                Repaint();

            GUI.enabled = oldEnabled;

            if (clipboardView.showNewEmptyClipboardInfo)
            {
                DisplayClipboardText(Styles.emptyPaletteInfo, position);
            }
        }

        private void ConvertGridPrefabToPalette(Rect targetPosition)
        {
            if (!targetPosition.Contains(Event.current.mousePosition)
                || (Event.current.type != EventType.DragPerform
                    && Event.current.type != EventType.DragUpdated)
                || DragAndDrop.objectReferences.Length != 1)
                return;

            var draggedObject = DragAndDrop.objectReferences[0];
            if (!PrefabUtility.IsPartOfRegularPrefab(draggedObject))
                return;

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    Event.current.Use();
                    GUI.changed = true;
                }
                break;
                case EventType.DragPerform:
                {
                    var path = AssetDatabase.GetAssetPath(draggedObject);
                    var assets = AssetDatabase.LoadAllAssetsAtPath(path);
                    bool hasNewPaletteAsset = false;
                    Grid gridPrefab = null;
                    foreach (var asset in assets)
                    {
                        var gridPalette = asset as GridPalette;
                        hasNewPaletteAsset |= gridPalette != null;
                        GameObject go = asset as GameObject;
                        if (go != null)
                        {
                            var grid = go.GetComponent<Grid>();
                            if (grid != null)
                                gridPrefab = grid;
                        }
                    }
                    if (!hasNewPaletteAsset && gridPrefab != null)
                    {
                        var cellLayout = gridPrefab.cellLayout;
                        var cellSizing = (cellLayout == GridLayout.CellLayout.Rectangle
                            || cellLayout == GridLayout.CellLayout.Hexagon)
                            ? GridPalette.CellSizing.Automatic
                            : GridPalette.CellSizing.Manual;
                        var newPalette = GridPaletteUtility.CreateGridPalette(cellSizing);
                        AssetDatabase.AddObjectToAsset(newPalette, path);
                        AssetDatabase.ForceReserializeAssets(new string[] {path});
                        AssetDatabase.SaveAssets();
                        Event.current.Use();
                        GUIUtility.ExitGUI();
                    }
                }
                break;
            }
        }

        private void OnSceneViewGUI(SceneView sceneView)
        {
            if (GridPaintingState.defaultBrush != null && GridPaintingState.scenePaintTarget != null)
                SceneViewOverlay.Window(Styles.rendererOverlayTitleLabel, DisplayFocusMode, (int)SceneViewOverlay.Ordering.TilemapRenderer, SceneViewOverlay.WindowDisplayOption.OneWindowPerTitle);
            else if (focusMode != TilemapFocusMode.None)
            {
                // case 946284: Disable Focus if focus mode is set but there is nothing to focus on
                DisableFocus();
                focusMode = TilemapFocusMode.None;
            }
        }

        internal void SetFocusMode(TilemapFocusMode tilemapFocusMode)
        {
            if (tilemapFocusMode != focusMode)
            {
                DisableFocus();
                focusMode = tilemapFocusMode;
                EnableFocus();
            }
        }

        private void DisplayFocusMode(Object displayTarget, SceneView sceneView)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            var fieldWidth = EditorGUIUtility.fieldWidth;
            EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth =
                0.5f * (EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth);
            var newFocus = (TilemapFocusMode)EditorGUILayout.EnumPopup(Styles.focusLabel, focusMode);
            SetFocusMode(newFocus);
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUIUtility.fieldWidth = fieldWidth;
        }

        private void FilterSingleSceneObjectInScene(int instanceID)
        {
            if (SceneView.lastActiveSceneView != null)
                SceneView.lastActiveSceneView.SetSceneViewFiltering(true);

            StageHandle currentStageHandle = StageUtility.GetCurrentStageHandle();
            if (currentStageHandle.IsValid() && !currentStageHandle.isMainStage)
            {
                HierarchyProperty.FilterSingleSceneObjectInScene(instanceID
                    , false
                    , new UnityEngine.SceneManagement.Scene[] { currentStageHandle.customScene });
            }
            else
            {
                HierarchyProperty.FilterSingleSceneObject(instanceID, false);
            }

            if (SceneView.lastActiveSceneView != null)
                SceneView.lastActiveSceneView.Repaint();
        }

        private void EnableFocus()
        {
            if (GridPaintingState.scenePaintTarget == null)
                return;

            switch (focusMode)
            {
                case TilemapFocusMode.Tilemap:
                {
                    FilterSingleSceneObjectInScene(GridPaintingState.scenePaintTarget.GetInstanceID());
                    break;
                }
                case TilemapFocusMode.Grid:
                {
                    Tilemap tilemap = GridPaintingState.scenePaintTarget.GetComponent<Tilemap>();
                    if (tilemap != null && tilemap.layoutGrid != null)
                    {
                        FilterSingleSceneObjectInScene(tilemap.layoutGrid.gameObject.GetInstanceID());
                    }
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        private void DisableFocus()
        {
            if (focusMode == TilemapFocusMode.None)
                return;

            StageHandle currentStageHandle = StageUtility.GetCurrentStageHandle();
            if (currentStageHandle.IsValid() && !currentStageHandle.isMainStage)
            {
                HierarchyProperty.ClearSceneObjectsFilterInScene(new UnityEngine.SceneManagement.Scene[] { currentStageHandle.customScene });
            }
            else
            {
                HierarchyProperty.ClearSceneObjectsFilter();
            }

            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.SetSceneViewFiltering(false);
                SceneView.lastActiveSceneView.Repaint();
            }
        }

=======
>>>>>>> Stashed changes
        [MenuItem("Window/2D/Tile Palette", false, 2)]
        internal static void OpenTilemapPalette()
        {
            GridPaintPaletteWindow w = GetWindow<GridPaintPaletteWindow>();
            w.titleContent = Styles.tilePalette;
<<<<<<< Updated upstream
        }

        // TODO: Better way of clearing caches than AssetPostprocessor
        public class AssetProcessor : AssetPostprocessor
        {
            public override int GetPostprocessOrder()
            {
                return int.MaxValue;
            }

            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
            {
                if (GridPaintingState.savingPalette)
                    return;

                foreach (var window in instances)
                {
                    window.DelayedResetPreviewInstance();
                }
            }
        }

        public class PaletteAssetModificationProcessor : AssetModificationProcessor
        {
            static void OnWillCreateAsset(string assetName)
            {
                SavePalettesIfRequired(null);
            }

            static string[] OnWillSaveAssets(string[] paths)
            {
                SavePalettesIfRequired(paths);
                return paths;
            }

            static void SavePalettesIfRequired(string[] paths)
            {
                if (GridPaintingState.savingPalette)
                    return;

                foreach (var window in instances)
                {
                    if (window.clipboardView.isModified)
                    {
                        window.clipboardView.CheckRevertIfChanged(paths);
                        window.clipboardView.SavePaletteIfNecessary();
                        window.Repaint();
                    }
                }
            }
=======
            w.m_IsUtilityWindow = false;
>>>>>>> Stashed changes
        }
    }
}
