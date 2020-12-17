using Af.Core.Common.LogHelper;
using Castle.DynamicProxy;
using System.Linq;
using System.Threading.Tasks;

namespace Af.Core.AOP
{
    public class LogAOP : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var dataIntercept = $"方法：{invocation.Method.Name}\r\n";
            dataIntercept += $"参数：{string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())}\r\n";
            invocation.Proceed();
            dataIntercept += $"结果：{invocation.ReturnValue}";

            //输出日志文件
            Parallel.For(0, 1, e =>
            {
                LogLock.OutSql2Log("AOPLog", new string[] { dataIntercept });
            });
        }
    }
}
