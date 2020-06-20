using System;

namespace MailChemist.Core.Attributes
{
    /// <summary>
    /// Indicates that this class should be registered into global types in Fluid.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MailChemistModelAttribute : Attribute { }
}