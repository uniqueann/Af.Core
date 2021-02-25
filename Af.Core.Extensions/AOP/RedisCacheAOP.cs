using Af.Core.Common;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Af.Extensions.AOP
{
    public class RedisCacheAOP : AOPbase
    {
        private readonly IRedisBasketRepository _cache;

        public RedisCacheAOP(IRedisBasketRepository cache)
        {
            _cache = cache;
        }

        public override void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            if (method.ReturnType == typeof(void) || method.ReturnType==typeof(Task))
            {
                invocation.Proceed();
                return;
            }
            var cachingAttribute = method.GetCustomAttributes(true).FirstOrDefault(a=>a.GetType()==typeof(CachingAttribute)) as CachingAttribute;
            if (cachingAttribute!= null)
            {
                var cacheKey = CustomCacheKey(invocation);
                var cacheValue = _cache.GetValue(cacheKey).Result;
                if (cacheValue!=null)
                {
                    //将当前获取到的缓存值，赋值给当前执行方法
                    Type returnType;
                    if (typeof(Task).IsAssignableFrom(method.ReturnType))
                    {
                        returnType = method.ReturnType.GenericTypeArguments.FirstOrDefault();
                    }
                    else
                    {
                        returnType = method.ReturnType;
                    }

                    dynamic _result = JsonConvert.DeserializeObject(cacheValue,returnType);
                    invocation.ReturnValue = (typeof(Task).IsAssignableFrom(method.ReturnType)) ? Task.FromResult(_result) : _result;
                    return;
                }
                invocation.Proceed();
                // 存入缓存
                if (!string.IsNullOrWhiteSpace(cacheValue))
                {
                    object response;
                    var type = invocation.Method.ReturnType;
                    if (typeof(Task).IsAssignableFrom(type))
                    {
                        var resultProperty = type.GetProperty("Result");
                        response = resultProperty.GetValue(invocation.ReturnValue);
                    }
                    else
                    {
                        response = invocation.ReturnValue;
                    }
                    if (response==null)
                    {
                        response = string.Empty;
                    }

                    _cache.Set(cacheKey,response,TimeSpan.FromMinutes(cachingAttribute.AbsoluteExpiration)).Wait();
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}
