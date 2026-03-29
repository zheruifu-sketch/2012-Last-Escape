using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class LoopingBackgroundStrip2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private SpriteRenderer sourceRenderer;

    [Header("Follow")]
    [SerializeField] private float horizontalParallax = 0.2f;
    [SerializeField] private float verticalParallax = 0.05f;
    [SerializeField] private bool followCameraY = true;

    [Header("Looping")]
    [SerializeField] private int extraTilesPerSide = 1;

    private readonly List<SpriteRenderer> generatedTiles = new List<SpriteRenderer>();
    private Vector3 basePosition;
    private Vector3 cameraStartPosition;
    private float tileWidth;
    private int currentTileCount;

    private void Reset()
    {
        sourceRenderer = GetComponent<SpriteRenderer>();
        targetCamera = Camera.main;
    }

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
        UpdateTiles();
    }

    private void LateUpdate()
    {
        if (!EnsureReady())
        {
            return;
        }

        UpdateTiles();
    }

    private void OnValidate()
    {
        if (sourceRenderer == null)
        {
            sourceRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Initialize()
    {
        if (sourceRenderer == null)
        {
            sourceRenderer = GetComponent<SpriteRenderer>();
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        basePosition = transform.position;
        cameraStartPosition = targetCamera != null ? targetCamera.transform.position : Vector3.zero;

        if (!EnsureReady())
        {
            return;
        }

        sourceRenderer.drawMode = SpriteDrawMode.Simple;
        tileWidth = sourceRenderer.bounds.size.x;
        RebuildTilesIfNeeded();
    }

    private bool EnsureReady()
    {
        if (sourceRenderer == null || sourceRenderer.sprite == null)
        {
            return false;
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            return false;
        }

        tileWidth = sourceRenderer.bounds.size.x;
        return tileWidth > 0.001f;
    }

    private void RebuildTilesIfNeeded()
    {
        int visibleTileCount = Mathf.CeilToInt(GetCameraWidth() / tileWidth);
        int tilesPerSide = Mathf.Max(1, visibleTileCount / 2 + 1 + extraTilesPerSide);
        int requiredCount = tilesPerSide * 2 + 1;

        if (requiredCount == currentTileCount && generatedTiles.Count == currentTileCount)
        {
            return;
        }

        ClearGeneratedTiles();

        currentTileCount = requiredCount;
        int centerIndex = tilesPerSide;

        for (int i = 0; i < currentTileCount; i++)
        {
            SpriteRenderer tileRenderer;
            if (i == centerIndex)
            {
                tileRenderer = sourceRenderer;
            }
            else
            {
                GameObject tileObject = new GameObject($"LoopTile_{i}");
                tileObject.transform.SetParent(transform, false);
                tileRenderer = tileObject.AddComponent<SpriteRenderer>();
                CopyRendererSettings(tileRenderer, sourceRenderer);
            }

            generatedTiles.Add(tileRenderer);
        }
    }

    private void UpdateTiles()
    {
        RebuildTilesIfNeeded();

        if (generatedTiles.Count == 0)
        {
            return;
        }

        float cameraOffsetX = (targetCamera.transform.position.x - cameraStartPosition.x) * horizontalParallax;
        float desiredCenterX = basePosition.x + cameraOffsetX;
        float snappedCenterX = basePosition.x + Mathf.Round((desiredCenterX - basePosition.x) / tileWidth) * tileWidth;

        float targetY = basePosition.y;
        if (followCameraY)
        {
            targetY += (targetCamera.transform.position.y - cameraStartPosition.y) * verticalParallax;
        }

        int centerIndex = generatedTiles.Count / 2;
        for (int i = 0; i < generatedTiles.Count; i++)
        {
            SpriteRenderer tile = generatedTiles[i];
            if (tile == null)
            {
                continue;
            }

            float offsetX = (i - centerIndex) * tileWidth;
            Transform tileTransform = tile.transform;
            tileTransform.position = new Vector3(snappedCenterX + offsetX, targetY, basePosition.z);
        }
    }

    private float GetCameraWidth()
    {
        if (targetCamera.orthographic)
        {
            return targetCamera.orthographicSize * 2f * targetCamera.aspect;
        }

        float distance = Mathf.Abs(basePosition.z - targetCamera.transform.position.z);
        float height = 2f * distance * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        return height * targetCamera.aspect;
    }

    private void ClearGeneratedTiles()
    {
        for (int i = 0; i < generatedTiles.Count; i++)
        {
            SpriteRenderer tile = generatedTiles[i];
            if (tile == null || tile == sourceRenderer)
            {
                continue;
            }

            if (Application.isPlaying)
            {
                Destroy(tile.gameObject);
            }
            else
            {
                DestroyImmediate(tile.gameObject);
            }
        }

        generatedTiles.Clear();
    }

    private static void CopyRendererSettings(SpriteRenderer target, SpriteRenderer source)
    {
        target.sprite = source.sprite;
        target.color = source.color;
        target.flipX = source.flipX;
        target.flipY = source.flipY;
        target.material = source.sharedMaterial;
        target.sortingLayerID = source.sortingLayerID;
        target.sortingOrder = source.sortingOrder;
        target.maskInteraction = source.maskInteraction;
        target.drawMode = SpriteDrawMode.Simple;
    }
}
