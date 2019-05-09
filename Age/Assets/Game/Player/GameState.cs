using Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GameState : NetworkBehaviour {

    private static GameState instance;
    public static GameState Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameState>();
            return instance;
        }
    }

    public List<Unit> Units { get; private set; }
    public List<Building> Buildings { get; private set; }
    public List<Resource> Resources { get; private set; }
    public List<TemporaryBuilding> TemporaryBuildings { get; private set; }

    public override void OnStartClient()
    {
        Units = new List<Unit>();
        Buildings = new List<Building>();
        Resources = new List<Resource>();
        TemporaryBuildings = new List<TemporaryBuilding>();
        foreach (Resource resource in FindObjectsOfType<Resource>())
            Resources.Add(resource);
    }

    public void UpdateGraph(Bounds bounds)
    {
        var guo = new GraphUpdateObject(bounds)
        {
            modifyWalkability = true,
            updatePhysics = true
        };
        AstarPath.active?.UpdateGraphs(guo);
    }

    public T GetNearestResource<T>(T resource, Vector3 position, int maxDistance) where T : Resource
    {
        return (T)Resources.Where(r => r is T && r != resource && Vector3.Distance(position, r.transform.position) < maxDistance).
            OrderBy(r => Vector3.Distance(position, r.transform.position)).FirstOrDefault();
    }

    public Selectable GetNearestTarget(Vector3 position, int maxDistance)
    {
        return ((IEnumerable<Selectable>)Units).Concat(Buildings).Where(s => s != null && !s.hasAuthority && Vector3.Distance(position, s.transform.position) < maxDistance).
            OrderBy(s => Vector3.Distance(position, s.transform.position)).FirstOrDefault();
    }
     
    [ClientRpc]
    public void RpcPlaceBuilding(Vector3 position, NetworkInstanceId tempBuildingId)
    {
        GameObject temporaryBuilding = ClientScene.objects[tempBuildingId].gameObject;
        temporaryBuilding.transform.position = position;
        temporaryBuilding.SetActive(true);
        Collider collider = temporaryBuilding.GetComponent<Collider>();
        collider.enabled = true;
        var guo = new GraphUpdateObject(collider.bounds)
        {
            modifyWalkability = true,
            updatePhysics = true
        };
        AstarPath.active.UpdateGraphs(guo);
    }

    [ClientRpc]
    public void RpcEnterBuilding(NetworkInstanceId unitId)
    {
        ClientScene.objects[unitId].gameObject.SetActive(false);
    }

    [ClientRpc]
    public void RpcExitBuildingDestination(NetworkInstanceId unitId, Vector3 position, Vector3 destination)
    {
        Unit unit = ClientScene.objects[unitId].GetComponent<Unit>();
        unit.transform.position = position;
        if (unit.hasAuthority)
            unit.SetJob(new JobGo(destination));
        unit.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void RpcExitBuilding(NetworkInstanceId unitId, Vector3 position)
    {
        Unit unit = ClientScene.objects[unitId].GetComponent<Unit>();
        unit.transform.position = position;
        unit.ResetJob();
        unit.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void RpcCreateBuilding(NetworkInstanceId tempBuildingID, BuildingEnum buildingType)
    {
        var tempBuilding = ClientScene.objects[tempBuildingID].GetComponent<TemporaryBuilding>();
        Building building;
        switch (buildingType)
        {
            case BuildingEnum.MainBuilding:
                building = tempBuilding.gameObject.AddComponent<MainBuilding>() as MainBuilding;
                break;
            case BuildingEnum.Library:
                building = tempBuilding.gameObject.AddComponent<Library>() as Library;
                break;
            case BuildingEnum.Barracks:
                building = tempBuilding.gameObject.AddComponent<Barracks>() as Barracks;
                break;
            case BuildingEnum.Infirmary:
                building = tempBuilding.gameObject.AddComponent<Infirmary>() as Infirmary;
                break;
            case BuildingEnum.House:
                building = tempBuilding.gameObject.AddComponent<House>() as House;
                break;
            default:
                building = null;
                break;
        }
        building.healthBarOffset = tempBuilding.healthBarOffset;
        building.owner = tempBuilding.owner;
        building.Init();
        if (PlayerState.Instance.SelectedObject == tempBuilding)
            PlayerState.Instance.Select(building);
        Destroy(tempBuilding);
    }
}
