using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap nextTilemap;
    public Piece activePiece { get; private set; }
    public Piece nextPiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int[] spawnPosition;
    public int nextNumber =-1;

    public RectInt Bounds 
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        activePiece = GetComponentInChildren<Piece>();
        nextPiece = GameManager.instance.GetComponent<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++) 
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int spawnIndex = Random.Range(0, spawnPosition.Length-1);

        GameManager.instance.blockCount++;
        if (spawnIndex == 0)
        {
            GameManager.instance.spawnLocation = GameManager.SpawnLocation.Top;
        }else if (spawnIndex == 1)
        {
            GameManager.instance.spawnLocation = GameManager.SpawnLocation.Right;
        }else if (spawnIndex == 2)
        {
            GameManager.instance.spawnLocation = GameManager.SpawnLocation.Left;
        }else if (spawnIndex == 3)
        {
            GameManager.instance.spawnLocation = GameManager.SpawnLocation.Bottom;
        }

        if (GameManager.instance.nextNumber == -1)
        {
            int random = Random.Range(0, tetrominoes.Length);
            TetrominoData data = tetrominoes[random];
            activePiece.Initialize(this, spawnPosition[spawnIndex], data);
            GameManager.instance.nextNumber = Random.Range(0, tetrominoes.Length);
            TetrominoData nextData = tetrominoes[GameManager.instance.nextNumber];
            SetNext(nextData, spawnPosition[4]);
        }
        else
        {
            nextTilemap.ClearAllTiles();
            TetrominoData data = tetrominoes[GameManager.instance.nextNumber];
            activePiece.Initialize(this, spawnPosition[spawnIndex], data);
            GameManager.instance.nextNumber = Random.Range(0, tetrominoes.Length);
            TetrominoData nextData = tetrominoes[GameManager.instance.nextNumber];
            SetNext(nextData, spawnPosition[4]);
            
        }
        
        if (IsValidPosition(activePiece, new Vector3Int(0, 0, 0))) 
        {
            Set(activePiece);
        } 
        else 
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        //tilemap.ClearAllTiles();
        GameManager.instance.gameOverUI.Show();
        
        // Do anything else you want on game over here..
    }

    public void SetNext(TetrominoData data, Vector3Int position)
    {
        foreach (Vector2Int cell in data.cells)
        {
            Vector3Int tilePosition = (Vector3Int)cell + position;
            nextTilemap.SetTile(tilePosition, data.tile);
        }
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition)) 
            {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition)) 
            {
                return false;
            }
        }

        return true;
    }

    // 모든 완성된 줄을 지웁니다.
    public void ClearLines()
    {
        RectInt bounds = Bounds;
    
        // 가로 방향으로 줄을 지웁니다.
        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            if (IsLineFull(row)) 
            {
                LineClear(row, true); // true는 가로 방향
            }
        } 
    
        
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        { 
            if (IsColumnFull(col)) 
            {
                LineClear(col, false);
            }
        }
    }
    
    
   
    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;
    
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
    
            // 타일이 없으면 줄이 가득 차지 않은 것으로 간주합니다.
            if (!tilemap.HasTile(position)) {
                return false;
            }
        }
    
        return true;
    }

    public bool IsColumnFull(int col)
    {
        RectInt bounds = Bounds;

        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }
        
        return true;
    }
    
    
    public void LineClear(int index, bool isRow)
    {
        GameManager.instance.currentPoiont += GameManager.instance.Point;
        GameManager.instance.LineClearSound.Play();
        RectInt bounds = Bounds;
    
        if (isRow)
        {
            // 해당 줄의 모든 타일을 지웁니다.
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                    Vector3Int position = new Vector3Int(col, index, 0);
                tilemap.SetTile(position, null);
            }

            if (index <= 0)
            {
                // 위의 모든 줄을 한 칸 아래로 이동시킵니다.
                for (int row = index; row < bounds.yMax/2; row++)
                {
                    for (int col = bounds.xMin; col < bounds.xMax; col++)
                    {
                        Vector3Int position = new Vector3Int(col, row + 1, 0);
                        TileBase above = tilemap.GetTile(position);
        
                        position = new Vector3Int(col, row, 0);
                        tilemap.SetTile(position, above);
                    }
                }
                
                Debug.Log("아래");
            }
            else
            {
                // 위의 모든 줄을 한 칸 위로 이동시킵니다.
                for (int row =  index; row > bounds.yMax/2; row--)
                {
                    for (int col = bounds.xMin; col < bounds.xMax; col++)
                    {
                        Vector3Int position = new Vector3Int(col, row - 1, 0);
                        TileBase above = tilemap.GetTile(position);
        
                        position = new Vector3Int(col, row, 0);
                        tilemap.SetTile(position, above);
                    }
                }
                Debug.Log("위");
            }
            
        }
        else
        {
            
            for (int row = bounds.yMin; row < bounds.yMax; row++)
            {
                Vector3Int position = new Vector3Int(index, row, 0);
                tilemap.SetTile(position, null);
            }

            if (index <= 0)
            {
                // 오른쪽의 모든 열을 한 칸 왼쪽으로 이동시킵니다.
                for (int col = index; col <bounds.xMax / 2; col++)
                {
                    for (int row = bounds.yMin; row < bounds.yMax; row++)
                    {
                        Vector3Int position = new Vector3Int(col + 1, row, 0);
                        TileBase right = tilemap.GetTile(position);

                        position = new Vector3Int(col, row, 0);
                        tilemap.SetTile(position, right);
                    }
                }

                Debug.Log("왼쪽");
            }
            else
            {
                // 오른쪽의 모든 열을 한 칸 왼쪽으로 이동시킵니다.
                for (int col = index ; col >bounds.xMax/2 ; col--)
                {
                    for (int row = bounds.yMin; row < bounds.yMax; row++)
                    {
                        Vector3Int position = new Vector3Int(col - 1, row, 0);
                        TileBase right = tilemap.GetTile(position);

                        position = new Vector3Int(col, row, 0);
                        tilemap.SetTile(position, right);
                    }
                }
                Debug.Log("오르쪽");
            }
            
            
        }
    }
}
