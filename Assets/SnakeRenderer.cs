using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeRenderer : MonoBehaviour
{
    [SerializeField] private GameObject snakeComponentPrefab;
    [SerializeField] private GameObject foodPrefab;
    
    private SnakeController _snakeController;
    private List<GameObject> _snakeComponents = new List<GameObject>();
    
    private GameObject _food;

    public static SnakeRenderer Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        if(_food == null)
            _food = Instantiate(foodPrefab, Vector3.zero, Quaternion.identity);
        
    }

    public void Draw()
    {
        Clear();
        DrawFood();
        var component = SnakeController.Instance.Head;
        while (component != null)
        {
            var position = new Vector2(component.Position.x, component.Position.y);
            _snakeComponents.Add(Instantiate(snakeComponentPrefab, position, Quaternion.identity));
            component = component.PrevComponent;
        }
    }
    
    private void Clear()
    {
        foreach (var snakeComponent in  _snakeComponents)
        {
            Destroy(snakeComponent);
        }
        _snakeComponents.Clear();
    }

    private void DrawFood()
    {
        _food.transform.position = new Vector3(SnakeController.Instance.FoodPosition.x, SnakeController.Instance.FoodPosition.y);
    }
}
