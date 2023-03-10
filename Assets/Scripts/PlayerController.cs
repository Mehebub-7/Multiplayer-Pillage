using System;

public class PlayerController
{
    public Player model { get; private set; }
    public PlayerView view { get; private set; }
    public PlayerController(Player model, PlayerView view)
    {
        this.model = model;
        this.view = view;
        this.model.OnPositionChanged += OnPositionChanged;
    }
    private void OnPositionChanged(Position position)
    {
        // Sync
        Position pos = this.model.position;
        // Unity call required here! (we lost portability)
        this.view.SetPosition(new UnityEngine.Vector3(pos.x, pos.y, pos.z));
    }
    // Calling this will fire the OnPositionChanged event
    private void SetPosition(Position position)
    {
        this.model.position = position;
    }
}
