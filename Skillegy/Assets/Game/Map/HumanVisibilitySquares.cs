public class HumanVisibilitySquares : VisibilitySquares {

    private bool seeEverything = false;

    protected override void Start()
    {
        base.Start();
         SeeEverything();
    }

    /// <summary>
    /// If seeEverything is active, activates every square in each frame.
    /// Else set activated to true for all squares near the player's building, unit or temporary building and then update visibility.
    /// </summary>
	protected override void Update ()
    {
        if (seeEverything)
            foreach (var square in squares.Values)
            {
                square.activated = true;
                square.UpdateVisibility();
            }
        else
        {
            foreach (var square in squares.Values)
                if (square.ContainsFriend)
                    square.AdjoiningSquares.ForEach(s => s.activated = true);
            foreach (var square in squares.Values)
                square.UpdateVisibility();
        }
    }

    public void SeeEverything()
    {
        seeEverything = true;
    }
}
