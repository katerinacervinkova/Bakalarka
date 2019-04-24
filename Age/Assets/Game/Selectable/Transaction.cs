using System;

public class Transaction
{
    public float Progress = 0;
    public readonly float MaxProgress = 10;
    private readonly float Speed;

    public LoadingPurchase purchase;

    public Transaction(LoadingPurchase purchase, float Speed)
    {
        this.purchase = purchase;
        this.Speed = Speed;
    }

    public bool Load(float deltaTime)
    {
        Progress = Math.Min(Progress + Speed * deltaTime, MaxProgress);
        if (Progress == MaxProgress)
        {
            purchase.InvokeAction();
            return true;
        }
        return false;
    }

    public void Reset()
    {
        purchase.Reset();
    }
}

