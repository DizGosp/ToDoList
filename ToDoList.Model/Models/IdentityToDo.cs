using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Model.Models
{
   public class IdentityToDo
    {
       
        public string ToDoId { get; set; }

       public  List<Rows> users { get; set; }

        public class Rows
        {
            public string UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string UserName { get; set; }
        }
       

    }

   
}
