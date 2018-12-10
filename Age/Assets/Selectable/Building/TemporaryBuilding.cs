using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class TemporaryBuilding : Selectable
{
    float progress = 0;
    readonly int maxProgress = 100;
    public bool placed = false;

    protected void OnGUI()
    {
        if (placed)
            DrawProgressBar(progress / maxProgress);
    }
    public void Build(JobBuild job)
    {
        progress += job.worker.Strength;
        if (progress >= maxProgress)
        { 
            Building building = owner.factory.CreateMainBuilding(this);
            if (selected)
            {
                // špatně!! musí se jinak řešit ten player
                SetSelection(false, owner);
                building.SetSelection(true, owner);
                owner.SelectedObject = building;
            }
            job.Completed = true;
            Destroy(gameObject);
        }
        if (selected)
            DrawSelectedObjectText();
    }

    public void PlaceBuilding()
    {
        placed = true;
    }

    public override void DrawBottomBar()
    {
        nameText.text = "Building";
        DrawSelectedObjectText();
    }

    private void DrawSelectedObjectText()
    {
        selectedObjectText.text = string.Format("progress {0}/{1}", progress, maxProgress);

    }

    protected override Job CreateOwnJob(Commandable worker)
    {
        return new JobBuild(worker as Unit, this);
    }

    protected override Job CreateEnemyJob(Commandable worker)
    {
        return new AttackJob(worker, this);
    }
}