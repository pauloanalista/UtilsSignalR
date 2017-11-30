using SignalRConnection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppClient1
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
            Task.Factory.StartNew(() => InvokeExempleHub());

            Task.Factory.StartNew(() => InvokeNotificationHub());

            Console.ReadKey();

        }

        private static void InvokeExempleHub()
        {
            SignalRCreateConnection exemploHub = new SignalRCreateConnection(host, "ExampleHub");

            while (true)
            {
                Task<string> date = exemploHub.Invoke<string>("GetDateTimeFormated");

                if (!exemploHub.ServerOnline)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Server OffLine -> {(string.IsNullOrEmpty(date.Result) ? "No Connection - Result [GetDateTimeFormated]: Null" : "Connected - Result [GetDateTimeFormated]: " + date.Result)}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"Server OnLine -> {(string.IsNullOrEmpty(date.Result) ? "No Connection - Result [GetDateTimeFormated]: Null" : "Connected - Result [GetDateTimeFormated]: " + date.Result)}");
                }

                Thread.Sleep(1000);
            }
        }

        private static void InvokeNotificationHub()
        {
            SignalRCreateConnection notificationHub = new SignalRCreateConnection(host, "NotificationHub");

            int totalInt = 0;

            while (true)
            {
                Task<Pessoa> pessoa = notificationHub.Invoke<Pessoa>("GetPessoa");

                notificationHub.Invoke("SendNewGuid");
                notificationHub.Invoke("SendInt", totalInt++);

                if (!notificationHub.ServerOnline)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Server OffLine -> {(string.IsNullOrEmpty(pessoa.Result.Nome) ? "No Connection - Result [GetPessoa]: Null" : "Connected - Result [GetPessoa]: " + pessoa.Result.Nome)}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"Server OnLine -> {(string.IsNullOrEmpty(pessoa.Result.Nome) ? "No Connection - Result [GetPessoa]: Null" : "Connected - Result [GetPessoa]: " + pessoa.Result.Nome)}");
                }

                Thread.Sleep(1000);
            }
        }
    }
}
