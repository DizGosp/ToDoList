using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Model.Models;

namespace ToDoList.Interfaces.Interfaces
{
    public interface IToDo:IGeneric<ToDo>
    {
        List<ToDo> getByUser(string user);

        bool UpdateStatus(ToDo entity);
    }
}
