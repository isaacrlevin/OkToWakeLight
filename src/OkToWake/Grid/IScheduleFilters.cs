namespace OkToWake.Grid
{
    /// <summary>
    /// Interface for filtering.
    /// </summary>
    public interface IScheduleFilters
    {
        /// <summary>
        /// The <see cref="ScheduleFilterColumns"/> being filtered on.
        /// </summary>
        ScheduleFilterColumns FilterColumn { get; set; }

        /// <summary>
        /// Loading indicator.
        /// </summary>
        bool Loading { get; set; }

        /// <summary>
        /// The text of the filter.
        /// </summary>
        string FilterText { get; set; }

        /// <summary>
        /// Paging state in <see cref="PageHelper"/>.
        /// </summary>
        IPageHelper PageHelper { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the sort is ascending or descending.
        /// </summary>
        bool SortAscending { get; set; }

        /// <summary>
        /// The <see cref="ScheduleFilterColumns"/> being sorted.
        /// </summary>
        ScheduleFilterColumns SortColumn { get; set; }
    }
}
