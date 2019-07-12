using System;

public class Transaction
{
    public float Progress = 0;
    public readonly float MaxProgress = 10;
    private readonly float speed;
    private Building building;

    public LoadingPurchase purchase;

    public Transaction(Building building, LoadingPurchase purchase, float speed)
    {
        this.purchase = purchase;
        this.speed = speed;
        this.building = building;
    }

    /// <summary>
    /// Add corresponding amount of progress to this transaction.
    /// </summary>
    /// <param name="deltaTime">time elapsed since the last call</param>
    /// <returns>true if the loading has been completed</returns>
    public bool Load(float deltaTime)
    {
        Progress = Math.Min(Progress + speed * deltaTime, MaxProgress);
        if (Progress == MaxProgress)
        {
            purchase.InvokeAction(building);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the paymant back.
    /// </summary>
    public void Reset()
    {
        purchase.Reset(building);
    }
}

