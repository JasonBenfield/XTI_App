﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace XTI_App.Abstractions
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

        public XtiPath(string appKey)
            : this(appKey, AppVersionKey.Current.DisplayText, "", "", ModifierKey.Default)
        {
        }

        public XtiPath(string appName, string version, string group, string action, ModifierKey modifier)
            : this
            (
                 new AppName(appName),
                 string.IsNullOrWhiteSpace(version) ? AppVersionKey.Current : AppVersionKey.Parse(version),
                 new ResourceGroupName(group),
                 new ResourceName(action),
                 modifier
            )
        {
        }

        public XtiPath(AppName appName, AppVersionKey version, ResourceGroupName group, ResourceName action, ModifierKey modifier)
        {
            if (string.IsNullOrWhiteSpace(appName.Value) && (!string.IsNullOrWhiteSpace(group.Value) || !string.IsNullOrWhiteSpace(action.Value))) { throw new ArgumentException($"{nameof(appName)} is required"); }
            if (string.IsNullOrWhiteSpace(group.Value) && !string.IsNullOrWhiteSpace(action.Value)) { throw new ArgumentException($"{nameof(group)} is required when there is an action"); }
            App = appName;
            Version = version;
            Group = group;
            Action = action;
            Modifier = modifier;
            value = $"/{App.Value}/{Version.Value}/{Group.Value}/{Action.Value}/{Modifier.Value}";
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

        public XtiPath WithNewGroup(XtiPath path)
        {
            var newPath = WithNewGroup(path.Group);
            if (!string.IsNullOrWhiteSpace(path.Action.Value))
            {
                newPath = newPath.WithAction(path.Action);
            }
            return newPath.WithModifier(path.Modifier);
        }

        public XtiPath WithNewGroup(string groupName)
            => WithNewGroup(new ResourceGroupName(groupName));

        public XtiPath WithNewGroup(ResourceGroupName groupName)
            => new XtiPath(App.DisplayText, Version.DisplayText, groupName.DisplayText, "", Modifier);

        public XtiPath WithGroup(string groupName) => WithGroup(new ResourceGroupName(groupName));

        public XtiPath WithGroup(ResourceGroupName groupName)
        {
            if (!string.IsNullOrWhiteSpace(Group)) { throw new ArgumentException("Cannot create group for a group"); }
            return new XtiPath(App.DisplayText, Version.DisplayText, groupName.DisplayText, "", Modifier);
        }

        public XtiPath WithAction(string actionName) => WithAction(new ResourceName(actionName));

        public XtiPath WithAction(ResourceName action)
        {
            if (!string.IsNullOrWhiteSpace(Action)) { throw new ArgumentException("Cannot create action for an action"); }
            return new XtiPath(App.DisplayText, Version.DisplayText, Group.DisplayText, action, Modifier);
        }

        public XtiPath WithModifier(ModifierKey modKey)
        {
            EnsureActionResource();
            return new XtiPath(App.DisplayText, Version.DisplayText, Group.DisplayText, Action.DisplayText, modKey);
        }

        public XtiPath WithVersion(AppVersionKey versionKey)
        {
            return new XtiPath(App.DisplayText, versionKey.DisplayText, Group.DisplayText, Action.DisplayText, Modifier);
        }

        public string Format()
        {
            var parts = new string[]
            {
                App.DisplayText, Version.DisplayText, Group.DisplayText, Action.DisplayText, Modifier.DisplayText
            }
            .TakeWhile(str => !string.IsNullOrWhiteSpace(str));
            var joined = string.Join("/", parts.Select(part => part.Replace(" ", "")));
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

        public bool Equals(string other) => Equals(Parse(other));

        public bool Equals(XtiPath other) => value == other?.value;

        public override int GetHashCode() => hashCode;

        public override string ToString()
        {
            var str = string.IsNullOrWhiteSpace(App.Value) ? "Empty" : Format();
            return $"{nameof(XtiPath)} {str}";
        }

    }
}
