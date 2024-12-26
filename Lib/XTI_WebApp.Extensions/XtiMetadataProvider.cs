using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace XTI_WebApp.Extensions;

public sealed class XtiMetadataProvider : IMetadataDetailsProvider, IDisplayMetadataProvider
{
    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        if(context.Key.MetadataKind == ModelMetadataKind.Property)
        {
            context.DisplayMetadata.ConvertEmptyStringToNull = false;
        }
    }
}
