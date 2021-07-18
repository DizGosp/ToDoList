using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Model.Models;

namespace ToDoList.Interfaces.Interfaces
{
    public interface INotification
    {
        List<Notification> Get(string id);

        bool Update(string id);
    }
}
