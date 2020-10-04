using System.Collections.Generic;

namespace ApiStudent
{
    public static class StudentRepo
    {
        public static List<Student> Student=new List<Student>()
        {
            new Student()
            {
                id=1,
                nume="Rad",
                prenume="Iuliana",
                facultate="AC",
                an=4
            },
            new Student()
            {
                id=2,
                nume="Budica",
                prenume="Catalin",
                facultate="AC",
                an=3
            }
        };
    }
}