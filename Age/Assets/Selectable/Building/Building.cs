using Age;
using System.Collections.Generic;
using UnityEngine;

public class Building : Selectable {
    public float maxBuildProgress;
    protected Queue<string> buildQueue;
    private float currentBuildProgress = 0.0f;
    private Vector3 spawnPoint;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}