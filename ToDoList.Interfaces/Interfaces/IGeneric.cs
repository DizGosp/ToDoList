using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Model.Models;

namespace ToDoList.Interfaces.Interfaces
{
    public interface IGeneric<T> where T:class
    {

        T Get(string id);

        List<T> GetAll();

        bool Add(T entity);
        bool Delete(string id);

        bool Update(T entity);



      
    }
}
