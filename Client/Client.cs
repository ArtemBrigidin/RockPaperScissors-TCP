using System;
using System.IO;
using System.Net.Sockets;

class Client
{
    static void Main()
    {
        TcpClient client = new TcpClient("127.0.0.1", 8888);

        using (StreamReader reader = new StreamReader(client.GetStream()))
        using (StreamWriter writer = new StreamWriter(client.GetStream()) { AutoFlush = true })
        {
            string welcomeMessage = reader.ReadLine();
            Console.WriteLine(welcomeMessage);

            int victories = 0;

            while (victories != 5)
            {
                string result = reader.ReadLine();
                Console.WriteLine(result);

                if (result.Contains("Победа") || result.Contains("Поражение"))
                {
                    victories++;
                }
                if (victories == 5)
                {
                    Console.WriteLine("Пять побед! Игра завершена.");
                    break;
                }

                string message = reader.ReadLine();
                Console.WriteLine(message);

                Console.Write("Ваш выбор: ");
                int choice = int.Parse(Console.ReadLine());
                writer.WriteLine(choice);

                string score = reader.ReadLine();
                Console.WriteLine(score);
            }

            Console.WriteLine("До встречи!");

        }

        client.Close();
    }
}
