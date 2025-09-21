using System;
using System.Threading;
using BasePuzzle.Core.Scripts.Logs;
using BasePuzzle.Core.Scripts.Services.GameObjs;
using BasePuzzle.Core.Scripts.Utils.Entities;
using UnityEngine;

namespace BasePuzzle.Core.Scripts.Utils.FActions.Base
{
    using BasePuzzle.Core.Scripts.Logs;
    using BasePuzzle.Core.Scripts.Services.GameObjs;
    using BasePuzzle.Core.Scripts.Utils.Entities;

    public abstract class FAction : IFAction
    {
        public abstract Exception Exception { get;}
        public abstract bool Done { get; }
        
        public virtual void Schedule()
        {
            ActionQueue.Enqueue(this);
        }

        public virtual void Cancel()
        {
            ActionQueue.Remove(this);
        }

        public abstract void Invoke();
        
        public abstract bool CanInvoke();

        #region Scheduling

        private static readonly FQueue<FAction> ActionQueue = new FQueue<FAction>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitFAction()
        {
            FGameObj.OnUpdate += (a, b) =>
            {
                FAction action;
                if (ActionQueue.TryDequeue(out action))
                {
                    if (action.CanInvoke())
                        ThreadPool.QueueUserWorkItem(_ => { action.Invoke(); });
                    else
                        ActionQueue.Enqueue(action);
                }
            };
            CoreLogger.Instance.Info("FAction init complete");

        }

        #endregion
    }
}