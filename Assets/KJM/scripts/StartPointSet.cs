using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StartPointSet : MonoBehaviour
{
    [Header("Data")]
    public StartPointSO startPointSO;
    public int currentStageNumber;

    [Header("Tilemap Snap")]
    public Tilemap tilemap;

    [Header("Target Objects")]
    public Transform startObject;
    public Transform goalObject;
    public Transform tentObject;

    [Header("Position Settings")]
    public float startX = 0f;
    public float goalX = 0f;
    public float tentX = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        SetPositions();
    }

    public void SetPositions()
    {
        var stage = startPointSO.stages
            .Find(s => s.stageNumber == currentStageNumber);

        if (stage == null)
            return;

        SetSnappedY(startObject, startX, stage.startPosition);

        SetSnappedY(goalObject, goalX, stage.goalPosition);

        SetSnappedY(tentObject, tentX, stage.startPosition);
    }

    void SetSnappedY(Transform target, float fixedX, int tileY)
    {
        Vector3Int origin = tilemap.cellBounds.min;

        Vector3Int cellPos = new Vector3Int(
            origin.x,          
            origin.y + tileY,      
            0
        );

        Vector3 cellWorld = tilemap.GetCellCenterWorld(cellPos);

        target.position = new Vector3(
            fixedX,
            cellWorld.y,
            target.position.z
        );
    }
}
