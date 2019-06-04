using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayerList : MonoBehaviour {

    private Dictionary <Color, bool> ColorsAvailable = new Dictionary<Color, bool>();
    private Color[] Colors = new Color[] { Color.red, Color.yellow, Color.green, Color.blue, Color.cyan, Color.magenta, Color.black};

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

    public void RemoveColor(Color color)
    {
        ColorsAvailable[color] = true;
    }
}
