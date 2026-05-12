using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Tilemaps;

public class GenerateCACave : MonoBehaviour
{
    [Header("Size")]
    [SerializeField] private Vector2Int size = new Vector2Int(20, 20);

    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;

    [Header("Tiles")]
    [SerializeField] private TileBase groundTile;
    [SerializeField] private TileBase wallTile;

    [Header("Generation")]
    [Range(0f, 1f)]
    [SerializeField] private float initialWallChance = 0.45f;

    [SerializeField] private int smoothingIterations = 5;

    [Tooltip("If true, uses the seed value. If false, uses a random seed each time.")]
    [SerializeField] private bool useSeed = false;

    [SerializeField] private int seed = 12345;

    private bool[,] walls;

    [Button("Generate")]
    protected void Generate()
    {
        if (groundTilemap == null || wallTilemap == null || groundTile == null || wallTile == null)
        {
            Debug.LogWarning("Missing tilemap or tile reference.");
            return;
        }

        ClearTilemaps();

        GenerateInitialNoise();

        for (int i = 0; i < smoothingIterations; i++)
        {
            SmoothCave();
        }

        DrawTilemaps();
    }

    private void ClearTilemaps()
    {
        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    private void GenerateInitialNoise()
    {
        walls = new bool[size.x, size.y];

        System.Random rng = useSeed
            ? new System.Random(seed)
            : new System.Random();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                bool isBorder =
                    x == 0 ||
                    y == 0 ||
                    x == size.x - 1 ||
                    y == size.y - 1;

                if (isBorder)
                {
                    walls[x, y] = true;
                }
                else
                {
                    walls[x, y] = rng.NextDouble() < initialWallChance;
                }
            }
        }
    }

    private void SmoothCave()
    {
        bool[,] nextWalls = new bool[size.x, size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int wallNeighbours = CountWallNeighbours(x, y);

                bool isBorder =
                    x == 0 ||
                    y == 0 ||
                    x == size.x - 1 ||
                    y == size.y - 1;

                if (isBorder)
                {
                    nextWalls[x, y] = true;
                }
                else
                {
                    if (wallNeighbours > 4)
                    {
                        nextWalls[x, y] = true;
                    }
                    else if (wallNeighbours < 4)
                    {
                        nextWalls[x, y] = false;
                    }
                    else
                    {
                        nextWalls[x, y] = walls[x, y];
                    }
                }
            }
        }

        walls = nextWalls;
    }

    private int CountWallNeighbours(int centerX, int centerY)
    {
        int count = 0;

        for (int offsetX = -1; offsetX <= 1; offsetX++)
        {
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                if (offsetX == 0 && offsetY == 0)
                    continue;

                int x = centerX + offsetX;
                int y = centerY + offsetY;

                if (x < 0 || x >= size.x || y < 0 || y >= size.y)
                {
                    // Treat outside the map as wall.
                    count++;
                }
                else if (walls[x, y])
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void DrawTilemaps()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);

                // Fill the whole area with ground.
                groundTilemap.SetTile(cell, groundTile);

                // Add walls only where needed.
                if (walls[x, y])
                {
                    wallTilemap.SetTile(cell, wallTile);
                }
            }
        }
    }
}
