using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeComponent
{
    public SnakeComponent PrevComponent { get; private set; }
    public SnakeComponent NextComponent { get; private set; }
    public Vector2Int Position { get; set; }

    public SnakeComponent(Vector2Int position)
    {
        Position = position;
    }

    public void UpdateComponent()
    {
        if (NextComponent == null) return;
        
        Position = NextComponent.Position;
        NextComponent.UpdateComponent();
    }
    
    public void SetPrevComponent(SnakeComponent prevComponent)
    {
        PrevComponent = prevComponent;
        prevComponent.NextComponent = this;
    }
    
    public void SetNextComponent(SnakeComponent nextComponent)
    {
        NextComponent = nextComponent;
        nextComponent.PrevComponent = this;
    }
}
