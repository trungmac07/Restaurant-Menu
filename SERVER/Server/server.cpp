#include <iostream>

#include <WS2tcpip.h>
#include <string>

#define PORT 6969

#pragma comment (lib, "ws2_32.lib")

//-lwsock32
int main()
{
	// Initialze winsock
	WSADATA wsData;
	WORD ver = MAKEWORD(2, 2);

	int wsOk = WSAStartup(ver, &wsData);
	if (wsOk != 0)
	{
		std::cerr << "Cant initialize winsock!!!\n";
		return 1;
	}

	// Create a socket
	SOCKET listening = socket(AF_INET, SOCK_STREAM, 0);
	if (listening == INVALID_SOCKET)
	{
		std::cerr << "Cant create a socket!!!\n";
		return 1;
	}

	// Bind the ip address and port to a socket
	sockaddr_in hint;
	hint.sin_family = AF_INET;
	hint.sin_port = htons(PORT);
	hint.sin_addr.S_un.S_addr = INADDR_ANY; // Binds the socket to all available interface

	bind(listening, (sockaddr*)&hint, sizeof(hint));

	// Tell Winsock the socket is for listening 
	listen(listening, SOMAXCONN);

	fd_set master;
	FD_ZERO(&master);
	FD_SET(listening, &master);

	while (1) 
	{
		fd_set copy = master;
		int count = select(0, &copy, nullptr, nullptr, nullptr);
		for (int i = 0; i < count; i++) 
		{
			SOCKET sock = copy.fd_array[i];
			if (sock == listening) 
			{
				SOCKET client = accept(listening, nullptr, nullptr);
				FD_SET(client, &master);
				std::string welcomeMSG = "Hi";
				send(client, welcomeMSG.c_str(), sizeof(welcomeMSG) + 1, 0);
			}
			else 
			{
				char buffer[4096];
				
				ZeroMemory(buffer, 4096);

				// Wait for client to send data
				int bytesReceived = recv(sock, buffer, 4096, 0);
				if (bytesReceived == SOCKET_ERROR)
				{
					FD_CLR(sock, &master);
					break;
				}

				if (bytesReceived == 0)
				{
					std::cout << "Client has disconnected " << "\n";
					FD_CLR(sock, &master);
				}
				else {
					std::cout << buffer << "\n";
				}
			}
		}
	}






	// Wait for client connection
	//sockaddr_in client;
	//int clientSize = sizeof(client);

	//SOCKET clientSocket = accept(listening, (sockaddr*)&client, &clientSize);

	//char host[NI_MAXHOST];		// NAME
	//char service[NI_MAXSERV];	// PORT

	//ZeroMemory(host, NI_MAXHOST);
	//ZeroMemory(service, NI_MAXSERV);

	//if (getnameinfo((sockaddr*)&client, sizeof(client), host, NI_MAXHOST, service, NI_MAXSERV, 0) == 0)
	//{
	//	std::cout << host << " connected on port " << service << "\n";
	//}
	//else
	//{
	//	inet_ntop(AF_INET, &client.sin_addr, host, NI_MAXHOST);
	//	std::cout << host << " connected on port " << ntohs(client.sin_port) << "\n";
	//}

	//// Close listening socket
	//closesocket(listening);

	//char buffer[4096];
	//while (1)
	//{
	//	ZeroMemory(buffer, 4096);

	//	// Wait for client to send data
	//	int bytesReceived = recv(clientSocket, buffer, 4096, 0);
	//	if (bytesReceived == SOCKET_ERROR)
	//	{
	//		std::cerr << "Error in receiving" << "\n";
	//		break;
	//	}

	//	if (bytesReceived == 0)
	//	{
	//		std::cout << "Client has disconnected " << "\n";
	//		break;
	//	}

	//	std::cout << host << ": " << std::string(buffer, 0, bytesReceived) << "\n";

	//	// Echo message back to client
	//	char message[10] = "may im";
	//	send(clientSocket, message, sizeof(message) + 1, 0);

	//}


	//closesocket(clientSocket);
	WSACleanup();


	return 0;
}
