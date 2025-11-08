using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BFS
{
    private class PathNode
    {
        public Vector2Int position;
        public PathNode parent;

        public PathNode(Vector2Int position, PathNode parent = null)
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
        Vector2Int startPos = TilemapReader_YYJ.Instance.WorldToArray(startWorldPos);
        Vector2Int endPos = TilemapReader_YYJ.Instance.WorldToArray(endWorldPos);

        Queue<PathNode> queue = new Queue<PathNode>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

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
            foreach (Vector2Int neighbourPos in GetNeighbourPositions(currentNode.position))
            {
                /*
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
                    continue;
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
    private static List<Vector2Int> GetNeighbourPositions(Vector2Int currentPos)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        neighbours.Add(currentPos + new Vector2Int(0, 1));
        neighbours.Add(currentPos + new Vector2Int(0, -1));
        neighbours.Add(currentPos + new Vector2Int(1, 0));
        neighbours.Add(currentPos + new Vector2Int(-1, 0));
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
            worldPath.Add(TilemapReader_YYJ.Instance.ArrayToWorld(node.position));
        }
        return worldPath;
    }
}
