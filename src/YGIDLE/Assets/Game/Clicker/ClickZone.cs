using UnityEngine;

namespace Game.Clicker
{
    public enum ZoneType
    {
        Idle,
        UI,
        Crit
    }

    public class ClickZone
    {
        public ZoneType Type;
        public int Priority;

        private Bounds _bounds;

        public ClickZone(ZoneType type, Bounds bounds, int priority)
        {
            Type = type;
            _bounds = bounds;
            Priority = priority;
        }

        public void Init(ZoneType type, Bounds bounds, int priority)
        {
            Type = type;
            _bounds = bounds;
            Priority = priority;
        }

        public bool Contains(Vector2 pos)
        {
            return _bounds.Contains(pos);
        }
    }
}