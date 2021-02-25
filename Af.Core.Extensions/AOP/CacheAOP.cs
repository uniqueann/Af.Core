using Af.Core.Common;
using Af.Core.Common.Helper;
using Castle.DynamicProxy;
using System.Linq;

namespace Af.Extensions.AOP
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
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            // 当前方法特性
            var cachingAttribute = method.GetCustomAttributes(true).FirstOrDefault(a=>a.GetType()==typeof(CachingAttribute)) as CachingAttribute;
            // 指定特性的才可以缓存
            if (cachingAttribute != null)
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
            else
            {
                invocation.Proceed();
            }           
        }
    }
}
