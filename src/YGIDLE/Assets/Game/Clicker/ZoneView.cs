using UnityEngine;

namespace Game.Clicker
{
    public class ZoneView : MonoBehaviour
    {
        public ZoneType Type;
        public int Priority = 0;

        [Header("Shape")]
        public Vector2 Size = Vector2.one;

        public ClickZone ToZone()
        {
            var bounds = new Bounds(transform.position, Size);
            return new ClickZone(Type, bounds, Priority);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = GetColor();
            Gizmos.DrawWireCube(transform.position, Size);
        }

        private Color GetColor()
        {
            return Type switch
            {
                ZoneType.Idle => Color.green,
                ZoneType.UI => Color.blue,
                ZoneType.Crit => Color.red,
                _ => Color.white
            };
        }
#endif
    }
}