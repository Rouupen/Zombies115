using UnityEngine;

namespace CharacterStates
{
    public enum CharacterStates
    {
        Idle,
        Running
    }


    public class StateIdle : State
    {
        public override void OnEnter()
        {
            Debug.Log("OnEnter");
        }

        public override void OnUpdate()
        {
            Debug.Log("OnUpdate");
        }

        public override void OnExit()
        {
            Debug.Log("OnExit");
        }
    }

    public class StateRunning : State
    {
        public override void OnEnter()
        {
            Debug.Log("OnEnter");
        }

        public override void OnUpdate()
        {
            Debug.Log("OnUpdate");
        }

        public override void OnExit()
        {
            Debug.Log("OnExit");
        }
    }
}
