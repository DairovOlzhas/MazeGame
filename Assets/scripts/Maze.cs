using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Maze : MonoBehaviour
{

    private int Rows = 10;
    private int Columns = 10;

    public GameObject Floor;
    public GameObject Wall;
    public GameObject Player;
    //public GameObject Camera;


    private MazeCell[,] grid;
    private GameObject player;

    private int currentRow = 0;
    private int currentColumn = 0;

    private int playerX = 0;
    private int playerY= 0;

    void Start()
    {

        string url = Application.absoluteURL;
//        url = "example.com?h=15&w=15h";
        int pm = url.IndexOf('?');
        if (pm != -1)
        {
            string queries = url.Split('?')[1];
            foreach (string q in queries.Split('&')) {
                string qname = q.Split('=')[0];
                string val = q.Split('=')[1];


                if(qname == "h" && int.TryParse(val, out Rows))
                {
                    Debug.Log("Height Parsed");
                }
                else
                {
                    Debug.Log("Height Not Parsed");
                    Rows = 10;
                }

                if (qname == "w" && int.TryParse(val, out Columns))
                {
                    Debug.Log("Width Parsed");
                }
                else
                {
                    Columns = 10;
                    Debug.Log("Width Not Parsed");
                }

            }
        }


        player = Instantiate(Player, new Vector3(0, 0), Quaternion.identity);
        player.name = "Player";
        player.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;

        float size = Wall.transform.localScale.x;

        Camera.main.orthographicSize = size * Rows/2+2;
        Camera.main.aspect = (Columns+0.0f)/Rows;
        Camera.main.transform.position = new Vector3(Columns*size/2 + 0.0f, -Rows*size/2 + 0.0f, 0.0f);
        CreateGrid();   

        HuntAndKill();

        Destroy(grid[0, 0].LeftWall);
        Destroy(grid[Rows - 1, Columns - 1].RightWall);
    }

    void CreateGrid(){
        grid = new MazeCell[Rows, Columns];
        
        float size = Wall.transform.localScale.x;

        for(int i = 0; i < Rows; i++)
        {
            for(int j = 0; j < Columns; j++)
            {

                GameObject floor = Instantiate(Floor, new Vector3(j * size + 2.5f, -i * size - 2.5f), Quaternion.identity);
                floor.name = "Floor_"+i+"_"+j;

                GameObject upWall = Instantiate(Wall, new Vector3(j * size + 2.5f, -i * size), Quaternion.identity);
                upWall.name = "UpWall_"+i+"_"+j;
                upWall.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;


                GameObject downWall = Instantiate(Wall, new Vector3(j * size + 2.5f, -i * size  - 5f), Quaternion.identity);
                downWall.name = "DownWall_"+i+"_"+j;
                downWall.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

                GameObject leftWall = Instantiate(Wall, new Vector3(j * size, -i * size - 2.5f), Quaternion.Euler(0,0,90));
                leftWall.name = "LeftWall_"+i+"_"+j;
                leftWall.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

                GameObject rightWall = Instantiate(Wall, new Vector3(j * size + 5f, -i * size - 2.5f), Quaternion.Euler(0,0,90));
                rightWall.name = "RightWall_"+i+"_"+j;
                rightWall.gameObject.GetComponent<SpriteRenderer>().sortingOrder= 1;


                floor.transform.parent = transform;
                upWall.transform.parent = transform;
                downWall.transform.parent = transform;
                leftWall.transform.parent = transform;
                rightWall.transform.parent = transform;

                grid[i, j] = new MazeCell();

                grid[i, j].UpWall = upWall;
                grid[i, j].DownWall = downWall;
                grid[i, j].RightWall = rightWall;
                grid[i, j].LeftWall = leftWall;
            }
        }

    }

    bool AreThereUnvisitedNeighbors() 
    {
        if(IsCellUnVisited(currentRow - 1, currentColumn))
        {
            return true;
        }

        if (IsCellUnVisited(currentRow + 1, currentColumn))
        {
            return true;
        }

        if (IsCellUnVisited(currentRow, currentColumn - 1))
        {
            return true;
        }

        if (IsCellUnVisited(currentRow, currentColumn + 1))
        {
            return true;
        }

        return false;
    }

    bool IsCellUnVisited(int row, int column)
    {
        if(row >= 0 && row < Rows && column >= 0 && column < Columns
            && !grid[row,column].Visited)
        {
            return true;
        }
        return false;
    }

    void HuntAndKill()
    {
        Walk();
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (!grid[i, j].Visited)
                {
                    currentRow = i;
                    currentColumn = j;
                    grid[i, j].Visited = true;
                    DestroyRandomWallBetweenVisitedNeighbor();
                    Walk();
                }
            }
        }

    }

    void Walk()
    {
        grid[currentRow, currentColumn].Visited = true;


        while (AreThereUnvisitedNeighbors())
        {
            int direction = Random.Range(0, 4);

            // up
            if (direction == 0)
            {
                Debug.Log("Check UP");
                if (IsCellUnVisited(currentRow - 1, currentColumn))
                {
                    if (grid[currentRow, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }

                    currentRow--;
                    grid[currentRow, currentColumn].Visited = true;

                    if (grid[currentRow, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }

                }
            }
            // down
            else if (direction == 1)
            {
                if (IsCellUnVisited(currentRow + 1, currentColumn))
                {
                    if (grid[currentRow, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }

                    currentRow++;
                    grid[currentRow, currentColumn].Visited = true;

                    if (grid[currentRow, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }

                }
            }
            // left        
            else if (direction == 2)
            {
                if (IsCellUnVisited(currentRow, currentColumn - 1))
                {
                    if (grid[currentRow, currentColumn].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }

                    currentColumn--;
                    grid[currentRow, currentColumn].Visited = true;

                    if (grid[currentRow, currentColumn].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }
                }
            }
            // right        
            else if (direction == 3)
            {
                if (IsCellUnVisited(currentRow, currentColumn + 1))
                {
                    if (grid[currentRow, currentColumn].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }

                    currentColumn++;
                    grid[currentRow, currentColumn].Visited = true;

                    if (grid[currentRow, currentColumn].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }
                }
            }
        }

    }


    void DestroyRandomWallBetweenVisitedNeighbor()
    {
        int direction = Random.Range(0, 4);
        Debug.Log("Direction: " + direction);
        bool destroyed = false;
        // check up
        if (direction == 0) { 
            if(currentRow > 0 && grid[currentRow-1, currentColumn].Visited)
            {
                Destroy(grid[currentRow, currentColumn].UpWall);
                Destroy(grid[currentRow - 1, currentColumn].DownWall);
                destroyed = true;
            }
        } 
        // check down
        else if (direction == 1)
        {
            if (currentRow < Rows - 1 && grid[currentRow + 1, currentColumn].Visited)
            {
                Destroy(grid[currentRow, currentColumn].DownWall);
                Destroy(grid[currentRow + 1, currentColumn].UpWall);
                destroyed = true;
            }
        }
        // check left
        else if (direction == 2)
        {
            if (currentColumn > 0 && grid[currentRow, currentColumn - 1].Visited)
            {
                Destroy(grid[currentRow, currentColumn].LeftWall);
                Destroy(grid[currentRow, currentColumn - 1].RightWall);
                destroyed = true;
            }
        }
        // check right
        else if (direction == 3)
        {
            if (currentColumn < Columns - 1 && grid[currentRow, currentColumn + 1].Visited)
            {
                Destroy(grid[currentRow, currentColumn].RightWall);
                Destroy(grid[currentRow, currentColumn + 1].LeftWall);
                destroyed = true;
            }
        }

        if (!destroyed)
        {
            DestroyRandomWallBetweenVisitedNeighbor();
        }
    }


    void Move(MoveDirection direction)
    {
        float size = Wall.transform.localScale.x;

        if (direction == MoveDirection.Left)
        {
            if (playerX > 0 && !grid[playerY, playerX].LeftWall)
            {
                playerX--;
            }
        }
        else if (direction == MoveDirection.Right)
        {
            if (playerX < Columns - 1 && !grid[playerY, playerX].RightWall)
            {
                playerX++;
            }
        }
        else if (direction == MoveDirection.Up)
        {
            if (playerY > 0 && !grid[playerY, playerX].UpWall)
            {
                playerY--;
            }
        }
        else if (direction == MoveDirection.Down)
        {
            if (playerY < Rows - 1 && !grid[playerY, playerX].DownWall)
            {
                playerY++;
            }
        }
        player.transform.position = new Vector2(playerX * 5 + 2.5f, -playerY * 5 - 2.5f);
    }

    // Update is called once per frame
    void Update()
    {
        ListeningArrowPressing();
        ListeningAndroidSwiping();
    }

    void ListeningArrowPressing()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(MoveDirection.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(MoveDirection.Right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(MoveDirection.Up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(MoveDirection.Down);
        }
    }

    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    [SerializeField]
    private bool detectSwipeOnlyAfterRelease = false;

    [SerializeField]
    private float minDistanceForSwipe = 20f;


    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    void ListeningAndroidSwiping()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }
        }

    }


    private void DetectSwipe()
    {
        if (SwipeDistanceCheckMet())
        {
            if (IsVerticalSwipe())
            {
                var direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? MoveDirection.Up : MoveDirection.Down;
                Move(direction);
            }
            else
            {
                var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? MoveDirection.Right : MoveDirection.Left;
                Move(direction);
            }
            fingerUpPosition = fingerDownPosition;
        }
    }


    private bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }

    private bool SwipeDistanceCheckMet()
    {
        return VerticalMovementDistance() > minDistanceForSwipe || HorizontalMovementDistance() > minDistanceForSwipe;
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
    }


}
