﻿using ExtraDry.Server.Internal;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace ExtraDry.Server {

    /// <summary>
    /// A very lightweight dynamic linq builder, just enough to satisfy needs of filtering, sorting and paging API result sets.
    /// </summary>
    public static class QueryableExtensions {
        private const string MissingStabilizerErrorMessage = "Sort requires that a single EF key is uniquely defined to stabalize the sort, even if another sort property is present.  Use a single unique key following EF conventions or specify a Stabilizer in the FilterQuery.";
        private const string SortErrorUserMessage = "Unable to Sort. 0x0F3F241C";

        /// <summary>
        /// Given a `PageQuery`, dynamically constructs an expression query that applies the indicated filtering, sorting, and paging.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection.</typeparam>
        /// <param name="source">The extension source</param>
        /// <param name="partialQuery">A page query that contains filtering, sorting, and paging information.</param>
        /// <param name="defaultFilter">An expression that provides default filtering support, which can be overridden by `partialQuery`</param>
        public static IPartialQueryable<T> QueryWith<T>(this IQueryable<T> source, PageQuery partialQuery, Expression<Func<T, bool>>? defaultFilter = null)
        {
            return new PartialQueryable<T>(source, partialQuery, defaultFilter);
        }

        /// <summary>
        /// Given a `FilterQuery`, dynamically constructs an expression query that applies the indicated filtering and sorting.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection.</typeparam>
        /// <param name="source">The extension source</param>
        /// <param name="filterQuery">A filter query that contains filtering and sorting information.</param>
        /// <param name="defaultFilter">An expression that provides default filtering support, which can be overridden by `filterQuery`</param>
        public static IPartialQueryable<T> QueryWith<T>(this IQueryable<T> source, FilterQuery filterQuery, Expression<Func<T, bool>>? defaultFilter = null)
        {
            return new PartialQueryable<T>(source, filterQuery, defaultFilter);
        }

        /// <summary>
        /// Given a `FilterQuery`, dynamically constructs an expression query that applies the indicated filtering but not the indicated sorting.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection.</typeparam>
        /// <param name="source">The extension source</param>
        /// <param name="filterQuery">A filter query that contains filtering and sorting information.</param>
        public static IQueryable<T> Filter<T>(this IQueryable<T> source, FilterQuery filterQuery)
        {
            return source.Filter(filterQuery.Filter);
        }

        /// <summary>
        /// Given a `filter`, dynamically constructs an expression query that applies the indicated filtering.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection.</typeparam>
        /// <param name="source">The extension source</param>
        /// <param name="filter">A filter query that contains filtering information.</param>
        public static IQueryable<T> Filter<T>(this IQueryable<T> source, string? filter)
        {
            if(string.IsNullOrWhiteSpace(filter)) {
                return source;
            }
            var description = new ModelDescription(typeof(T));
            if(!description.FilterProperties.Any()) {
                return source;
            }
            return source.WhereFilterConditions(description.FilterProperties.ToArray(), filter);
        }

        /// <summary>
        /// Given a `FilterQuery`, dynamically constructs an expression query that applies the indicated sorting but not the indicated filtering.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection.</typeparam>
        /// <param name="source">The extension source</param>
        /// <param name="filterQuery">A filter query that contains sorting information.</param>
        public static IQueryable<T> Sort<T>(this IQueryable<T> source, FilterQuery filterQuery)
        {
            var token = (filterQuery as PageQuery)?.Token; // Only need the token if it's a PageQuery, null if FilterQuery.
            return source.Sort(filterQuery.Sort, filterQuery.Ascending, token);
        }

        /// <summary>
        /// Given a name of a property and an order, dynamically creates a sort query.
        /// Also merges in the continuation token to supply values for consisted paging.
        /// On conflict, the token will override the specified sort.
        /// </summary>
        /// <param name="source">The queryable source, typically from EF, this is from `DbSet.AsQueryable()`</param>
        /// <param name="sort">The name of the property to sort by (optional, case insensitive)</param>
        /// <param name="ascending">Indicates if the order is ascending or not (optional, default true)</param>
        /// <param name="continuationToken">If this is not a new request, the token passed back from the previous request to maintain stability (optional)</param>
        internal static IQueryable<T> Sort<T>(this IQueryable<T> source, string? sort, bool? ascending, string? continuationToken)
        {
            var token = ContinuationToken.FromString(continuationToken);
            var actualSort = token?.Sort ?? sort;
            var actualAscending = token?.Ascending ?? ascending ?? true;
            var query = source;
            var modelDescription = new ModelDescription(typeof(T));
            if(modelDescription.StabilizerProperty == default) {
                throw new DryException(MissingStabilizerErrorMessage, SortErrorUserMessage);
            }
            if(!string.IsNullOrWhiteSpace(actualSort)) {
                query = actualAscending ? 
                    query.OrderBy(actualSort).ThenBy(modelDescription.StabilizerProperty.ExternalName) : 
                    query.OrderByDescending(actualSort).ThenByDescending(modelDescription.StabilizerProperty.ExternalName);
            }
            else {
                query = query.OrderBy(modelDescription.StabilizerProperty.ExternalName);
            }
            return query;
        }

        /// <summary>
        /// Given a `PageQuery`, dynamically constructs an expression query that applies the indicated paging but not the indicated filtering or sorting.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection.</typeparam>
        /// <param name="source">The extension source</param>
        /// <param name="pageQuery">A page query that contains paging information.</param>
        public static IQueryable<T> Page<T>(this IQueryable<T> source, PageQuery pageQuery)
        {
            // TODO: Implement stabilizer using model meta-data.
            return source.Page(pageQuery.Skip, pageQuery.Take, pageQuery.Token);
        }

        /// <summary>
        /// Given a starting position and page size, returns a subset of the a collection.
        /// Use will typically immediately follow a call to `Sort`.
        /// Also merges in continuation token to supply values for paging if skip and take are missing.
        /// On conflict, the skip and take values will override the token.
        /// </summary>
        /// <param name="source">The queryable source, typically from result of call to `Sort`</param>
        /// <param name="skip">The number of records to skip, if paging this is the page number times the take size.</param>
        /// <param name="take">the number of records to take, this is the page size of the fetch.  Use to balance call API latency versus bandwidth</param>
        /// <param name="continuationToken">If this is not a new request, the token passed back from the previous request to maintain stability (optional)</param>
        public static IQueryable<T> Page<T>(this IQueryable<T> source, int skip, int take, string? continuationToken)
        {
            var token = ContinuationToken.FromString(continuationToken);
            var actualSkip = ContinuationToken.ActualSkip(token, skip);
            var actualTake = ContinuationToken.ActualTake(token, take);
            return source.Skip(actualSkip).Take(actualTake);
        }

    }
}
