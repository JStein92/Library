using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using Library.Models;

namespace Library.Tests
{
  [TestClass]
  public class BookTests : IDisposable
  {
    public BookTests()
    {
        DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=library_test;";
    }

    [TestMethod]
    public void GetAll_ChecksForEmptyDatabaseBeforeEntries_0()
    {
      int expected = 0;

      int actual = Book.GetAll().Count;

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Save_SavesBookToDatabase_BookList()
    {
      DateTime testPublishDate = DateTime.Now;

      Book newBook = new Book("Bible", "History", testPublishDate);

      newBook.Save();

      List<Book> expected = new List<Book> {newBook};
      List<Book> actual = Book.GetAll();

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void DeleteAll_DeletesAllBooksFromDatabase_BookList()
    {
      DateTime testPublishDate = DateTime.Now;

      Book newBook = new Book("Bible", "History", testPublishDate);

      newBook.Save();

      List<Book> expected = new List<Book> {};
      Book.DeleteAll();
      List<Book> actual = Book.GetAll();

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Find_FindBookById_Book()
    {
      DateTime testPublishDate = DateTime.Now;

      Book newBook = new Book("Bible", "History", testPublishDate);

      newBook.Save();

      Book expected = newBook;
      Book actual =  Book.Find(newBook.GetId());

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Delete_DeleteBook_ListBooks()
    {
      DateTime testPublishDate = DateTime.Now;

      Book newBook = new Book("Bible", "History", testPublishDate);

      newBook.Save();

      Book newBook2 = new Book("Harry Potter", "Fiction", testPublishDate);
      newBook2.Save();

      newBook2.Delete();

      List<Book> expected = new List<Book>{newBook};
      List<Book> actual = Book.GetAll();

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void AddAuthor_AddsAuthorToBook_AuthorList()
    {
      DateTime publishDate = DateTime.Now;
      Author newAuthor = new Author("Robert Jordan");
      newAuthor.Save();
      Book newBook = new Book("Eye of the World", "Fantasy", publishDate);
      newBook.Save();

      newBook.AddAuthor(newAuthor);

      List<Author> expected = new List<Author> {newAuthor};
      List<Author> actual = newBook.GetAuthors();

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Update_UpdatesBookInformationInDatabase_Book()
    {
      DateTime publishDate = DateTime.Now;
      Book newBook = new Book("Eye of the World", "Fantasy", publishDate);
      newBook.Save();

      newBook.Update("The Name of the Wind", "Fantasy", publishDate);

      Book expected = newBook;
      Book actual = Book.Find(newBook.GetId());

      Assert.AreEqual(expected,actual);
    }




    public void Dispose()
    {
      Author.DeleteAll();
      Book.DeleteAll();
    }
  }

}
