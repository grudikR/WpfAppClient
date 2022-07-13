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
using System.Windows.Shapes;

namespace WpfAppClient
{
    /// <summary>
    /// Interaction logic for TaskItemWindow.xaml
    /// </summary>
    public partial class TaskItemWindow : Window
    {
        private Category? _category = null;
        HttpClient httpClient = new HttpClient();
        public TaskItemWindow(Category category)
        {
            _category = category;
            InitializeComponent();
            TextBoxCategoryName.Text = _category.Name;
            TextBoxCategoryID.Text = _category.ID.ToString();

            httpClient.BaseAddress = new Uri("https://localhost:7056/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                );
            this.GetAllTaskItems(_category);

        }

        //get All Categories
        private async void GetAllTaskItems(Category category)
        {
            try
            {
                var responce = await httpClient.GetStringAsync("TaskItems/bycategory/" + category.ID.ToString());
                var taskitems = JsonConvert.DeserializeObject<List<TaskItem>>(responce);
                dgTaskItems.DataContext = taskitems;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnToMainWindows_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnSaveTaskItem_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxTaskItemName.Text.Length == 0)
                return;

            var taskitem = new TaskItem
            {

                Id = Int32.Parse(TextBoxTaskItemID.Text),
                Name = TextBoxTaskItemName.Text,
                IsDone = (bool)CheckBoxIsDone.IsChecked,
                CategoryID = _category.ID
            };
            if (taskitem.Id == 0)
            {
                await this.PostTaskItem(taskitem);
            }
            else
            {
                await this.UpdateTaskItem(taskitem);
            }
            TextBoxTaskItemID.Text = 0.ToString();
            TextBoxTaskItemName.Text = "";
            CheckBoxIsDone.IsChecked = false;
            this.GetAllTaskItems(_category);
        }

        public async void dgEditTaskItemClick(object sender, RoutedEventArgs e)
        {
            TaskItem? taskitem = ((FrameworkElement)sender).DataContext as TaskItem;
            TextBoxTaskItemID.Text = taskitem.Id.ToString();
            TextBoxTaskItemName.Text = taskitem.Name;
            CheckBoxIsDone.IsChecked = taskitem.IsDone;
        }

        public async void dgDeleteTaskItemClick(object sender, RoutedEventArgs e)
        {
            TaskItem? taskitem = ((FrameworkElement)sender).DataContext as TaskItem;
            await this.DeleteTaskItem(taskitem.Id);
            this.GetAllTaskItems(_category);
        }

        //post taskitem
        private async Task PostTaskItem(TaskItem taskItem)
        {
            await httpClient.PostAsJsonAsync("TaskItems", taskItem);
        }

        //update TaskItem
        private async Task UpdateTaskItem(TaskItem taskItem)
        {
            await httpClient.PutAsJsonAsync("TaskItems/" + taskItem.Id.ToString(), taskItem);
        }

        //delete TaskItem
        private async Task DeleteTaskItem(int TaskItemID)
        {
            await httpClient.DeleteAsync("TaskItems/" + TaskItemID);
        }

    }
}
