#pragma once

#include <iostream>
#include "pch.h"

using namespace System;

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
			System::Windows::Forms::MessageBox::Show("Can't connect");
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
