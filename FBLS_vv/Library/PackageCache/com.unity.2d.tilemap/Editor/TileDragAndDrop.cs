using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace UnityEditor.Tilemaps
{
    internal static class TileDragAndDrop
    {
        private enum UserTileCreationMode
        {
            Overwrite,
            CreateUnique,
            Reuse,
        }

        private static readonly string k_TileExtension = "asset";

        private static List<Sprite> GetSpritesFromTexture(Texture2D texture)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            List<Sprite> sprites = new List<Sprite>();

            foreach (Object asset in assets)
            {
                if (asset is Sprite sprite)
                {
                    sprites.Add(sprite);
                }
            }

            return sprites;
        }

        private static bool AllSpritesAreSameSize(List<Sprite> sprites)
        {
            if (!sprites.Any())
            {
                return false;
            }

            // If sprites are different sizes (not grid sliced). So we abort.
            for (int i = 1; i < sprites.Count - 1; i++)
            {
                if ((int)sprites[i].rect.width != (int)sprites[i + 1].rect.width ||
                    (int)sprites[i].rect.height != (int)sprites[i + 1].rect.height)
                {
                    return false;
                }
            }
            return true;
        }

<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/TileDragAndDrop.cs
        // Input:
        // sheetTextures -> textures containing 2-N equal sized Sprites)
        // singleSprites -> All the leftover Sprites that were in same texture but different sizes or just dragged in as Sprite
        // tiles -> Just plain tiles
        public static Dictionary<Vector2Int, Object> CreateHoverData(List<Texture2D> sheetTextures, List<Sprite> singleSprites, List<TileBase> tiles)
        {
            Dictionary<Vector2Int, Object> result = new Dictionary<Vector2Int, Object>();

            Vector2Int currentPosition = new Vector2Int(0, 0);
            int width = 0;
=======
        /// <summary>
        /// Converts Objects that can be laid out in the Tile Palette and organises them for placement into a given CellLayout
        /// </summary>
        /// <param name="sheetTextures">Textures containing 2-N equal sized Sprites</param>
        /// <param name="singleSprites">All the leftover Sprites that were in same texture but different sizes or just dragged in as Sprite</param>
        /// <param name="tiles">Just plain Tiles</param>
        /// <param name="gos">Good old GameObjects</param>
        /// <param name="cellLayout">Cell Layout to place objects on</param>
        /// <returns>Dictionary mapping the positions of the Objects on the Grid Layout with details of how to place the Objects</returns>
        public static Dictionary<Vector2Int, TileDragAndDropHoverData> CreateHoverData(List<Texture2D> sheetTextures
            , List<Sprite> singleSprites
            , List<TileBase> tiles
            , List<GameObject> gos
            , GridLayout.CellLayout cellLayout)
        {
            var result = new Dictionary<Vector2Int, TileDragAndDropHoverData>();
            var currentPosition = new Vector2Int(0, 0);
            var width = 0;
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/TileDragAndDrop.cs

            if (sheetTextures != null)
            {
                foreach (var sheetTexture in sheetTextures)
                {
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/TileDragAndDrop.cs
                    Dictionary<Vector2Int, Object> sheet = CreateHoverData(sheetTexture);
                    foreach (KeyValuePair<Vector2Int, Object> item in sheet)
=======
                    var sheet = CreateHoverData(sheetTexture, cellLayout);
                    foreach (KeyValuePair<Vector2Int, TileDragAndDropHoverData> item in sheet)
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/TileDragAndDrop.cs
                    {
                        result.Add(item.Key + currentPosition, item.Value);
                    }
                    Vector2Int min = GetMinMaxRect(sheet.Keys).min;
                    currentPosition += new Vector2Int(0, min.y - 1);
                }
            }
            if (currentPosition.x > 0)
                currentPosition = new Vector2Int(0, currentPosition.y - 1);

            if (singleSprites != null)
            {
                width = Mathf.RoundToInt(Mathf.Sqrt(singleSprites.Count));
                foreach (Sprite sprite in singleSprites)
                {
                    result.Add(currentPosition, sprite);
                    currentPosition += new Vector2Int(1, 0);
                    if (currentPosition.x >= width)
                        currentPosition = new Vector2Int(0, currentPosition.y - 1);
                }
            }
            if (currentPosition.x > 0)
                currentPosition = new Vector2Int(0, currentPosition.y - 1);

            if (tiles != null)
            {
                width = Math.Max(Mathf.RoundToInt(Mathf.Sqrt(tiles.Count)), width);
                foreach (TileBase tile in tiles)
                {
                    result.Add(currentPosition, tile);
                    currentPosition += new Vector2Int(1, 0);
                    if (currentPosition.x >= width)
                        currentPosition = new Vector2Int(0, currentPosition.y - 1);
                }
            }
            if (currentPosition.x > 0)
                currentPosition = new Vector2Int(0, currentPosition.y - 1);

            if (gos != null)
            {
                width = Math.Max(Mathf.RoundToInt(Mathf.Sqrt(gos.Count)), width);
                foreach (var go in gos)
                {
                    result.Add(currentPosition, new TileDragAndDropHoverData(go));
                    currentPosition += new Vector2Int(1, 0);
                    if (currentPosition.x >= width)
                        currentPosition = new Vector2Int(0, currentPosition.y - 1);
                }
            }

            return result;
        }

        // Get all textures that are valid spritesheets. More than one Sprites and all equal size.
        public static List<Texture2D> GetValidSpritesheets(Object[] objects)
        {
            List<Texture2D> result = new List<Texture2D>();
            foreach (Object obj in objects)
            {
                if (obj is Texture2D texture)
                {
                    List<Sprite> sprites = GetSpritesFromTexture(texture);
                    if (sprites.Count() > 1 && AllSpritesAreSameSize(sprites))
                    {
                        result.Add(texture);
                    }
                }
            }
            return result;
        }

        // Get all single Sprite(s) and all Sprite(s) that are part of Texture2D that is not valid sheet (it sprites of varying sizes)
        public static List<Sprite> GetValidSingleSprites(Object[] objects)
        {
            List<Sprite> result = new List<Sprite>();
            foreach (Object obj in objects)
            {
                if (obj is Sprite sprite)
                {
                    result.Add(sprite);
                }
                else if (obj is Texture2D texture)
                {
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/TileDragAndDrop.cs
                    Texture2D texture = obj as Texture2D;
                    List<Sprite> sprites = GetSpritesFromTexture(texture);
                    if (sprites.Count == 1 || !AllSpritesAreSameSize(sprites))
=======
                    var sprites = GetSpritesFromTexture(texture);
                    if (sprites.Count == 1 || !AllSpritesAreSameSizeOrMultiples(sprites))
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/TileDragAndDrop.cs
                    {
                        result.AddRange(sprites);
                    }
                }
            }
            return result;
        }

        public static List<TileBase> GetValidTiles(Object[] objects)
        {
            var result = new List<TileBase>();
            foreach (var obj in objects)
            {
                if (obj is TileBase tileBase)
                {
                    result.Add(tileBase);
                }
            }
            return result;
        }

        public static List<GameObject> GetValidGameObjects(Object[] objects)
        {
            var result = new List<GameObject>();
            foreach (var obj in objects)
            {
                if (obj is GameObject gameObject)
                {
                    result.Add(gameObject);
                }
            }
            return result;
        }

        public static void FilterForValidGameObjectsForPrefab(Object prefab, List<GameObject> gameObjects)
        {
            for (var i = 0; i < gameObjects.Count; ++i)
            {
                var go = gameObjects[i];
                if (PrefabUtility.IsPartOfAnyPrefab(go))
                {
                    if (PrefabUtility.CheckIfAddingPrefabWouldResultInCyclicNesting(prefab, go))
                    {
                        gameObjects.Remove(go);
                        i--;
                    }
                }
            }
        }

        private static Vector2Int GetMinimum(List<Sprite> sprites, Func<Sprite, float> minX, Func<Sprite, float> minY)
        {
            Vector2 minVector = new Vector2(Int32.MaxValue, Int32.MaxValue);
            foreach (var sprite in sprites)
            {
                minVector.x = Mathf.Min(minVector.x, minX(sprite));
                minVector.y = Mathf.Min(minVector.y, minY(sprite));
            }
            return Vector2Int.FloorToInt(minVector);
        }

        public static Vector2Int EstimateGridPixelSize(List<Sprite> sprites)
        {
            if (sprites.Count == 0)
                return Vector2Int.zero;

            foreach (var sprite in sprites)
            {
                if (sprite == null)
                    return Vector2Int.zero;
            }

            if (sprites.Count == 1)
                return Vector2Int.FloorToInt(sprites[0].rect.size);

            return GetMinimum(sprites, s => s.rect.width, s => s.rect.height);
        }

        public static Vector2Int EstimateGridOffsetSize(List<Sprite> sprites)
        {
            if (sprites.Count == 0)
                return Vector2Int.zero;

            foreach (var sprite in sprites)
            {
                if (sprite == null)
                    return Vector2Int.zero;
            }

            if (sprites.Count == 1)
                return Vector2Int.FloorToInt(sprites[0].rect.position);

            return GetMinimum(sprites, s => s.rect.xMin, s => s.rect.yMin);
        }

        public static Vector2Int EstimateGridPaddingSize(List<Sprite> sprites, Vector2Int cellSize, Vector2Int offsetSize)
        {
            if (sprites.Count < 2)
                return Vector2Int.zero;

            foreach (var sprite in sprites)
            {
                if (sprite == null)
                    return Vector2Int.zero;
            }

            var paddingSize = GetMinimum(sprites
                , (s =>
                {
                    var xMin = s.rect.xMin - cellSize.x - offsetSize.x;
                    return xMin >= 0 ? xMin : Int32.MaxValue;
                })
                , (s =>
                {
                    var yMin = s.rect.yMin - cellSize.y - offsetSize.y;
                    return yMin >= 0 ? yMin : Int32.MaxValue;
                })
            );

            // Assume there is no padding if the detected padding is greater than the cell size
            if (paddingSize.x >= cellSize.x)
                paddingSize.x = 0;
            if (paddingSize.y >= cellSize.y)
                paddingSize.y = 0;
            return paddingSize;
        }

        // Turn texture pixel position into integer grid position based on cell size, offset size and padding
        private static Vector2Int GetGridPosition(Sprite sprite, Vector2Int cellPixelSize, Vector2Int offsetSize, Vector2Int paddingSize)
        {
            return new Vector2Int(
                Mathf.FloorToInt((sprite.rect.center.x - offsetSize.x) / (cellPixelSize.x + paddingSize.x)),
                Mathf.FloorToInt(-(sprite.texture.height - sprite.rect.center.y - offsetSize.y) / (cellPixelSize.y + paddingSize.y)) + 1
            );
        }

        // Organizes all the sprites in a single texture nicely on a 2D "table" based on their original texture position
        // Only call this with spritesheet with all Sprites equal size
        public static Dictionary<Vector2Int, Object> CreateHoverData(Texture2D sheet)
        {
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/TileDragAndDrop.cs
            Dictionary<Vector2Int, Object> result = new Dictionary<Vector2Int, Object>();
            List<Sprite> sprites = GetSpritesFromTexture(sheet);
            Vector2Int cellPixelSize = EstimateGridPixelSize(sprites);
=======
            var result = new Dictionary<Vector2Int, TileDragAndDropHoverData>();
            var sprites = GetSpritesFromTexture(sheet);
            var cellPixelSize = EstimateGridPixelSize(sprites);
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/TileDragAndDrop.cs

            // Get Offset
            var offsetSize = EstimateGridOffsetSize(sprites);

            // Get Padding
            var paddingSize = EstimateGridPaddingSize(sprites, cellPixelSize, offsetSize);

            foreach (Sprite sprite in sprites)
            {
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/TileDragAndDrop.cs
                Vector2Int position = GetGridPosition(sprite, cellPixelSize, offsetSize, paddingSize);
                result[position] = sprite;
=======
                foreach (Sprite sprite in sprites)
                {
                    GetIsometricGridPosition(sprite, cellPixelSize, offsetSize, out Vector2Int position);
                    result[position] = new TileDragAndDropHoverData(sprite, Vector3.zero, (Vector2)cellPixelSize / sprite.pixelsPerUnit, false);
                }
            }
            else
            {
                foreach (Sprite sprite in sprites)
                {
                    GetGridPosition(sprite, cellPixelSize, offsetSize, paddingSize, out Vector2Int position, out Vector3 offset);
                    if (cellLayout == GridLayout.CellLayout.Hexagon)
                        offset -= new Vector3(0.5f, 0.5f, 0.0f);
                    result[position] = new TileDragAndDropHoverData(sprite, offset, (Vector2)cellPixelSize / sprite.pixelsPerUnit);
                }
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/TileDragAndDrop.cs
            }

            return result;
        }

        public static Dictionary<Vector2Int, TileBase> ConvertToTileSheet(Dictionary<Vector2Int, Object> sheet)
        {
            Dictionary<Vector2Int, TileBase> result = new Dictionary<Vector2Int, TileBase>();

<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/TileDragAndDrop.cs
            string defaultPath = ProjectBrowser.s_LastInteractedProjectBrowser
                ? ProjectBrowser.s_LastInteractedProjectBrowser.GetActiveFolderPath()
                : "Assets";

            // Early out if all objects are already tiles
            if (sheet.Values.ToList().FindAll(obj => obj is TileBase).Count == sheet.Values.Count)
=======
        internal static string GenerateUniqueNameForNamelessSprite(Sprite sprite, HashSet<string> uniqueNames, ref int count)
        {
            var baseName = "Nameless";
            if (sprite.texture != null)
                baseName = sprite.texture.name;
            string name;
            do
            {
                name = $"{baseName}_{count++}";
            }
            while (uniqueNames.Contains(name));
            return name;
        }

        public static List<TileBase> ConvertToTileSheet(Dictionary<Vector2Int, TileDragAndDropHoverData> sheet, String tileDirectory = null)
        {
            var result = new List<TileBase>();
            var defaultPath = TileDragAndDropManager.GetDefaultTileAssetDirectoryPath();

            // Early out if all objects are already tiles or GOs
            var sheetCount = sheet.Count;
            var tileCount = 0;
            var nonTileCount = 0;
            string firstName = null;
            foreach (var sheetData in sheet.Values)
            {
                if (sheetData.hoverObject is TileBase)
                    tileCount++;
                if (sheetData.hoverObject is GameObject)
                    nonTileCount++;
                if (string.IsNullOrEmpty(firstName) && sheetData.hoverObject != null)
                    firstName = sheetData.hoverObject.name;
            }
            if (tileCount == sheetCount)
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/TileDragAndDrop.cs
            {
                foreach (KeyValuePair<Vector2Int, Object> item in sheet)
                {
                    result.Add(item.Key, item.Value as TileBase);
                }
            }
            if (tileCount == sheetCount || nonTileCount == sheetCount)
                return result;

            var userTileCreationMode = UserTileCreationMode.Overwrite;
            var path = tileDirectory;
            var multipleTiles = sheetCount > 1;
            var i = 0;
            var uniqueNames = new HashSet<string>();

<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/TileDragAndDrop.cs
            UserTileCreationMode userTileCreationMode = UserTileCreationMode.Overwrite;
            string path = "";
            bool multipleTiles = sheet.Count > 1;
=======
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/TileDragAndDrop.cs
            if (multipleTiles)
            {
                var userInterventionRequired = false;
                if (string.IsNullOrEmpty(path))
                {
                    path = EditorUtility.SaveFolderPanel("Generate tiles into folder ", defaultPath, "");
                    path = FileUtil.GetProjectRelativePath(path);
                }

                // Check if this will overwrite any existing assets
                foreach (var item in sheet.Values)
                {
                    if (item is Sprite)
                    {
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/TileDragAndDrop.cs
                        var tilePath = FileUtil.CombinePaths(path, String.Format("{0}.{1}", item.name, k_TileExtension));
=======
                        var name = sprite.name;
                        if (string.IsNullOrEmpty(name) || uniqueNames.Contains(name))
                        {
                            name = GenerateUniqueNameForNamelessSprite(sprite, uniqueNames, ref i);
                        }
                        uniqueNames.Add(name);
                        var tilePath = FileUtil.CombinePaths(path, string.Format("{0}.{1}", name, k_TileExtension));
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/TileDragAndDrop.cs
                        if (File.Exists(tilePath))
                        {
                            userInterventionRequired = true;
                            break;
                        }
                    }
                }
                // There are existing tile assets in the folder with names matching the items to be created
                if (userInterventionRequired)
                {
                    var option = EditorUtility.DisplayDialogComplex("Overwrite?", string.Format("Assets exist at {0}. Do you wish to overwrite existing assets?", path), "Overwrite", "Create New Copy", "Reuse");
                    switch (option)
                    {
                        case 0: // Overwrite
                        {
                            userTileCreationMode = UserTileCreationMode.Overwrite;
                        }
                        break;
                        case 1: // Create New Copy
                        {
                            userTileCreationMode = UserTileCreationMode.CreateUnique;
                        }
                        break;
                        case 2: // Reuse
                        {
                            userTileCreationMode = UserTileCreationMode.Reuse;
                        }
                        break;
                    }
                }
            }
            else if (string.IsNullOrEmpty(path))
            {
                // Do not check if this will overwrite new tile as user has explicitly selected the file to save to
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/TileDragAndDrop.cs
                path = EditorUtility.SaveFilePanelInProject("Generate new tile", sheet.Values.First().name, k_TileExtension, "Generate new tile", defaultPath);
=======
                path = EditorUtility.SaveFilePanelInProject("Generate new tile", firstName, k_TileExtension, "Generate new tile", defaultPath);
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/TileDragAndDrop.cs
            }

            if (string.IsNullOrEmpty(path))
                return result;

<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/TileDragAndDrop.cs
            int i = 0;
            EditorUtility.DisplayProgressBar("Generating Tile Assets (" + i + "/" + sheet.Count + ")", "Generating tiles", 0f);
=======
            i = 0;
            uniqueNames.Clear();
            EditorUtility.DisplayProgressBar("Generating Tile Assets (" + i + "/" + sheetCount + ")", "Generating tiles", 0f);
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/TileDragAndDrop.cs

            try
            {
                var createTileMethod = GridPaintActiveTargetsPreferences.GetCreateTileFromPaletteUsingPreferences();
                if (createTileMethod == null)
                    return null;

                foreach (KeyValuePair<Vector2Int, Object> item in sheet)
                {
                    TileBase tile;
                    string tilePath = "";
                    if (item.Value is Sprite)
                    {
                        tile = createTileMethod.Invoke(null, new object[] { item.Value as Sprite }) as TileBase;
                        if (tile == null)
                            continue;

<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/TileDragAndDrop.cs
                        tilePath = multipleTiles
                            ? FileUtil.CombinePaths(path, String.Format("{0}.{1}", tile.name, k_TileExtension))
=======
                        var name = tile.name;
                        if (string.IsNullOrEmpty(name) || uniqueNames.Contains(name))
                        {
                            name = GenerateUniqueNameForNamelessSprite(sprite, uniqueNames, ref i);
                        }
                        uniqueNames.Add(name);

                        tilePath = multipleTiles || String.IsNullOrWhiteSpace(FileUtil.GetPathExtension(path))
                            ? FileUtil.CombinePaths(path, $"{name}.{k_TileExtension}")
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/TileDragAndDrop.cs
                            : path;
                        // Case 1216101: Fix path slashes for Windows
                        tilePath = FileUtil.NiceWinPath(tilePath);
                        switch (userTileCreationMode)
                        {
                            case UserTileCreationMode.CreateUnique:
                            {
                                if (File.Exists(tilePath))
                                    tilePath = AssetDatabase.GenerateUniqueAssetPath(tilePath);
                                AssetDatabase.CreateAsset(tile, tilePath);
                            }
                            break;
                            case UserTileCreationMode.Overwrite:
                            {
                                AssetDatabase.CreateAsset(tile, tilePath);
                            }
                            break;
                            case UserTileCreationMode.Reuse:
                            {
                                if (File.Exists(tilePath))
                                    tile = AssetDatabase.LoadAssetAtPath<TileBase>(tilePath);
                                else
                                    AssetDatabase.CreateAsset(tile, tilePath);
                            }
                            break;
                        }
                    }
                    else
                    {
                        tile = item.Value as TileBase;
                    }
<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap@1.0.0/Editor/TileDragAndDrop.cs
                    EditorUtility.DisplayProgressBar("Generating Tile Assets (" + i + "/" + sheet.Count + ")", "Generating " + tilePath, (float)i++ / sheet.Count);
                    result.Add(item.Key, tile);
=======
                    EditorUtility.DisplayProgressBar($"Generating Tile Assets ({i}/{sheet.Count})", $"Generating {tilePath}", (float)i++ / sheet.Count);
                    if (tile != null)
                        result.Add(tile);
>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.2d.tilemap/Editor/TileDragAndDrop.cs
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.Refresh();
            return result;
        }

        internal static RectInt GetMinMaxRect(List<Vector2Int> positions)
        {
            if (positions == null || positions.Count == 0)
                return new RectInt();

            return GridEditorUtility.GetMarqueeRect(
                new Vector2Int(positions.Min(p1 => p1.x), positions.Min(p1 => p1.y)),
                new Vector2Int(positions.Max(p1 => p1.x), positions.Max(p1 => p1.y))
            );
        }
    }
}
