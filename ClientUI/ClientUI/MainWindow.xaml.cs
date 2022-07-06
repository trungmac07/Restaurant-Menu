
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
        bool haveBill ;
        public MainWindow()
        {

            InitializeComponent();  //Vay thi chay bth
            client = new Client.Client();
            client.order = new Client.ORDER();
            //client.recvPic();
            haveBill = false;

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
            //TrainsitionigContentSlide.OnApplyTemplate();
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
            back.Foreground = new SolidColorBrush(Colors.Black);
            back.Height = 45;
            back.Width = 100;
            back.FontSize = 35;
            back.Margin = new Thickness(0, 0, 0, 10);
            back.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#SVN-Bali Script");
            back.Background = new SolidColorBrush(Colors.Gold);
            back.Click += backToMenu;
            DockPanel.SetDock(back, Dock.Bottom);

            //addfood button
            Button addfood = new Button();
            addfood.HorizontalContentAlignment = HorizontalAlignment.Center;
            addfood.Content = " Add Food\n\n" + (sender as TextBlock).Text;
            addfood.Foreground = new SolidColorBrush(Colors.Black);
            addfood.Height = 45;
            addfood.Width = 100;
            addfood.FontSize = 30;
            addfood.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#SVN-Bali Script");
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
            des.TextWrapping = TextWrapping.Wrap;
            des.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Comic Sans MS");

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
                        textBlock.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Comic Sans MS");
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
                        textBlock.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Comic Sans MS");
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
                        textBlock.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Comic Sans MS");
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
                        textBlock.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Comic Sans MS");
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
                //client.requestOrder();
                showMyList();
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
            menuArea.Children.Clear();
            //Receive image
            /* BitmapImage bi = new BitmapImage();
             client.recvPic(ref bi);*/

            Border border = new Border();
            border.Background = new SolidColorBrush(Colors.White);
            border.CornerRadius = new CornerRadius(45, 45, 0, 0);

            ScrollViewer scrollViewer = new ScrollViewer();

            StackPanel stackPanel = new StackPanel();

            DockPanel dockPanel = new DockPanel();
            dockPanel.Margin = new Thickness(50, 50, 0, 0);

            TextBlock name = new TextBlock();
            name.Height = 25;
            name.Width = 350;
            name.FontSize = 21;
            name.Text = "DISH";

            TextBlock price = new TextBlock();
            price.Height = 25;
            price.Width = 250;
            price.FontSize = 21;
            price.Text = "PRICE";

            Button buttonPlus = new Button();
            buttonPlus.Height = 25;
            buttonPlus.Width = 25;
            buttonPlus.Content = "+";
            buttonPlus.FontSize = 21;

            TextBlock num = new TextBlock();
            num.Height = 25;
            num.Width = 150;
            num.FontSize = 21;
            num.Text = "AMOUNT";
            num.TextAlignment = TextAlignment.Left;
            DockPanel.SetDock(num, Dock.Left);

            Button buttonMinus = new Button();
            buttonMinus.Height = 25;
            buttonMinus.Width = 25;
            buttonMinus.Content = "-";
            buttonMinus.FontSize = 21;

            Button trick = new Button();
            trick.Height = 3;
            trick.Width = 1060;
            trick.Background = new SolidColorBrush(Colors.Black);
            trick.BorderThickness = new Thickness(0, 0, 0, 0);

            var style = new Style
            {
                TargetType = typeof(Border),
                Setters = { new Setter { Property = Border.CornerRadiusProperty, Value = new CornerRadius(7) } }
            };
            Button x = new Button();
            x.Height = 50;
            x.Width = 100;
            x.Content = "ORDER";
            x.FontSize = 21;
            x.Background = new SolidColorBrush(Colors.Gold);
            x.Click += sendOrderRequest;
            x.Resources.Add(style.TargetType, style);

            Button y = new Button();
            y.Height = 50;
            y.Width = 100;
            y.Content = "BILL";
            y.FontSize = 21;
            y.Background = new SolidColorBrush(Colors.Gold);
            y.Click += bill;
            y.Resources.Add(style.TargetType, style);

            dockPanel.Children.Add(name);
            dockPanel.Children.Add(price);
            dockPanel.Children.Add(num);
            dockPanel.Children.Add(x);
            dockPanel.Children.Add(y);

            border.Child = scrollViewer;

            scrollViewer.Content = stackPanel;

            stackPanel.Children.Add(dockPanel);
            stackPanel.Children.Add(trick);

            menuArea.Children.Add(border);


            foreach (var dish in client.dic)
            {
                string dishName = dish.Key.Key;
                int dishPrice = dish.Key.Value;
                int dishNum = dish.Value;

                dockPanel = new DockPanel();
                dockPanel.Margin = new Thickness(50, 35, 0, 0);

                name = new TextBlock();
                name.Height = 25;
                name.Width = 350;
                name.FontSize = 21;
                name.Text = dishName;

                price = new TextBlock();
                price.Height = 25;
                price.Width = 250;
                price.FontSize = 21;
                price.Text = dishPrice.ToString();

                buttonPlus = new Button();
                buttonPlus.Height = 25;
                buttonPlus.Width = 25;
                buttonPlus.Content = "+";
                buttonPlus.FontSize = 21;
                buttonPlus.Tag = dishName + " " + dishPrice;
                buttonPlus.Click += addDish;

                num = new TextBlock();
                num.Height = 25;
                num.Width = 25;
                num.FontSize = 21;
                num.Text = dishNum.ToString();
                num.TextAlignment = TextAlignment.Center;

                buttonMinus = new Button();
                buttonMinus.Height = 25;
                buttonMinus.Width = 25;
                buttonMinus.Content = "-";
                buttonMinus.FontSize = 21;
                buttonMinus.Tag = dishName + " " + dishPrice;
                buttonMinus.Click += removeDish;

                x = new Button();
                x.Height = 25;
                x.Width = 25;
                x.Content = "X";
                x.FontSize = 21;
                x.Click += removeAllDish;
                x.Tag = dishName + " " + dishPrice;

                dockPanel.Children.Add(name);
                dockPanel.Children.Add(price);
                dockPanel.Children.Add(buttonMinus);
                dockPanel.Children.Add(num);
                dockPanel.Children.Add(buttonPlus);
                dockPanel.Children.Add(x);

                stackPanel.Children.Add(dockPanel);

            }

        }

        void addDish(object sender, RoutedEventArgs e)
        {
            string name = (sender as Button).Tag as string;

            int found = 0;
            for (int i = name.Length - 1; i >= 0; --i)
                if (name[i] == ' ')
                {
                    found = i;
                    break;
                }
            string price = name.Substring(found + 1);
            name = name.Remove(found, name.Length - found);
            int num = Int32.Parse(price);
            KeyValuePair<string, int> pair = new KeyValuePair<string, int>(name, num);
            ++client.dic[pair];
            showMyList();
        }

        void removeDish(object sender, RoutedEventArgs e)
        {
            string name = (sender as Button).Tag as string;

            int found = 0;
            for (int i = name.Length - 1; i >= 0; --i)
                if (name[i] == ' ')
                {
                    found = i;
                    break;
                }
            string price = name.Substring(found + 1);
            name = name.Remove(found, name.Length - found);
            int num = Int32.Parse(price);
            KeyValuePair<string, int> pair = new KeyValuePair<string, int>(name, num);
            --client.dic[pair];
            if (client.dic[pair] == 0)
                client.dic.Remove(pair);

            showMyList();
        }

        void removeAllDish(object sender, RoutedEventArgs e)
        {
            string name = (sender as Button).Tag as string;

            int found = 0;
            for (int i = name.Length - 1; i >= 0; --i)
                if (name[i] == ' ')
                {
                    found = i;
                    break;
                }
            string price = name.Substring(found + 1);
            name = name.Remove(found, name.Length - found);
            int num = Int32.Parse(price);
            KeyValuePair<string, int> pair = new KeyValuePair<string, int>(name, num);
            client.dic.Remove(pair);
            showMyList();
        }

        
        void sendOrderRequest(object sender, RoutedEventArgs e)
        {
            if (client.dic.Count == 0)
            {
                MessageBox.Show("Please choose some dished first.");
                return;
            }
                
            //Ha`
            client.requestOrder();
            
            client.recvBill();
           
            haveBill = true;
            bill(null,null);
            client.dic.Clear();

        }

        void bill(object sender, RoutedEventArgs e)
        {
            if(sender == null)
            {
                listViewMenu.Visibility = Visibility.Collapsed;
            }
            else
            {
                if(haveBill == false)
                {
                    MessageBox.Show("You have no bill, please order something");
                    return;
                }
            }
            menuArea.Background = new SolidColorBrush(Colors.Snow);
            //UI BILL
            menuArea.Children.Clear();

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Width = 530;
            scrollViewer.Background = new SolidColorBrush(Colors.Snow);

            StackPanel stackPanel = new StackPanel();

            bool pink = false;
            foreach (var dish in client.order.dishOrder)
            {

                StackPanel info = new StackPanel();

                if (pink)
                    info.Background = new SolidColorBrush(Colors.Pink);

                pink = true ^ pink;

                TextBlock name = new TextBlock();
                name.Margin = new Thickness(35, 20, 0, 0);
                name.Height = 25;
                name.Width = 350;
                name.FontSize = 21;
                name.Text = dish.dish.name;
                name.HorizontalAlignment = HorizontalAlignment.Left;

                TextBlock price = new TextBlock();
                price.Margin = new Thickness(35, 0, 0, 0);
                price.Height = 25;
                price.Width = 350;
                price.FontSize = 17;
                price.Text = "Price: " + dish.dish.price.ToString();
                price.HorizontalAlignment = HorizontalAlignment.Left;

                DockPanel totalPanel = new DockPanel();
                totalPanel.Margin = new Thickness(0, 0, 0, 0);

                TextBlock amount = new TextBlock();
                amount.Margin = new Thickness(35, 0, 0, 20);
                amount.Height = 25;
                amount.Width = 300;
                amount.FontSize = 17;
                amount.Text = "Amount: " + dish.numberOfDishes.ToString();

                TextBlock total = new TextBlock();
                total.Margin = new Thickness(0, 0, 0, 20);
                total.Height = 25;
                total.Width = 300;
                total.FontSize = 17;
                total.Text = dish.totalMoney.ToString();

                totalPanel.Children.Add(amount);
                totalPanel.Children.Add(total);

                info.Children.Add(name);
                info.Children.Add(price);
                info.Children.Add(totalPanel);

                stackPanel.Children.Add(info);
            }

            scrollViewer.Content = stackPanel;

            menuArea.Children.Add(scrollViewer);

            StackPanel stackPanel1 = new StackPanel();

            TextBlock restaurantName = new TextBlock();
            restaurantName.Margin = new Thickness(0, 20, 0, 10);
            restaurantName.Height = 57;
            restaurantName.Width = 475;
            restaurantName.FontSize = 47;
            restaurantName.TextAlignment = TextAlignment.Center;
            restaurantName.Text = "RESTAURANT NAME";
            restaurantName.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#SVN-Bali Script");

            TextBlock bill = new TextBlock();
            bill.Height = 50;
            bill.Margin = new Thickness(0, 0, 0, 10);
            bill.Width = 475;
            bill.FontSize = 37;
            bill.TextAlignment = TextAlignment.Center;
            bill.Text = "~~BILL~~";
            bill.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#SVN-Bali Script");

            TextBlock date = new TextBlock();
            date.Height = 25;
            date.Width = 475;
            date.FontSize = 17;
            date.TextAlignment = TextAlignment.Center;
            date.Text = client.order.dateTime;

            Button line = new Button();
            line.Margin = new Thickness(0, 30, 0, 0);
            line.Width = 1060;
            line.Height = 3;
            line.Background = new SolidColorBrush(Colors.Black);
            line.BorderThickness = new Thickness(0, 0, 0, 0);

            TextBlock totalBill = new TextBlock();
            totalBill.Margin = new Thickness(130, 40, 0, 10);
            totalBill.Width = 350;
            totalBill.Height = 25;
            totalBill.FontSize = 21;
            totalBill.TextAlignment = TextAlignment.Right;
            totalBill.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#comic sans ms");
            totalBill.FontWeight = FontWeights.Bold;
            totalBill.Text = "TOTAL: " + client.order.totalMoney.ToString() + " VND";

            Button line2 = new Button();
            line2.Margin = new Thickness(0, 0, 0, 0);
            line2.Width = 1060;
            line2.Height = 3;
            line2.Background = new SolidColorBrush(Colors.Black);
            line2.BorderThickness = new Thickness(0, 0, 0, 0);

            Label payment = new Label();
            payment.Margin = new Thickness(0, 40, 0, 0);
            payment.Width = 300;
            payment.FontSize = 40;
            payment.HorizontalContentAlignment = HorizontalAlignment.Center;
            payment.Content = "PAYMENT";
            payment.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#comic sans ms");

            Button cash = new Button();
            cash.Background = new SolidColorBrush(Colors.Pink);
            cash.Margin = new Thickness(25, 25, 25, 25);
            cash.Width = 200;
            cash.Height = 50;
            cash.Content = "CASH";
            cash.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#comic sans ms");
            cash.FontSize = 27;
            cash.BorderThickness = new Thickness(0, 0, 0, 0);
            cash.Click += cashPay;

            Button bank = new Button();
            bank.Background = new SolidColorBrush(Colors.Pink);
            bank.Margin = new Thickness(25, 0, 25, 0);
            bank.Width = 200;
            bank.Height = 50;
            bank.Content = "BANK";
            bank.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#comic sans ms");
            bank.FontSize = 27;
            bank.BorderThickness = new Thickness(0, 0, 0, 0);
            bank.Click += banking;
            var style = new Style
            {
                TargetType = typeof(Border),
                Setters = { new Setter { Property = Border.CornerRadiusProperty, Value = new CornerRadius(15) } }
            };

            cash.Resources.Add(style.TargetType, style);
            bank.Resources.Add(style.TargetType, style);

            if(sender != null)
            {
                payment.Visibility = Visibility.Hidden;
                cash.Visibility = Visibility.Hidden;
                bank.Visibility = Visibility.Hidden;
            }

            TextBox bankId = new TextBox();


            bankId.Name = "stk";
            bankId.Background = new SolidColorBrush(Colors.Pink);
            bankId.Width = 350;
            bankId.Height = 50;
            bankId.HorizontalAlignment = HorizontalAlignment.Center;
            bankId.Margin = new Thickness(0, 15, 0, 0);
            bankId.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#comic sans ms");
            bankId.Resources.Add(style.TargetType, style);
            bankId.FontSize = 27;
            bankId.TextAlignment = TextAlignment.Center;
            bankId.Visibility = Visibility.Collapsed;
            bankId.KeyDown += new KeyEventHandler(typeBankingId);

            if(this.FindName("stk") == null)
                this.RegisterName(bankId.Name, bankId);


            stackPanel1.Children.Add(restaurantName);
            stackPanel1.Children.Add(bill);
            stackPanel1.Children.Add(date);
            stackPanel1.Children.Add(line);
            stackPanel1.Children.Add(totalBill);
            stackPanel1.Children.Add(line2);
            stackPanel1.Children.Add(payment);
            stackPanel1.Children.Add(cash);
            stackPanel1.Children.Add(bank);
            stackPanel1.Children.Add(bankId);

            menuArea.Children.Add(stackPanel1);
            //hien bill
            Console.WriteLine(client.order.dateTime);
            Console.WriteLine(client.order.numofDishOrders);
            for (int i = 0; i < client.order.numofDishOrders; i++)
            {
                Console.WriteLine(client.order.dishOrder[i].dish.name);
                Console.WriteLine(client.order.dishOrder[i].dish.price);
                Console.WriteLine(client.order.dishOrder[i].numberOfDishes);

            }
            Console.WriteLine(client.order.totalMoney);
        }

        void banking(object sender, RoutedEventArgs e)
        {
            TextBox bankNum = (this.FindName("stk") as TextBox);
            if(bankNum != null)
                bankNum.Visibility = Visibility.Visible;
            
        }

        void sendBankingId()
        {
            MessageBox.Show("SENDED");
        }
        void typeBankingId(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                string bankId = (sender as TextBox).Text;
                client.sendPayMent("0", bankId);
                
               
                if (this.FindName("stk") != null)
                    this.UnregisterName("stk");
            }
           
        }
        void cashPay(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cam on nha cuc cung <3");
            client.sendPayMent("1","");
            listViewMenu.IsEnabled = true;
            
            listViewMenu.Visibility = Visibility.Visible;
            if (this.FindName("stk") != null)
                this.UnregisterName("stk");
            bill(sender, null);
        }

        
    }
}

