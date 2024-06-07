using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Runtime.Extentions
{
    public static class Extention
    {
        public static void CreateAndFadeOut(this TextMeshProUGUI original, Transform parent, string message, float duration)
        {
            var text = Object.Instantiate(original, parent);
            text.transform.position = new Vector3(original.transform.position.x, original.transform.position.y, original.transform.position.z - 1);
            text.enabled = true;
            text.text = message;
            text.DOFade(0, duration).OnComplete(() => Object.Destroy(text.gameObject));
        }
    }
}

