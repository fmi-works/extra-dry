﻿using ExtraDry.Core;
using ExtraDry.Server.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ExtraDry.Server {

    public interface IPartialQueryable<T> : IQueryable<T> {

        PagedCollection<T> ToPagedCollection();
    
        Task<PagedCollection<T>> ToPagedCollectionAsync(CancellationToken cancellationToken = default);

        FilteredCollection<T> ToFilteredCollection();

        Task<FilteredCollection<T>> ToFilteredCollectionAsync(CancellationToken cancellationToken = default);

    }

    public class PartialQueryable<T> : IPartialQueryable<T> {

        public PartialQueryable(IQueryable<T> queryable, FilterQuery filterQuery, Func<T, bool>? defaultFilter)
        {
            query = filterQuery;
            if(filterQuery.Filter == null && defaultFilter != null) {
                filteredQuery = queryable.Where(defaultFilter).AsQueryable();
            }
            else {
                filteredQuery = queryable.Filter(filterQuery);
            }
            sortedQuery = filteredQuery.Sort(filterQuery);
            pagedQuery = sortedQuery.Page(0, PageQuery.DefaultTake, null);
        }

        public PartialQueryable(IQueryable<T> queryable, PageQuery pageQuery, Func<T, bool>? defaultFilter)
        {
            query = pageQuery;
            token = ContinuationToken.FromString(pageQuery.Token);
            if(pageQuery.Filter == null && defaultFilter != null) {
                filteredQuery = queryable.Where(defaultFilter).AsQueryable();
            }
            else {
                filteredQuery = queryable.Filter(pageQuery);
            }
            sortedQuery = filteredQuery.Sort(pageQuery);
            pagedQuery = sortedQuery.Page(pageQuery);
        }

        public Type ElementType => pagedQuery.ElementType;

        public Expression Expression => pagedQuery.Expression;

        public IQueryProvider Provider => pagedQuery.Provider;

        public IEnumerator GetEnumerator() => pagedQuery.GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => pagedQuery.GetEnumerator();


        public FilteredCollection<T> ToFilteredCollection()
        {
            var items = sortedQuery.ToList();
            return CreateFilteredCollection(items);
        }

        public async Task<FilteredCollection<T>> ToFilteredCollectionAsync(CancellationToken cancellationToken = default)
        {
            // Logic like EF Core `.ToListAsync` but without taking a dependency on that entire package.
            var items = new List<T>();
            if(sortedQuery is IAsyncEnumerable<T> sortedAsyncQuery) {
                await foreach(var element in sortedAsyncQuery.WithCancellation(cancellationToken)) {
                    items.Add(element);
                }
            }
            else if(sortedQuery is IEnumerable<T> sortedSyncQuery) {
                items.AddRange(sortedSyncQuery);
            }
            else {
                throw new InvalidOperationException("");
            }
            return CreateFilteredCollection(items);
        }

        private FilteredCollection<T> CreateFilteredCollection(List<T> items)
        {
            return new FilteredCollection<T> {
                Items = items,
                Filter = query.Filter,
                Sort = query.Sort,
            };
        }

        public PagedCollection<T> ToPagedCollection()
        {
            var items = pagedQuery.ToList();
            return CreatePartialCollection(items);
        }

        public async Task<PagedCollection<T>> ToPagedCollectionAsync(CancellationToken cancellationToken = default)
        {
            // Logic like EF Core `.ToListAsync` but without taking a dependency on that package into Blazor.
            var items = new List<T>();
            if(pagedQuery is IAsyncEnumerable<T> pagedAsyncQuery) {
                await foreach(var element in pagedAsyncQuery.WithCancellation(cancellationToken)) {
                    items.Add(element);
                }
            }
            else {
                throw new InvalidOperationException("");
            }
            return CreatePartialCollection(items);
        }

        private PagedCollection<T> CreatePartialCollection(List<T> items)
        {
            var skip = (query as PageQuery)?.Skip ?? 0;
            var take = (query as PageQuery)?.Take ?? PageQuery.DefaultTake;
            
            var nextToken = (token ?? new ContinuationToken(query.Filter, query.Sort, query.Ascending, query.Stabilizer, take, take)).Next(skip, take);
            var previousTake = ContinuationToken.ActualTake(token, take);
            var previousSkip = ContinuationToken.ActualSkip(token, skip);
            var total = items.Count == previousTake ? filteredQuery.Count() : previousSkip + items.Count;
            return new PagedCollection<T> {
                Items = items,
                Filter = nextToken.Filter,
                Sort = nextToken.Sort,
                Start = previousSkip,
                Total = total,
                ContinuationToken = nextToken.ToString(),
            };
        }

        private readonly FilterQuery query;

        private readonly ContinuationToken? token;

        private readonly IQueryable<T> filteredQuery;

        private readonly IQueryable<T> sortedQuery;

        private readonly IQueryable<T> pagedQuery;
    }
}