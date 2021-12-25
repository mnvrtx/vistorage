using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Core.Model
{
    public abstract class BaseViModel<T> where T : BaseViModel<T>
    {
        private readonly List<BaseViModelRegion> _regions = new();
        private readonly List<FieldInfo> _fieldInfos = new();

        private readonly string _typeName;
        protected BaseViModel()
        {
            _typeName = GetType().Name;

            var allFields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var fieldInfo in allFields)
            {
                _regions.Add((BaseViModelRegion)fieldInfo.GetValue(this));
                _fieldInfos.Add(fieldInfo);
            }
        }

        public void ContinuouslyModifyRegions(Action action)
        {
            foreach (var region in _regions)
                region.BeginContinuousModify();

            action();

            var haveErrorsWhileModifying = false;
            foreach (var region in _regions)
            {
                ref var modifyResult = ref region.ModifyRegionResult;
                if (modifyResult.IsSuccessModified)
                    continue;

                haveErrorsWhileModifying = true;
                ViLog.Error($"Exception while modifying {region.TypeName} transaction:\n{modifyResult.Error}\nStarting rollback all regions...");
                break;
            }

            if (haveErrorsWhileModifying)
            {
                foreach (var region in _regions)
                    region.RevertFromSnapshot();
                ViLog.Error($"Rollback {_regions.Count} regions in {_typeName} completed successfully!");
            }
            
            foreach (var region in _regions)
                region.EndContinuousModify();
        }

        public string GenerateJson(bool pretty = false)
        {
            var jsonModel = new JObject();
            for (int i = 0; i < _fieldInfos.Count; i++)
            {
                var fieldInfo = _fieldInfos[i];
                var region = _regions[i];
                
                jsonModel.Add(fieldInfo.Name, new JRaw(region.Json));
            }

            var result = jsonModel.ToString(Formatting.None);

            if (pretty)
                result = StaticHelpers.FormatJson(result);
            
            return result;
        }
    }
}