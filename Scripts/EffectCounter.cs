using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PickupsPlus
{
    public class EffectCounter : MonoBehaviour
    {
        public Image indicator;

        public void StartCountdown(float duration)
        {
            StartCoroutine(DoCountdown(duration));
        }

        private IEnumerator DoCountdown(float duration)
        {
            var startTime = Time.time;
            while (Time.time - startTime <= duration)
            {
                indicator.fillAmount = (Time.time - startTime) / duration;
                yield return null;
            }
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
