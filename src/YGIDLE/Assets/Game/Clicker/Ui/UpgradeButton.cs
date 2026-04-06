using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Clicker.Ui
{
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;

        public ISubject<int> Click;

        private int _id;

        public void Initialize()
        {
            Click = new Subject<int>();
            _button.OnClickAsObservable().Subscribe(_ => Click.OnNext(_id)).AddTo(this);
        }

        public void UpdateInfo(int id, string text)
        {
            _id = id;
            _text.text = text;
        }
    }
}