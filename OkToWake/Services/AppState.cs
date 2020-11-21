using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OkToWake
{
    public class AppState
    {
        private void NotifyStateChanged() => OnChange?.Invoke();

        public event Action OnChange;

        public IEnumerable<object> LIFXLights { get; set; }

        public string LIFXLightId { get; set; }

        public string CustomColor { get; set; }



        public void SetCustomColor(string color)
        {
            CustomColor = color;
            NotifyStateChanged();
        }

        public void SetLIFXLights(IEnumerable<object> lights)
        {
            LIFXLights = lights;
            NotifyStateChanged();
        }

        public void SetLIFXLight(string lightId)
        {
            LIFXLightId = lightId;
            NotifyStateChanged();
        }
    }
}