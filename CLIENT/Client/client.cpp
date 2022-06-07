#include <iostream>
#include <string>

#include <WS2tcpip.h>

#define PORT 6969

#pragma comment(lib, "ws2_32.lib")

int main()
{
	std::string ipAddress = "127.0.0.1";							     

	// Initialize WinSock
	WSAData data;
	int wsResult = WSAStartup(MAKEWORD(2, 2), &data);

	if (wsResult != 0)
	{
		std::cerr << "Cant initialize winsock!!!\n";
		return 1;
	}

	// Create socket
	SOCKET sock = socket(AF_INET, SOCK_STREAM, 0);
	if (sock == INVALID_SOCKET)
	{
		std::cerr << "Cant create a socket\n";
		WSACleanup();
		return 1;
	}

	sockaddr_in hint;
	hint.sin_family = AF_INET;
	hint.sin_port = htons(PORT);
	inet_pton(AF_INET, ipAddress.c_str(), &hint.sin_addr);

	// Connect to server
	int connectionResult = connect(sock, (sockaddr*)&hint, sizeof(hint));
	if (connectionResult == SOCKET_ERROR)
	{
		std::cerr << "Cant connect to server\n";
		closesocket(sock);
		WSACleanup();
		return 1;
	}


	char buffer[4096];
	std::string input;
	do
	{
		ZeroMemory(buffer, 4096);
		int bytesReceived = recv(sock, buffer, 4096, 0);
		if (bytesReceived > 0)
			std::cout << "SERVER: " << std::string(buffer, 0, bytesReceived) << "\n";
		std::cout << "> ";
		getline(std::cin, input);
		if (input.size() > 0)
		{
			// Send the text
			int sendResult = send(sock, input.c_str(), input.size() + 1, 0);
		}
	} while (input.size() > 0);


	closesocket(sock);
	WSACleanup();

	return 0;
}
