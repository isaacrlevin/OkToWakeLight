namespace OkToWake.Grid
{
    /// <summary>
    /// State of grid filters.
    /// </summary>
    public class GridControls : IScheduleFilters
    {
        /// <summary>
        /// Keep state of paging.
        /// </summary>
        public IPageHelper PageHelper { get; set; }

        public GridControls(IPageHelper pageHelper)
        {
            PageHelper = pageHelper;
        }

        /// <summary>
        /// Avoid multiple concurrent requests.
        /// </summary>
        public bool Loading { get; set; }

        /// <summary>
        /// Column to sort by.
        /// </summary>
        public ScheduleFilterColumns SortColumn { get; set; }
            = ScheduleFilterColumns.ScheduleName;

        /// <summary>
        /// True when sorting ascending, otherwise sort descending.
        /// </summary>
        public bool SortAscending { get; set; } = true;

        /// <summary>
        /// Column filtered text is against.
        /// </summary>
        public ScheduleFilterColumns FilterColumn { get; set; }
            = ScheduleFilterColumns.ScheduleName;

        /// <summary>
        /// Text to filter on.
        /// </summary>
        public string FilterText { get; set; }
    }
}
