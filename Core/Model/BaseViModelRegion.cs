namespace Core.Model
{
    public abstract class BaseViModelRegion
    {
        public string Json { get; protected set; }

        internal string TypeName;
        internal ModifyRegionResult ModifyRegionResult;

        internal virtual void BeginContinuousModify() {}
        internal virtual void RevertFromSnapshot() {}
        internal virtual void EndContinuousModify() {}
    }
}