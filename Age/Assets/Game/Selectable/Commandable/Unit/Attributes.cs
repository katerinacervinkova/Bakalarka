using UnityEngine.Networking;

public class Attributes : NetworkBehaviour {

    private Unit unit;

    [SyncVar(hook = "OnGatheringChange")]
    private float Gathering;
    [SyncVar(hook = "OnIntelligenceChange")]
    private float Intelligence;
    [SyncVar(hook = "OnAgilityChange")]
    private float Agility;
    [SyncVar(hook = "OnHealingChange")]
    private float Healing;
    [SyncVar(hook = "OnCraftingChange")]
    private float Crafting;
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
            case AttEnum.Agility:
                return Agility;
            case AttEnum.Healing:
                return Healing;
            case AttEnum.Crafting:
                return Crafting;
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
            case AttEnum.Agility:
                Agility = value;
                break;
            case AttEnum.Healing:
                Healing = value;
                break;
            case AttEnum.Crafting:
                Crafting = value;
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

    private void OnAgilityChange(float value)
    {
        Agility = value;
        OnChange();
    }
    private void OnHealingChange(float value)
    {
        Gathering = value;
        OnChange();
    }
    private void OnCraftingChange(float value)
    {
        Crafting = value;
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
        return $"Gathering: {Gathering}\n" +
            $"Intelligence: {Intelligence}\n" +
            $"Agility: {Agility}\n" +
            $"Healing: {Healing}\n" +
            $"Crafting: {Crafting}\n" +
            $"Accuracy: {Accuracy}";
    }
}
