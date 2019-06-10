public class HumanVisibilitySquares : VisibilitySquares {

	protected override void Update ()
    {
        base.Update();
        foreach (var square in squares.Values)
            square.UpdateVisibility();
    }
}
