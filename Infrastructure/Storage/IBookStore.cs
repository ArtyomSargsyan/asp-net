using ToDoApi.Domain;

namespace ToDoApi.Infrastructure.Storage;

    public interface IBookStore
    {
        IEnumerable<Book> GetAll();
        Book? GetById(int id);
        void Add(Book book);
        bool Update(int id, Book book);
        bool Delete(int id);
    }
