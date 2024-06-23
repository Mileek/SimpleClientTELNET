using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ClientTELNET
{
    const int IAC = 255;
    const int DONT = 254;
    const int DO = 253;
    const int WONT = 252;
    const int WILL = 251;
    const int NOECHO = 0;
    const int ECHO = 1;
    const int SUPPRESS_GO_AHEAD = 3;
    private TcpClient tcpClient;
    private NetworkStream networkStream;
    private bool isConnected = false;
    string command;

    public ClientTELNET(string hostname, int port)
    {
        try
        {
            tcpClient = new TcpClient(hostname, port);
            networkStream = tcpClient.GetStream();
            networkStream.ReadTimeout = 3000000; //Timeout 3000 seconds
            isConnected = true;
            //Command format - IAC -> Command code -> Option code
            byte[] wontEcho = new byte[] { IAC, WONT, NOECHO }; //255 is the IAC sign, 252 is the WONT sign "willingness to handle" the "option", 1 is the ECHO sign
            //To send Will, change WONT to WILL and NOECHO to ECHO
            networkStream.Write(wontEcho, 0, wontEcho.Length);
            // We create a byte array representing the Telnet message "DO SUPPRESS GO AHEAD", which tells the server not to send additional "go ahead" messages. IAC, DO meaning execute and Suppress Go Ahead
            byte[] doSuppressGoAhead = new byte[] { IAC, DO, SUPPRESS_GO_AHEAD };
            networkStream.Write(doSuppressGoAhead, 0, doSuppressGoAhead.Length);

            Listen();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            isConnected = false;
        }
    }

    private void Listen()
    {
        //Thread listening for messages from the server
        new Thread(() =>
        {
            byte[] buffer = new byte[2048]; //Large buffer to receive more data, in the lectures I showed 1024
            try
            {
                while (isConnected)
                {
                    int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        if (buffer[0] == IAC)
                        {
                            // Read the command and option
                            int command = buffer[1];
                            int option = buffer[2];

                            // Handle the command and display information
                            switch (command)
                            {
                                case WILL:
                                    Console.WriteLine("The server wants us to handle the option " + WILL.ToString());
                                    break;
                                case WONT:
                                    Console.WriteLine("The server does not want us to handle the option " + WONT.ToString());
                                    break;
                                case DO:
                                    Console.WriteLine("The server wants us to send the option " + DO.ToString());
                                    break;
                                case DONT:
                                    Console.WriteLine("The server does not want us to send the option " + DONT.ToString());
                                    break;
                            }
                            //Attempt to disable Server ECHO
                            byte[] dontEcho = new byte[] { IAC, DONT, ECHO };
                            networkStream.Write(dontEcho, 0, dontEcho.Length);
                        }
                        else
                        {
                            //A way to bypass ECHO, if the server did not agree to disable ECHO then we simply remove the returned data, remove the command, i.e., what was entered, and the end-of-line characters so that everything is in the appropriate lines (talking about the prompt character)
                            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            if (command != null && response.StartsWith(command))
                            {
                                response = response.Substring(command.Length);
                            }
                            else if (response.EndsWith("\r\n"))
                            {
                                response = response.Substring(0, response.Length - 2);
                            }
                            else if (response.StartsWith("\r\n"))
                            {
                                response = response.Substring(2);
                            }
                            Console.Write(response);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                isConnected = false;
            }
        }).Start();
    }

    public void Send(string command)
    {

        if (!isConnected)
        {
            Console.WriteLine("Not connected to the server.");
            return;
        }

        if (!command.EndsWith("\r\n")) //If there are no end-of-line characters, add them CR LF (Carriage Return, Line Feed)
        {
            command += "\r\n"; //End-of-line characters
        }

        this.command = command;
        byte[] buffer = Encoding.ASCII.GetBytes(command);
        networkStream.Write(buffer, 0, buffer.Length);
    }

    public void Close()
    {
        isConnected = false;
        networkStream.Close();
        tcpClient.Close();
    }
}

class Program
{
    static void Main(string[] args)
    {
        ClientTELNET client = new ClientTELNET("127.0.0.1", 23);//Hardcoded local IP and telnet port
        string input;
        while ((input = Console.ReadLine()) != "q") //Exit the loop after entering q or ctrl+c
        {
            client.Send(input);
        }
        client.Close();
    }
}