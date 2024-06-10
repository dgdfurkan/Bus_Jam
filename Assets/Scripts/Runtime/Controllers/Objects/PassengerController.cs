using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.Controllers.Objects
{
    public class PassengerController : MonoBehaviour
    {
        #region Self Variables

        #region Public  Variables

        public ColorTypes colorType;
        private PassengerArea _data;
        public bool IsMoving { get; set; } = false;

        #endregion

        #region Serialized Variables

        //

        #endregion

        #region Private Variables

        //

        #endregion

        #endregion
        
        public void Initialize(PassengerArea data)
        {
            _data = data;
        }
    }
}