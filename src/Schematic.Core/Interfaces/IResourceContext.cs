using System;
using System.Collections.Generic;

namespace Schematic.Core
{
    public interface IResourceContext<T> 
    {
        ResourceModel<T> OnNew(ResourceModel<T> model);

        ResourceModel<T> OnCreate(ResourceModel<T> model);

        ResourceModel<T> OnRead(ResourceModel<T> model);

        ResourceModel<T> OnUpdate(ResourceModel<T> model);
    }
}