using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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

        public void Init(ZoneType type, Bounds bounds)
        {
            transform.position = bounds.center;
            Size = bounds.size;
            Type = type;
        }
        
        public Vector2 RandomPoint(Vector3 padding)
        {
            var bounds = new Bounds(transform.position, Size);
            return new Vector2(
                Random.Range(bounds.min.x + padding.x/2, bounds.max.x - padding.x/2),
                Random.Range(bounds.min.y + padding.y/2, bounds.max.y - padding.y/2)
            );
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