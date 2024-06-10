using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.Controllers.Objects
{
    public class BusController : MonoBehaviour
    {
        #region Self Variables

        #region Public  Variables

        //

        #endregion

        #region Serialized Variables

        //

        #endregion

        #region Private Variables

        private ColorTypes _colorType;
        private BusArea _data;

        #endregion

        #endregion

        public void Initialize(BusArea data)
        {
            _data = data;
        }
    }
}