using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target = null;

        // когда в игре будет много всего то может возникнуть глюк когда камера пытается двигаться раньше(в некоторых фреймах) чем движется игрок / фрейм  
        // движения камеры срабатывает раньше чем фрейм анимации движения. Игрока будет как будто дёргать туда - сюда.
        // нужен будет LateUpdate, а не Update
        void Update()
        {
            transform.position = target.position;
        }
    }

    // позиция FollowCameraEmpty_GameObject'a(коробки) = позиции игрока, но внутри коробки стоит камера которая отдалена от нулевой позиции коробки/игрока 
}
