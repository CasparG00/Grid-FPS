using System.Collections.Generic;

public static class EventSystem
{
    private static readonly Dictionary<EventTypes, System.Action> Listeners = new();

    public static void AddListener(EventTypes eventType, System.Action listener)
    {
        if (!Listeners.ContainsKey(eventType))
        {
            Listeners.Add(eventType, null);
        }

        Listeners[eventType] += listener;
    }
    
    public static void RemoveListener(EventTypes eventType, System.Action listener)
    {
        if (Listeners.ContainsKey(eventType))
        {
            Listeners[eventType] -= listener;
        }
    }
    
    public static void Invoke(EventTypes eventType)
    {
        if (Listeners.ContainsKey(eventType))
        {
            Listeners[eventType]?.Invoke();
        }
    }
}

public enum EventTypes
{
    endTurn
}