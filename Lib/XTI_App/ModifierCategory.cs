using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MainDB.Entities;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class ModifierCategory : IModifierCategory
    {
        private readonly AppFactory factory;
        private readonly ModifierCategoryRecord record;

        internal ModifierCategory(AppFactory factory, ModifierCategoryRecord record)
        {
            this.factory = factory;
            this.record = record ?? new ModifierCategoryRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public ModifierCategoryName Name() => new ModifierCategoryName(record.Name);

        public Task<Modifier> TryAddDefaultModifier()
            => addOrUpdateModifier(() => ModifierKey.Default, "", "");

        public Task<Modifier> AddOrUpdateModifier(int targetID, string displayText)
            => AddOrUpdateModifier(targetID.ToString(), displayText);

        public Task<Modifier> AddOrUpdateModifier(string targetKey, string displayText)
            => addOrUpdateModifier(() => ModifierKey.Generate(), targetKey, displayText);

        private async Task<Modifier> addOrUpdateModifier(Func<ModifierKey> createKey, string targetKey, string displayText)
        {
            var modifier = await Modifier(targetKey);
            if (modifier.ID.IsValid() && modifier.TargetKey == targetKey)
            {
                await modifier.SetDisplayText(displayText);
            }
            else
            {
                modifier = await addModifier(createKey(), targetKey, displayText);
            }
            return modifier;
        }

        private Task<Modifier> addModifier(ModifierKey modKey, string targetKey, string displayText)
            => factory.Modifiers().Add(this, modKey, targetKey, displayText);

        public Task<Modifier> Modifier(ModifierKey modKey) => factory.Modifiers().Modifier(modKey);
        public Task<Modifier> Modifier(int targetID) => Modifier(targetID.ToString());
        public Task<Modifier> Modifier(string targetKey) => factory.Modifiers().Modifier(this, targetKey);

        public Task<IEnumerable<Modifier>> Modifiers() => factory.Modifiers().Modifiers(this);

        public Task<IEnumerable<ResourceGroup>> ResourceGroups() => factory.Groups().Groups(this);

        public ModifierCategoryModel ToModel() => new ModifierCategoryModel
        {
            ID = ID.Value,
            Name = Name().DisplayText
        };

        public override string ToString() => $"{nameof(ModifierCategory)} {ID.Value}";
    }
}
