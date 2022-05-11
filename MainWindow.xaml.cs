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
using System.IO;

namespace MyNotesApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();          
            GetData();
        }

        //Данные о заметках
        public List<string> GetData()
        {
            string notesFolder = @"C:\\MyNotesApp\\Notes";
            string[] files = Directory.GetFiles(notesFolder, "*.txt");
            List<string> notesdata = new List<string>();
            foreach (var file in files)
            {
                string filename = System.IO.Path.GetFileName(file).Split('.').First();
                notesdata.Add(filename);
            }
            notesAmount.Content = "Всего заметок: " + notesdata.Count();
            notesList.ItemsSource = notesdata;
            return notesdata;
        }

        //Создать заметку
        private void MakeNoteButton_Click(object sender, EventArgs e)
        {
            noteField.IsReadOnly = false;
            string folder = @"C:\\MyNotesApp\\Notes\\";
            string notelabel = "Новая заметка";
            string fileExt = ".txt";
            string path = String.Concat(folder, notelabel, fileExt);

            if (!File.Exists(path))
            {
                File.Create(path);
            }

            GetData();
        }

        //Кнопка "Сохранить"
        private void SaveNote(object sender, EventArgs e)
        {
            TextRange range = new TextRange(noteField.Document.ContentStart, noteField.Document.ContentEnd);
            string currentFilename = @"C:\\MyNotesApp\\Notes\\" + notesList.SelectedItem.ToString() + ".txt";
            FileStream stream = new FileStream(currentFilename, FileMode.Create);
            range.Save(stream, DataFormats.Text);
            stream.Close();
            string newLabel = noteLabel.Text;
            if (newLabel != "Новая заметка")
            {
                File.Move(currentFilename, @"C:\\MyNotesApp\\Notes\\" + newLabel + ".txt");
            }
            else
            {
                MessageBox.Show("Пожалуйста, дайте название вашей заметке");
            }

            GetData();
        }

        //Кнопка "Удалить"
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            string path = @"C:\\MyNotesApp\\Notes\\" + notesList.SelectedItem.ToString() + ".txt";

            if (MessageBox.Show("Вы действительно хотите удалить записку " + "\"" + notesList.SelectedItem.ToString() + "\"?", 
                "Подтвердите удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                File.Delete(path);
                noteField.Document.Blocks.Clear();
                noteLabel.Text = "Откройте любую заметку или создайте новую";
                saveButton.Visibility = Visibility.Hidden;
                deleteButton.Visibility = Visibility.Hidden;
            }
            GetData();
        }

        //Выбор заметки
        private void SelectNote(object sender, SelectionChangedEventArgs args)
        {
            if (notesList.SelectedItem != null)
            {
                deleteButton.Visibility = Visibility.Visible;
                noteLabel.IsReadOnly = false;

                noteLabel.Text = notesList.SelectedItem.ToString();
                string currentNote = notesList.SelectedItem.ToString();
                Console.WriteLine("Selected " + currentNote);

                noteField.CaretPosition = noteField.Document.ContentEnd;
                noteField.ScrollToEnd();
                noteField.Focus();

                string path = @"C:\\MyNotesApp\\Notes\\" + currentNote + ".txt";
                TextRange range = new TextRange(noteField.Document.ContentStart, noteField.Document.ContentEnd);
                try
                {
                    var fStream = File.Open(path, FileMode.Open);
                    range.Load(fStream, DataFormats.Text);
                    fStream.Close();
                }
                catch (IOException e)
                {
                    Console.WriteLine(
                        "{0}: The write operation could not be performed because the specified " +
                        "part of the file is locked.", e.GetType().Name);
                }
                
            }
        }

        //Если текст изменился
        private void NoteField_TextChanged(object sender, TextChangedEventArgs e)
        {
            saveButton.Visibility = Visibility.Visible;
        }

        //Информация о приложении
        private void AppInfoButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Приложение " + "\"" + "Заметки" + "\"\n" + "Версия 1.0\nРазработчик: Максим Кузькин",
                "О приложении");
        }
    }
}
