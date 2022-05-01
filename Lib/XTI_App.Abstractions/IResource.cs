namespace XTI_App.Abstractions;

public interface IResource
{
    int ID { get; }

    ResourceName Name();
}