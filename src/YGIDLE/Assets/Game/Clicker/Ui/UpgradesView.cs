using System.Collections.Generic;
using Game.Clicker.AutoClicker;
using UniRx;
using UnityEngine;

namespace Game.Clicker.Ui
{
    public class UpgradesView : MonoBehaviour
    {
        [SerializeField] private UpgradeButton _prefab;
        [SerializeField] private RectTransform _parent;
        
        public ISubject<int> Click;
        
        public void Init(List<AutoClickerConfig> upgrades)
        {
            Click = new Subject<int>();
            
            

            for (int i = 0; i < upgrades.Count; i++)
            {
                var button = Instantiate(_prefab, _parent);
                button.Initialize();
                button.UpdateInfo(i, upgrades[i].Id);
                button.Click.Subscribe(id => Click.OnNext(id)).AddTo(this);
            }
        }
    }
}