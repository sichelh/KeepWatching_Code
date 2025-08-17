using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class IdleState : IState<PlayerController, PlayerState>
    {
        public void OnEnter(PlayerController owner)
        {
        }

        public void OnUpdate(PlayerController owner)
        {
        }

        public void OnFixedUpdate(PlayerController owner)
        {
            owner.PlayerStat.Recover(StatType.CurrentStamina, 0.5f);
        }

        public void OnExit(PlayerController owner)
        {
        }

        public PlayerState? CheckTransition(PlayerController owner)
        {
            if (GameManager.Instance.InputHandler.MovementInput != Vector2.zero)
                return PlayerState.Move;

            return null;
        }
    }

    public class MoveState : IState<PlayerController, PlayerState>
    {
        private bool isSprinting;

        public void OnEnter(PlayerController owner)
        {
        }

        public void OnUpdate(PlayerController owner)
        {
            isSprinting = GameManager.Instance.InputHandler.IsSprintPressed && owner.PlayerStat.GetValue(StatType.CurrentStamina) > 0;
        }

        public void OnFixedUpdate(PlayerController owner)
        {
            if (isSprinting)
            {
                owner.PlayerStat.Consume(StatType.CurrentStamina, 1f);
            }

            owner.Movement(isSprinting);
        }

        public void OnExit(PlayerController owner)
        {
        }

        public PlayerState? CheckTransition(PlayerController owner)
        {
            if (GameManager.Instance.InputHandler.MovementInput == Vector2.zero)
                return PlayerState.Idle;
            return null;
        }
    }
}