public class HumanVisibilitySquares : VisibilitySquares {

    private bool seeEverything = false;

    protected override void Start()
    {
        base.Start();
        SeeEverything();
    }

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
