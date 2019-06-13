using System;
using System.Collections.Generic;
using System.Text;

namespace ResearchLibrary.Locations
{
    public class WebURIs
    {
        /// <summary>
        /// Returns the string URL for the WallStreet Journal History Stock Data
        /// </summary>
        /// <returns></returns>
        internal string WSJHistorical(string stock, string startDate, string endDate)
        {
            string wsj = "http://quotes.wsj.com/" + stock + "/historical-prices/download?MOD_VIEW=page&num_rows=" +
                         "6299.041666666667&range_days=6299.041666666667&startDate=+" + startDate + "&endDate=" + endDate;
            return wsj;
        }
    }
}
