/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VPrinting.Colections;
using VPrinting.Common;
using VPrinting.Data;
using VPrinting.Extentions;
using VPrinting.PartyManagement;
using VPrinting.ScanServiceRef;
using mng = VPrinting.PartyManagement;

namespace VPrinting
{
    partial class MainForm
    {
        #region SEARCH

        private int CountryId
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["CountryId"]);
            }
        }

        private void InitializeSearch()
        {
            var service = new mng.PartyManagement();
            var countries = new Func<mng.PartyManagement, CountryDetail[]>((s) => s.GetPtfCountryList()).ReTry(service);
            if (countries == null)
                throw new ApplicationException("Can not connect to the server.");

            foreach (var country in countries.OrderBy(c => c.Nationality))
                cbCountryId.Items.Add(country);      

            cbCountryId.SetSelected<CountryDetail>((c) => c.Number == CountryId);
        }

        private void ShowHistory_Click(object sender, EventArgs e)
        {
            if (cbHistoryType.SelectedItem == null)
                return;
            var data = (OperationHistory)cbHistoryType.SelectedItem;
            var list = ServiceDataAccess.Instance.ReadHistory(data, historyFromTime.Value, historyToTime.Value).ToList();
            dgvSearchData.DataSource = new SortableBindingList<HistoryByCountryInfo>(list);
        }

        private void SearchSort_Click(object sender, EventArgs e)
        {
            // Check which column is selected, otherwise set NewColumn to null.
            DataGridViewColumn newColumn = dgvSearchData.SelectedCells.Count > 0 ? dgvSearchData.SelectedCells[0].OwningColumn : null;

            DataGridViewColumn oldColumn = dgvSearchData.SortedColumn;
            ListSortDirection direction;

            // If oldColumn is null, then the DataGridView is not currently sorted. 
            if (oldColumn != null)
            {
                // Sort the same column again, reversing the SortOrder. 
                if (oldColumn == newColumn && dgvSearchData.SortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    // Sort a new column and remove the old SortGlyph.
                    direction = ListSortDirection.Ascending;
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }

            // If no column has been selected, display an error dialog  box. 
            if (newColumn != null)
            {
                dgvSearchData.Sort(newColumn, direction);
                newColumn.HeaderCell.SortGlyphDirection = direction == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending;
            }
            else
            {
                this.ShowExclamation("Select a single column and try again.");
            }
        }

        private void Search_Click(object sender, EventArgs e)
        {
            var sql = new StringBuilder();
            if (!Program.IsAdmin)
                sql.Append(" location=".concat(Program.currentUser.CountryID));

            if (cbCountryId.SelectedItem != null)
            {
                var country = (CountryDetail)cbCountryId.SelectedItem;
                if (!sql.IsEmpty())
                    sql.Append(" AND ");
                sql.AppendFormat(" iso_id={0} ", country.Number);
            }

            //Add JobID

            if (tbBrId.Text.CanConvertTo<string, int>())
            {
                if (!sql.IsEmpty())
                    sql.Append(" AND ");
                sql.AppendFormat(" branch_id={0} ", tbBrId.Text);
            }

            if (tbVoucherId.Text.CanConvertTo<string, int>())
            {
                if (!sql.IsEmpty())
                    sql.Append(" AND ");
                sql.AppendFormat(" v_number={0} ", tbVoucherId.Text);
            }

            if (dtFrom.Value.HasValue)
            {
                if (!sql.IsEmpty())
                    sql.Append(" AND ");
                sql.AppendFormat(" scandate>'{0:yyyy-MM-dd}' ", dtFrom.Value);
            }

            if (dtTo.Value.HasValue)
            {
                if (!sql.IsEmpty())
                    sql.Append(" AND ");
                sql.AppendFormat(" scandate<'{0:yyyy-MM-dd}' ", dtTo.Value);
            }

            if (!sql.IsEmpty())
            {
                var list = ServiceDataAccess.Instance.SelectFilesBySql(sql.toString()).ToList();
                dgvSearchData.DataSource = new SortableBindingList<fileInfo>(list);
            }
            else
            {
                this.ShowExclamation("Select anything");
            }
        }

        private void SearchData_DoubleClick(object sender, EventArgs e)
        {

        }

        private void SearchData_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            tslItemsCount.Text = dgvSearchData.RowCount.toString();
        }

        #endregion
    }
}
