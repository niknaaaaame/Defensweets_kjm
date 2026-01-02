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

    public bool ComputeAndLockPath(Vector3 startWorld, Vector3 goalWorld)
    {
        Debug.Log("[Path] ComputeAndLockPath 호출");
        if (isLocked) return lockedPathWorld.Count > 0;

        List<Vector3> path = BFS.FindPath(startWorld, goalWorld);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("[Path] 유효한 경로를 찾지 못했습니다.");
            lockedPathWorld.Clear();
            isLocked = false;
            return false;
        }

        lockedPathWorld.Clear();
        lockedPathWorld.AddRange(path);

        isLocked = true;
        return true;
    }
    public bool HasValidPath(Vector3 startWorld, Vector3 goalWorld)
    {
        // 잠금(isLocked)과 상관없이, 지금 타일 상태에서 경로가 있는지만 확인
        List<Vector3> path = BFS.FindPath(startWorld, goalWorld);
        return path != null && path.Count > 0;
    }

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
