using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;


namespace L04
{
    class Program
	    {
	        private static CloudTable studentsTable;
	        private static CloudTableClient tableClient;
	        private static TableOperation tableOperation;
	        private static TableResult tableResult;
	        private static List<Studententity> students  = new List<Studententity>();
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
	            
	            int option = -1;
	            do
	            {
	                System.Console.WriteLine("1.Adauga student.");
	                System.Console.WriteLine("2.Update student.");
	                System.Console.WriteLine("3.Sterge student.");
	                System.Console.WriteLine("4.Afisare studenti.");
	                System.Console.WriteLine("5.Exit");
	                System.Console.WriteLine("Alege optiunea:");
	                string opt = System.Console.ReadLine();
	                option =Int32.Parse(opt);
	                switch(option)
	                {
	                    case 1:
	                        await AddNewStudent();
	                        break;
	                    case 2:
	                        await EditStudent();
	                        break;
	                    case 3:
	                        await DeleteStudent();
	                        break;
	                    case 4:
	                        await DisplayStudents();
	                        break;
	                    case 5:
	                        System.Console.WriteLine("Thank you for visit!");
	                        break;
	                }
	            }while(option != 5);
	            
	        }
	        private static async Task<Studententity> RetrieveRecordAsync(CloudTable table,string partitionKey,string rowKey)
	        {
	            tableOperation = TableOperation.Retrieve<Studententity>(partitionKey, rowKey);
	            tableResult = await table.ExecuteAsync(tableOperation);
	            return tableResult.Result as Studententity;
	        }
	        private static async Task AddNewStudent()
	        {
	            System.Console.WriteLine("Insert universitate:");
	            string universitate = Console.ReadLine();
	            System.Console.WriteLine("Insert cnp:");
	            string cnp = Console.ReadLine();
	            System.Console.WriteLine("Insert nume:");
	            string nume = Console.ReadLine();
	            System.Console.WriteLine("Insert prenume:");
	            string prenume = Console.ReadLine();
	            System.Console.WriteLine("Insert facultate:");
	            string facultate= Console.ReadLine();
	            System.Console.WriteLine("Insert an studiu:");
	            string an = Console.ReadLine();
	
	            Studententity stud = await RetrieveRecordAsync(studentsTable, universitate, cnp);
	            if(stud == null)
	            {
	                var student = new Studententity(universitate,cnp);
	                student.Nume = nume;
	                student.Prenume = prenume;
	                student.Facultate = facultate;
	                student.An = Convert.ToInt32(an);
	                var insertOperation = TableOperation.Insert(student);
	                await studentsTable.ExecuteAsync(insertOperation);
	                System.Console.WriteLine("Record inserted!");
	            }
	            else
	            {
	                System.Console.WriteLine("Record exists!");
	            }
	        }
	        private static async Task EditStudent()
	        {
	            System.Console.WriteLine("Insert universitate:");
	            string universitate = Console.ReadLine();
	            System.Console.WriteLine("Insert cnp:");
	            string cnp = Console.ReadLine();
	            Studententity stud = await RetrieveRecordAsync(studentsTable, universitate, cnp);
	            if(stud != null)
	            {
	                System.Console.WriteLine("Record exists!");
	                var student = new Studententity(universitate,cnp);
	                System.Console.WriteLine("Insert Nume:");
	                string nume = Console.ReadLine();
	                System.Console.WriteLine("Insert Prenume:");
	                string prenume = Console.ReadLine();
	                System.Console.WriteLine("Insert facultate:");
	                string facultate = Console.ReadLine();
	                System.Console.WriteLine("Insert an studiu:");
	                string an = Console.ReadLine();
	                student.Nume= nume;
	                student.Prenume = prenume;
	                student.Facultate = facultate;
	                student.An = Convert.ToInt32(an);
	                student.ETag = "*";
	                var updateOperation = TableOperation.Replace(student);
	                await studentsTable.ExecuteAsync(updateOperation);
	                System.Console.WriteLine("Record updated!");
	            }
	            else
	            {
	                System.Console.WriteLine("Record does not exists!");
	            }
	        }
	        private static async Task DeleteStudent()
	        {
	            System.Console.WriteLine("Insert universitatea:");
	            string universitatea = Console.ReadLine();
	            System.Console.WriteLine("Insert cnp:");
	            string cnp = Console.ReadLine();
	
	            Studententity stud = await RetrieveRecordAsync(studentsTable, universitatea, cnp);
	            if(stud != null)
	            {
	                var student = new Studententity(universitatea,cnp);
	                student.ETag = "*";
	                var deleteOperation = TableOperation.Delete(student);
	                await studentsTable.ExecuteAsync(deleteOperation);
	                System.Console.WriteLine("Record deleted!");
	            }
	            else
	            {
	                System.Console.WriteLine("Record does not exists!");
	            }
	        }
	        private static async Task<List<Studententity>> GetAllStudents()
	        {
	            TableQuery<Studententity> tableQuery = new TableQuery<Studententity>();
	            TableContinuationToken token = null;
	            do
	            {
	                TableQuerySegment<Studententity> result = await studentsTable.ExecuteQuerySegmentedAsync(tableQuery,token);
	                token = result.ContinuationToken;
	                students.AddRange(result.Results);
	            }while(token != null);
	            return students;
	        }
	        private static async Task DisplayStudents()
	        {
	            await GetAllStudents();
	
	            foreach(Studententity std in students)
	            {
	                Console.WriteLine("Student facultate : {0}", std.Facultate);
	                Console.WriteLine("Student nume : {0}", std.Nume);
	                Console.WriteLine("Student prenume : {0}", std.Prenume);
	                Console.WriteLine("Student an : {0}", std.An);
	                Console.WriteLine("------------------------------------");
	            }
	            students.Clear();
	            
	        }
	    }
	}

