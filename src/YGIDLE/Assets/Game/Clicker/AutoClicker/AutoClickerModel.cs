namespace Game.Clicker.AutoClicker
{
    [System.Serializable]
    public class AutoClickerModel
    {
        public AutoClickerConfig Config;
        public int Count;

        public float TotalCPS => Config.ClicksPerSecond * Count;
    }
}
