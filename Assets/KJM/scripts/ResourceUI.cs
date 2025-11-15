using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    public Text CrystalCandy;
    //public Text Sugar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CrystalCandy.text = $"{ResourceSystem.Instance.Crystal}";
        //Sugar.text = $"{ResourceSystem.Instance.Sugar}";
    }
}
