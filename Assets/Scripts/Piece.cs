using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.5f;
    

    private float stepTime;
    private float moveTime;
    private float lockTime;

    

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.data = data;
        this.board = board;
        this.position = position;

        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        if (cells == null) 
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++) 
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Awake()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.lockButton.onClick.AddListener(() =>
            {
                board.Clear(this);
                HardDrop();
                GameManager.instance.downSound.Play();
                board.Set(this);
            });
            GameManager.instance.rotateButton.onClick.AddListener(() =>
            {
                board.Clear(this);
                Rotate(1);
                board.Set(this);
            });
        }
        
    }
    
    private void Update()
    {
        if (board != null)
        {
            board.Clear(this);
        }
        else
        {
            Debug.LogError("board가 null입니다.");
        }
        
        // We use a timer to allow the player to make adjustments to the piece
        lockTime += Time.deltaTime;
        
        // Handle rotation
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            Rotate(-1);
        } 
        else if (Input.GetKeyDown(KeyCode.E)) 
        {
            Rotate(1);
        }
        
        // Handle hard drop
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            HardDrop();
            if (GameManager.instance != null && GameManager.instance.downSound != null)
            {
                GameManager.instance.downSound.Play();
            }
            else
            {
                Debug.LogError("GameManager 또는 downSound가 null입니다.");
            }
        }
        
        // Allow the player to hold movement keys but only after a move delay
        if (Time.time > moveTime) 
        {
            HandleMoveInputs();
        }
        
        // Advance the piece to the next row every x seconds
        if (Time.time > stepTime) 
        {
            Step();
        }
        
        if (board != null)
        {
            board.Set(this);
        }
        else
        {
            Debug.LogError("board가 null입니다.");
        }
        
    }

    public void MoveUp()
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.spawnLocation != GameManager.SpawnLocation.Top)
            {
                board.Clear(this);
                if (Move(Vector2Int.up)) 
                {
                    // Update the step time to prevent double movement
                    stepTime = Time.time + stepDelay;
                }
                Debug.Log("up");
                board.Set(this);
            }
            Debug.Log("click");
        }

        
    }

    public void MoveDown()
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.spawnLocation != GameManager.SpawnLocation.Bottom)
            {
                board.Clear(this);
                if (Move(Vector2Int.down))
                {
                    // Update the step time to prevent double movement
                    stepTime = Time.time + stepDelay;
                }

                Debug.Log("down");
                board.Set(this);
            }

            Debug.Log("click");
        }


    }

    public void MoveLeft()
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.spawnLocation != GameManager.SpawnLocation.Left)
            {
                board.Clear(this);
                if (Move(Vector2Int.left))
                {
                    stepTime = Time.time + stepDelay;
                }

                Debug.Log("left");
                board.Set(this);
            }

            Debug.Log("click");
        }
    }

    public void MoveRight()
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.spawnLocation != GameManager.SpawnLocation.Right)
            {
                board.Clear(this);
                if (Move(Vector2Int.right))
                {
                    stepTime = Time.time + stepDelay;
                }

                Debug.Log("right");
                board.Set(this);
            }

            Debug.Log("click");
        }
    }

    private void HandleMoveInputs()
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.spawnLocation == GameManager.SpawnLocation.Top)
            {

                // Soft drop movement
                if (Input.GetKey(KeyCode.S))
                {
                    if (Move(Vector2Int.down))
                    {
                        // Update the step time to prevent double movement
                        stepTime = Time.time + stepDelay;
                    }
                }

                // Left/right movement
                if (Input.GetKey(KeyCode.A))
                {
                    if (Move(Vector2Int.left))
                    {

                        stepTime = Time.time + stepDelay;
                    }
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    if (Move(Vector2Int.right))
                    {
                        stepTime = Time.time + stepDelay;
                    }
                }
            }
            else if (GameManager.instance.spawnLocation == GameManager.SpawnLocation.Bottom)
            {


                // Soft drop movement
                if (Input.GetKey(KeyCode.W))
                {
                    if (Move(Vector2Int.up))
                    {
                        // Update the step time to prevent double movement
                        stepTime = Time.time + stepDelay;
                    }
                }

                // Left/right movement
                if (Input.GetKey(KeyCode.A))
                {
                    if (Move(Vector2Int.left))
                    {
                        stepTime = Time.time + stepDelay;
                    }
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    if (Move(Vector2Int.right))
                    {
                        stepTime = Time.time + stepDelay;
                    }
                }
            }
            else if (GameManager.instance.spawnLocation == GameManager.SpawnLocation.Left)
            {


                if (Input.GetKey(KeyCode.W))
                {
                    if (Move(Vector2Int.up))
                    {
                        // Update the step time to prevent double movement
                        stepTime = Time.time + stepDelay;
                    }
                }

                // Soft drop movement
                if (Input.GetKey(KeyCode.S))
                {
                    if (Move(Vector2Int.down))
                    {
                        // Update the step time to prevent double movement
                        stepTime = Time.time + stepDelay;
                    }
                }

                // Left/right movement
                if (Input.GetKey(KeyCode.D))
                {
                    if (Move(Vector2Int.right))
                    {
                        stepTime = Time.time + stepDelay;
                    }
                }


            }
            else if (GameManager.instance.spawnLocation == GameManager.SpawnLocation.Right)
            {

                if (Input.GetKey(KeyCode.W))
                {
                    if (Move(Vector2Int.up))
                    {
                        // Update the step time to prevent double movement
                        stepTime = Time.time + stepDelay;
                    }
                }

                // Soft drop movement
                if (Input.GetKey(KeyCode.S))
                {
                    if (Move(Vector2Int.down))
                    {
                        // Update the step time to prevent double movement
                        stepTime = Time.time + stepDelay;
                    }
                }

                // Left/right movement
                if (Input.GetKey(KeyCode.A))
                {
                    if (Move(Vector2Int.left))
                    {
                        stepTime = Time.time + stepDelay;
                    }
                }
            }
        }


    }

    private void Step()
    {
        if (GameManager.instance != null)
        {
        if(GameManager.instance.spawnLocation == GameManager.SpawnLocation.Top)
        {
            stepTime = Time.time + stepDelay;

            // Step down to the next row
            Move(Vector2Int.down);

            // Once the piece has been inactive for too long it becomes locked
            if (lockTime >= lockDelay) 
            {
                Lock();
            }
        }
        else if(GameManager.instance.spawnLocation == GameManager.SpawnLocation.Bottom)
        {
            stepTime = Time.time + stepDelay;

            // Step down to the next row
            Move(Vector2Int.up);

            // Once the piece has been inactive for too long it becomes locked
            if (lockTime >= lockDelay) 
            {
                Lock();
            }
        }
        else if(GameManager.instance.spawnLocation == GameManager.SpawnLocation.Left)
        {
            stepTime = Time.time + stepDelay;

            // Step down to the next row
            Move(Vector2Int.right);

            // Once the piece has been inactive for too long it becomes locked
            if (lockTime >= lockDelay) 
            {
                Lock();
            }
        }
        else if(GameManager.instance.spawnLocation == GameManager.SpawnLocation.Right)
        {
            stepTime = Time.time + stepDelay;

            // Step down to the next row
            Move(Vector2Int.left);

            // Once the piece has been inactive for too long it becomes locked
            if (lockTime >= lockDelay) 
            {
                Lock();
            }
        }
        
        }
        
    }

    private void HardDrop()
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.spawnLocation == GameManager.SpawnLocation.Top)
            {
                while (Move(Vector2Int.down))
                {
                    continue;
                }
            }
            else if (GameManager.instance.spawnLocation == GameManager.SpawnLocation.Bottom)
            {
                while (Move(Vector2Int.up))
                {
                    continue;
                }
            }
            else if (GameManager.instance.spawnLocation == GameManager.SpawnLocation.Left)
            {
                while (Move(Vector2Int.right))
                {
                    continue;
                }
            }
            else if (GameManager.instance.spawnLocation == GameManager.SpawnLocation.Right)
            {
                while (Move(Vector2Int.left))
                {
                    continue;
                }
            }
        }


        Lock();
    }

    private void Lock()
    {
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();
        if (GameManager.instance != null)
        {
            GameManager.instance.downSound.Play();
        }
    }

    private bool Move(Vector2Int translation)
    {
        
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;
        
        bool valid = board.IsValidPosition(this, newPosition);

        // Only save the movement if the new position is valid
        if (valid)
        {
            position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f; // reset
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        // Store the current rotation in case the rotation fails
        // and we need to revert
        int originalRotation = rotationIndex;

        // Rotate all of the cells using a rotation matrix
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        // Revert the rotation if the wall kick tests fail
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        // Rotate all of the cells using the rotation matrix
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" and "O" are rotated from an offset center point
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation)) 
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0) 
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min) 
        {
            return max - (min - input) % (max - min);
        } 
        else 
        {
            return min + (input - min) % (max - min);
        }
    }

}
