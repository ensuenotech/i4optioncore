using DocumentFormat.OpenXml.InkML;
using i4optioncore.DBModels;
using i4optioncore.DBModelsMaster;
using i4optioncore.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace i4optioncore.Repositories
{
    public class ImportBL(MasterdataDbContext dbMaster, i4option_dbContext db) : IImportBL
    {
        private readonly MasterdataDbContext dbMaster = dbMaster;
        private readonly i4option_dbContext db = db;

        public void Import52WeekData(List<_52weekHighLow> request)
        {
            var table = new DataTable();
            table.Columns.Add("Symbol");
            table.Columns.Add("Date");
            table.Columns.Add("High");
            table.Columns.Add("HighDate");
            table.Columns.Add("Low");
            table.Columns.Add("LowDate");

            foreach (var item in request)
            {
                table.Rows.Add(item.Symbol, item.Date, item.High, item.HighDate, item.Low, item.LowDate);
            }
            using var connection = new SqlConnection(dbMaster.Database.GetDbConnection().ConnectionString);
            SqlTransaction transaction = null;
            connection.Open();
            try
            {
                transaction = connection.BeginTransaction();
                using (var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction))
                {
                    sqlBulkCopy.BulkCopyTimeout = 300;
                    sqlBulkCopy.DestinationTableName = "52WeekHighLow";
                    sqlBulkCopy.ColumnMappings.Add("Symbol", "Symbol");
                    sqlBulkCopy.ColumnMappings.Add("Date", "Date");
                    sqlBulkCopy.ColumnMappings.Add("High", "High");
                    sqlBulkCopy.ColumnMappings.Add("HighDate", "HighDate");
                    sqlBulkCopy.ColumnMappings.Add("Low", "Low");
                    sqlBulkCopy.ColumnMappings.Add("LowDate", "LowDate");

                    sqlBulkCopy.WriteToServer(table);
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
        }
        public void ImportEarningRatioData(ImportModel.EarningRatioRequest request)
        {
            var table = new DataTable();

            // Add columns based on the JSON and updated table structure
            table.Columns.Add("IndexName", typeof(string)); // Symbol
            table.Columns.Add("IndexDate", typeof(DateTime)); // Date (mapped to ErDate in the table)
            table.Columns.Add("OpenIndexValue", typeof(string)); // Series
            table.Columns.Add("HighIndexValue", typeof(decimal)); // High
            table.Columns.Add("LowIndexValue", typeof(decimal)); // Low
            table.Columns.Add("ClosingIndexValue", typeof(decimal)); // Open (mapped to OpenValue in JSON)
            table.Columns.Add("PointsChange", typeof(decimal)); // PointsChange
            table.Columns.Add("ChangePercentage", typeof(decimal)); // ChangePercentage
            table.Columns.Add("Volume", typeof(long)); // Volume
            table.Columns.Add("Turnover", typeof(decimal)); // Turnover
            table.Columns.Add("PE", typeof(decimal)); // PE
            table.Columns.Add("PB", typeof(decimal)); // PB
            table.Columns.Add("DivYield", typeof(decimal)); // DivYield

            // Populate rows from the request data
            foreach (var item in request.Data)
            {
                table.Rows.Add(
                    item.IndexName,         // Symbol
                    item.IndexDate,        // ErDate
                    item.OpenIndexValue,         // Series
                    item.HighIndexValue,           // High
                    item.LowIndexValue ,            // Low
                    item.ClosingIndexValue,   // ClosingValue
                    item.PointsChange,  // PointsChange
                    item.ChangePercentage, // ChangePercentage
                    item.Volume,         // Volume
                    item.Turnover,       // Turnover
                    item.PE,             // PE
                    item.PB,             // PB
                    item.DivYield        // DivYield,
                );
            }
            using var connection = new SqlConnection(db.Database.GetDbConnection().ConnectionString);
            SqlTransaction transaction = null;
            connection.Open();
            try
            {
                transaction = connection.BeginTransaction();
                using (var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction))
                {
                    sqlBulkCopy.BulkCopyTimeout = 300;
                    sqlBulkCopy.DestinationTableName = "EarningRatios";

                    // Column mappings based on the JSON
                    sqlBulkCopy.ColumnMappings.Add("indexName", "IndexName");
                    sqlBulkCopy.ColumnMappings.Add("indexDate", "IndexDate");
                    sqlBulkCopy.ColumnMappings.Add("openIndexValue", "OpenIndexValue");
                    sqlBulkCopy.ColumnMappings.Add("highIndexValue", "HighIndexValue");
                    sqlBulkCopy.ColumnMappings.Add("lowIndexValue", "LowIndexValue");
                    sqlBulkCopy.ColumnMappings.Add("closingIndexValue", "ClosingIndexValue");
                    sqlBulkCopy.ColumnMappings.Add("pointsChange", "PointsChange");
                    sqlBulkCopy.ColumnMappings.Add("changePercentage", "ChangePercentage");
                    sqlBulkCopy.ColumnMappings.Add("volume", "Volume");
                    sqlBulkCopy.ColumnMappings.Add("turnover", "Turnover");
                    sqlBulkCopy.ColumnMappings.Add("pe", "PE");
                    sqlBulkCopy.ColumnMappings.Add("pb", "PB");
                    sqlBulkCopy.ColumnMappings.Add("divYield", "DivYield");

                    sqlBulkCopy.WriteToServer(table);
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
        }
    }
}
