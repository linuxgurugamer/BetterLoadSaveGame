using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace BetterLoadSaveGame
{
    class ActionGenerator
    {
        private object _lock = new object();
        private Queue<Action> _actions = new Queue<Action>();
        protected void EnqueueAction(Action action)
        {
            lock (_lock)
            {
                _actions.Enqueue(action);
            }
        }

        public virtual void LateUpdate()
        {
            Action action = null;
            lock (_lock)
            {
                if (_actions.Count > 0)
                {
                    action = _actions.Dequeue();
                }
            }
            if (action != null)
            {
                action();
            }
        }
    }
}
