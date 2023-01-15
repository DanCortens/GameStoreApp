using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace FinalDanielCortens
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PopulateGameGrid();
            PopulateAllTransactions();
            PopulateCustomers();
        }

        private void PopulateGameGrid()
        {
            using (var context = new GameShoppingDBEntities())
            {
                var games = context.Games.Include("Orders").ToList<Game>();
                gridGames.ItemsSource = games;
            }
        }
        public void UpdateGameQuantities(object sender, RoutedEventArgs e)
        {
            using (var context = new GameShoppingDBEntities())
            {
                var games = from g in context.Games
                            select g;
                Random r = new Random();
                foreach (Game game in games) {
                    game.Stock = r.Next(21);
                }
                context.SaveChanges();
            }
            PopulateGameGrid();
        }

        public void TryPurchase(object sender, RoutedEventArgs e)
        {
            if (tboxCustomerName.Text == "")
            {
                MessageBox.Show("Customer name cannot be empty.", "Purchase error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (tboxCustomerQuantity.Text == "")
            {
                MessageBox.Show("Quantity cannot be empty.", "Purchase error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Int32.Parse(tboxCustomerQuantity.Text) <= 0)
            {
                MessageBox.Show("Quantity must be 1 or greater.", "Purchase error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (gridGames.SelectedIndex < 0)
            {
                MessageBox.Show("There must be an item selected.", "Purchase error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var selectedGame = gridGames.SelectedItem as Game;
                var quantity = Int32.Parse(tboxCustomerQuantity.Text);
                if (selectedGame.Stock < quantity)
                {
                    MessageBox.Show("The order cannot exceed the items in stock.", "Purchase error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    SubmitPurchase();
                }
            }
        }
        private void SubmitPurchase()
        {
            using (var context = new GameShoppingDBEntities())
            {
                var customers = from c in context.Customers
                               where c.CustomerName == tboxCustomerName.Text
                               select c;
                var customer = new Customer();
                if (customers.Any())
                {
                    customer = customers.First<Customer>();
                }
                else
                {
                    customer.CustomerName = tboxCustomerName.Text;
                    context.Customers.Add(customer);
                    context.SaveChanges();
                }
                
                Order order = new Order();
                Game gridGame = gridGames.SelectedItem as Game;
                var game = (from g in context.Games
                           where g.GameId == gridGame.GameId
                           select g).Single();
                order.CustomerId = customer.CustomerId;
                order.GameId = game.GameId;
                order.Date = DateTime.Now;
                var q = Int32.Parse(tboxCustomerQuantity.Text);
                order.Quantity = q;
                order.Discount = (q >= 5) ? (game.Price * 0.1 * q) : 0.0;
                context.Orders.Add(order);
                game.Stock = game.Stock - q;
                context.SaveChanges();

                lblPrice.Content = $"{(game.Price).ToString("C")}";
                lblQuantity.Content = $"{q}";
                lblTotal.Content = $"{(game.Price * q).ToString("C")}";
                lblDiscount.Content = $"{(order.Discount).ToString("C")}";
                var subtotal = (game.Price * q) - order.Discount;
                var tax = (subtotal * 0.13);
                lblTax.Content = $"{(tax.ToString("C"))}";
                lblNetTotal.Content = $"{(subtotal + tax).ToString("C")}";

                
            }
            PopulateGameGrid();
            PopulateCustomers();
            PopulateAllTransactions();
        }

        public void ClearPurchase(object sender, RoutedEventArgs e)
        {
            tboxCustomerName.Text = "";
            tboxCustomerQuantity.Text = "";

            lblPrice.Content = "";
            lblQuantity.Content = "";
            lblTotal.Content = "";
            lblDiscount.Content = "";
            lblTax.Content = "";
            lblNetTotal.Content = "";
        }

        private void PopulateCustomers()
        {
            using (var context = new GameShoppingDBEntities())
            {
                var customers = from c in context.Customers
                                select c;

                listboxCustomers.Items.Clear();
                foreach (var c in customers)
                    listboxCustomers.Items.Add(c.CustomerName);
            }
        }
        private void DeleteAll(object sender, RoutedEventArgs e)
        {
            using (var context = new GameShoppingDBEntities())
            {
                context.Orders.RemoveRange(context.Orders.ToList());
                context.Customers.RemoveRange(context.Customers.ToList());
                var games = from g in context.Games
                            select g;
                foreach (Game g in games.ToList())
                    g.Orders.Clear();
                context.SaveChanges();
            }
            listboxCustomers.Items.Clear();
            gridAllTransactions.ItemsSource = null;
            gridCustomerTrans.ItemsSource = null;
        }

        private void PopulateAllTransactions()
        {
            using (var context = new GameShoppingDBEntities())
            {
                var transactions = (from o in context.Orders
                                   join c in context.Customers
                                   on o.CustomerId equals c.CustomerId
                                   join g in context.Games
                                   on o.GameId equals g.GameId
                                   select new
                                    {
                                       ID = o.OrderId,
                                       Date = o.Date,
                                       CustomerName = c.CustomerName,
                                       Game = g.GameName,
                                       Price = g.Price,
                                       Quantity = o.Quantity,
                                       Discount = o.Discount,
                                       Tax = (((g.Price * o.Quantity) - o.Discount) * 0.13),
                                       NetTotal = (((g.Price * o.Quantity) - o.Discount) + (((g.Price * o.Quantity) - o.Discount) * 0.13))
                                   });
                gridAllTransactions.ItemsSource = transactions.ToList();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        
        private void listboxCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var context = new GameShoppingDBEntities())
            {
                var custName = listboxCustomers.SelectedValue as string;
                var customerOrders = (from co in context.Orders
                                     join c in context.Customers
                                     on co.CustomerId equals c.CustomerId
                                     join g in context.Games
                                     on co.GameId equals g.GameId
                                     where co.Customer.CustomerName == custName
                                     select new
                                     {
                                         ID = co.OrderId,
                                         Date = co.Date,
                                         CustomerName = c.CustomerName,
                                         Game = g.GameName,
                                         Price = g.Price,
                                         Quantity = co.Quantity,
                                         Discount = co.Discount,
                                         Tax = (((g.Price * co.Quantity) - co.Discount) * 0.13),
                                         NetTotal = (((g.Price * co.Quantity) - co.Discount) + (((g.Price * co.Quantity) - co.Discount) * 0.13))
                                     });
                gridCustomerTrans.ItemsSource = customerOrders.ToList();
            }
        }
    }
    
}
