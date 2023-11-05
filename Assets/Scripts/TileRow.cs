using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRow : MonoBehaviour
{
    // Start is called before the first frame update
    public TileCell [] cells { get; private set;}

    private void Awake() 
    {
        cells= GetComponentsInChildren<TileCell>();
        
    }
}
