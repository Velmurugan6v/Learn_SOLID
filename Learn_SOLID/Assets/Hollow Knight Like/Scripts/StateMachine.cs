namespace Hollow_Knight_Like.Scripts
{
    public class StateMachine
    {
        public EntityState CurrentState { get; private set; }

        public void InitializeState(EntityState initialState)
        {
            CurrentState = initialState;
            CurrentState.Enter();
        }

        public void ChangeState(EntityState newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}