﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MaterialSkin;
using MaterialSkin.Controls;

namespace OOPR_LR2
{
    delegate void AnonymousDelegate(int x);
    public partial class Form1 : MaterialForm
    {
        Faculty[] faculties;
        public Form1()
        {
            InitializeComponent();
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            // Configure color schema
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.DeepPurple400, Primary.DeepPurple500,
                Primary.DeepPurple500, Accent.Indigo200,
                TextShade.WHITE
                );
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            faculties = new Faculty[2];
            Teacher[] teachers = new Teacher[] { new Teacher("Лисенко С.М." , "Чоловік", "xtozna.gmail.com", "0678684321", 1),
                                                 new Teacher("Кисіль В.В."  , "Чоловік", "xtozna.gmail.com", "0678655321", 2),
                                                 new Teacher("Пасічник О.С.", "Чоловік", "xtozna.gmail.com", "0678684345", 3),
                                                 new Teacher("Павлова О.О." , "Чоловік", "xtozna.gmail.com", "0678456432", 4),};
            if (File.Exists("Faculty0.dat"))
            {
                faculties[0] = Faculty.LoadFromFile("Faculty0.dat");
            }
            else {
                faculties[0] = new Faculty("Компютерна Інженерія", new int[] { 1, 2, 3 }, 10);
                teachers[0].registerOnFaculty(faculties[0]); teachers[1].registerOnFaculty(faculties[0]); teachers[2].registerOnFaculty(faculties[0]);
            }
            if (File.Exists("Faculty1.dat"))
            {
                faculties[1] = Faculty.LoadFromFile("Faculty1.dat");
            }
            else
            {
                faculties[1] = new Faculty("Програмна Інженерія", new int[] { 1, 2, 4 }, 12);
                teachers[0].registerOnFaculty(faculties[1]); teachers[1].registerOnFaculty(faculties[1]); teachers[3].registerOnFaculty(faculties[1]);
            }
            

            for (int i = 0; i < faculties.Length; i++)
            {
                comboBox1.Items.Add(faculties[i].name);
            }
            comboBox1.SelectedItem = faculties[0].name;
        }

        private void materialCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            materialCheckBox2.Checked = !materialCheckBox1.Checked;
        }

        private void materialCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            materialCheckBox1.Checked = !materialCheckBox2.Checked;
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < faculties.Length; i++)
            {
                if (faculties[i].name == comboBox1.SelectedItem) {
                    Applicant app;
                    if (materialCheckBox1.Checked)
                        app=new Applicant(materialSingleLineTextField6.Text, "Чоловік", materialSingleLineTextField7.Text, materialSingleLineTextField8.Text);
                    else
                        app=new Applicant(materialSingleLineTextField6.Text, "Жінка", materialSingleLineTextField7.Text, materialSingleLineTextField8.Text);

                    app.registerOnFaculty(faculties[i]);
                }
            }
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            for (int i = 0; i < faculties.Length; i++)
            {
                textBox1.Text += "Студенти факультету " + faculties[i].name + "\r\n";
                setRandomMarks(faculties[i]);
                faculties[i].sortDelegate = (x, y) => x.exam.min().CompareTo(y.exam.min());//2 5 лямбда вираз 1
                faculties[i].enroll();
                for (int j = 0; j < faculties[i].applicants.Count; j++) {
                    textBox1.Text += "Студент " + faculties[i].applicants[j].displayData() + "\r\n";
                }
                ISaveable fac = faculties[i];//1 інтерфейсні посилання
                fac.SaveToFile("Faculty" + i + ".dat");
                textBox1.Text += "\r\n";
            }
            //5
            {
                // Анонімні методи
                AnonymousDelegate anonymousDelegate1 = delegate (int x)
                {
                    textBox1.Text += "Анонімний метод 1: " +x + "\r\n";
                };

                AnonymousDelegate anonymousDelegate2 = delegate (int x)
                {
                    textBox1.Text += "Анонімний метод 2: " + x * 2 + "\r\n";
                };

                AnonymousDelegate anonymousDelegate3 = delegate (int x)
                {
                    textBox1.Text += "Анонімний метод 3: " + (x + 5).ToString() + "\r\n";
                };

                anonymousDelegate1(3); // Виведе: Анонімний метод 1: 3
                anonymousDelegate2(3); // Виведе: Анонімний метод 2: 6
                anonymousDelegate3(3); // Виведе: Анонімний метод 3: 8
            }
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            materialTabControl1.SelectedIndex = 0;
        }

        private void materialRaisedButton4_Click(object sender, EventArgs e)
        {
            materialTabControl1.SelectedIndex = 1;
        }

        private void materialRaisedButton5_Click(object sender, EventArgs e)
        {
            materialTabControl1.SelectedIndex = 0;
        }

        private void materialRaisedButton6_Click(object sender, EventArgs e)
        {
            materialTabControl1.SelectedIndex = 1;
        }

        private void setRandomMarks(Faculty f) {
            Random random = new Random();
            for (int i = 0; i < f.teachers.Count; i++) {
                for (int j = 0; j < f.applicants.Count; j++) {
                    f.teachers[i].setMark(f.applicants[j].exam.subject1, random.Next(1, 6));
                    f.teachers[i].setMark(f.applicants[j].exam.subject2, random.Next(1, 6));
                    f.teachers[i].setMark(f.applicants[j].exam.subject3, random.Next(1, 6));
                }
            }
        }
    }
    public interface ISaveable //1  Визначити
    {
        void SaveToFile(string filePath);
        Applicant this[int index] { get; set; }//1 інтерфейсні індексатори
    }
    public interface ITaskrequired: ISaveable //1 наслідування інтерфейсів
    {
        string Data { get; set; }//1 інтерфейсні властивості
    }
    [Serializable]
    public class Mark
    {
        public int value { get; private set; }
        public int subjectID { get; private set; }
        public bool evaluated { get; private set; }

        public Mark()
        {
            this.value = 0;
            this.subjectID = 0;
            this.evaluated = false;
        }
        public Mark(int subjectID)
        {
            this.value = 0;
            this.subjectID = subjectID;
            this.evaluated = false;
        }

        public void evaluation(int value, Teacher teacher) {
            if (teacher.subjectID == this.subjectID) {
                if (value < 5)
                    this.value = Math.Max(2, value);
                else
                    this.value = 5;
                this.evaluated = true;
            }
        }

    }
    [Serializable]
    public class Exam {
        public DateTime date { get; private set; }
        public Mark subject1 { get; private set; }
        public Mark subject2 { get; private set; }
        public Mark subject3 { get; private set; }

        public Exam() {
            this.date = DateTime.Now;
        }
        public Exam(int subject1ID, int subject2ID, int subject3ID) {
            this.date = DateTime.Now;
            this.subject1 = new Mark(subject1ID);
            this.subject2 = new Mark(subject2ID);
            this.subject3 = new Mark(subject3ID);  
        }
        public Exam(Faculty faculty) : this(faculty.subjectIDs[0], faculty.subjectIDs[1], faculty.subjectIDs[2]) {}

        public decimal averege() {
            return ((decimal)subject1.value + (decimal)subject2.value + (decimal)subject3.value) / 3;
        }
        public int min() {
            return Math.Min(subject1.value, Math.Min(subject2.value, subject3.value));
        }
    }
    [Serializable]
    public abstract class Person {
        public string name { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }

        public Person() {
            this.name = "default name";
            this.gender = "default gender";
            this.email = "default email";
            this.phoneNumber = "default phone number";
        }
        public Person(string name, string gender, string email, string phoneNumber)
        {
            this.name = name;
            this.gender = gender;
            this.email = email;
            this.phoneNumber = phoneNumber;
        }

        public abstract void registerOnFaculty(Faculty faculty);
        public virtual string displayData() {
            return "ПІБ: " + name + "; Номер тел. " + phoneNumber + "; email: " + email;
        }
        public void changeFirstName(string firstName)
        {
            name = name.Split(' ')[0] + " " + firstName + " " + name.Split(' ')[2];
        }
        public void changeLastName(string lastName)
        {
            name = lastName + " " + name.Split(' ')[1] + " " + name.Split(' ')[2];
        }
        public void changeFatherName(string fatherName) {
             name = name.Split(' ')[0] + " " + name.Split(' ')[1] + " " + fatherName;
        } 
    }
    [Serializable]
    public sealed class Applicant: Person{
        public Exam exam { get; private set; }
        public Applicant() : base() { }
        public Applicant(string name, string gender, string email, string phoneNumber) : base(name, gender, email, phoneNumber) { }

        public override void registerOnFaculty(Faculty faculty) {
            faculty.applicants.Add(this);
            doExam(faculty);
        }
        public override string displayData()
        {
            return base.displayData() + "; Середній бал " + displayExam();
        }
        public string displayExam() {
            return exam.averege().ToString("F2");
        }
        public void doExam(int subject1ID, int subject2ID, int subject3ID)
        {
            this.exam = new Exam(subject1ID, subject2ID, subject3ID);
        }
        public void doExam(Faculty faculty) {
            this.exam = new Exam(faculty);
        }
    }
    [Serializable]
    public class Teacher: Person {
        public int subjectID { get; private set; }
        new public string email { get; private set; }
        public Teacher() : base() { this.subjectID = 0; }
        public Teacher(string name, string gender, string email, string phoneNumber, int subjectID) : base(name, gender, email, phoneNumber) { this.subjectID = subjectID; }

        public override void registerOnFaculty(Faculty faculty) {
            faculty.teachers.Add(this);
        }
        public sealed override string displayData()
        {
            return base.displayData() + " " + subjectID;
        }
        public virtual void changeMark(Mark mark, int value) {
            mark.evaluation(value, this);
        }
        public void changeMark(Exam exam, int value)
        {
            changeMark(exam.subject1, value);
            changeMark(exam.subject2, value);
            changeMark(exam.subject3, value);
        }
        public void changeMark(Applicant applicant, int value)
        {
            changeMark(applicant.exam.subject1, value);
            changeMark(applicant.exam.subject2, value);
            changeMark(applicant.exam.subject3, value);
        }
        public void setMark(Mark mark,int value) {
            if (!mark.evaluated)
                changeMark(mark, value);
        }
        public void setMark(Exam exam, int value) {
            setMark(exam.subject1, value);
            setMark(exam.subject2, value);
            setMark(exam.subject3, value);
        }
        public void setMark(Applicant applicant, int value)
        {
            setMark(applicant.exam.subject1, value);
            setMark(applicant.exam.subject2, value);
            setMark(applicant.exam.subject3, value);
        }
    }
    [Serializable]
    public class Faculty: ISaveable // використати
    {
        public string name { get; private set; }
        public int[] subjectIDs { get; private set; }
        public int numberOfStudents { get; private set; }
        public List<Teacher> teachers { get; private set; }
        public List<Applicant> applicants { get; private set; }
        [Serializable]
        public delegate int SortDelegate(Applicant x, Applicant y);//2 //3 Параметри-значення
        public SortDelegate sortDelegate { get; set; }

        public Faculty() {
            this.name = "default faculty name";
            this.subjectIDs = new int[] { 0, 1, 2 };
            this.numberOfStudents = 1;
            this.teachers = new List<Teacher>();
            this.applicants = new List<Applicant>();
        }
        public Faculty(string name, int[] subjectIDs,int numberOfStudents) {
            this.name = name;
            this.subjectIDs = subjectIDs;
            this.numberOfStudents = numberOfStudents;
            this.teachers = new List<Teacher>();
            this.applicants = new List<Applicant>();
        }

        public List<Applicant> enroll() {
            sortApplicants();
            if(this.numberOfStudents< this.applicants.Count - 1)
            this.applicants.RemoveRange(this.numberOfStudents, this.applicants.Count - 1);
            return this.applicants;
        }
        public void sortApplicants() {
            if (sortDelegate == null)
                this.applicants.Sort((x, y) => x.exam.averege().CompareTo(y.exam.averege()));//5 лямбда вираз 2
            else
                this.applicants.Sort((x, y) => sortDelegate(x, y));//2 5 лямбда вираз 3
        }

        public void SaveToFile(string filePath)
        {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, this);
                }
        }
        public static Faculty LoadFromFile(string filePath)
        {
            Faculty loadedData = null;

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    loadedData = (Faculty)formatter.Deserialize(fileStream);
                }

            return loadedData;
        }

        //4
        public Applicant this[int index]
        {
            get
            {
                if (index >= 0 && index < applicants.Count)
                    return applicants[index];
                else
                    throw new IndexOutOfRangeException();
            }
            set
            {
                if (index >= 0 && index < applicants.Count)
                    applicants[index] = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }
        //4
        public Applicant this[string name]
        {
            get
            {
                for (int i = 0; i < applicants.Count; i++)
                {
                    if (applicants[i].name == name)
                        return applicants[i];
                }
                return null; 
            }
        }
    }


    delegate void Task3Delegate(ref int b, out int c, int[] numbers, int a = 10);//3 параметри-посилання, вихідні параметри, параметри-масиви, параметри за замовчуванням
    class LinkedListNode
    {
        public Faculty faculty { get; set; }
        public LinkedListNode Next { get; set; }

        public LinkedListNode(Faculty data)
        {
            faculty = data;
            Next = null;
        }
    }
    class LinkedList
    {
        // Статичний компонент - вказівник на початок зв’язного списку
        private static LinkedListNode head;

        // Статичний метод для додавання нового елемента в зв’язний список
        public static void AddNode(Faculty data)
        {
            LinkedListNode newNode = new LinkedListNode(data);
            newNode.Next = head;
            head = newNode;
        }

        // Статичний метод для перегляду зв’язного списку та запису даних у файл
        public static void ViewListAndWriteToFile(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                LinkedListNode current = head;
                while (current != null)
                {
                    Console.WriteLine(current.faculty.name);
                    writer.WriteLine(current.faculty.name);
                    current = current.Next;
                }
            }
        }
    }
}
