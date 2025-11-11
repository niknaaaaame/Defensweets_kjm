using System.Collections.Generic;
using UnityEngine;

public class PathSystem : MonoBehaviour
{
    public static PathSystem Instance { get; private set; }

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private Color gizmoColor = Color.cyan;

    private bool isLocked;
    private readonly List<Vector3> lockedPathWorld = new List<Vector3>();

    public bool IsLocked => isLocked;
    public int WaypointCount => lockedPathWorld.Count;
    public Vector3 GetWaypoint(int idx) => lockedPathWorld[idx];
    public IReadOnlyList<Vector3> CurrentPath => lockedPathWorld;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// </summary>
    /// <param name="startWorld">시작점(월드 좌표)</param>
    /// <param name="goalWorld">도착점(월드 좌표)</param>
    /// <returns>경로 계산 및 잠금 성공 여부</returns>
    public bool ComputeAndLockPath(Vector3 startWorld, Vector3 goalWorld)
    {
        if (isLocked) return lockedPathWorld.Count > 0;

        List<Vector3> path = BFS.FindPath(startWorld, goalWorld);

        if (path == null || path.Count == 0)
        {
            lockedPathWorld.Clear();
            isLocked = false;
            return false;
        }

        lockedPathWorld.Clear();
        lockedPathWorld.AddRange(path);

        isLocked = true;
        return true;
    }

    /// <summary>
    /// 경로 잠금 해제. (다음 Ready 단계에서 플레이어가 길을 수정할 수 있게 함)
    /// </summary>
    public void Unlock()
    {
        isLocked = false;
        lockedPathWorld.Clear();
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || lockedPathWorld == null || lockedPathWorld.Count < 2) return;

        Gizmos.color = gizmoColor;
        for (int i = 0; i < lockedPathWorld.Count - 1; i++)
        {
            Gizmos.DrawLine(lockedPathWorld[i], lockedPathWorld[i + 1]);
            Gizmos.DrawSphere(lockedPathWorld[i], 0.08f);
        }
        Gizmos.DrawSphere(lockedPathWorld[lockedPathWorld.Count - 1], 0.08f);
    }
}
