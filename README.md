# SimpleClientTELNET

SimpleClientTELNET is a basic TELNET client written in C#. It allows connecting to a TELNET server and performing various TELNET operations. The client is built using .NET sockets, ensuring efficient and reliable network communication.

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Usage](#usage)
  - [Command Examples](#command-examples)
- [Architecture](#architecture)
- [Contributing](#contributing)
- [License](#license)

## Features

- Connect to a TELNET server with a specified hostname and port
- Send commands to the TELNET server
- Receive and display server responses
- Handle TELNET options and commands
- Built using .NET sockets for efficient communication
- Can be used as a batch file or a console application via the command line

## Requirements

- .NET Framework or .NET Core
- Visual Studio or any C# IDE

## Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/Mileek/SimpleClientTELNET.git
    ```
2. Open the solution in Visual Studio.
3. Build the solution.

## Usage

To use SimpleClientTELNET, run the compiled executable from the command line. 

### Command Examples

- Connect to a TELNET server:
    ```sh
    ClientTELNET client = new ClientTELNET("hostname", port);
    ```
- Send a command to the server:
    ```sh
    client.Send("your-command");
    ```
- Close the connection:
    ```sh
    client.Close();
    ```

## Architecture

The `ClientTELNET` class is the core component of the TELNET client, handling the connection, sending and receiving of messages, and managing TELNET commands and options.

- **ClientTELNET**: Manages the TELNET connection and communication.
  - **Connect**: Establishes a connection to the TELNET server.
  - **Send**: Sends commands to the TELNET server.
  - **Close**: Closes the connection to the TELNET server.
  - **Listen**: Listens for responses from the TELNET server.

## Contributing

Contributions are welcome! Please fork this repository and submit pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
