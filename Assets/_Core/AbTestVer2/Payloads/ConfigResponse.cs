using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting;

namespace BasePuzzle.Core.AbTestVer2.Payloads
{
    [Serializable]
    public class ConfigResponse
    {
        public string runningAbTesting;

        public string[] abTestingFields;

        public Dictionary<string, bool> campaignMeta;

        public Dictionary<string, object> configs;

        public string[] AbTestingField => abTestingFields ??= Array.Empty<string>();
        public Dictionary<string, bool> CampaignMeta => campaignMeta ??= new Dictionary<string, bool>();
        public Dictionary<string, object> Configs => configs ??= new Dictionary<string, object>();

        [Preserve]
        public ConfigResponse()
        {
        }

        public Dictionary<string, object> TestingConfigs()
        {
            var result = new Dictionary<string, object>();

            foreach (var abTestingField in AbTestingField)
                result.Add(abTestingField, Configs.TryGetValue(abTestingField, out var val) ? val : null);
            return result;
        }

        public Dictionary<string, object> NonTestConfigs()
        {
            var result = new Dictionary<string, object>();

            foreach (var keyValuePair in Configs)
                if (!AbTestingField.Contains(keyValuePair.Key))
                    result.Add(keyValuePair.Key, keyValuePair.Value);
            return result;
        }
    }
}