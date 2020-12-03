using System;
using LifxCloud.NET;
using LifxCloud.NET.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OkToWake
{
    public class LIFXService
    {
        private LifxCloudClient _client;
        private string _apiKey;

        public LIFXService(string lifxApiKey)
        {
            _apiKey = lifxApiKey;
        }

        public async Task<List<Light>> GetAllLightsAsync()
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return new List<Light>();
            }

            _client = await LifxCloudClient.CreateAsync(_apiKey);
            return await _client.ListLights(Selector.All);
        }

        public async Task<List<Group>> GetAllGroupsAsync()
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return new List<Group>();
            }
            _client = await LifxCloudClient.CreateAsync(_apiKey);
            return await _client.ListGroups(Selector.All);
        }
        public async Task<ApiResponse> SetColor(string color, int brightness, Selector selector)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new NotImplementedException("Api Key Not Specified");
            }
            _client = await LifxCloudClient.CreateAsync(_apiKey);


            if (brightness == 0)
            {
                return await _client.SetState(selector, new LifxCloud.NET.Models.SetStateRequest
                {
                    Power = PowerState.Off,
                    Fast = true
                });
            }
            else
            {
                return await _client.SetState(selector, new LifxCloud.NET.Models.SetStateRequest
                {
                    Brightness = Convert.ToDouble(brightness) / 100,
                    Color = color,
                    Duration = 0,
                    Fast = true
                });
            }
        }
    }
}