using OkToWake.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OkToWake.Grid
{
    /// <summary>
    /// Creates the right expressions to filter and sort.
    /// </summary>
    public class GridQueryAdapter
    {
        /// <summary>
        /// Holds state of the grid.
        /// </summary>
        private readonly IScheduleFilters _controls;

        /// <summary>
        /// Expressions for sorting.
        /// </summary>
        private readonly Dictionary<ScheduleFilterColumns, Expression<Func<Schedule, string>>> _expressions
            = new Dictionary<ScheduleFilterColumns, Expression<Func<Schedule, string>>>
            {
                { ScheduleFilterColumns.ScheduleName, c => c.ScheduleName }
            };

        /// <summary>
        /// Queryables for filtering.
        /// </summary>
        private readonly Dictionary<ScheduleFilterColumns, Func<IQueryable<Schedule>, IQueryable<Schedule>>> _filterQueries;

        /// <summary>
        /// Creates a new instance of the <see cref="GridQueryAdapter"/> class.
        /// </summary>
        /// <param name="controls">The <see cref="IScheduleFilters"/> to use.</param>
        public GridQueryAdapter(IScheduleFilters controls)
        {
            _controls = controls;

            // set up queries
            _filterQueries = new Dictionary<ScheduleFilterColumns, Func<IQueryable<Schedule>, IQueryable<Schedule>>>
            {
                { ScheduleFilterColumns.ScheduleName, cs => cs.Where(c => c.ScheduleName.Contains(_controls.FilterText)) }
            };
        }

        /// <summary>
        /// Uses the query to return a count and a page.
        /// </summary>
        /// <param name="query">The <see cref="IQueryable{Schedule}"/> to work from.</param>
        /// <returns>The resulting <see cref="ICollection{Schedule}"/>.</returns>
        public async Task<ICollection<Schedule>> FetchAsync(IQueryable<Schedule> query)
        {
            query = FilterAndQuery(query);
            await CountAsync(query);
            var collection = await FetchPageQuery(query)
                .ToListAsync();
            _controls.PageHelper.PageItems = collection.Count;
            return collection;
        }

        /// <summary>
        /// Get total filtered items count.
        /// </summary>
        /// <param name="query">The <see cref="IQueryable{Schedule}"/> to use.</param>
        /// <returns>Asynchronous <see cref="Task"/>.</returns>
        public async Task CountAsync(IQueryable<Schedule> query)
        {
            _controls.PageHelper.TotalItemCount = await query.CountAsync();
        }

        /// <summary>
        /// Build the query to bring back a single page.
        /// </summary>
        /// <param name="query">The <see cref="IQueryable{Schedule}"/> to modify.</param>
        /// <returns>The new <see cref="IQueryable{Schedule}"/> for a page.</returns>
        public IQueryable<Schedule> FetchPageQuery(IQueryable<Schedule> query)
        {
            return query
                .Skip(_controls.PageHelper.Skip)
                .Take(_controls.PageHelper.PageSize)
                .AsNoTracking();
        }

        /// <summary>
        /// Builds the query.
        /// </summary>
        /// <param name="root">The <see cref="IQueryable{Schedule}"/> to start with.</param>
        /// <returns>
        /// The resulting <see cref="IQueryable{Schedule}"/> with sorts and
        /// filters applied.
        /// </returns>
        private IQueryable<Schedule> FilterAndQuery(IQueryable<Schedule> root)
        {
            var sb = new System.Text.StringBuilder();

            // apply a filter?
            if (!string.IsNullOrWhiteSpace(_controls.FilterText))
            {
                var filter = _filterQueries[_controls.FilterColumn];
                sb.Append($"Filter: '{_controls.FilterColumn}' ");
                root = filter(root);
            }

            // apply the expression
            var expression = _expressions[_controls.SortColumn];
            sb.Append($"Sort: '{_controls.SortColumn}' ");

            // fix up name
            if (_controls.SortColumn == ScheduleFilterColumns.ScheduleName)
            {
                sb.Append($"(first name first) ");
                expression = c => c.ScheduleName;
            }

            var sortDir = _controls.SortAscending ? "ASC" : "DESC";
            sb.Append(sortDir);

            Debug.WriteLine(sb.ToString());
            // return the unfiltered query for total count, and the filtered for fetching
            return _controls.SortAscending ? root.OrderBy(expression)
                : root.OrderByDescending(expression);
        }
    }
}
