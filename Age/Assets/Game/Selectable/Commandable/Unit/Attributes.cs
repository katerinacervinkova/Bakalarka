using UnityEngine.Networking;

public class Attributes : NetworkBehaviour {

    private Unit unit;

    [SyncVar(hook = "OnGatheringChange")]
    private float Gathering;
    [SyncVar(hook = "OnIntelligenceChange")]
    private float Intelligence;
    [SyncVar(hook = "OnSwordsmanshipChange")]
    private float Swordsmanship;
    [SyncVar(hook = "OnHealingChange")]
    private float Healing;
    [SyncVar(hook = "OnBuildingChange")]
    private float Building;
    [SyncVar(hook = "OnAccuracyChange")]
    private float Accuracy;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public float Get(AttEnum attribute)
    {
        switch (attribute)
        {
            case AttEnum.Gathering:
                return Gathering;
            case AttEnum.Intelligence:
                return Intelligence;
            case AttEnum.Swordsmanship:
                return Swordsmanship;
            case AttEnum.Healing:
                return Healing;
            case AttEnum.Building:
                return Building;
            case AttEnum.Accuracy:
                return Accuracy;
            default:
                return 0;
        }
    }

    public void Set(AttEnum attribute, float value)
    {
        switch (attribute)
        {
            case AttEnum.Gathering:
                Gathering = value;
                break;
            case AttEnum.Intelligence:
                Intelligence = value;
                break;
            case AttEnum.Swordsmanship:
                Swordsmanship = value;
                break;
            case AttEnum.Healing:
                Healing = value;
                break;
            case AttEnum.Building:
                Building = value;
                break;
            case AttEnum.Accuracy:
                Accuracy = value;
                break;
        }
    }
    private void OnGatheringChange(float value)
    {
        Gathering = value;
        OnChange();
    }

    private void OnIntelligenceChange(float value)
    {
        Intelligence = value;
        OnChange();
    }

    private void OnSwordsmanshipChange(float value)
    {
        Swordsmanship = value;
        OnChange();
    }
    private void OnHealingChange(float value)
    {
        Gathering = value;
        OnChange();
    }
    private void OnBuildingChange(float value)
    {
        Building = value;
        OnChange();
    }
    private void OnAccuracyChange(float value)
    {
        Accuracy = value;
        OnChange();
    }

    private void OnChange()
    {
        if (PlayerState.Instance.SelectedObject == unit)
            UIManager.Instance.ShowObjectText(unit.Name, unit.GetObjectDescription());
    }

    public string GetDescription()
    {
        return $"Gathering: {(int)Gathering}\n" +
            $"Intelligence: {(int)Intelligence}\n" +
            $"Swordsmanship: {(int)Swordsmanship}\n" +
            $"Healing: {(int)Healing}\n" +
            $"Building: {(int)Building}\n" +
            $"Accuracy: {(int)Accuracy}";
    }
}
