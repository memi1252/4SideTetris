using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board mainBoard;
    public Piece trackingPiece;

    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("Tilemap 컴포넌트를 찾을 수 없습니다.");
        }

        cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        if (trackingPiece == null)
        {
            Debug.LogError("trackingPiece가 null입니다.");
            return;
        }

        if (mainBoard == null)
        {
            Debug.LogError("mainBoard가 null입니다.");
            return;
        }

        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        if (trackingPiece.cells.Length != cells.Length)
        {
            Debug.LogError("trackingPiece.cells 배열 크기가 cells 배열 크기와 일치하지 않습니다.");
            return;
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = trackingPiece.cells[i];
        }
    }

    private void Drop()
    {
        if (GameManager.instance == null) return;

        Vector3Int position = trackingPiece.position;

        switch (GameManager.instance.spawnLocation)
        {
            case GameManager.SpawnLocation.Top:
            {
                int current = position.y;
                int bottom = -mainBoard.boardSize.y / 2 - 1;
                mainBoard.Clear(trackingPiece);
                for (int row = current; row >= bottom; row--)
                {
                    position.y = row;
                    if (mainBoard.IsValidPosition(trackingPiece, position))
                    {
                        this.position = position;
                    }
                    else
                    {
                        break;
                    }
                }

                break;
            }
            case GameManager.SpawnLocation.Bottom:
            {
                int current = position.y;
                int top = mainBoard.boardSize.y / 2;
                mainBoard.Clear(trackingPiece);
                for (int row = current; row <= top; row++)
                {
                    position.y = row;
                    if (mainBoard.IsValidPosition(trackingPiece, position))
                    {
                        this.position = position;
                    }
                    else
                    {
                        break;
                    }
                }

                break;
            }
            case GameManager.SpawnLocation.Left:
            {
                int current = position.x;
                int right = mainBoard.boardSize.x / 2;
                mainBoard.Clear(trackingPiece);
                for (int col = current; col <= right; col++)
                {
                    position.x = col;
                    if (mainBoard.IsValidPosition(trackingPiece, position))
                    {
                        this.position = position;
                    }
                    else
                    {
                        break;
                    }
                }

                break;
            }
            case GameManager.SpawnLocation.Right:
            {
                int current = position.x;
                int left = -mainBoard.boardSize.x / 2;
                mainBoard.Clear(trackingPiece);
                for (int col = current; col >= left-1; col--)
                {
                    position.x = col;
                    if (mainBoard.IsValidPosition(trackingPiece, position))
                    {
                        this.position = position;
                    }
                    else
                    {
                        break;
                    }
                }

                break;
            }
        }

        mainBoard.Set(trackingPiece);
    }
    

    private void Set()
    {
        
        
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, tile);
        }
    }

}
