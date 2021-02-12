namespace XTI_App.Abstractions
{
    public interface IResource
    {
        EntityID ID { get; }

        ResourceName Name();
    }
}