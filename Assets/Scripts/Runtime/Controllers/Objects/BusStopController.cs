using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.Controllers.Objects
{
    public class BusStopController : MonoBehaviour
    {
        public PassengerArea Data { get; private set; }
        public bool isOccupied;
        public PassengerController PassengerController { get; private set; }

        public void AssignPassenger(PassengerController passenger)
        {
            if(isOccupied) return;
            Data = passenger.Data;
            PassengerController = passenger;
            isOccupied = true;
        }
        
        public void FirePassenger()
        {
            Data = new PassengerArea{colorType = ColorTypes.None};
            isOccupied = false;
        }
    }
}