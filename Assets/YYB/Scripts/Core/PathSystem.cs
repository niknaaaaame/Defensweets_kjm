using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// - IsPathReady / WaypointCount / GetWaypoint(idx)     : 몬스터가 이동할 때 사용
public class PathSystem : MonoBehaviour
{
    public static PathSystem Instance { get; private set; }

    [Header("Grid Settings")]
    [SerializeField] private float tileSize = 1f;     // 그리드 크기(셀 한 칸 크기)
    [SerializeField] private bool snapToGrid = true;  // 좌표 스냅 여부

    // 웨이브 동안 사용하는 '잠긴 경로'
    private readonly List<Vector3> lockedWaypoints = new();
    private bool isLocked = false;

    public bool IsPathReady => isLocked && lockedWaypoints.Count >= 2;
    public int WaypointCount => lockedWaypoints.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// 웨이브 시작 직전에 호출: 현재 편집된 타일 상태(walkables)로 최단 경로 계산 → 경로 잠금
    /// <param name="walkableCells">이동 가능한 셀들의 집합 (타일 편집 결과)</param>
    /// <param name="startCell">스폰 지점의 셀 좌표</param>
    /// <param name="goalCell">목표 지점(게이트)의 셀 좌표</param>
    /// <returns>경로 계산 성공 여부</returns>
    public bool ComputeAndLockPath(HashSet<Vector2Int> walkableCells, Vector2Int startCell, Vector2Int goalCell)
    {
        lockedWaypoints.Clear();
        isLocked = false;

        var pathCells = BFSShortestPath(walkableCells, startCell, goalCell);
        if (pathCells == null || pathCells.Count == 0)
        {
            Debug.LogWarning("[PathSystem] 경로 계산 실패 (연결 불가).");
            return false;
        }

        // 셀 → 월드 좌표로 변환하여 웨이포인트 등록
        foreach (var cell in pathCells)
        {
            Vector3 p = CellToWorld(cell);
            if (snapToGrid) p = SnapToGrid(p);
            p.z = 0f; // 2D
            lockedWaypoints.Add(p);
        }

        isLocked = true; // 웨이브 동안 경로 고정
        return true;
    }

    /// <summary>
    /// 웨이브 종료 후 호출: 경로 잠금 해제(다음 Ready에서 편집 가능)
    /// </summary>
    public void Unlock()
    {
        isLocked = false;
        lockedWaypoints.Clear();
    }

    // 몬스터가 사용하는 API
    public Vector3 GetWaypoint(int index)
    {
        if (!IsPathReady) return Vector3.zero;
        index = Mathf.Clamp(index, 0, lockedWaypoints.Count - 1);
        return lockedWaypoints[index];
    }

    public IReadOnlyList<Vector3> GetLockedPath() => lockedWaypoints;

    // ─────────────────────────────────────────────────────────────────────────────
    // 내부: BFS 최단경로 (4방향, 동일 가중치)
    private static readonly Vector2Int[] Neighbor4 = new Vector2Int[]
    {
        new Vector2Int(0,1), new Vector2Int(1,0), new Vector2Int(0,-1), new Vector2Int(-1,0)
    };

    private List<Vector2Int> BFSShortestPath(HashSet<Vector2Int> walkable, Vector2Int start, Vector2Int goal)
    {
        if (!walkable.Contains(start) || !walkable.Contains(goal)) return null;

        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var visited = new HashSet<Vector2Int>();
        var q = new Queue<Vector2Int>();

        visited.Add(start);
        q.Enqueue(start);

        bool found = false;
        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            if (cur == goal) { found = true; break; }

            foreach (var d in Neighbor4)
            {
                var nxt = cur + d;
                if (!walkable.Contains(nxt)) continue;
                if (visited.Contains(nxt)) continue;

                visited.Add(nxt);
                cameFrom[nxt] = cur;
                q.Enqueue(nxt);
            }
        }

        if (!found) return null;

        // 경로 역추적(Goal→Start)
        var path = new List<Vector2Int>();
        var node = goal;
        path.Add(node);
        while (node != start)
        {
            node = cameFrom[node];
            path.Add(node);
        }
        path.Reverse(); // Start→Goal 순서로

        return path;
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // 좌표 변환 유틸
    public Vector2Int WorldToCell(Vector3 world)
    {
        int cx = Mathf.RoundToInt(world.x / tileSize);
        int cy = Mathf.RoundToInt(world.y / tileSize);
        return new Vector2Int(cx, cy);
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        return new Vector3(cell.x * tileSize, cell.y * tileSize, 0f);
    }

    private Vector3 SnapToGrid(Vector3 world)
    {
        world.x = Mathf.Round(world.x / tileSize) * tileSize;
        world.y = Mathf.Round(world.y / tileSize) * tileSize;
        world.z = 0f;
        return world;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!isLocked || lockedWaypoints.Count < 2) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < lockedWaypoints.Count; i++)
        {
            Gizmos.DrawSphere(lockedWaypoints[i], tileSize * 0.15f);
            if (i < lockedWaypoints.Count - 1)
                Gizmos.DrawLine(lockedWaypoints[i], lockedWaypoints[i + 1]);
        }
    }
#endif
}
