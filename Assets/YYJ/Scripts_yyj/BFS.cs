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
            Debug.LogError("TilemapReader�� ���� �����ϴ�.");
            return null;
        }

        // ���� ��ǥ -> �� ��ǥ
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
                // ��ǥ ���� �� ��� ������
                return RetracePath(currentNode);
            }

            // �̿� Ž��
            foreach (Vector3Int neighbourPos in GetNeighbourPositions(currentNode.position))
            {
                if (visited.Contains(neighbourPos))
                {
                    continue;
                }

                // �̵� �������� Ȯ��
                if (!TilemapReader_YYJ.Instance.IsWalkable(neighbourPos))
                {
                    visited.Add(neighbourPos);  // �̵� �Ұ����� �� �湮 ó��
                    continue;   // �̵� �Ұ����̸� ����
                }

                // �� ��� ť�� �߰�
                visited.Add(neighbourPos);
                PathNode neighbourNode = new PathNode(neighbourPos, currentNode);
                queue.Enqueue(neighbourNode);
            }
        }

        return null;
    }

    // �����¿� �̿�
    private static List<Vector3Int> GetNeighbourPositions(Vector3Int currentPos)
    {
        List<Vector3Int> neighbours = new List<Vector3Int>();
        neighbours.Add(currentPos + new Vector3Int(0, 1, 0));
        neighbours.Add(currentPos + new Vector3Int(0, -1, 0));
        neighbours.Add(currentPos + new Vector3Int(1, 0, 0));
        neighbours.Add(currentPos + new Vector3Int(-1, 0, 0));
        return neighbours;
    }

    //��� ������
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
            // �� ��ǥ -> ���� ��ǥ
            worldPath.Add(TilemapReader_YYJ.Instance.CellToWorld(node.position));
        }
        return worldPath;
    }
}
