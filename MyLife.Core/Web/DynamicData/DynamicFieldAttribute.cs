using System;

namespace MyLife.Web.DynamicData
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class DynamicFieldAttribute : Attribute
    {
        public string FriendlyName { get; set; }
    }
}