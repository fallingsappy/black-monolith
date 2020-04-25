using System.ComponentModel;

namespace DDrop.BE.Enums.Options
{
    public enum CalculationVariants
    {
        [Description("Внутренний")] CalculateWithCSharp,
        [Description("Внешний скрипт")] CalculateWithPython,
        [Description("Общий")] Common
    }
}