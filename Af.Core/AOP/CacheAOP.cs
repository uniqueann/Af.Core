using Af.Core.Common.Helper;
using Castle.DynamicProxy;

namespace Af.Core.AOP
{
    public class CacheAOP : AOPbase
    {
        private readonly ICaching _cache;

        public CacheAOP(ICaching cache)
        {
            _cache = cache;
        }

        public override void Intercept(IInvocation invocation)
        {
            var cacheKey = CustomCacheKey(invocation);
            var cacheValue = _cache.Get(cacheKey);
            if (cacheValue != null)
            {
                invocation.ReturnValue = cacheValue;
                return;
            }
            invocation.Proceed();
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                _cache.Set(cacheKey, invocation.ReturnValue);
            }
        }
    }
}
