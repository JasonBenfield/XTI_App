﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace XTI_App
{
    public interface IAppUser
    {
        EntityID ID { get; }
        AppUserName UserName();
        Task<IEnumerable<IAppUserRole>> RolesForApp(IApp app);
        Task<bool> IsModCategoryAdmin(IModifierCategory modCategory);
        Task<bool> HasModifier(ModifierKey modKey);
    }
}
