using System;
using Newtonsoft.Json;

namespace Core.Model
{
    public sealed class ViModelRegion<T> : BaseViModelRegion
    {
        public IModelValidator<T> Validator;
        public T SafeModel;

        private T Model;
        private string _modelSnapshot;
        private bool _isInContinuousModify;

        public ViModelRegion()
        {
            TypeName = typeof(T).Name;

            Model = Activator.CreateInstance<T>();
            UpdateJsonAndSafeModel();
        }

        internal override void BeginContinuousModify()
        {
            _modelSnapshot = JsonConvert.SerializeObject(Model);
            _isInContinuousModify = true;
        }

        public void ContinuousModify(Action<T> modify)
        {
            ViAssert.True(_isInContinuousModify, "Use this method only inside ContinuouslyModifyRegions");
            try
            {
                InternalModifyAndValidate(modify);
                ModifyRegionResult = new ModifyRegionResult {IsSuccessModified = true};
            }
            catch (Exception e)
            {
                ModifyRegionResult = new ModifyRegionResult
                {
                    IsSuccessModified = false,
                    Error = e.ToString()
                };
            }
        }

        internal override void RevertFromSnapshot()
        {
            JsonConvert.PopulateObject(_modelSnapshot, Model);
            UpdateJsonAndSafeModel();
        }

        internal override void EndContinuousModify()
        {
            _isInContinuousModify = false;
        }

        public bool Modify(Action<T> modify)
        {
            ViAssert.True(!_isInContinuousModify, "You cannot use modify inside ContinuouslyModifyRegions");
            bool isSuccess;
            _modelSnapshot = JsonConvert.SerializeObject(Model);

            try
            {
                InternalModifyAndValidate(modify);
                isSuccess = true;
            }
            catch (Exception e)
            {
                ViLog.Error($"Exception while modifying {TypeName} transaction:\n{e}\nStarting rollback...");
                JsonConvert.PopulateObject(_modelSnapshot, Model);
                ViLog.Error($"Rollback {TypeName} completed successfully!");
                isSuccess = false;
            }

            return isSuccess;
        }

        private void InternalModifyAndValidate(Action<T> modify)
        {
            modify(Model);
            Validator?.Validate(Model);
            UpdateJsonAndSafeModel();
        }

        private void UpdateJsonAndSafeModel()
        {
            Json = JsonConvert.SerializeObject(Model);
            SafeModel = JsonConvert.DeserializeObject<T>(Json);
        }
    }
}