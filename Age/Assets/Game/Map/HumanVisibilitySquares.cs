public class HumanVisibilitySquares : VisibilitySquares {

    private bool seeEverything = false;

    protected override void Start()
    {
        base.Start();
        SeeEverything();
    }

	protected override void Update ()
    {
        if (!seeEverything)
        {
            base.Update();
            foreach (var square in squares.Values)
                square.UpdateVisibility();
        }
    }

    public void SeeEverything()
    {
        foreach (var square in squares.Values)
        {
            square.activated = true;
            square.UpdateVisibility();
        }
        seeEverything = true;

    }
}
