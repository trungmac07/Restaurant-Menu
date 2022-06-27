using System;
using System.Collections.Generic;
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
        Client.Client client = new Client.Client();
        public MainWindow()
        {
            InitializeComponent();
            drawMenu();
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

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListViewMenu.SelectedIndex;
            MoveCursorMenu(index);

            switch (index)
            {
                case 0:


                    break;
                case 1:


                    break;
                default:
                    break;
            }
        }

        private void MoveCursorMenu(int index)
        {
            TrainsitionigContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, (100 + (60 * index)), 0, 0);
        }

        private void chooseDishes(object sender, RoutedEventArgs e)
        {
            TextBlock a = sender as TextBlock;
            MessageBox.Show(a.Tag.ToString());
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
                                return (new Thickness(75, 50, 0, 30));
                            case 2:
                                return (new Thickness(80, 140, 0, 30));
                            default:
                                return (new Thickness(85, 225, 0, 30));

                        }
                        break;
                    }
                case 2:
                    {
                        switch (x)
                        {
                            case 1:
                                return (new Thickness(65, 230, 0, 30));
                            case 2:
                                return (new Thickness(80, 140, 0, 30));
                            default:
                                return (new Thickness(85, 60, 0, 30));

                        }
                        break;
                    }
                case 3:
                    {
                        switch (x)
                        {
                            case 1:
                                return (new Thickness(75, 110, 0, 30));
                            case 2:
                                return (new Thickness(80, 270, 0, 30));
                            default:
                                return (new Thickness(85, 168, 0, 30));

                        }
                        break;
                    }
                case 4:
                    {
                        switch (x)
                        {
                            case 1:
                                return (new Thickness(75, 50, 0, 30));
                            case 2:
                                return (new Thickness(80, 130, 0, 30));
                            default:
                                return (new Thickness(85, 215, 0, 30));

                        }
                        break;
                    }
                default:
                    return new Thickness(0, 0, 0, 0);
            }

        }

        private void mainCourseMenu()
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("C:/Users/Trung/Desktop/Image/1.png"));
            menuArea.Background = imageBrush;
            List<string> a = client.recvMenu();
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
            foreach (object child in menuArea.Children)
            {
                if (child is StackPanel)
                {
                    ++i;
                    var label = new Label();
                    label.Name = "content" + i + "Label";
                    label.HorizontalAlignment = HorizontalAlignment.Left;
                    label.Content = "Content " + i;
                    label.Margin = chooseThickness(1, i);
                    label.FontFamily = new FontFamily("SVN-Bali Script");
                    label.FontSize = 57;
                    (child as StackPanel).Children.Add(label);
                    for (int j = 0; j < 6; ++j)
                    {
                        var textBlock = new TextBlock();
                        textBlock.Text = a[++c];
                        textBlock.MouseDown += chooseDishes;
                        textBlock.Tag = "dish" + i.ToString();
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
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("C:/Users/Trung/Desktop/Image/2.jpg"));
            menuArea.Background = imageBrush;
            List<string> a = client.recvMenu();
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
            foreach (object child in menuArea.Children)
            {
                if (child is StackPanel)
                {
                    ++i;
                    var label = new Label();
                    label.Name = "content" + i + "Label";
                    label.HorizontalAlignment = HorizontalAlignment.Left;
                    label.Content = "Content " + i;
                    label.Margin = chooseThickness(2, i);
                    label.FontFamily = new FontFamily("SVN-Bali Script");
                    label.FontSize = 57;
                    (child as StackPanel).Children.Add(label);
                    for (int j = 0; j < 6; ++j)
                    {
                        var textBlock = new TextBlock();
                        textBlock.Text = a[c++];
                        textBlock.MouseDown += chooseDishes;
                        textBlock.Tag = "dish" + i.ToString();
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
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("C:/Users/Trung/Desktop/Image/3.jpg"));
            menuArea.Background = imageBrush;
            List<string> a = client.recvMenu();
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
            foreach (object child in menuArea.Children)
            {
                if (child is StackPanel)
                {
                    ++i;
                    var label = new Label();
                    label.Name = "content" + i + "Label";
                    label.HorizontalAlignment = HorizontalAlignment.Left;
                    label.Content = "Content " + i;
                    label.Margin = chooseThickness(3, i);
                    label.FontFamily = new FontFamily("SVN-Bali Script");
                    label.FontSize = 57;
                    (child as StackPanel).Children.Add(label);
                    for (int j = 0; j < 6; ++j)
                    {
                        var textBlock = new TextBlock();
                        textBlock.Text = a[++c];
                        textBlock.MouseDown += chooseDishes;
                        textBlock.Tag = "dish" + i.ToString();
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
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("C:/Users/Trung/Desktop/Image/4.png"));
            menuArea.Background = imageBrush;
            List<string> a = client.recvMenu();
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
            foreach (object child in menuArea.Children)
            {
                if (child is StackPanel)
                {

                    var label = new Label();
                    label.Name = "content" + ++i + "Label";
                    label.HorizontalAlignment = HorizontalAlignment.Left;
                    label.Content = "Content " + i;
                    label.Margin = chooseThickness(4, i);
                    label.FontFamily = new FontFamily("SVN-Bali Script");
                    label.FontSize = 57;
                    (child as StackPanel).Children.Add(label);
                    for (int j = 0; j < 6; ++j)
                    {
                        var textBlock = new TextBlock();
                        textBlock.Text = a[++c];
                        textBlock.MouseDown += chooseDishes;
                        textBlock.Tag = "dish" + i.ToString();
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
                case 0:
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
            int mode = 2;
            chooseMenuLayout(mode);

        }

    }
}
