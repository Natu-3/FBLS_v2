using System;
using UnityEngine;

namespace UnityEditor.Tilemaps
{
    internal class GridPaletteWhiteboxPalettesDropdown : FlexibleMenu
    {
        public static bool IsOpen = false;

        public GridPaletteWhiteboxPalettesDropdown(IFlexibleMenuItemProvider itemProvider, int selectionIndex, FlexibleMenuModifyItemUI modifyItemUi, Action<int, object> itemClickedCallback, float minWidth)
            : base(itemProvider, selectionIndex, modifyItemUi, itemClickedCallback)
        {
            minTextWidth = minWidth;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            IsOpen = true;
        }

        public override void OnClose()
        {
            IsOpen = false;
            base.OnClose();
        }

        internal class MenuItemProvider : IFlexibleMenuItemProvider
        {
            public int Count()
            {
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPalettesDropdown.cs
                return GridPalettes.palettes.Count + 1;
=======
                return TilePaletteWhiteboxSamplesUtility.whiteboxSampleNames.Count;
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaletteWhiteboxPalettesDropdown.cs
            }

            public object GetItem(int index)
            {
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPalettesDropdown.cs
                if (index < GridPalettes.palettes.Count)
                    return GridPalettes.palettes[index];

=======
                if (index < TilePaletteWhiteboxSamplesUtility.whiteboxSampleNames.Count)
                    return TilePaletteWhiteboxSamplesUtility.whiteboxSampleNames[index];
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaletteWhiteboxPalettesDropdown.cs
                return null;
            }

            public int Add(object obj)
            {
                throw new NotImplementedException();
            }

            public void Replace(int index, object newPresetObject)
            {
                throw new NotImplementedException();
            }

            public void Remove(int index)
            {
                throw new NotImplementedException();
            }

            public object Create()
            {
                throw new NotImplementedException();
            }

            public void Move(int index, int destIndex, bool insertAfterDestIndex)
            {
                throw new NotImplementedException();
            }

            public string GetName(int index)
            {
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPalettesDropdown.cs
                if (index < GridPalettes.palettes.Count)
                    return GridPalettes.palettes[index].name;
                else if (index == GridPalettes.palettes.Count)
                    return "Create New Palette";
                else
                    return "";
=======
                if (index < TilePaletteWhiteboxSamplesUtility.whiteboxSampleNames.Count)
                    return TilePaletteWhiteboxSamplesUtility.whiteboxSampleNames[index];
                return "";
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaletteWhiteboxPalettesDropdown.cs
            }

            public bool IsModificationAllowed(int index)
            {
                return false;
            }

            public int[] GetSeperatorIndices()
            {
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/GridPalettesDropdown.cs
                return new int[] { GridPalettes.palettes.Count - 1 };
=======
                return Array.Empty<int>();
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/GridPaletteWhiteboxPalettesDropdown.cs
            }
        }
    }
}
