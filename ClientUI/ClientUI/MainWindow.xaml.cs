
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
using System.Windows.Media.Animation;
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

        private Storyboard myStoryboard, desStoryboard;
        private void chooseDishes(object sender, RoutedEventArgs e)
        {

            foreach (var child in menuArea.Children)
            {
                if (child is StackPanel)
                    (child as StackPanel).Visibility = Visibility.Collapsed;
            }

            TextBlock thisTextBlock = sender as TextBlock;

            client.sendRequest(thisTextBlock.Name);


            BitmapImage bi = new BitmapImage();
            client.recvPic(ref bi); //receive image

            //Mother border loves her children like your mom <3 ;
            Border mother = new Border();
            mother.Background = new SolidColorBrush(Colors.Pink);
            mother.Width = 1060;
            mother.Height = 550;
            mother.Name = "mother";
            this.RegisterName(mother.Name, mother);
            DockPanel.SetDock(mother, Dock.Top);

            //Border Appear Animation
            Border border = new Border();
            border.Name = "dishImage";
            this.RegisterName(border.Name, border);
            border.Background = new ImageBrush(bi);
            border.CornerRadius = new CornerRadius(60);
            border.BorderThickness = new Thickness(7, 7, 7, 7);
            border.BorderBrush = Brushes.Black;
            border.Width = 1060;
            border.Height = 550;

            var hAnimation = new DoubleAnimation();
            hAnimation.From = 0;
            hAnimation.To = 550;
            hAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            var wAnimation = new DoubleAnimation();
            wAnimation.From = 0;
            wAnimation.To = 1060;
            wAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            myStoryboard = new Storyboard();
            myStoryboard.Children.Add(hAnimation);
            myStoryboard.Children.Add(wAnimation);
            Storyboard.SetTargetName(wAnimation, border.Name);
            Storyboard.SetTargetName(hAnimation, border.Name);
            Storyboard.SetTargetProperty(wAnimation, new PropertyPath(Border.WidthProperty));
            Storyboard.SetTargetProperty(hAnimation, new PropertyPath(Border.HeightProperty));
            border.Loaded += showDish;
            DockPanel.SetDock(border, Dock.Top);
            mother.Child = border;
            menuArea.Children.Add(mother);


            //description area 
            DockPanel desArea = new DockPanel();
            desArea.Name = "desArea";
            this.RegisterName(desArea.Name, desArea);
            desArea.Height = 120;
            desArea.Width = 1060;
            desArea.Background = new SolidColorBrush(Colors.Pink);
            DockPanel.SetDock(desArea, Dock.Bottom);


            //back button
            Button back = new Button();
            back.Content = "Back";
            back.Height = 50;
            back.Width = 100;
            back.FontSize = 37;
            back.FontFamily = new FontFamily("SVN-Bali Script");
            back.Background = new SolidColorBrush(Colors.Gold);
            back.Click += backToMenu;
            DockPanel.SetDock(back, Dock.Bottom);
            
            //addfood button
            Button addfood = new Button();
            addfood.Content = "Add Food\n\n" + (sender as TextBlock).Text;
            addfood.Height = 50;
            addfood.Width = 100;
            addfood.FontSize = 22;
            addfood.FontFamily = new FontFamily("SVN-Bali Script");
            addfood.Background = new SolidColorBrush(Colors.Gold);
            addfood.Click += addFoodToCart;
            DockPanel.SetDock(addfood, Dock.Top);

            //Button Area
            DockPanel buttonArea = new DockPanel();
            buttonArea.Height = 100;
            buttonArea.Width = 100;
            buttonArea.Children.Add(back);
            buttonArea.Children.Add(addfood);

            //Description appear animation
            TextBlock des = new TextBlock();
            des.Name = "dishDes";
            this.RegisterName(des.Name, des);
            des.Height = 120;
            des.Width = 940;
            des.Text = client.recvDescription(); //reveice desceripniton;
            des.Opacity = 0;
            des.Background = new SolidColorBrush(Colors.Pink);
            des.FontSize = 23;


            var oAnimation = new DoubleAnimation();
            oAnimation.From = 0;
            oAnimation.To = 1;
            oAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            oAnimation.BeginTime = TimeSpan.FromSeconds(0.5);

            desStoryboard = new Storyboard();
            desStoryboard.Children.Add(oAnimation);
            Storyboard.SetTargetName(oAnimation, des.Name);
            Storyboard.SetTargetProperty(oAnimation, new PropertyPath(Border.OpacityProperty));
            des.Loaded += showDes;

            desArea.Children.Add(des);
            desArea.Children.Add(buttonArea);


            menuArea.Children.Add(desArea);


            /*ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("./a.jpg"));
            menuArea.Background = imageBrush;*/
        }
        private void showDish(object sender, RoutedEventArgs e)
        {
            myStoryboard.Begin(this);
            this.UnregisterName("dishImage");
        }
        private void showDes(object sender, RoutedEventArgs e)
        {
            desStoryboard.Begin(this);
            this.UnregisterName("dishDes");
        }
        private void backToMenu(object sender, RoutedEventArgs e)
        {
            var find = menuArea.FindName("desArea");
            if (find != null)
            {
                menuArea.Children.Remove(find as DockPanel);
                this.UnregisterName("desArea");
                menuArea.Children.Remove(menuArea.FindName("mother") as Border);
                this.UnregisterName("mother");
            }

            //menuArea.Children.Clear();
            foreach (var child in menuArea.Children)
            {
                if (child is StackPanel)
                    (child as StackPanel).Visibility = Visibility.Visible;
            }

        }

        private void addFoodToCart(object sender, RoutedEventArgs e)
        {
            var x = (sender as Button).Content;
            string str = x.ToString();
           

            string[] word = str.Split("\n\n");
            //MessageBox.Show(word[1]);
            
            str = word[1];
            //MessageBox.Show("123" + str);
            int found = str.IndexOf('.');
            string foodname = str.Substring(0, found);
            int index = str.Length - 1;
            //MessageBox.Show(index.ToString());
            for (; index >= 0; index--)
            {
                if (str[index] == '.')
                    break;
            }
            string price = str.Substring(index + 1, str.Length - index - 1);
            Client.DISH dish = new Client.DISH();
            dish.name = foodname;
            dish.price = int.Parse(price);
            
            client.putinCart(dish);
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
                    label.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#SVN-Bali Script");

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
                    label.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#SVN-Bali Script");
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
                        catch (Exception ex)
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
                    label.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#SVN-Bali Script");
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
                    label.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#SVN-Bali Script");
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
            backToMenu(null, null);
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
                client.requestOrder();

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

