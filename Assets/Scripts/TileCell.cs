using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector2Int coordinates {get; set;}
     
    public Tile tile {get; set;}

    public bool empty => tile == null;
    
    public bool occupied => tile != null;
    
}
