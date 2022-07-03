
using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace ClientUI
{

    public partial class MainWindow : Window
    {
        Client.Client client;
 
        public MainWindow()
        {

            InitializeComponent();  //Vay thi chay bth
            client = new Client.Client();

            //client.recvPic();


            /* InitializeComponent();  //Vay thi chay sai
             client.recvByte();*/

            //comment lai cho de^~ sua?
        }

        private void ButtonFechar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }



        private void MoveCursorMenu(int index)
        {
            TrainsitionigContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, (100 + (60 * index)), 0, 0);
        }

      
        private void chooseDishes(object sender, RoutedEventArgs e)
        {
            
            TextBlock thisTextBlock = sender as TextBlock;
      
            client.sendRequest(thisTextBlock.Name);

            menuArea.Children.Clear();
            Image dish = new Image();
            
            BitmapImage bi = new BitmapImage();
            client.recvPic(ref bi); //receive image
            dish.Height = 550;
            dish.Width = 1060;
            dish.Stretch = Stretch.Fill;
           
            dish.Source = bi;
           
            DockPanel.SetDock(dish, Dock.Top);
            TextBlock des = new TextBlock();
            des.Height = 120;
            des.Width = 1060;
            des.Text = client.recvDescription(); //reveice desceripniton;
         
            menuArea.Children.Add(dish);
            menuArea.Children.Add(des);

            
            /*ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("./a.jpg"));
            menuArea.Background = imageBrush;*/
        }

        private Thickness chooseThickness(int mode, int x)
        {
            switch (mode)
            {
                case 1:
                    {
                        switch (x)
                        {
                            case 1:
                                return (new Thickness(57, 60, 0, 30));
                            case 2:
                                return (new Thickness(80, 150, 0, 30));
                            default:
                                return (new Thickness(120, 230, 0, 30));

                        }
                        break;
                    }
                case 2:
                    {
                        switch (x)
                        {
                            case 1:
                                return (new Thickness(47, 230, 0, 30));
                            case 2:
                                return (new Thickness(97, 140, 0, 30));
                            default:
                                return (new Thickness(70, 60, 0, 30));

                        }
                        break;
                    }
                case 3:
                    {
                        switch (x)
                        {
                            case 1:
                                return (new Thickness(120, 105, 0, 30));
                            case 2:
                                return (new Thickness(125, 270, 0, 30));
                            default:
                                return (new Thickness(90, 168, 0, 30));

                        }
                        break;
                    }
                case 4:
                    {
                        switch (x)
                        {
                            case 1:
                                return (new Thickness(110, 45, 0, 30));
                            case 2:
                                return (new Thickness(80, 130, 0, 30));
                            default:
                                return (new Thickness(95, 215, 0, 30));

                        }
                        break;
                    }
                default:
                    return new Thickness(0, 0, 0, 0);
            }

        }

        private void mainCourseMenu()
        {
            //Receive image
            BitmapImage bi = new BitmapImage();
            client.recvPic(ref bi);

         
            try
            {
               
                var imageBrush = new ImageBrush { ImageSource = bi };
                menuArea.Background = imageBrush;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // GC.Collect();
            
            var c = 0;
            var i = 1;
            var foodNum = 0;
            for (; i <= 3; ++i)
            {
                var stackPanel = new StackPanel();
                stackPanel.Name = "content" + i + "Stack";
                stackPanel.Width = 353.33;
                stackPanel.Height = 670;
                menuArea.Children.Add(stackPanel);
            }
            
            int[] numberFood = new int[] { 8, 8, 6 };
            i = 0;
            List<string> a = client.recvMenu();
            
            foreach (object child in menuArea.Children)
            {
                if (child is StackPanel)
                {
                    // Get menu from server
                    ++i;
                    var label = new Label();
                    label.Name = "content" + i + "Label";
                    label.HorizontalAlignment = HorizontalAlignment.Left;
                    label.Content = a[c++];
                    label.Margin = chooseThickness(1, i);
                    label.FontFamily = new FontFamily("SVN-Bali Script");
                    label.FontSize = 40;
                    (child as StackPanel).Children.Add(label);
                    for (int j = 0; j < numberFood[i - 1]; ++j)
                    {
                        var textBlock = new TextBlock();
                        textBlock.Text = a[c++];
                        textBlock.MouseDown += chooseDishes;
                        textBlock.Name = "_1_" + (++foodNum).ToString();
                        textBlock.Margin = new Thickness(40, 7, 0, 0);
                        textBlock.FontSize = 18;
                        textBlock.Cursor = Cursors.Hand;
                        textBlock.FontFamily = new FontFamily("Comic Sans MS");
                        textBlock.Style = (Style)Resources["changeColor"];
                        (child as StackPanel).Children.Add(textBlock);
                    }
                }

            }
           
        }
        private void soupMenu()
        {
            //Receive image

            BitmapImage bi = new BitmapImage();
            client.recvPic(ref bi);


            try
            {
                var imageBrush = new ImageBrush { ImageSource = bi };
                menuArea.Background = imageBrush;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            var foodNum = 0;
            var c = 0;
            var i = 1;
            for (; i <= 3; ++i)
            {
                var stackPanel = new StackPanel();
                stackPanel.Name = "content" + i + "Stack";
                stackPanel.Width = 353.33;
                stackPanel.Height = 670;
                menuArea.Children.Add(stackPanel);
            }

            int[] numberFood = new int[] { 6, 8, 8 };
            i = 0;
            List<string> a = client.recvMenu();
            foreach (object child in menuArea.Children)
            {
                if (child is StackPanel)
                {

                    ++i;
                    var label = new Label();
                    label.Name = "content" + i + "Label";
                    label.HorizontalAlignment = HorizontalAlignment.Left;
                    label.Content = a[c++];
                    label.Margin = chooseThickness(2, i);
                    label.FontFamily = new FontFamily("SVN-Bali Script");
                    label.FontSize = 57;
                    (child as StackPanel).Children.Add(label);

                    for (int j = 0; j < numberFood[i - 1]; ++j)
                    {
                        var textBlock = new TextBlock();
                        textBlock.Text = a[c++];
                        textBlock.MouseDown += chooseDishes;
                        try
                        {
                            textBlock.Name = "_2_" + (++foodNum).ToString();
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        textBlock.Margin = new Thickness(40, 7, 0, 0);
                        textBlock.FontSize = 18;
                        textBlock.Cursor = Cursors.Hand;
                        textBlock.FontFamily = new FontFamily("Comic Sans MS");
                        textBlock.Style = (Style)Resources["changeColor"];

                        (child as StackPanel).Children.Add(textBlock);
                    }
                }

            }
        }
        private void dessertMenu()
        {
            //Receive image
            BitmapImage bi = new BitmapImage();
            client.recvPic(ref bi);

            try
            {
                var imageBrush = new ImageBrush { ImageSource = bi };
                menuArea.Background = imageBrush;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //Reveive List of menu
            //List<string> a = client.recvMenu();
            var foodNum = 0;
            var c = 0;
            var i = 1;
            for (; i <= 3; ++i)
            {
                var stackPanel = new StackPanel();
                stackPanel.Name = "content" + i + "Stack";
                stackPanel.Width = 353.33;
                stackPanel.Height = 670;
                menuArea.Children.Add(stackPanel);
            }

            i = 0;
            int[] numberFood = new int[] { 8, 6, 8 };
            List<string> a = client.recvMenu();

            foreach (object child in menuArea.Children)
            {
                if (child is StackPanel)
                {
                    ++i;
                    var label = new Label();
                    label.Name = "content" + i + "Label";
                    label.HorizontalAlignment = HorizontalAlignment.Left;
                    label.Content = a[c++];
                    label.Margin = chooseThickness(3, i);
                    label.FontFamily = new FontFamily("SVN-Bali Script");
                    label.FontSize = 57;
                    (child as StackPanel).Children.Add(label);
                    for (int j = 0; j < numberFood[i - 1]; ++j)
                    {
                        var textBlock = new TextBlock();
                        textBlock.Text = a[c++];
                        textBlock.MouseDown += chooseDishes;
                        textBlock.Name = "_3_" + (++foodNum).ToString();
                        textBlock.Margin = new Thickness(40, 7, 0, 0);
                        textBlock.FontSize = 18;
                        textBlock.Cursor = Cursors.Hand;
                        textBlock.FontFamily = new FontFamily("Comic Sans MS");
                        textBlock.Style = (Style)Resources["changeColor"];
                        (child as StackPanel).Children.Add(textBlock);
                    }
                }

            }
        }
        private void drinkMenu()
        {
            //Receive image
            BitmapImage bi = new BitmapImage();
            client.recvPic(ref bi);


            try
            {
                var imageBrush = new ImageBrush { ImageSource = bi };
                menuArea.Background = imageBrush;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            var foodNum = 0;
            var c = 0;
            var i = 1;
            for (; i <= 3; ++i)
            {
                var stackPanel = new StackPanel();
                stackPanel.Name = "content" + i + "Stack";
                stackPanel.Width = 353.33;
                stackPanel.Height = 670;
                menuArea.Children.Add(stackPanel);
            }

            i = 0;
            int[] numberFood = new int[] { 9, 7, 6 };
            List<string> a = client.recvMenu();

            foreach (object child in menuArea.Children)
            {
                if (child is StackPanel)
                {

                    var label = new Label();
                    label.Name = "content" + ++i + "Label";
                    label.HorizontalAlignment = HorizontalAlignment.Left;
                    label.Content = a[c++];
                    label.Margin = chooseThickness(4, i);
                    label.FontFamily = new FontFamily("SVN-Bali Script");
                    label.FontSize = 57;
                    (child as StackPanel).Children.Add(label);
                    for (int j = 0; j < numberFood[i - 1]; ++j)
                    {
                        var textBlock = new TextBlock();
                        textBlock.Text = a[c++];
                        textBlock.MouseDown += chooseDishes;
                        textBlock.Name = "_4_" + (++foodNum).ToString();
                        textBlock.Margin = new Thickness(40, 7, 0, 0);
                        textBlock.FontSize = 18;
                        textBlock.Cursor = Cursors.Hand;
                        textBlock.FontFamily = new FontFamily("Comic Sans MS");
                        textBlock.Style = (Style)Resources["changeColor"];
                        (child as StackPanel).Children.Add(textBlock);
                    }
                }

            }
        }

        private void chooseMenuLayout(int x)
        {
            switch (x)
            {
                case -1:
                    {
                        break;
                    }
                case 1:
                    {
                        mainCourseMenu();
                        break;
                    }
                case 2:
                    {
                        soupMenu();
                        break;
                    }
                case 3:
                    {
                        dessertMenu();
                        break;
                    }
                case 4:
                    {
                        drinkMenu();
                        break;
                    }
            }
        }
        private void drawMenu()
        {
            menuArea.Children.Clear();
            int mode = 1;
            chooseMenuLayout(1);

        }

        private void selectMenu(object sender, RoutedEventArgs e)
        {
            menuArea.Children.Clear();


            int index = 1 + listViewMenu.SelectedIndex;
            MoveCursorMenu(index - 1);

            if (index == 0)
            {
                //send Welcome
            }
            else if (index == 5)
            {
                //send List
            }
            else
            {
                //send menu(index)
                client.sendRequest(index, 0);
                chooseMenuLayout(index);
            }

        }

        private void showMyList()
        {
            //Receive image
            BitmapImage bi = new BitmapImage();
            client.recvPic(ref bi);


            try
            {
                var imageBrush = new ImageBrush { ImageSource = bi };
                menuArea.Background = imageBrush;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //Receive List
            List<string> a = client.recvList();

            var c = 0;
            var i = 0;

            var stackPanel = new StackPanel();
            stackPanel.Name = "myListStack";
            menuArea.Children.Add(stackPanel);


            foreach (object child in menuArea.Children)
            {
                if (child is StackPanel)
                {
                    for (int j = 0; j < a.Count; ++j)
                    {
                        DockPanel dockPanel = new DockPanel();


                        var textBlock = new TextBlock();
                        textBlock.Text = a[++c];
                        textBlock.Tag = "dish" + i.ToString();
                        textBlock.Margin = new Thickness(40, 7, 0, 0);
                        textBlock.Width = 400;
                        textBlock.Height = 50;
                        textBlock.FontSize = 18;
                        textBlock.Cursor = Cursors.Hand;
                        textBlock.FontFamily = new FontFamily("Comic Sans MS");
                        textBlock.Style = (Style)Resources["changeColor"];
                        DockPanel.SetDock(textBlock, Dock.Left);

                        Button buttonPlus = new Button();
                        buttonPlus.Tag = "";
                        buttonPlus.Click += client.add;

                        var numberDish = new TextBlock();
                        //numberDish.Text = numberOfDish;

                        Button buttonMinus = new Button();
                        buttonMinus.Tag = "";
                        buttonPlus.Click += client.remove;

                        dockPanel.Children.Add(textBlock);
                        dockPanel.Children.Add(buttonPlus);
                        dockPanel.Children.Add(buttonMinus);

                        (child as StackPanel).Children.Add(dockPanel);

                    }
                }

            }



        }
    }
}

