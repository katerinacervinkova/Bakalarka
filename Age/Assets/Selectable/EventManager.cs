using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    private Dictionary<Selectable, Dictionary<string, UnityEvent>> eventDictionary;
    private static EventManager eventManager;

    private static EventManager Instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                eventManager.Init();
            }
            return eventManager;
        }
    }

    private void Init()
    {
        eventDictionary = new Dictionary<Selectable, Dictionary<string, UnityEvent>>();
    }

    public static void StartListening(Selectable selectable, string eventName, UnityAction listener)
    {
        Dictionary<string, UnityEvent> dictionary = null;
        UnityEvent unityEvent = null;
        if (!Instance.eventDictionary.TryGetValue(selectable, out dictionary))
        {
            dictionary = new Dictionary<string, UnityEvent>();
            Instance.eventDictionary.Add(selectable, dictionary);
        }
        if (!dictionary.TryGetValue(eventName, out unityEvent))
        {
            unityEvent = new UnityEvent();
            dictionary.Add(eventName, unityEvent);
        }
        unityEvent.AddListener(listener);
    }

    public static void StopListening(Selectable selectable, string eventName, UnityAction listener)
    {
        if (eventManager == null) return;
        Dictionary<string, UnityEvent> dictionary = null;
        UnityEvent unityEvent = null;
        if (Instance.eventDictionary.TryGetValue(selectable, out dictionary) && dictionary.TryGetValue(eventName, out unityEvent))
            unityEvent.RemoveListener(listener);
    }

    public static void StopAllListening(Selectable selectable, string eventName)
    {
        if (eventManager == null) return;
        Dictionary<string, UnityEvent> dictionary = null;
        UnityEvent unityEvent = null;
        if (Instance.eventDictionary.TryGetValue(selectable, out dictionary) && dictionary.TryGetValue(eventName, out unityEvent))
            unityEvent.RemoveAllListeners();
    }

    public static void TriggerEvent(Selectable selectable, string eventName)
    {
        if (eventManager == null) return;
        Dictionary<string, UnityEvent> dictionary = null;
        UnityEvent unityEvent = null;
        if (Instance.eventDictionary.TryGetValue(selectable, out dictionary) && dictionary.TryGetValue(eventName, out unityEvent))
            unityEvent.Invoke();
    }
}

