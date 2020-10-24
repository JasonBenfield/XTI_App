﻿using System;

namespace XTI_App
{
    public sealed class AppKey : IEquatable<AppKey>
    {
        public static readonly AppKey Unknown = new AppKey(AppName.Unknown, AppType.Values.NotFound);

        public AppKey(string name, AppType appType)
            : this(new AppName(name), appType)
        {
        }

        public AppKey(AppName name, AppType appType)
        {
            Name = name;
            Type = appType;
            value = $"{Name.Value}/{Type.Value}";
        }

        private readonly string value;

        public AppName Name { get; }
        public AppType Type { get; }

        public bool Equals(AppKey other) => value == other?.value;
        public override bool Equals(object obj)
        {
            if (obj is AppKey appKey)
            {
                return Equals(appKey);
            }
            return base.Equals(obj);
        }
        public override int GetHashCode() => value.GetHashCode();
    }
}
