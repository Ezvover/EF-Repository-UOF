using laba4.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace laba4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> strList = new List<string>();

        public MainWindow()
        {
            InitializeComponent(); // инит
            ToGrid();
          
            MainGrid.SelectedIndex = 0; // Устанавливаем первый элемент как выбранный
            MainGrid.IsReadOnly = true;
        }

        public void ToGrid()
        {
            using (var unitOfWork = new UnitOfWork(new CodeFirstModel()))
            {
                try
                {
                    var goodsRepository = unitOfWork.GoodsRepository;

                    var goodsList = goodsRepository.GetAll()
                        .Select(g => new Goods
                        {
                            id = g.id,
                            name = g.name,
                            desc = g.desc,
                            category = g.category,
                            rate = g.rate ?? 0,
                            price = g.price ?? 0,
                            amount = g.amount ?? 0,
                            other = File.Exists(g.other) ? new Uri(g.other).ToString() : null
                        })
                        .ToList();

                    MainGrid.ItemsSource = goodsList;

                    var strList = goodsList.Select(g => g.category).Distinct();
                    FilterBOx.ItemsSource = strList;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        MessageBox.Show("Inner Exception: " + ex.InnerException.Message);
                    }
                    else
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e) // delete button
        {
            if (MainGrid.SelectedItem != null)
            {
                var selectedObject = MainGrid.SelectedItem as Goods;

                using (var unitOfWork = new UnitOfWork(new CodeFirstModel()))
                {
                    try
                    {
                        var goodsRepository = unitOfWork.GoodsRepository;
                        var categoriesRepository = unitOfWork.CategoryRepository;

                        var goodsToDelete = goodsRepository.Get(selectedObject.id);

                        if (goodsToDelete != null)
                        {
                            if (goodsToDelete.Category1 != null)
                            {
                                categoriesRepository.Remove(goodsToDelete.Category1);
                            }

                            goodsRepository.Remove(goodsToDelete);
                            unitOfWork.SaveChanges();

                            ToGrid();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

            MainGrid.SelectedIndex = 0; // Set the first item as selected
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int minPrice = int.Parse(SearchText.Text);
                int maxPrice = int.Parse(SearchText2.Text);

                using (var unitOfWork = new UnitOfWork(new CodeFirstModel()))
                {
                    var goodsRepository = unitOfWork.GoodsRepository;

                    var filteredGoods = goodsRepository.GetAll()
                        .Where(g => g.price >= minPrice && g.price <= maxPrice)
                        .ToList();

                    MainGrid.ItemsSource = filteredGoods;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            MainGrid.SelectedIndex = 0; // Set the first item as selected
        }


        private void Button_Click_1(object sender, RoutedEventArgs e) // categorysort
        {
            if (FilterBOx.SelectedItem != null)
            {
                string selectedCategory = FilterBOx.SelectedItem.ToString();

                using (var unitOfWork = new UnitOfWork(new CodeFirstModel()))
                {
                    try
                    {
                        var goodsRepository = unitOfWork.GoodsRepository;

                        var goodsInCategory = goodsRepository.GetAll()
                            .Where(g => g.category == selectedCategory)
                            .ToList();

                        MainGrid.ItemsSource = goodsInCategory;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                MainGrid.SelectedIndex = 0; // Set the first item as selected
            }
        }



        private void Button_Click_2(object sender, RoutedEventArgs e) // add window
        {
            AddWindow add = new AddWindow();
            add.Show();
            
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            ToGrid();
            FilterBOx.SelectedItem= null;
            SearchText.Clear();
            SearchText2.Clear();
            SearchText3.Clear();
            MainGrid.SelectedIndex = 0; // Устанавливаем первый элемент как выбранный
        }

        private void SearchButton2_Click(object sender, RoutedEventArgs e) // partSearch
        {
            string searchText = SearchText3.Text;

            using (var unitOfWork = new UnitOfWork(new CodeFirstModel()))
            {
                try
                {
                    var goodsRepository = unitOfWork.GoodsRepository;

                    var allGoods = goodsRepository.GetAll();

                    var searchResults = allGoods
                        .Where(g =>
                            g.name.Contains(searchText) ||
                            g.desc.Contains(searchText) ||
                            g.category.Contains(searchText) ||
                            g.price.ToString().Contains(searchText) ||
                            g.amount.ToString().Contains(searchText))
                        .ToList();

                    MainGrid.ItemsSource = searchResults;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            MainGrid.SelectedIndex = 0; // Set the first item as selected
        }




        private void Button_Click_3(object sender, RoutedEventArgs e) // edit
        {
            try
            {
                File.Delete("TempEditGood.xml");
            }
            catch
            {

            }
            if (MainGrid.SelectedItem!= null) 
            {
                var selectedObject = MainGrid.SelectedItem as Goods;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
                using (FileStream stream = new FileStream($"TempEditGood.xml", FileMode.OpenOrCreate))
                {
                    xmlSerializer.Serialize(stream, selectedObject);
                }

                EditWindow editWindow = new EditWindow();
                editWindow.Show();
            }
        }
        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedItem != null)
            {
                int index = MainGrid.SelectedIndex;
                if (index < MainGrid.Items.Count - 1)
                {
                    MainGrid.SelectedIndex = index + 1;
                    MainGrid.ScrollIntoView(MainGrid.SelectedItem);
                }
            }
        }
    }
}
