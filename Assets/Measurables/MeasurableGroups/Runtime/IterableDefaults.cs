using System;

namespace OMG.Measurables.MeasurableGroups.Runtime
{
    public class IterableDefaults : Attribute
    {
        public readonly int[] DefaultValues;

        public IterableDefaults(params int[] defaultValues) {
            DefaultValues = defaultValues;
        }
    }
}