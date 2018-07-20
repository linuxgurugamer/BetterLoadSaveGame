using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public virtual void Update()
        {
            Queue<Action> frameQueue;

            lock (_lock)
            {
                frameQueue = _actions;
                _actions = new Queue<Action>();
            }

            while (frameQueue.Count > 0)
            {
                try
                {
                    frameQueue.Dequeue()();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }
    }
}
