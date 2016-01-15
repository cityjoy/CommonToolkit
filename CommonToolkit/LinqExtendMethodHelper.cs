using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Aspose.Words.Properties;

namespace CommonToolKit
{
    /// <summary>
    ///  1.Linq OrderBy封装的传递查询，由于本质Linq To EF支持OrderBy，所以只要模拟个keySelector后，
    ///     将剩余的工作交还给Linq To EF Provider来处理
    ///  2.扩展Linq Join方法(组装字符串的操作)
    /// </summary>
    public static class LinqExtendMethodHelper
    {
        /// <summary>
        /// 扩展Linq的OrderBy方法，实现根据属性和顺序(倒序)进行排序，调用和linq的方法一致
        /// </summary>
        /// <typeparam name="TEntity">需要排序的实体对象</typeparam>
        /// <param name="source">结果集信息</param>
        /// <param name="propertyStr">动态排序的属性名(从前台获取)</param>
        /// <param name="isDesc">排序方式，不传递表示顺序，默认true，false表示倒序</param>
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string propertyStr,
            bool isDesc = true) where TEntity : class
        {
            //以下四句用来建立c>c.propertyStr的Expression对象，实现Lambda表达式的状态
            ParameterExpression parameterExpression = Expression.Parameter(typeof (TEntity), "c");
            PropertyInfo propertyInfo = typeof (TEntity).GetProperty(propertyStr);
            Expression expression = Expression.MakeMemberAccess(parameterExpression, propertyInfo);
            LambdaExpression lambdaExpression = Expression.Lambda(expression, parameterExpression);
            Type type = typeof (TEntity);

            //读取排序的顺序信息，如果传递的参数(isDesc)是true，则为顺序排序，否则为倒序排序
            string ascDesc = isDesc ? "OrderBy" : "OrderByDescending";

            //Expression.Call跟上面的信息一样，这里采用重载的形式，上面的GetCurrentMethod结果也是ascDesc
            //Expression.Call方法会利用typeof(Queryable),ascDesc,new Type[]{type,property,PropertyType}三个参数
            //合成跟MethodInfo等同的消息
            MethodCallExpression methodCallExpression = Expression.Call(typeof (Queryable), ascDesc,
                new Type[] {type, propertyInfo.PropertyType}, source.Expression, Expression.Quote(lambdaExpression));

            //返回成功
            return (IOrderedQueryable<TEntity>) source.Provider.CreateQuery<TEntity>(methodCallExpression);

        }

        /// <summary>
        /// 扩展linq的Join方法，使其传递的集合或者数组调用Join方法可以转换成按照规定格式转换的字符串
        /// 使用：string[] strJoin={"kencery","liuxiaoji"};
        ///       strJoin.Join("需要分隔的格式，不传递默认按照，分隔")
        /// </summary>
        /// <param name="source">需要转换成字符串格式的集合信息</param>
        /// <param name="separator">以某种格式分隔集合的信息，不传递默认则是,</param>
        public static string Join(this IEnumerable<string> source, string separator = ",")
        {
            if (source == null)
            {
                throw new Exception("source is null,but source is not null");
            }
            return source.Aggregate((x, y) => x + separator + y);
        }


    }
}