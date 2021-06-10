using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;
        public void StartAction(IAction action)
        {
            if (currentAction == action) return;
            if (currentAction != null)
            {
                currentAction.Cancel();
            }
            currentAction = action;
        }
    }
}







//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace RPG.Core
//{
//    public class ActionScheduler : MonoBehaviour
//    {
//        MonoBehaviour currentAction;
//        public void StartAction(MonoBehaviour action)
//        {
//            if (currentAction == action) return;
//            if (currentAction != null)
//            {
//                print("Cancelling" + currentAction);
//            }
//            currentAction = action;
//        }
//        // перс побежал, сработал currentAction = action;
//        // затем второй заход, вызывается if (currentAction == action) return;
//        // поэтому мы не попадаем в if (currentAction != null)
//        // потом атакуем action != currentAction и уже теперь
//        // срабатывает if (currentAction != null)
//        // так мы узнаём когда закончилось последнее действие
//    }
//}
