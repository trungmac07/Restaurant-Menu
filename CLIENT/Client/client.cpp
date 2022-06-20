#include <iostream>
#include <string>

#include <WS2tcpip.h>

#define PORT 6969

#pragma comment(lib, "ws2_32.lib")

class Client
{
private:
	SOCKET sock;
	sockaddr_in hint;
	std::string ipAddress = "127.0.0.1";
	char buffer[4096];
public:
	Client()
	{
		sock = socket(AF_INET, SOCK_STREAM, 0);
		ZeroMemory(buffer, 4096);
	}
	~Client()
	{
		closesocket(sock);
	}

	bool connectToServer(int port)
	{
		hint.sin_family = AF_INET;
		hint.sin_port = htons(port);
		inet_pton(AF_INET, ipAddress.c_str(), &hint.sin_addr);
		int connectionResult = connect(sock, (sockaddr*)&hint, sizeof(hint));
		if (connectionResult == SOCKET_ERROR)
		{
			std::cerr << "Cant connect to server\n";
			closesocket(sock);
			return 0;
		}
		return 1;
	}

	void sendMessage(const char message[])
	{
		int sendResult = send(sock, message, strlen(message) + 1, 0);
	}

	bool receiveMessage()
	{
		ZeroMemory(buffer, 4096);
		int bytesRecv = recv(sock, buffer, 4096, 0);
		if (bytesRecv == 0)
			return 0;
		return 1;
	}

};


int main()
{

	// Initialize WinSock
	WSAData data;
	int wsResult = WSAStartup(MAKEWORD(2, 2), &data);

	if (wsResult != 0)
	{
		std::cerr << "Cant initialize winsock!!!\n";
		return 1;
	}


	// Create socket
	Client a;
	a.connectToServer(6969);
	a.sendMessage("hahaha");
	a.receiveMessage();


	WSACleanup();

	return 0;
}
