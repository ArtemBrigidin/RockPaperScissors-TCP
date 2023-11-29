using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Server
{
    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 8888);
        server.Start();
        Console.WriteLine("Сервер запущен. Ожидание подключений...");

        while (true)
        {
            TcpClient client1 = server.AcceptTcpClient();
            TcpClient client2 = server.AcceptTcpClient();

            ServerObject serverObject = new ServerObject(client1, client2);
            Thread clientThread = new Thread(serverObject.Process);
            clientThread.Start();
        }
    }
}

class ServerObject
{
    private TcpClient client1;
    private TcpClient client2;

    public ServerObject(TcpClient client1, TcpClient client2)
    {
        this.client1 = client1;
        this.client2 = client2;
    }

    public void Process()
    {
        try
        {
            using (StreamReader reader1 = new StreamReader(client1.GetStream()))
            using (StreamWriter writer1 = new StreamWriter(client1.GetStream()) { AutoFlush = true })
            using (StreamReader reader2 = new StreamReader(client2.GetStream()))
            using (StreamWriter writer2 = new StreamWriter(client2.GetStream()) { AutoFlush = true })
            {
                SendWelcomeMessage(writer1, "Выберите: 1 - Камень, 2 - Ножницы, 3 - Бумага");
                SendWelcomeMessage(writer2, "Выберите: 1 - Камень, 2 - Ножницы, 3 - Бумага");

                int score1 = 0;
                int score2 = 0;

                while (score1 < 5 && score2 < 5)
                {
                    int choice1 = int.Parse(ReceiveChoice(reader1, writer1));
                    int choice2 = int.Parse(ReceiveChoice(reader2, writer2));

                    var updatedScores = UpdateScores(choice1, choice2, score1, score2, writer1, writer2);
                    score1 = updatedScores.Item1;
                    score2 = updatedScores.Item2;
                }

                SendResult(score1, score2, writer1, writer2);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            client1.Close();
            client2.Close();
        }
    }

    private void SendWelcomeMessage(StreamWriter writer, string message)
    {
        writer.WriteLine("Добро пожаловать в игру Камень, Ножницы, Бумага!");
        writer.WriteLine(message);
    }

    private string ReceiveChoice(StreamReader reader, StreamWriter writer)
    {
        writer.WriteLine("Ваш ход:");
        return reader.ReadLine();
    }

    private (int score1, int score2) UpdateScores(int choice1, int choice2, int score1, int score2, StreamWriter writer1, StreamWriter writer2)
    {
        int updatedScore1 = score1;
        int updatedScore2 = score2;

        if ((choice1 == 1 && choice2 == 2) || (choice1 == 2 && choice2 == 3) || (choice1 == 3 && choice2 == 1))
        {
            updatedScore1++;
            SendRoundResult(writer1, writer2, "Победа", "Поражение");
        }
        else if ((choice2 == 1 && choice1 == 2) || (choice2 == 2 && choice1 == 3) || (choice2 == 3 && choice1 == 1))
        {
            updatedScore2++;
            SendRoundResult(writer2, writer1, "Победа", "Поражение");
        }
        else
        {
            SendRoundResult(writer1, writer2, "Ничья", "Ничья");
        }

        SendScores(writer1, writer2, updatedScore1, updatedScore2);

        return (updatedScore1, updatedScore2);
    }


    private void SendRoundResult(StreamWriter winner, StreamWriter loser, string winMessage, string loseMessage)
    {
        winner.WriteLine(winMessage);
        loser.WriteLine(loseMessage);
    }

    private void SendScores(StreamWriter writer1, StreamWriter writer2, int score1, int score2)
    {
        writer1.WriteLine($"Счет: {score1} - {score2}");
        writer2.WriteLine($"Счет: {score2} - {score1}");
    }

    private void SendResult(int score1, int score2, StreamWriter writer1, StreamWriter writer2)
    {
        if (score1 == 5)
        {
            SendGameResult(writer1, writer2, "Победа!", "Поражение.");
        }
        else
        {
            SendGameResult(writer2, writer1, "Победа!", "Поражение.");
        }
    }

    private void SendGameResult(StreamWriter winner, StreamWriter loser, string winMessage, string loseMessage)
    {
        winner.WriteLine(winMessage);
        loser.WriteLine(loseMessage);
    }
}
