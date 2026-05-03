using System;
using System.Collections.Generic;

namespace AppInit.InitSteps
{
    public class InitStepOrder : Attribute
    {
        public readonly int GroupId;
        public readonly int Order;
        public readonly IReadOnlyList<Type> Dependencies;

        public InitStepOrder(int groupId, int order, params Type[] dependencies)
        {
            GroupId = groupId;
            Order = order;
            Dependencies = dependencies;
        }

        public static readonly InitStepOrder Default = new(0, 0);
    }
}