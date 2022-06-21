#pragma once
#include "Client.h"
#include <msclr\marshal_cppstd.h>
namespace ClientUI {
	
	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;

	/// <summary>
	/// Summary for Form1
	/// </summary>
	///


	public ref class Form1 : public System::Windows::Forms::Form
	{
	public:
		Form1(void)
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~Form1()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::Panel^ menu;
	private: System::Windows::Forms::PictureBox^ pictureArea;

	private: System::Windows::Forms::Panel^ list;
	private: System::Windows::Forms::Button^ buttonMenu;

	private: System::Windows::Forms::ComboBox^ selectMenu;
	
	private: Client* client;
	
	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>
		System::ComponentModel::Container^ components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			WSAData data;
			int wsResult = WSAStartup(MAKEWORD(2, 2), &data);
			client = new Client;
			this->client->connectToServer(6969);
			this->menu = (gcnew System::Windows::Forms::Panel());
			this->selectMenu = (gcnew System::Windows::Forms::ComboBox());
			this->buttonMenu = (gcnew System::Windows::Forms::Button());
			this->pictureArea = (gcnew System::Windows::Forms::PictureBox());
			this->list = (gcnew System::Windows::Forms::Panel());
			this->menu->SuspendLayout();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->pictureArea))->BeginInit();
			this->SuspendLayout();
			// 
			// menu
			// 
			this->menu->Controls->Add(this->selectMenu);
			this->menu->Controls->Add(this->buttonMenu);
			this->menu->Dock = System::Windows::Forms::DockStyle::Right;
			this->menu->Location = System::Drawing::Point(1120, 0);
			this->menu->Name = L"menu";
			this->menu->Size = System::Drawing::Size(230, 729);
			this->menu->TabIndex = 0;
			// 
			// selectMenu
			// 
			this->selectMenu->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->selectMenu->FormattingEnabled = true;
			this->selectMenu->Items->AddRange(gcnew cli::array< System::Object^  >(20) {
				L"Com Ga`", L"Chi.", L"87977466", L"fdafsadf",
					L"fdsafsdafs", L"nvbmghfjg", L"pghsdgwez", L"qewr", L"r", L"ryughjghnv", L"sd", L"tyrutr", L"v", L"vnmgjrtr", L"vxcv", L"wq",
					L"wqoczmv", L"x", L"xc", L"xcv"
			});
			this->selectMenu->Location = System::Drawing::Point(0, 97);
			this->selectMenu->Name = L"selectMenu";
			this->selectMenu->Size = System::Drawing::Size(230, 33);
			this->selectMenu->Sorted = true;
			this->selectMenu->TabIndex = 0;
			this->selectMenu->SelectedIndexChanged += gcnew System::EventHandler(this, &Form1::selectDish);
			// 
			// buttonMenu
			// 
			this->buttonMenu->Location = System::Drawing::Point(0, 59);
			this->buttonMenu->Name = L"buttonMenu";
			this->buttonMenu->Size = System::Drawing::Size(230, 43);
			this->buttonMenu->TabIndex = 1;
			this->buttonMenu->Text = L"MENU";
			this->buttonMenu->UseVisualStyleBackColor = true;
			this->buttonMenu->Click += gcnew System::EventHandler(this, &Form1::showMenuList);
			// 
			// pictureArea
			// 
			this->pictureArea->Location = System::Drawing::Point(0, 0);
			this->pictureArea->Name = L"pictureArea";
			this->pictureArea->Size = System::Drawing::Size(857, 729);
			this->pictureArea->TabIndex = 1;
			this->pictureArea->TabStop = false;
			// 
			// list
			// 
			this->list->Location = System::Drawing::Point(855, 0);
			this->list->Name = L"list";
			this->list->Size = System::Drawing::Size(270, 729);
			this->list->TabIndex = 1;
			// 
			// Form1
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(1350, 729);
			this->Controls->Add(this->pictureArea);
			this->Controls->Add(this->menu);
			this->Controls->Add(this->list);
			this->Name = L"Form1";
			this->Text = L"Form1";
			this->menu->ResumeLayout(false);
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->pictureArea))->EndInit();
			this->ResumeLayout(false);
			
		}
#pragma endregion
	private: System::Void listBox1_SelectedIndexChanged(System::Object^ sender, System::EventArgs^ e) 
	{

	}
	private: System::Void showMenuList(System::Object^ sender, System::EventArgs^ e) 
	{
		selectMenu->Visible = 1 - selectMenu->Visible;
	}
	private: System::Void selectDish(System::Object^ sender, System::EventArgs^ e) 
	{
		MessageBox::Show("Selecting " + selectMenu->SelectedItem->ToString());
		std::string unmanaged = msclr::interop::marshal_as<std::string>(selectMenu->SelectedItem->ToString());
		client->sendMessage(unmanaged.c_str());
	}
	
};
}
