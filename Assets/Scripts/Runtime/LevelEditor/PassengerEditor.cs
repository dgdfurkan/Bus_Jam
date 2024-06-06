using Runtime.Enums;
using UnityEngine;

namespace Runtime.LevelEditor
{
    public class PassengerEditor : MonoBehaviour
    {
        public GameObject passengerPrefab;
        public ColorTypes colorType;

        public void PlacePassenger(Vector3 position)
        {
            GameObject passenger = Instantiate(passengerPrefab, position, Quaternion.identity);
        }
    }
}