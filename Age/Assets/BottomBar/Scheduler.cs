using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scheduler : MonoBehaviour {

    public float Speed;
    public Image image;
    public Action ActionToPerform { get; set; }
    public List<Scheduler> schedulers;

    public Animator animator;

	void Awake () {
        animator = GetComponent<Animator>();
        animator.SetFloat("duration", Speed);
    }
    public void Animate()
    {
        if (animator != null & animator.isActiveAndEnabled)
            animator.Play("Creating", 0);
    }
	public void PerformAction()
    {
        ActionToPerform();
        Destroy(gameObject);
    }
    public void OnDestroy()
    {
        schedulers.RemoveAt(0);
        schedulers.ForEach(scheduler =>
        {
            Vector3 position = scheduler.transform.position;
            position.x -= 50;
            scheduler.transform.position = position;
        });
        if (schedulers.Count > 0)
            schedulers[0].Animate();
    }
}
