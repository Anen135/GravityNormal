using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject tilePrefab;

    [Header("Settings")]
    public float tileSize = 1f;
    public float spawnHeight = -5f;
    public float fallSpeed = 5f;
    public float riseSpeed = 5f;

    // Все активные плитки
    private Dictionary<Vector2Int, GameObject> tiles =
        new Dictionary<Vector2Int, GameObject>();

    // Плитки которые падают
    private List<GameObject> fallingTiles =
        new List<GameObject>();

    private Vector2Int currentTile;

    void Start()
    {
        UpdateTiles(true);
    }

    void Update()
    {
        UpdateTiles(false);
        HandleFallingTiles();
    }

    void UpdateTiles(bool instant)
    {
        Vector2Int playerTile = new Vector2Int(
            Mathf.RoundToInt(player.position.x / tileSize),
            Mathf.RoundToInt(player.position.z / tileSize)
        );

        // Если персонаж перешёл на другую плитку
        if (playerTile != currentTile || instant)
        {
            currentTile = playerTile;

            // Создаем новую плитку если её нет
            if (!tiles.ContainsKey(currentTile))
            {
                Vector3 spawnPos = new Vector3(
                    currentTile.x * tileSize,
                    instant ? 0 : spawnHeight,
                    currentTile.y * tileSize
                );

                GameObject tile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);

                tiles.Add(currentTile, tile);
            }

            // Все остальные плитки отправляем падать
            List<Vector2Int> toRemove = new List<Vector2Int>();

            foreach (var pair in tiles)
            {
                if (pair.Key != currentTile)
                {
                    fallingTiles.Add(pair.Value);
                    toRemove.Add(pair.Key);
                }
            }

            foreach (var key in toRemove)
            {
                tiles.Remove(key);
            }
        }

        // Поднимаем текущую плитку
        if (tiles.ContainsKey(currentTile))
        {
            GameObject tile = tiles[currentTile];

            Vector3 targetPos = new Vector3(
                currentTile.x * tileSize,
                0,
                currentTile.y * tileSize
            );

            tile.transform.position = Vector3.Lerp(
                tile.transform.position,
                targetPos,
                Time.deltaTime * riseSpeed
            );
        }
    }

    void HandleFallingTiles()
    {
        for (int i = fallingTiles.Count - 1; i >= 0; i--)
        {
            GameObject tile = fallingTiles[i];

            if (tile == null)
            {
                fallingTiles.RemoveAt(i);
                continue;
            }

            tile.transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            // Удаляем далеко упавшие
            if (tile.transform.position.y < -20f)
            {
                Destroy(tile);
                fallingTiles.RemoveAt(i);
            }
        }
    }
}