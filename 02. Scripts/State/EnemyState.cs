using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


namespace EnemyStates
{
    public class IdleState : IState<EnemyController, EnemyState>
    {
        public void OnEnter(EnemyController owner)
        {
            owner.Agent.isStopped = true;
            owner.Agent.speed = 3;
        }

        public void OnUpdate(EnemyController owner)
        {
        }

        public void OnFixedUpdate(EnemyController owner)
        {
        }

        public void OnExit(EnemyController owner)
        {
        }

        public EnemyState? CheckTransition(EnemyController owner)
        {
            if (owner.IsPlayerInSight(out bool isPlayerLooking))
            {
                if (isPlayerLooking)
                {
                    owner.Agent.SetDestination(PlayerController.Instance.transform.position);
                    return EnemyState.Idle;
                }
                else
                {
                    return EnemyState.Chase;
                }
            }
            else
            {
                return EnemyState.Patrol;
            }

            return null;
        }
    }

    public class ChaseState : IState<EnemyController, EnemyState>
    {
        private readonly int isMove = Animator.StringToHash("IsMove");
        private bool isArrived = false;

        public void OnEnter(EnemyController owner)
        {
            owner.Agent.isStopped = false;
            owner.Agent.stoppingDistance = 2f;
            owner.Animator.SetBool(isMove, true);
            isArrived = false;
            owner.transform.LookAt(owner.PlayerTransform);
            owner.Agent.SetDestination(owner.PlayerTransform.position);
            owner.Agent.speed = 5;
        }

        public void OnUpdate(EnemyController owner)
        {
            if (owner.Agent.remainingDistance > owner.Agent.stoppingDistance)
            {
                isArrived = false;
            }
            else
            {
                GameManager.Instance.GameOver();
            }
        }

        public void OnFixedUpdate(EnemyController owner)
        {
        }

        public void OnExit(EnemyController owner)
        {
            owner.Agent.stoppingDistance = 0f;
            owner.Animator.SetBool(isMove, false);
        }

        public EnemyState? CheckTransition(EnemyController owner)
        {
            if (isArrived)
                return EnemyState.Idle;
            if (owner.IsPlayerInSight(out bool isPlayerLooking))
            {
                if (isPlayerLooking)
                {
                    owner.Agent.SetDestination(PlayerController.Instance.transform.position);
                    return EnemyState.Idle;
                }
            }
            else
            {
                return EnemyState.Patrol;
            }

            return null;
        }
    }

    public class PatrolState : IState<EnemyController, EnemyState>
    {
        private readonly int isMove = Animator.StringToHash("IsMove");

        private bool isSecondFloor = true;

        private int currentIndex = 0;
        private float waitTimer = 0f;
        private bool isWaiting = false;

        private Quaternion startRotation;
        private Quaternion targetRotation;
        private float rotationDuration = 1.5f;
        private float rotationTimer = 0f;

        public void OnEnter(EnemyController owner)
        {
            waitTimer = 0f;
            isWaiting = false;
            owner.Agent.speed = 3;
            owner.Animator.SetBool(isMove, true);
            SetNextPatrolPoint(owner);
        }

        public void OnUpdate(EnemyController owner)
        {
            if (owner.SecondFloor.Count == 0) return;

            if (isWaiting)
            {
                waitTimer += Time.deltaTime;

                rotationTimer += Time.deltaTime;
                float t = Mathf.Clamp01(rotationTimer / rotationDuration);
                owner.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                if (waitTimer >= 3f)
                {
                    waitTimer = 0f;
                    isWaiting = false;

                    currentIndex = (currentIndex + 1) % (isSecondFloor ? owner.SecondFloor.Count : owner.FirstFloor.Count);
                    //owner의 rotate를 랜덤하게 
                    SetNextPatrolPoint(owner);
                }
            }
            else if (!owner.Agent.pathPending && owner.Agent.remainingDistance < 0.2f)
            {
                owner.Agent.isStopped = true;
                isWaiting = true;

                float randomY = Random.Range(-180f, 180f);
                startRotation = owner.transform.rotation;
                targetRotation = Quaternion.Euler(0, randomY, 0);
                rotationTimer = 0f;
            }

            owner.Animator.SetBool(isMove, !isWaiting);
        }

        public void OnFixedUpdate(EnemyController owner)
        {
        }

        public void OnExit(EnemyController owner)
        {
            owner.Animator.SetBool(isMove, false);
            isWaiting = false;
            waitTimer = 0f;
        }

        public EnemyState? CheckTransition(EnemyController owner)
        {
            if (owner.IsPlayerInSight(out bool isPlayerLooking))
            {
                if (!NavMesh.SamplePosition(owner.PlayerTransform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                {
                    return EnemyState.Patrol;
                }

                if (isPlayerLooking)
                {
                    owner.Agent.SetDestination(PlayerController.Instance.transform.position);
                    return EnemyState.Idle;
                }
                else
                {
                    return EnemyState.Chase;
                }
            }

            return null;
        }

        private void SetNextPatrolPoint(EnemyController owner)
        {
            if (owner.SecondFloor.Count == 0)
                return;

            owner.Agent.isStopped = false;
            if (PlayerController.Instance.transform.position.y > 25f)
            {
                if (!isSecondFloor)
                {
                    isSecondFloor = true;
                    currentIndex = 0;
                }

                owner.Agent.SetDestination(owner.SecondFloor[currentIndex].position);
            }
            else if (PlayerController.Instance.transform.position.y < 25f)
            {
                if (isSecondFloor)
                {
                    isSecondFloor = false;
                    currentIndex = 0;
                }

                owner.Agent.SetDestination(owner.FirstFloor[currentIndex].position);
            }
        }
    }
}