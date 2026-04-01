using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Clicker
{
    public class ZoneManager
    {
        private List<ClickZone> _zones = new List<ClickZone>();

        public void InitializeFromScene()
        {
            var views = Object.FindObjectsByType<ZoneView>(FindObjectsSortMode.None);

            _zones = views
                .Select(v => v.ToZone())
                .ToList();
        }

        public void AddZone(ClickZone zone)
        {
            _zones.Add(zone);
        }

        public ClickZone GetZone(Vector2 pos)
        {
            ClickZone best = null;
            int bestPriority = int.MinValue;

            foreach (var zone in _zones)
            {
                if (!zone.Contains(pos)) continue;

                if (zone.Priority > bestPriority)
                {
                    bestPriority = zone.Priority;
                    best = zone;
                }
            }

            return best;
        }
    }
}