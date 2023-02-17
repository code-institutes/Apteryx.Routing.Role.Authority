using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority
{
    public static class QueryableHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="page">1~2147483647</param>
        /// <param name="limit">建议5~200</param>
        /// <returns></returns>
        public static IQueryable<T> ToPageList<T>(this IOrderedQueryable<T> obj, [Range(1,int.MaxValue)]int page,int limit)
        {
            return obj.Skip((page - 1) * limit).Take(limit);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="page">1~2147483647</param>
        /// <param name="limit">建议5~200</param>
        /// <returns></returns>
        public static IQueryable<T> ToPageList<T>(this IQueryable<T> obj, [Range(1, int.MaxValue)] int page, [Range(2, 200)] int limit)
        {
            return obj.Skip((page - 1) * limit).Take(limit);
        }


    }
}
