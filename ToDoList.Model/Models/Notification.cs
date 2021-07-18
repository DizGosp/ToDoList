using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Model.Models
{
    public class Notification 
    {
        public string UserNotId { get; set; }
        public string Title { get; set; }
        public bool StatusOfNotification { get; set; }
    }
}
