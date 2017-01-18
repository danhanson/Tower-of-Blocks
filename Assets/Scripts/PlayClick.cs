
public class PlayClick : ExplosionClick {

    override public void OnClick()
    {
        base.OnClick();
        LevelManager.Manager.Continue(PlayerDataManager.Player);
    }
}
