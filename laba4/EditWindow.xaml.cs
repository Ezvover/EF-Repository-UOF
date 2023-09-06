﻿using laba4.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace laba4
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {

        int id = 0;

        Goods goods = new Goods();

        public EditWindow()
        {
            InitializeComponent();


            Goods goods;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Goods));
            using (FileStream stream = new FileStream($"TempEditGood.xml", FileMode.OpenOrCreate))
            {
                goods = (xmlSerializer.Deserialize(stream) as Goods);
            }
            id = goods.id;
            LoadDB();
            ToText();
        }

        List<Goods> goodsList = new List<Goods>();

        public void LoadDB()
        {
            using (var unitOfWork = new UnitOfWork(new CodeFirstModel()))
            {
                try
                {
                    var goodsRepository = unitOfWork.Repository<Goods>();

                    var loadedGoods = goodsRepository.GetById(id);

                    if (loadedGoods != null)
                    {
                        goods.id = loadedGoods.id;
                        goods.name = loadedGoods.name;
                        goods.desc = loadedGoods.desc;
                        goods.category = loadedGoods.category;
                        goods.rate = loadedGoods.rate;
                        goods.price = loadedGoods.price;
                        goods.amount = loadedGoods.amount;
                        goods.other = loadedGoods.other;

                        goodsList.Add(goods);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void ToText()
        {
            NameTextBox.Text = goodsList[0].name.ToString();
            DescTextBox.Text = goodsList[0].desc.ToString();
            CategoryTextBox.Text = goodsList[0].category.ToString();
            RateTextBox.Text = goodsList[0].rate.ToString();
            PriceTextBox.Text = goodsList[0].price.ToString();
            AmountTextBox.Text = goodsList[0].amount.ToString();
        }

        private async Task UpdateDB()
        {
            int goodsIdToUpdate = goodsList[0].id;

            using (var unitOfWork = new UnitOfWork(new CodeFirstModel()))
            {
                try
                {
                    var goodsRepository = unitOfWork.Repository<Goods>();
                    var categoriesRepository = unitOfWork.Repository<Categories>();

                    var goodsToUpdate = goodsRepository.GetById(goodsIdToUpdate);

                    if (goodsToUpdate != null)
                    {
                        string imagePath = $"C:\\Users\\vovas\\Desktop\\repos\\WPF-Shop-Service\\laba4\\bin\\Debug\\net7.0-windows\\images\\{NameTextBox.Text}.jpg";
                        string defaultImagePath = $"C:\\Users\\vovas\\Desktop\\repos\\WPF-Shop-Service\\laba4\\bin\\Debug\\net7.0-windows\\images\\img0.jpg";
                        byte[] imageBytes;

                        if (File.Exists(imagePath))
                        {
                            goodsToUpdate.other = imagePath;
                            imageBytes = File.ReadAllBytes(imagePath);
                        }
                        else
                        {
                            goodsToUpdate.other = defaultImagePath;
                            imageBytes = File.ReadAllBytes(defaultImagePath);
                        }

                        goodsToUpdate.picture = imageBytes;
                        goodsToUpdate.name = NameTextBox.Text;
                        goodsToUpdate.desc = DescTextBox.Text;
                        goodsToUpdate.category = CategoryTextBox.Text;
                        goodsToUpdate.rate = int.Parse(RateTextBox.Text);
                        goodsToUpdate.price = int.Parse(PriceTextBox.Text);
                        goodsToUpdate.amount = int.Parse(AmountTextBox.Text);

                        await unitOfWork.SaveChangesAsync();

                        var categoryToUpdate = categoriesRepository.GetById(goodsIdToUpdate);
                        if (categoryToUpdate != null)
                        {
                            categoryToUpdate.category1 = CategoryTextBox.Text;
                            await unitOfWork.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }



        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((int.Parse(RateTextBox.Text) < 0) || (double.Parse(PriceTextBox.Text) < 0) || (int.Parse(AmountTextBox.Text) < 0) || string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(CategoryTextBox.Text))
                {
                    MessageBox.Show("Данные меньше нуля");
                }
                else
                {
                    UpdateDB();
                    File.Delete("TempEditGood.xml");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

