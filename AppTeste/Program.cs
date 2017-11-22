﻿using BetterConsoleTables;
using SignalRConnection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppTeste
{
    class Program
    {
        private static SingletonSignalRConnection _instance => SingletonSignalRConnection.GetInstance("http://localhost:8082/signalr", "NotificationHub");

        static void Main(string[] args)
        {
            int totalGetInt = 0;

            new Thread(() =>
            {
                _instance.On<string>("listenDateTime", d => Console.WriteLine(d));
            }).Start();

            new Thread(() =>
            {
                _instance.On("listenDateTimeNoParameter", () => Console.WriteLine("Ouvindo método SEM retorno - Add 1 dia: " + DateTime.Now.AddDays(1)));
            }).Start();

            new Thread(() =>
            {
                while (true)
                {
                    _instance.Invoke("ListenDateTimeNoParameter");

                    var table = new ConsoleTables.ConsoleTable("Name Method", "Result");
                    table.Options.EnableCount = false;

                    Task<Pessoa> pessoa = _instance.Invoke<Pessoa>("GetPessoa");
                    table.AddRow("GetPessoa", pessoa.Result.Nome == null ? "SignalR Offline" : pessoa.Result.Nome);

                    Task<int> integer = _instance.Invoke<int>("GetInt", totalGetInt);
                    table.AddRow("GetInt", integer == null ? "SignalR Offline" : (totalGetInt = integer.Result).ToString());

                    Task<DateTime> dateTime = _instance.Invoke<DateTime>("GetDateTime");
                    table.AddRow("GetDateTime", dateTime == null ? "SignalR Offline" : dateTime.Result.ToString());

                    Task<string> dateTimeFormated = _instance.Invoke<string>("GetDateTimeFormated");
                    table.AddRow("GetDateTimeFormated", dateTimeFormated == null ? "SignalR Offline" : dateTimeFormated.Result);

                    Task<Guid> guid = _instance.Invoke<Guid>("GetGuid");
                    table.AddRow("GetGuid", guid == null ? "SignalR Offline" : guid.Result.ToString());

                    table.Write();

                    Thread.Sleep(1000);
                    Console.Clear();
                }
            }).Start();

            Console.ReadLine();

        }
    }

    public class Pessoa
    {
        public string Nome { get; set; }
    }
}