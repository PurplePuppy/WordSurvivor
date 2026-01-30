using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InfiniteTilemap : MonoBehaviour
{
    // =========================
    // Refs (필수 연결)
    // =========================

    [Header("Refs (필수)")]
    [Tooltip("플레이어 Transform. 이 좌표로 카메라를 강제로 맞춥니다.")]
    public Transform player;

    [Tooltip("메인 카메라. 비워두면 자동으로 Camera.main을 사용합니다.")]
    public Camera cam;

    [Tooltip("바닥 타일을 찍을 Tilemap (Ground).")]
    public Tilemap ground;

    // =========================
    // Ground 생성 설정
    // =========================

    [Header("Ground Generation")]
    [Tooltip("바닥에 깔 기본 타일(Tile Asset).")]
    public TileBase baseTile;

    [Min(1)]
    [Tooltip("청크 한 변의 타일 수. 예: 32면 32x32 타일을 한 번에 생성/삭제합니다.")]
    public int chunkSize = 32;

    [Min(0)]
    [Tooltip("카메라가 보는 영역 바깥쪽으로 추가 생성할 청크 여유분. 가장자리 빈 공간 방지용.")]
    public int paddingChunks = 2;

    // =========================
    // (선택) 월드 경계 벽 생성
    // =========================

    [Header("Optional: World Bounds Wall (선택)")]
    [Tooltip("true면 Start()에서 '월드 크기(mapSizeWorld)'에 맞춰 테두리 벽 타일을 자동 생성합니다.")]
    public bool useWorldBoundsWall = false;

    [Tooltip("맵의 월드 크기(가로,세로). 예: (600,600) => (0~600, 0~600) 또는 원점중심이면 (-300~300) 범위.")]
    public Vector2 mapSizeWorld = new Vector2(600f, 600f);

    [Tooltip("true면 원점(0,0)을 맵 중앙으로: (-w/2~w/2). false면 (0~w) 범위.")]
    public bool centeredAtOrigin = false;

    [Tooltip("벽 경계를 월드 단위로 살짝 밖/안으로 밀기. 0이면 경계 셀에 딱 맞춰 생성.")]
    public float wallMarginWorld = 0f;

    [Tooltip("벽 타일을 찍을 Tilemap (보통 Collision). 여기에 Tilemap Collider 2D를 붙여야 막힙니다.")]
    public Tilemap wallTilemap;

    [Tooltip("벽으로 쓸 타일(Tile Asset). 이 타일은 Collider Type이 None이 아니어야 실제로 막힙니다.")]
    public TileBase wallTile;

    // =========================
    // 내부 상태(런타임 관리)
    // =========================

    // 현재 로드(생성)되어 있는 청크 좌표 목록 (중복 생성 방지)
    private readonly HashSet<Vector2Int> loaded = new HashSet<Vector2Int>();

    void Awake()
    {
        // cam이 비어있으면 메인 카메라 자동 참조
        if (!cam) cam = Camera.main;
    }

    void Start()
    {
        // 벽 기능을 켰다면 시작하자마자 테두리 벽 타일을 깔아둠
        if (useWorldBoundsWall)
            BuildWorldBorderWalls();
    }

    void LateUpdate()
    {
        if (!player || !cam || !ground || !baseTile) return;
        if (!cam.orthographic) return; // 2D(직교) 카메라 기준 로직

        // 1) 카메라 좌표 = 플레이어 좌표 (Z는 카메라 값 유지)
        Vector3 cp = cam.transform.position;
        Vector3 pp = player.position;
        cam.transform.position = new Vector3(pp.x, pp.y, cp.z);

        // 2) 카메라가 현재 '보고 있는 월드 영역'의 4코너 구하기
        float h = cam.orthographicSize * 2f;
        float w = h * cam.aspect;
        Vector3 p = cam.transform.position;

        Vector3 bl = new Vector3(p.x - w * 0.5f, p.y - h * 0.5f, 0f);
        Vector3 br = new Vector3(p.x + w * 0.5f, p.y - h * 0.5f, 0f);
        Vector3 tl = new Vector3(p.x - w * 0.5f, p.y + h * 0.5f, 0f);
        Vector3 tr = new Vector3(p.x + w * 0.5f, p.y + h * 0.5f, 0f);

        // 3) 월드 -> 셀 변환 후, 최소/최대 셀 범위 계산 (아이소에서도 이 방식이 안전)
        Vector3Int c1 = ground.WorldToCell(bl);
        Vector3Int c2 = ground.WorldToCell(br);
        Vector3Int c3 = ground.WorldToCell(tl);
        Vector3Int c4 = ground.WorldToCell(tr);

        int minCellX = Mathf.Min(c1.x, c2.x, c3.x, c4.x);
        int maxCellX = Mathf.Max(c1.x, c2.x, c3.x, c4.x);
        int minCellY = Mathf.Min(c1.y, c2.y, c3.y, c4.y);
        int maxCellY = Mathf.Max(c1.y, c2.y, c3.y, c4.y);

        // 4) 셀 범위를 청크 범위로 변환 (+ 여유분)
        int minChunkX = Mathf.FloorToInt((float)minCellX / chunkSize) - paddingChunks;
        int maxChunkX = Mathf.FloorToInt((float)maxCellX / chunkSize) + paddingChunks;
        int minChunkY = Mathf.FloorToInt((float)minCellY / chunkSize) - paddingChunks;
        int maxChunkY = Mathf.FloorToInt((float)maxCellY / chunkSize) + paddingChunks;

        // 5) 필요한 청크 로드
        for (int y = minChunkY; y <= maxChunkY; y++)
        for (int x = minChunkX; x <= maxChunkX; x++)
        {
            var chunk = new Vector2Int(x, y);
            if (loaded.Add(chunk)) GenerateChunk(chunk);
        }

        // 6) 필요 없는 청크 언로드
        var remove = loaded.Where(chunk => 
            chunk.x < minChunkX || chunk.x > maxChunkX ||
            chunk.y < minChunkY || chunk.y > maxChunkY).ToList();

        foreach (var chunk in remove)
        {
            ClearChunk(chunk);
            loaded.Remove(chunk);
        }
    }

    // 청크(32x32 등)를 생성해서 바닥 타일로 채움
    void GenerateChunk(Vector2Int c)
    {
        int startX = c.x * chunkSize;
        int startY = c.y * chunkSize;

        var bounds = new BoundsInt(startX, startY, 0, chunkSize, chunkSize, 1);
        var tiles = new TileBase[chunkSize * chunkSize];

        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = baseTile;

        ground.SetTilesBlock(bounds, tiles);
    }

    // 청크를 지움(null 타일로 덮어쓰기)
    void ClearChunk(Vector2Int c)
    {
        int startX = c.x * chunkSize;
        int startY = c.y * chunkSize;

        var bounds = new BoundsInt(startX, startY, 0, chunkSize, chunkSize, 1);
        var empty = new TileBase[chunkSize * chunkSize];

        ground.SetTilesBlock(bounds, empty);
    }

    // (선택) 월드 크기에 맞춰 테두리 벽 타일을 깔아두는 함수
    void BuildWorldBorderWalls()
    {
        if (!ground || !wallTilemap || !wallTile) return;
        if (mapSizeWorld.x <= 0 || mapSizeWorld.y <= 0) return;

        Vector2 half = mapSizeWorld * 0.5f;

        float minX = centeredAtOrigin ? -half.x : 0f;
        float minY = centeredAtOrigin ? -half.y : 0f;
        float maxX = centeredAtOrigin ?  half.x : mapSizeWorld.x;
        float maxY = centeredAtOrigin ?  half.y : mapSizeWorld.y;

        // 벽 위치를 약간 보정하고 싶을 때
        minX -= wallMarginWorld; minY -= wallMarginWorld;
        maxX += wallMarginWorld; maxY += wallMarginWorld;

        // 4코너 -> 셀 변환 후 min/max (아이소에서 “막대기 벽” 방지)
        Vector3Int a = ground.WorldToCell(new Vector3(minX, minY, 0f));
        Vector3Int b = ground.WorldToCell(new Vector3(minX, maxY, 0f));
        Vector3Int c = ground.WorldToCell(new Vector3(maxX, minY, 0f));
        Vector3Int d = ground.WorldToCell(new Vector3(maxX, maxY, 0f));

        int x0 = Mathf.Min(a.x, b.x, c.x, d.x);
        int x1 = Mathf.Max(a.x, b.x, c.x, d.x);
        int y0 = Mathf.Min(a.y, b.y, c.y, d.y);
        int y1 = Mathf.Max(a.y, b.y, c.y, d.y);

        // 테두리 한 줄씩 벽 생성
        for (int x = x0; x <= x1; x++)
        {
            wallTilemap.SetTile(new Vector3Int(x, y0, 0), wallTile);
            wallTilemap.SetTile(new Vector3Int(x, y1, 0), wallTile);
        }
        for (int y = y0; y <= y1; y++)
        {
            wallTilemap.SetTile(new Vector3Int(x0, y, 0), wallTile);
            wallTilemap.SetTile(new Vector3Int(x1, y, 0), wallTile);
        }
    }
}