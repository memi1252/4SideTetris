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
    // 점수 및 사운드 로직 (변경 없음)
    GameManager.instance.currentPoiont += GameManager.instance.Point;
    GameManager.instance.LineClearSound.Play();
    RectInt bounds = Bounds; // Tilemap의 경계

    // 중앙선의 위치를 명확히 정의합니다.
    // 여기서는 중앙선이 x = 0 또는 y = 0에 걸쳐 있다고 가정합니다.
    int centerRow = bounds.yMax / 2;
    int centerCol = bounds.xMax / 2;

    if (isRow)
    {
        // 1. 해당 줄의 모든 타일을 지웁니다.
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, index, 0);
            tilemap.SetTile(position, null);
        }

        // 2. 타일 이동 로직 (Row Shift)
        if (index < centerRow) // 클리어된 줄이 중앙선 '아래' (Y가 작음)
        {
            // 중앙선 쪽 (Y 증가 방향)으로 타일을 당겨옴 (일반 테트리스와 동일)
            // index부터 시작해서 index + 1의 타일을 index로 옮깁니다.
            for (int row = index; row < centerRow - 1; row++)
            {
                for (int col = bounds.xMin; col < bounds.xMax; col++)
                {
                    Vector3Int abovePosition = new Vector3Int(col, row + 1, 0);
                    TileBase above = tilemap.GetTile(abovePosition);
    
                    Vector3Int currentPosition = new Vector3Int(col, row, 0);
                    tilemap.SetTile(currentPosition, above);
                }
            }
            
            // 3. 가장 바깥쪽 줄을 비웁니다.
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, centerRow - 1, 0);
                tilemap.SetTile(position, null);
            }
            Debug.Log("아래 영역: 위로 당기기 (Y 증가)");
        }
        else // 클리어된 줄이 중앙선 '위' (Y가 큼)
        {
            // 중앙선 쪽 (Y 감소 방향)으로 타일을 당겨옴
            // index부터 시작해서 index - 1의 타일을 index로 옮깁니다.
            for (int row = index; row > centerRow; row--)
            {
                for (int col = bounds.xMin; col < bounds.xMax; col++)
                {
                    Vector3Int belowPosition = new Vector3Int(col, row - 1, 0);
                    TileBase below = tilemap.GetTile(belowPosition);
    
                    Vector3Int currentPosition = new Vector3Int(col, row, 0);
                    tilemap.SetTile(currentPosition, below);
                }
            }
            // 3. 가장 바깥쪽 줄을 비웁니다.
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, bounds.yMax - 1, 0); // YMax 경계의 바로 안쪽
                tilemap.SetTile(position, null);
            }
            Debug.Log("위 영역: 아래로 당기기 (Y 감소)");
        }
        
    }
    else // Column Clear
    {
        // 1. 해당 열의 모든 타일을 지웁니다.
        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            Vector3Int position = new Vector3Int(index, row, 0);
            tilemap.SetTile(position, null);
        }

        // 2. 타일 이동 로직 (Col Shift)
        if (index < centerCol) // 클리어된 열이 중앙선 '왼쪽' (X가 작음)
        {
            // 중앙선 쪽 (X 증가 방향)으로 타일을 당겨옴
            // index부터 시작해서 index + 1의 타일을 index로 옮깁니다.
            for (int col = index; col < centerCol - 1; col++)
            {
                for (int row = bounds.yMin; row < bounds.yMax; row++)
                {
                    Vector3Int rightPosition = new Vector3Int(col + 1, row, 0);
                    TileBase right = tilemap.GetTile(rightPosition);

                    Vector3Int currentPosition = new Vector3Int(col, row, 0);
                    tilemap.SetTile(currentPosition, right);
                }
            }
            // 3. 가장 바깥쪽 열을 비웁니다.
            for (int row = bounds.yMin; row < bounds.yMax; row++)
            {
                Vector3Int position = new Vector3Int(centerCol - 1, row, 0);
                tilemap.SetTile(position, null);
            }
            Debug.Log("왼쪽 영역: 오른쪽으로 당기기 (X 증가)");
        }
        else // 클리어된 열이 중앙선 '오른쪽' (X가 큼)
        {
            // 중앙선 쪽 (X 감소 방향)으로 타일을 당겨옴
            // index부터 시작해서 index - 1의 타일을 index로 옮깁니다.
            for (int col = index ; col > centerCol; col--)
            {
                for (int row = bounds.yMin; row < bounds.yMax; row++)
                {
                    Vector3Int leftPosition = new Vector3Int(col - 1, row, 0);
                    TileBase left = tilemap.GetTile(leftPosition);

                    Vector3Int currentPosition = new Vector3Int(col, row, 0);
                    tilemap.SetTile(currentPosition, left);
                }
            }
            // 3. 가장 바깥쪽 열을 비웁니다.
            for (int row = bounds.yMin; row < bounds.yMax; row++)
            {
                Vector3Int position = new Vector3Int(bounds.xMax - 1, row, 0); // XMax 경계의 바로 안쪽
                tilemap.SetTile(position, null);
            }
            Debug.Log("오른쪽 영역: 왼쪽으로 당기기 (X 감소)");
        }
    }
}
    // public void LineClear(int index, bool isRow)
    // {
    //     GameManager.instance.currentPoiont += GameManager.instance.Point;
    //     GameManager.instance.LineClearSound.Play();
    //     RectInt bounds = Bounds;
    //
    //     if (isRow)
    //     {
    //         // 해당 줄의 모든 타일을 지웁니다.
    //         for (int col = bounds.xMin; col < bounds.xMax; col++)
    //         {
    //                 Vector3Int position = new Vector3Int(col, index, 0);
    //             tilemap.SetTile(position, null);
    //         }
    //
    //         if (index <= 0)
    //         {
    //             // 위의 모든 줄을 한 칸 아래로 이동시킵니다.
    //             for (int row = index; row < bounds.yMax/2; row++)
    //             {
    //                 for (int col = bounds.xMin; col < bounds.xMax; col++)
    //                 {
    //                     Vector3Int position = new Vector3Int(col, row + 1, 0);
    //                     TileBase above = tilemap.GetTile(position);
    //     
    //                     position = new Vector3Int(col, row, 0);
    //                     tilemap.SetTile(position, above);
    //                 }
    //             }
    //             
    //             Debug.Log("아래");
    //         }
    //         else
    //         {
    //             // 위의 모든 줄을 한 칸 위로 이동시킵니다.
    //             for (int row =  index; row > bounds.yMax/2; row--)
    //             {
    //                 for (int col = bounds.xMin; col < bounds.xMax; col++)
    //                 {
    //                     Vector3Int position = new Vector3Int(col, row - 1, 0);
    //                     TileBase above = tilemap.GetTile(position);
    //     
    //                     position = new Vector3Int(col, row, 0);
    //                     tilemap.SetTile(position, above);
    //                 }
    //             }
    //             Debug.Log("위");
    //         }
    //         
    //     }
    //     else
    //     {
    //         
    //         for (int row = bounds.yMin; row < bounds.yMax; row++)
    //         {
    //             Vector3Int position = new Vector3Int(index, row, 0);
    //             tilemap.SetTile(position, null);
    //         }
    //
    //         if (index <= 0)
    //         {
    //             // 오른쪽의 모든 열을 한 칸 왼쪽으로 이동시킵니다.
    //             for (int col = index; col <bounds.xMax / 2; col++)
    //             {
    //                 for (int row = bounds.yMin; row < bounds.yMax; row++)
    //                 {
    //                     Vector3Int position = new Vector3Int(col + 1, row, 0);
    //                     TileBase right = tilemap.GetTile(position);
    //
    //                     position = new Vector3Int(col, row, 0);
    //                     tilemap.SetTile(position, right);
    //                 }
    //             }
    //
    //             Debug.Log("왼쪽");
    //         }
    //         else
    //         {
    //             // 오른쪽의 모든 열을 한 칸 왼쪽으로 이동시킵니다.
    //             for (int col = index ; col >bounds.xMax/2 ; col--)
    //             {
    //                 for (int row = bounds.yMin; row < bounds.yMax; row++)
    //                 {
    //                     Vector3Int position = new Vector3Int(col - 1, row, 0);
    //                     TileBase right = tilemap.GetTile(position);
    //
    //                     position = new Vector3Int(col, row, 0);
    //                     tilemap.SetTile(position, right);
    //                 }
    //             }
    //             Debug.Log("오르쪽");
    //         }
    //         
    //         
    //     }
    // }
}
