using System;

public class GameManagerEvents
{
    // The event called each time GameManager.Instance.ChangeState is called. Any script that cares about a change of state should listen to this event
    public static Action<GameState> StateChanged;
}
