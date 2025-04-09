public interface IPlayerState
{
    void OnEnter(PlayerController playerController);
    void Update();
    void OnExit();
}
