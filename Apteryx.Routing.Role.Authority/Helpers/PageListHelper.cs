﻿using Apteryx.MongoDB.Driver.Extend;
using MongoDB.Driver;
using System.Numerics;

namespace Apteryx.Routing.Role.Authority
{
    public static class PageListHelper
    {
        private const int _page = 1;
        private const int _limit = 20;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page">1~2147483647</param>
        /// <param name="limit">建议5~200</param>
        /// <returns></returns>
        public static PageList<T> ToPageList<T>(this IOrderedQueryable<T> query, int page = _page, int limit = _limit) where T : BaseMongoEntity
        {
            var total = query.Count();
            var data = query.ToPageData(page, limit);
            return new PageList<T>(total, data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static Task<PageList<T>> ToPageListAsync<T>(this IOrderedQueryable<T> query, int page = _page, int limit = _limit) where T : BaseMongoEntity
        {
            return Task.Run(() => query.ToPageList(page, limit));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="query"></param>
        /// <param name="fun"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static PageList<TR> ToPageList<T, TR>(this IOrderedQueryable<T> query, Func<T, TR> fun, int page = _page, int limit = _limit)
            where T : BaseMongoEntity
            where TR : class
        {
            var total = query.Count();
            var data = query.ToPageData(page, limit).Select(fun).ToList();
            return new PageList<TR>(total, data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="query"></param>
        /// <param name="fun"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static Task<PageList<TR>> ToPageListAsync<T, TR>(this IOrderedQueryable<T> query, Func<T, TR> fun, int page = _page, int limit = _limit)
            where T : BaseMongoEntity
            where TR : class
        {
            return Task.Run(() => query.ToPageList(fun, page, limit));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page">1~2147483647</param>
        /// <param name="limit">建议5~200</param>
        /// <returns></returns>
        public static PageList<T> ToPageList<T>(this IQueryable<T> query, int page = _page, int limit = _limit) where T : BaseMongoEntity
        {
            var total = query.Count();
            var data = query.ToPageData<T>(page, limit);
            return new PageList<T>(total, data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static Task<PageList<T>> ToPageListAsync<T>(this IQueryable<T> query, int page = _page, int limit = _limit) where T : BaseMongoEntity
        {
            return Task.Run(() => query.ToPageList(page, limit));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="query"></param>
        /// <param name="fun"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static PageList<TR> ToPageList<T, TR>(this IQueryable<T> query, Func<T, TR> fun, int page = _page, int limit = _limit)
        where T : BaseMongoEntity
        where TR : class
        {
            var total = query.Count();
            var data = query.ToPageData<T>(page, limit).Select(fun);
            return new PageList<TR>(total, data);
        }
        public static Task<PageList<TR>> ToPageListAsync<T, TR>(this IQueryable<T> query, Func<T, TR> fun, int page = _page, int limit = _limit)
        where T : BaseMongoEntity
        where TR : class
        {
            return Task.Run(() => query.ToPageList(fun, page, limit));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page">1~2147483647</param>
        /// <param name="limit">建议5~200</param>
        /// <returns></returns>
        public static PageList<T> ToPageList<T>(this IFindFluent<T, T> query, int page = _page, int limit = _limit) where T : BaseMongoEntity
        {
            var total = query.CountDocuments();
            var data = query.ToPageData(page, limit);
            return new PageList<T>(total, data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static Task<PageList<T>> ToPageListAsync<T>(this IFindFluent<T, T> query, int page = _page, int limit = _limit) where T : BaseMongoEntity
        {
            return Task.Run(() => query.ToPageList(page, limit));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="query"></param>
        /// <param name="fun"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static PageList<TR> ToPageList<T, TR>(this IFindFluent<T, T> query, Func<T, TR> fun, int page = _page, int limit = _limit)
        where T : BaseMongoEntity
        where TR : class
        {
            var total = query.CountDocuments();
            var data = query.ToPageData(page, limit).Select(fun);
            return new PageList<TR>(total, data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="query"></param>
        /// <param name="fun"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static Task<PageList<TR>> ToPageListAsync<T, TR>(this IFindFluent<T, T> query, Func<T, TR> fun, int page = _page, int limit = _limit)
        where T : BaseMongoEntity
        where TR : class
        {
            return Task.Run(() => query.ToPageList(fun, page, limit));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private static IEnumerable<T> ToPageData<T>(this IOrderedQueryable<T> query, int page = _page, int limit = _limit) where T : BaseMongoEntity
        {
            return query.Skip((page - 1) * limit).Take(limit).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private static IEnumerable<T> ToPageData<T>(this IQueryable<T> query, int page = _page, int limit = _limit) where T : BaseMongoEntity
        {
            return query.Skip((page - 1) * limit).Take(limit).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private static IEnumerable<T> ToPageData<T>(this IFindFluent<T, T> query, int page = _page, int limit = _limit) where T : BaseMongoEntity
        {
            return query.Skip((page - 1) * limit).Limit(limit).ToList();
        }
    }
}
