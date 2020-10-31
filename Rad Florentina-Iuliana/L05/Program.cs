using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;

namespace L05
{
    class Program
    {
        private static CloudTable studentsTable;
        private static CloudTableClient tableClient;
        private static CloudTable universityMetrics;
        private static CloudTableClient tableMetrics;
        private static List<StudentEntity> students  = new List<StudentEntity>();
        private static List<Statistici> metrics  = new List<Statistici>();
        
        public static int previousTotal = 0;
        static void Main(string[] args)
        {
            Task.Run(async () => { await Initialize(); })
                .GetAwaiter()
                .GetResult();
        }
        static async Task Initialize()
        {
           string storageConnectionString = "DefaultEndpointsProtocol=https;"
	            + "AccountName=datc2020iuli;"
	            + "AccountKey=LmeyHC+SVaZNC+8X3dDxiC5dLmoqK9x/XNnn2IPnlchkWdUkFRRTgY+uIfnJpct6DSjTyq2n50QyJiIy4D5lVg==;"
	            + "EndpointSuffix=core.windows.net";

            var account = CloudStorageAccount.Parse(storageConnectionString);
            tableClient = account.CreateCloudTableClient();

            studentsTable = tableClient.GetTableReference("Studenti");

            await studentsTable.CreateIfNotExistsAsync();

            await DisplayStudents(storageConnectionString);
        }
        private static async Task<List<StudentEntity>> GetAllStudents()
        {
            TableQuery<StudentEntity> tableQuery = new TableQuery<StudentEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<StudentEntity> result = await studentsTable.ExecuteQuerySegmentedAsync(tableQuery,token);
                token = result.ContinuationToken;
                students.AddRange(result.Results);
            }while(token != null);
            return students;
        }
        private static async Task<List<Statistici>> GetAllMetrics()
        {
            TableQuery<Statistici> tableQuery = new TableQuery<Statistici>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<Statistici> result = await universityMetrics.ExecuteQuerySegmentedAsync(tableQuery,token);
                token = result.ContinuationToken;
                metrics.AddRange(result.Results);
            }while(token != null);
            return metrics;
        }
        private static async Task DisplayStudents(string storageConnectionString)
        {
            await GetAllStudents();
            
            var accountMetrics = CloudStorageAccount.Parse(storageConnectionString);
            tableMetrics = accountMetrics.CreateCloudTableClient();

            universityMetrics = tableMetrics.GetTableReference("TabelMetrici");
            await universityMetrics.CreateIfNotExistsAsync();
            await GetAllMetrics();
            List<int> totalStudents  = new List<int>();
            int UptCounter = 0;
            int UvtCounter = 0;
            foreach(StudentEntity std in students)
            {
                if(std.PartitionKey == "UPT")
                    UptCounter++;
                else
                    UvtCounter++;
            }
            foreach(Statistici stat in metrics)
            {
                totalStudents.Add(stat.TotalNrOfStudents);
            }

            int total = UptCounter + UvtCounter;
            previousTotal = Convert.ToInt32(totalStudents.Max());
            
            if(total != previousTotal)
            {
                var timeSpan1 = DateTime.Now.ToString("o");
                Statistici stat1 = new Statistici("UPT",timeSpan1);
                stat1.TotalNrOfStudents = UptCounter;
                var insertOperation1 = TableOperation.Insert(stat1);
                await universityMetrics.ExecuteAsync(insertOperation1);

                var timeSpan2 = DateTime.Now.ToString("o");
                Statistici stat2 = new Statistici("UVT",timeSpan2);
                stat2.TotalNrOfStudents = UvtCounter;
                var insertOperation2 = TableOperation.Insert(stat2);
                await universityMetrics.ExecuteAsync(insertOperation2);
                
                var timeSpan3 = DateTime.Now.ToString("o");
                Statistici stat3 = new Statistici("Total",timeSpan3);
                stat3.TotalNrOfStudents = total;
                var insertOperation3 = TableOperation.Insert(stat3);
                await universityMetrics.ExecuteAsync(insertOperation3);
                
            students.Clear();

            }
        }
    }
}