using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GridManager : MonoBehaviour
{
	public static GridManager Instance;

	[SerializeField] private Tile _tilePrefab;

	public Dictionary<Vector2, Tile> _tiles;

	private void Awake()
	{
		Instance = this;
	}

	public void GenerateGrid()
	{
		_tiles = new Dictionary<Vector2, Tile>();
		for (int x = 0; x < 10; x++)
		{
			for (int y = 0; y < 10; y++)
			{
				if ((x == 2 || x == 3 || x == 6 || x == 7) && (y == 4 || y == 5))
					continue;

				this.SpawnTile(x, y);
			}
		}
	}

	private void SpawnTile(int x, int y)
	{
		Tile spawnedTile = Instantiate(_tilePrefab, new Vector3(x + .5f, y + .5f), Quaternion.identity);
		spawnedTile.name = $"Tile {x} {y}";
		spawnedTile.transform.SetParent(this.transform);

		_tiles[new Vector2(x, y)] = spawnedTile;
	}

	public Tile GetTileAtPosition(Vector2 pos)
	{
		if (_tiles.TryGetValue(pos, out var tile))
			return tile;

		return null;
	}

	public Vector2 GetPositionOfTile(Tile tile)
	{
		return _tiles.FirstOrDefault(x => x.Value == tile).Key;
	}

	public Tile GetSpawnTile(int side)
	{
		/*
		 * side = 0 -> bottom
		 * side = 1 -> top
		 */
		if (side == 0)
			return _tiles.Where(t => t.Key.y < 4 && t.Value.Walkable).OrderBy(t => UnityEngine.Random.value).First().Value;
		else
			return _tiles.Where(t => t.Key.y > 5 && t.Value.Walkable).OrderBy(t => UnityEngine.Random.value).First().Value;
	}

	public void HightLightTileUpdateEveryTile()
	{
		foreach (var tile in _tiles)
		{
			tile.Value.HightLightTileUpdate();
		}
	}
}