using ConsoleTables.Core;
using SignalRConnection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppClient2
{
    class Pessoa
    {
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public int CartaoCredito { get; set; }

    }

    class Program
    {

        private static string host = "http://localhost:8082/signalr";

        static void Main()
        {
            ConsoleTable table = new ConsoleTable("Name Method", "Result");
            Task.Factory.StartNew(() => OnNotificationHub(table));
            Task.Factory.StartNew(() => OnExempleHub(table));
            Console.ReadKey();
        }

        private static void OnNotificationHub(ConsoleTable table)
        {

            Task.Factory.StartNew(() =>
            {
                SignalRCreateConnection notificationHub = new SignalRCreateConnection(host, "NotificationHub");
                notificationHub.On<string>("getNewGuid", d =>
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Gray;
                    table = WriteTableRow(table, "getNewGuid", d);
                });

                notificationHub.On<int>("getInt", i =>
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Gray;
                    table = WriteTableRow(table, "getInt", i);
                });

                notificationHub.On("onMethodWithoutReturnValue", () =>
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Gray;
                    table = WriteTableRow(table, "onMethodWithoutReturnValue", "Método sem valor de retorno -> " + DateTime.Now);
                });

                do
                {
                    if (!notificationHub.ServerOnline)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Server SignalR OffLine.");
                        OnNotificationHub(table);
                    }

                    Thread.Sleep(2000);
                } while (true);

            });

            Console.ReadKey();

        }

        private static void OnExempleHub(ConsoleTable table)
        {

            Task.Factory.StartNew(() =>
            {
                SignalRCreateConnection conn = new SignalRCreateConnection(host, "ExampleHub");
                conn.On<string>("listenDateTime", d =>
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Gray;
                    table = WriteTableRow(table, "listenDateTime", d);
                });


                do
                {
                    if (!conn.ServerOnline)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Server SignalR OffLine.");
                        OnExempleHub(table);
                    }

                    Thread.Sleep(2000);
                } while (true);

            });

            Console.ReadKey();

        }

        private static ConsoleTable WriteTableRow(ConsoleTable table, string nameMethod, object value)
        {
            if (table.Rows.Count == 50)
                table.Rows.Clear();

            table.AddRow(nameMethod, value);
            table.Write();
            return table;
        }

    }
    
}
