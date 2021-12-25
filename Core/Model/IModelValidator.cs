namespace Core.Model
{
    public interface IModelValidator<in T>
    {
        void Validate(T model);
    }
}