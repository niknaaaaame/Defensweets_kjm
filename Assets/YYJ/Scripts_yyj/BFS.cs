using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BFS
{
    private class PathNode
    {
        public Vector3Int position;
        public PathNode parent;

        public PathNode(Vector3Int position, PathNode parent = null)
        {
            this.position = position;
            this.parent = parent;
        }
    }

    public static List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        if (TilemapReader_YYJ.Instance == null)
        {
            Debug.LogError("TilemapReader가 씬에 없습니다.");
            return null;
        }

        // 월드 좌표 -> 셀 좌표
        Vector3Int startPos = TilemapReader_YYJ.Instance.WorldToCell(startWorldPos);
        Vector3Int endPos = TilemapReader_YYJ.Instance.WorldToCell(endWorldPos);

        Queue<PathNode> queue = new Queue<PathNode>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        PathNode startNode = new PathNode(startPos);
        queue.Enqueue(startNode);
        visited.Add(startPos);

        PathNode currentNode = null;

        while (queue.Count > 0)
        {
            currentNode = queue.Dequeue();

            if (currentNode.position == endPos)
            {
                // 목표 도달 시 경로 역추적
                return RetracePath(currentNode);
            }

            // 이웃 탐색
            foreach (Vector3Int neighbourPos in GetNeighbourPositions(currentNode.position))
            {
                /*
                // 경계 벗어났는지 확인
                if (!TilemapReader_YYJ.Instance.IsWithinBounds(neighbourPos))
                {
                    continue;
                }
                */
                if (visited.Contains(neighbourPos))
                {
                    continue;
                }

                // 이동 가능한지 확인
                if (!TilemapReader_YYJ.Instance.IsWalkable(neighbourPos))
                {
                    visited.Add(neighbourPos);  // 이동 불가능한 곳 방문 처리
                    continue;   // 이동 불가능이면 무시
                }

                // 새 경로 큐에 추가
                visited.Add(neighbourPos);
                PathNode neighbourNode = new PathNode(neighbourPos, currentNode);
                queue.Enqueue(neighbourNode);
            }
        }

        return null;
    }

    // 상하좌우 이웃
    private static List<Vector3Int> GetNeighbourPositions(Vector3Int currentPos)
    {
        List<Vector3Int> neighbours = new List<Vector3Int>();
        neighbours.Add(currentPos + new Vector3Int(0, 1, 0));
        neighbours.Add(currentPos + new Vector3Int(0, -1, 0));
        neighbours.Add(currentPos + new Vector3Int(1, 0, 0));
        neighbours.Add(currentPos + new Vector3Int(-1, 0, 0));
        return neighbours;
    }

    //경로 역추적
    private static List<Vector3> RetracePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;
        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        
        List<Vector3> worldPath = new List<Vector3>();
        foreach (var node in path)
        {
            // 셀 좌표 -> 월드 좌표
            worldPath.Add(TilemapReader_YYJ.Instance.CellToWorld(node.position));
        }
        return worldPath;
    }
}
