using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Swipe { None, Up, Down, Left, Right };
    public GameManager gameManager;
    private TileGrid grid;

    public TileState [] tileStates;
    private List<Tile> tiles;

    private bool waiting;
    public Tile tilePrefab;

    public AudioSource mergeMusic;




    public float minSwipeLength = 200f;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;
 
    public static Swipe swipeDirection;

    public TimerScript timer;

    private void Awake() 
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }

    public void ClearBoard ()
    {
        foreach (var cell in grid.cells)
        {
            cell.tile = null;
        }
        
        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }

        tiles.Clear(); 
    }
    public void CreateNewTile ()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }
    
    private void Update() 
    {
        DetectSwipe();

        if (!waiting)
        {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || TileBoard.swipeDirection == Swipe.Up) 
        {
            MoveTiles(Vector2Int.up, 0, 1, 1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || TileBoard.swipeDirection == Swipe.Left)
        {
            MoveTiles(Vector2Int.left, 1, 1, 0, 1);
        }   
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || TileBoard.swipeDirection == Swipe.Down)
        {
            MoveTiles(Vector2Int.down, 0, 1, grid.height-2, -1);
        }
        
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || TileBoard.swipeDirection == Swipe.Right)
        {
            MoveTiles(Vector2Int.right, grid.width-2, -1, 0, 1);
        }
        }
    }


    private void MoveTiles (Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {   
        bool changed = false;

        for (int x = startX; x>=0 && x < grid.width; x+= incrementX)
        {
            for (int y = startY; y>=0 && y < grid.height; y+= incrementY)
            {
                TileCell cell = grid.GetCell(x,y);

                if(cell.occupied)
                {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }

        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
             if(adjacent.occupied)
             {
                if (CanMerge(tile, adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
             }

             newCell = adjacent;
             adjacent = grid.GetAdjacentCell(adjacent,direction);
        }
        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
  
        }

        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return a.number == b.number && !b.locked;
    }

    private void Merge(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int number = b.number *2;

        b.SetState(tileStates[index], number);

        mergeMusic.Play(); 

        gameManager.IncreaseScore(number);

    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i])
            {
                return i;
            }
        }
        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;

        yield return new WaitForSeconds (0.1f);

        waiting = false;

        foreach (var tile in tiles)
        {
            tile.locked = false;
        }

        if(tiles.Count != grid.size)
        {
            CreateNewTile();
        }

        if (CheckForGameOver())
        {
            gameManager.GameOver();
            
        }
    }

    private bool CheckForGameOver ()
    {
        if (tiles.Count != grid.size)
         return false;


         foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile))
                return false;
                
            if (down != null && CanMerge(tile, down.tile))
                return false;
                
            if (left != null && CanMerge(tile, left.tile))
                return false;
                
            if (right != null && CanMerge(tile, right.tile))
                return false;


            
        }
        return true;
    }


 
    public void DetectSwipe ()
    {
        if (Input.touches.Length > 0) {
             Touch t = Input.GetTouch(0);
 
             if (t.phase == TouchPhase.Began) {
                 firstPressPos = new Vector2(t.position.x, t.position.y);
             }
 
             if (t.phase == TouchPhase.Ended) {
                secondPressPos = new Vector2(t.position.x, t.position.y);
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
           
                // Make sure it was a legit swipe, not a tap
                if (currentSwipe.magnitude < minSwipeLength) {
                    swipeDirection = Swipe.None;
                    return;
                }
           
                currentSwipe.Normalize();
 
                // Swipe up
                if (currentSwipe.y > 0  && currentSwipe.x > -0.5f  && currentSwipe.x < 0.5f) 
                {
                    swipeDirection = Swipe.Up;
                    Debug.Log("Up");
                // Swipe down
                } 
                else if (currentSwipe.y < 0 &&  currentSwipe.x > -0.5f  && currentSwipe.x < 0.5f)
                 {
                    swipeDirection = Swipe.Down;
                     Debug.Log("Down");
                // Swipe left
                }
                else if (currentSwipe.x < 0  && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) 
                {
                    swipeDirection = Swipe.Left;
                     Debug.Log("Left");
                // Swipe right
                } 
                else if (currentSwipe.x > 0  && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) 
                {
                    swipeDirection = Swipe.Right;
                     Debug.Log("Right");
                }
             }
        } else {
            swipeDirection = Swipe.None;
        }
    }
}
