using Apteryx.MongoDB.Driver.Extend;
using MongoDB.Driver;

namespace Apteryx.Routing.Role.Authority
{
    public static class PageListHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page">1~2147483647</param>
        /// <param name="limit">建议5~200</param>
        /// <returns></returns>
        public static PageList<T> ToPageList<T>(this IOrderedQueryable<T> query, int page = 1, int limit = 20) where T : BaseMongoEntity
        {
            var total = query.Count();
            var data = query.Skip((page - 1) * limit).Take(limit).ToList();
            return new PageList<T>(total, data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page">1~2147483647</param>
        /// <param name="limit">建议5~200</param>
        /// <returns></returns>
        public static PageList<T> ToPageList<T>(this IQueryable<T> query, int page = 1, int limit = 20) where T : BaseMongoEntity
        {
            var total = query.Count();
            var data = query.Skip((page - 1) * limit).Take(limit).ToList();
            return new PageList<T>(total, data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page">1~2147483647</param>
        /// <param name="limit">建议5~200</param>
        /// <returns></returns>
        public static PageList<T> ToPageList<T>(this IFindFluent<T, T> query, int page = 1, int limit = 20) where T : BaseMongoEntity
        {
            var total = query.CountDocuments();
            var data = query.Skip((page - 1) * limit).Limit(limit).ToList();
            return new PageList<T>(total, data);
        }
    }
}
