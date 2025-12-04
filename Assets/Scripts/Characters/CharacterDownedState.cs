using UnityEngine;

public class CharacterDownedState : CharacterBaseState
{
    public override void Start(CharacterStateManager manager)
    {
        manager.CharacterData.CharacterAnimator.SetTrigger("Downed");
        manager.CharacterData.AnimatorState = "Downed";

        // stop movement
        manager.RemainingDestinationWaypoints.Clear();
    }

    public override void Update(CharacterStateManager manager)
    {
        // Do nothing
    }
}
