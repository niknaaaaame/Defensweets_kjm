using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public static class Pathfinding
{
    private class PathNode      // A* �˰��� ����� ��� Ŭ����
    {
        public Vector2Int position;
        public int gCost;       // ���������κ����� ���
        public int hCost;       // ��ǥ�������� ���� ���
        public int fCost;       // G + H
        public PathNode parent;

        public PathNode(Vector2Int position)
        {
            this.position = position;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
    
    public static List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        Vector2Int startPos = new Vector2Int(Mathf.RoundToInt(startWorldPos.x), Mathf.RoundToInt(startWorldPos.y));
        Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(endWorldPos.x), Mathf.RoundToInt(endWorldPos.y));

        List<PathNode> openList = new List<PathNode>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        PathNode startNode = new PathNode(startPos);
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startPos, endPos);
        startNode.CalculateFCost();
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if (currentNode.position == endPos)
            {
                return RetracePath(currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode.position);

            foreach (PathNode neighbourNode in GetNeighbours(currentNode))
            {
                if (closedList.Contains(neighbourNode.position)) continue;

                // �����غ��ϱ� �̰� Ÿ�� �����ϴ°� �־���ϳ� �ϴ� ����
                // if (Ÿ���� �̵��Ҽ� ���� ���)
                // {
                //  closedList.Add(neighbourNode.position);
                //  continue;
                // }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode.position, neighbourNode.position);
                if (tentativeGCost < neighbourNode.gCost || !openList.Contains(neighbourNode))
                {
                    neighbourNode.parent = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode.position, endPos);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }

            }
        }

        return null;
    }

    private static int CalculateDistanceCost(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }

        return lowestFCostNode;
    }

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
            worldPath.Add(new Vector3(node.position.x, node.position.y));
        }
        return worldPath;
    }

    private static List<PathNode> GetNeighbours(PathNode currentNode)
    {
        List<PathNode> neighbours = new List<PathNode>();
        // 4���� (�����¿�)
        neighbours.Add(new PathNode(currentNode.position + new Vector2Int(0, 1)));
        neighbours.Add(new PathNode(currentNode.position + new Vector2Int(0, -1)));
        neighbours.Add(new PathNode(currentNode.position + new Vector2Int(1, 0)));
        neighbours.Add(new PathNode(currentNode.position + new Vector2Int(-1, 0)));
        return neighbours;
    }

}
*/