using System;

namespace MyFitnessPal.Data.Utility
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NoAnalyticsAttribute : Attribute
    {
    }
}