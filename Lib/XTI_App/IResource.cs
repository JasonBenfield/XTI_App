namespace XTI_App
{
    public interface IResource
    {
        EntityID ID { get; }

        ResourceName Name();
    }
}