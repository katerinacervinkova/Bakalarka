using UnityEngine;

/// <summary>
/// Class used for waiting until the player is ready to initialize
/// </summary>
public class FirstUnitCreator : MonoBehaviour {

    public Player player;
	
    /// <summary>
    /// Waits until the player initializes and then destroys itself.
    /// </summary>
	void Update ()
    {
        if (!player.hasAuthority || player.Init())
        {
            if (player.IsHuman)
                Destroy(GameObject.Find("Loading Screen Canvas"));
            Destroy(this);
        }
	}
}
