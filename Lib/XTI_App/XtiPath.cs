using System;
using System.Collections.Generic;
using System.Linq;

namespace XTI_App
{
    public sealed class XtiPath : IEquatable<XtiPath>, IEquatable<string>
    {
        public static XtiPath Parse(string str)
        {
            var parts = (str ?? "").Split('/', StringSplitOptions.RemoveEmptyEntries);
            var names = new List<string>(parts.Concat(Enumerable.Repeat("", 5 - parts.Length)));
            return new XtiPath
            (
                names[0], names[1], names[2], names[3], ModifierKey.FromValue(names[4])
            );
        }

        public XtiPath(string appKey, string version)
            : this(appKey, version, "", "")
        {
        }

        public XtiPath(string appKey, string version, string group)
            : this(appKey, version, group, "")
        {
        }

        public XtiPath(string appKey, string version, string group, string action)
            : this(appKey, version, group, action, ModifierKey.Default)
        {
        }

        public XtiPath(string appKey, string version, string group, string action, ModifierKey modifier)
        {
            if (string.IsNullOrWhiteSpace(appKey) && (!string.IsNullOrWhiteSpace(group) || !string.IsNullOrWhiteSpace(action))) { throw new ArgumentException($"{nameof(appKey)} is required"); }
            if (string.IsNullOrWhiteSpace(group) && !string.IsNullOrWhiteSpace(action)) { throw new ArgumentException($"{nameof(group)} is required when there is an action"); }
            App = new AppName(appKey);
            Version = string.IsNullOrWhiteSpace(version) ? AppVersionKey.Current : AppVersionKey.Parse(version);
            Group = new ResourceGroupName(group);
            Action = new ResourceName(action);
            Modifier = modifier;
            value = $"{App.Value}/{Version.Value}/{Group.Value}/{Action.Value}/{Modifier.Value}";
            hashCode = value.GetHashCode();
        }

        private readonly string value;
        private readonly int hashCode;

        public AppName App { get; }
        public AppVersionKey Version { get; }
        public ResourceGroupName Group { get; }
        public ResourceName Action { get; }
        public ModifierKey Modifier { get; }

        public bool IsCurrentVersion() => AppVersionKey.Current.Equals(Version);

        public void EnsureAppResource()
        {
            if (!string.IsNullOrWhiteSpace(Group.Value))
            {
                throw new ArgumentException($"{Format()} is not the name of an app");
            }
        }

        public void EnsureGroupResource()
        {
            if (string.IsNullOrWhiteSpace(Group.Value) || !string.IsNullOrWhiteSpace(Action.Value))
            {
                throw new ArgumentException($"{Format()} is not the name of a group");
            }
        }

        public void EnsureActionResource()
        {
            if (string.IsNullOrWhiteSpace(Action.Value))
            {
                throw new ArgumentException($"{Format()} is not the name of an action");
            }
        }

        public XtiPath WithNewGroup(string groupName)
            => new XtiPath(App.DisplayText, Version.DisplayText, groupName, "", Modifier);

        public XtiPath WithGroup(string groupName)
        {
            if (!string.IsNullOrWhiteSpace(Group)) { throw new ArgumentException("Cannot create group for a group"); }
            return new XtiPath(App.DisplayText, Version.DisplayText, groupName, "", Modifier);
        }

        public XtiPath WithAction(string actionName)
        {
            if (!string.IsNullOrWhiteSpace(Action)) { throw new ArgumentException("Cannot create action for an action"); }
            return new XtiPath(App.DisplayText, Version.DisplayText, Group.DisplayText, actionName, Modifier);
        }

        public XtiPath WithModifier(ModifierKey modKey)
        {
            EnsureActionResource();
            return new XtiPath(App.DisplayText, Version.DisplayText, Group.DisplayText, Action.DisplayText, modKey);
        }

        public string Format()
        {
            var parts = new string[]
            {
                App.DisplayText, Version.DisplayText, Group.DisplayText, Action.DisplayText, Modifier.DisplayText
            }
            .TakeWhile(str => !string.IsNullOrWhiteSpace(str));
            var joined = string.Join("/", parts);
            return $"/{joined}";
        }

        public string Value() => Format().ToLower();

        public override bool Equals(object obj)
        {
            if (obj is string str)
            {
                return Equals(str);
            }
            return Equals(obj as XtiPath);
        }

        public bool Equals(XtiPath other) => value == other?.value;

        public bool Equals(string other) => value == other;

        public override int GetHashCode() => hashCode;

        public override string ToString()
        {
            var str = string.IsNullOrWhiteSpace(App.Value) ? "Empty" : Format();
            return $"{nameof(XtiPath)} {str}";
        }

    }
}
