public class TemporaryBuilding : Selectable
{
    static readonly int maxProgress = 100;
    public bool placed = false;
    public static readonly string progressEvent = "progressChanged";

    float progress = 0;

    private Job buildJob = null;

    public float Progress
    {
        get { return progress; }
        set { progress = value; EventManager.TriggerEvent(this, progressEvent); }
    }

    protected override void RemoveEvents()
    {
        EventManager.StopListening(this, progressEvent, DrawHealthBar);
        EventManager.StopListening(this, progressEvent, DrawSelectedObjectText);
        EventManager.StopListening(this, progressEvent, ControlProgress);

    }
    protected override void SetEvents()
    {
        EventManager.StartListening(this, progressEvent, DrawHealthBar);
        EventManager.StartListening(this, progressEvent, DrawSelectedObjectText);
        EventManager.StartListening(this, progressEvent, ControlProgress);

    }

    public void Build(Unit worker)
    {
        Progress += worker.Strength;
    }

    private void ControlProgress()
    {
        buildJob.Completed = Progress >= maxProgress;
        if (buildJob.Completed)
        {
            Building building = owner.factory.CreateMainBuilding(this);
            if (Selected)
            {
                // špatně!! musí se jinak řešit ten player
                EventManager.TriggerEvent(this, deselectOwnEvent);
                EventManager.TriggerEvent(building, selectOwnEvent);
                owner.SelectedObject = building;
            }
            Destroy(gameObject);
        }
    }

    public void PlaceBuilding()
    {
        placed = true;
    }

    protected override void DrawNameText()
    {
        nameText.text = "Temporary Building";
    }

    protected override void DrawSelectedObjectText()
    {
        if (Selected)
            selectedObjectText.text = string.Format("progress {0}/{1}", Progress, maxProgress);
    }

    protected override Job CreateOwnJob(Commandable worker)
    {
        if (buildJob == null)
            buildJob = new JobBuild(this);
        return buildJob;
    }

    protected override Job CreateEnemyJob(Commandable worker)
    {
        return new AttackJob(this);
    }
    public override void DrawHealthBar()
    {
        DrawProgressBar(Progress / maxProgress);
    }
}