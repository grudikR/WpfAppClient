using Models.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace WpfAppClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient httpClient = new HttpClient();
        public MainWindow()
        {
            InitializeComponent();
            httpClient.BaseAddress = new Uri("https://localhost:7056/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                );
            this.GetCategories();
        }

        private async void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxCategoryName.Text.Length == 0)
                return;

            var categ = new Category
            {

                ID = Int32.Parse(TextBoxCategoryID.Text),
                Name = TextBoxCategoryName.Text
            };
            if (categ.ID == 0)
            {
                await this.PostCategory(categ);
            }
            else
            {
                await this.UpdateCategory(categ);
            }
            TextBoxCategoryID.Text = 0.ToString();
            TextBoxCategoryName.Text = "";
            this.GetCategories();
        }

        private void btnShowCategories_Click(object sender, RoutedEventArgs e)
        {
            this.GetCategories();
        }

        //get All Categories
        private async void GetCategories()
        {
            try
            {
                var responce = await httpClient.GetStringAsync("Categories");
                var categories = JsonConvert.DeserializeObject<List<Category>>(responce);
                dgCategories.DataContext = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //post Category
        private async Task PostCategory(Category category)
        {
            await httpClient.PostAsJsonAsync("Categories", category);
        }

        //update Category
        private async Task UpdateCategory(Category category)
        {
            await httpClient.PutAsJsonAsync("Categories/" + category.ID.ToString(), category);
        }

        //delete Category
        private async Task DeleteCategory(int categoryID)
        {
            await httpClient.DeleteAsync("Categories/" + categoryID);
        }

        public async void dgEditCategoryClick(object sender, RoutedEventArgs e)
        {
            Category? category = ((FrameworkElement)sender).DataContext as Category;
            TextBoxCategoryID.Text = category.ID.ToString();
            TextBoxCategoryName.Text = category.Name;
        }
        public async void dgDeleteCategoryClick(object sender, RoutedEventArgs e)
        {
            Category? category = ((FrameworkElement)sender).DataContext as Category;
            await this.DeleteCategory(category.ID);
            this.GetCategories();
        }
        public async void dgChooseCategoryClick(object sender, RoutedEventArgs e)
        {
            Category? category = ((FrameworkElement)sender).DataContext as Category;
            TaskItemWindow taskItemWindow = new TaskItemWindow(category);
            taskItemWindow.ShowDialog();
        }
    } 
}
