using System;
using XTI_App.Abstractions;

namespace XTI_App.Fakes
{
    public sealed class FakeEntityID
    {
        private int currentID = new Random((int)DateTime.UtcNow.Ticks).Next();

        public EntityID Next()
        {
            currentID++;
            return new EntityID(currentID);
        }
    }
}
