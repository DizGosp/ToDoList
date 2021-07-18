using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Model.Models
{
    public class Registration
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password{ get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
