using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles color distribution between players.
/// </summary>
public class MenuPlayerList : MonoBehaviour {

    private Dictionary <Color, bool> ColorsAvailable = new Dictionary<Color, bool>();
    private Color[] Colors = new Color[] { Color.red, Color.yellow, Color.green, Color.blue, Color.cyan, Color.magenta, Color.black};

    /// <summary>
    /// Gets some available color for new player.
    /// </summary>
    /// <param name="index">color index used for getting the next color</param>
    /// <returns>color for the player to use</returns>
    public Color GetColor(out int index)
    {
        index = 0;
        for (index = 0; index < Colors.Length; index++)
        {
            if (!ColorsAvailable.ContainsKey(Colors[index]) || ColorsAvailable[Colors[index]])
            {
                ColorsAvailable[Colors[index]] = false;
                return Colors[index];
            }
        }
        return Color.black;
    }
	
    /// <summary>
    /// Gets next available color for player.
    /// </summary>
    /// <param name="index">color index</param>
    /// <returns>color for the player to use</returns>
    public Color GetNextColor(ref int index)
    {
        ColorsAvailable[Colors[index]] = true;
        int startIndex = index;
        for (index = (index + 1) % Colors.Length; index != startIndex; index = (index + 1) % Colors.Length)
        {
            if (!ColorsAvailable.ContainsKey(Colors[index]) || ColorsAvailable[Colors[index]])
            {
                ColorsAvailable[Colors[index]] = false;
                return Colors[index];
            }
        }
        ColorsAvailable[Colors[index]] = false;
        return Colors[index];
    }

    /// <summary>
    /// Sets the color available again.
    /// </summary>
    /// <param name="color">color to be made available</param>
    public void RemoveColor(Color color)
    {
        ColorsAvailable[color] = true;
    }
}
