using System.Runtime.Remoting.Messaging;
using ServiceStack.Configuration;

namespace Autofac.Extras.ServiceStack
{
    public class AutofacIocAdapter : IContainerAdapter
    {
        public IContainer Container { get; }

        public AutofacIocAdapter(IContainer container) => Container = container;

        public T Resolve<T>() => GetCurrentContext().Resolve<T>();

        public T TryResolve<T>()
        {
            var context = GetCurrentContext();

            T result;
            return context.TryResolve(out result) ? result : default(T);
        }

        private ILifetimeScope GetCurrentContext() => CallContext.LogicalGetData(Consts.AutofacScopeLogicalContextKey) as ILifetimeScope ?? Container;
    }
}