using Af.Core.Common;
using Af.Core.IRepository.UnitOfWork;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Af.Extensions.AOP
{
    public class TranAOP : IInterceptor
    {
        private readonly IUnitOfWork _unitOfWork;

        public TranAOP(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            //当前方法特性验证
            //如果需要验证
            if (method.GetCustomAttributes(true).FirstOrDefault(a=>a.GetType()==typeof(UseTranAttribute)) is UseTranAttribute)
            {
                try
                {
                    Console.WriteLine("Begin Transaction");
                    _unitOfWork.BeginTran();
                    invocation.Proceed();

                    // 异步获取异常 先执行
                    if (IsAsyncMethod(invocation.Method))
                    {
                        var result = invocation.ReturnValue;
                        if (result is Task)
                        {
                            Task.WaitAll(result as Task);
                        }
                    }
                    _unitOfWork.CommitTran();
                }
                catch (Exception)
                {
                    Console.WriteLine("Rollback Transaction");
                    _unitOfWork.RollbackTran();
                    
                }
            }
            else
            {
                invocation.Proceed();
            }
        }

        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (method.ReturnType==typeof(Task) 
                || (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition()==typeof(Task<>)));
        }
    }
}
