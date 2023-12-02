using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private float tickTime;
    public SnakeComponent Head { get; private set; }
    public SnakeComponent Tail { get; private set; }
    public Vector2Int FoodPosition { get; private set; }

    public static SnakeController Instance { get; private set; }

    private Vector2Int _direction;
    private float _cameraHeight;
    private float _cameraWidth;
    
    private float _xBound;
    private float _yBound;
    
    private float _timer;
    private bool _isFoodSpawned;
    private Vector2Int _inputBuffer;
    private bool _isInputBuffered;
    private void Start()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (Camera.main != null)
        {
            var mainCamera = Camera.main;
            _cameraHeight = 2f * mainCamera.orthographicSize;
            _cameraWidth = _cameraHeight * mainCamera.aspect;

            var cameraPosition = mainCamera.transform.position;
            _xBound = cameraPosition.x + _cameraWidth / 2f;
            _yBound = cameraPosition.y + _cameraHeight / 2f;
        }
        else
        {
            Debug.LogError("Main camera not found");
        }

        CreateSnake();
        SnakeRenderer.Instance.Draw();
        
        _direction = Vector2Int.left;
        _inputBuffer = Vector2Int.left;
    }

    private void Update()
    {
        GetInput();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddComponent();
        }

        if (_timer > tickTime)
        {
            Tick();
            if (!IsInBounds() || IsColliding())
            {
                GameOver();
            }
            _timer = 0;
        }
        else
        {
            _timer += Time.deltaTime;
        }
    }

    private void GameOver()
    {
        DeleteSnake();
        CreateSnake();
        SpawnFood();
        _direction = Vector2Int.left;
    }

    private void Tick()
    {
        _direction = _inputBuffer;
        _isInputBuffered = false;
        Tail.UpdateComponent();
        Move(_direction);
        if(!_isFoodSpawned)
        {
            SpawnFood();
            _isFoodSpawned = true;
        }

        if (IsEating())
        {
            Eat();
        }
        SnakeRenderer.Instance.Draw();
    }

    private void CreateSnake()
    {
        Head = new SnakeComponent(Vector2Int.zero);
        Head.SetPrevComponent(new SnakeComponent(new Vector2Int(1,0)));
        Head.PrevComponent.SetPrevComponent(new SnakeComponent(new Vector2Int(2,0)));
        Tail = Head.PrevComponent.PrevComponent;
        //Head.PrevComponent.PrevComponent.SetPrevComponent(new SnakeComponent(new Vector2Int(3,0)));
        //Head.PrevComponent.PrevComponent.PrevComponent.SetPrevComponent(new SnakeComponent(new Vector2Int(4,0)));
    }

    private void DeleteSnake()
    {
        Head = null;
        Tail = null;
    }

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    private void GetInput()
    {
        if(_isInputBuffered)
            return;
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");

        if (x != 0 && -_direction.x != x )
        {
            _inputBuffer = new Vector2Int((int) x, 0);
            _isInputBuffered = true;
        }
        else if (y != 0 && -_direction.y != y)
        {
            _inputBuffer = new Vector2Int(0, (int) y);
            _isInputBuffered = true;
        }
    }

    private void AddComponent()
    {
        var newComponent = new SnakeComponent(Tail.Position);
        newComponent.SetNextComponent(Tail);
        Tail = newComponent;
    }

    private void Move(Vector2Int direction)
    {
        Head.Position += direction;
    }

    private bool IsInBounds()
    {
        if(Head.Position.x > _xBound || Head.Position.x < -_xBound)
            return false;
        return !(Head.Position.y > _yBound) && !(Head.Position.y < -_yBound);
    }
    
    private bool IsColliding()
    {
        var component = Head.PrevComponent;
        while (component != null)
        {
            if (Head.Position == component.Position)
            {
                return true;
            }
            component = component.PrevComponent;
        }

        return false;
    }

    private bool IsInSnake(Vector2Int position)
    {
        var component = Head;
        while (component != null)
        {
            if (position == component.Position)
            {
                return true;
            }
            component = component.PrevComponent;
        }

        return false;
    }
    
    private void SpawnFood()
    {
        var position = GetRandomPositionInBounds();
        while (IsInSnake(position))
        {
            position = GetRandomPositionInBounds();
        }
        FoodPosition = position;
    }

    private Vector2Int GetRandomPositionInBounds()
    {
        var xPosition = Random.Range(-_xBound + 1, _xBound - 1);
        var yPosition = Random.Range(-_yBound + 1, _yBound - 1);

        return new Vector2Int((int)xPosition, (int)yPosition);
    }
    
    private void Eat()
    {
        _isFoodSpawned = false;
        AddComponent();
    }
    
    private bool IsEating()
    {
        return Head.Position == FoodPosition;
    }
}
