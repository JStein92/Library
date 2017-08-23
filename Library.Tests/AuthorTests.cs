using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using Library.Models;

namespace Library.Tests
{
  [TestClass]
  public class AuthorTests : IDisposable
  {
    public AuthorTests()
    {
        DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=library_test;";
    }

    [TestMethod]
    public void GetAll_ChecksForEmptyDatabaseBeforeEntries_0()
    {
      int expected = 0;

      int actual = Author.GetAll().Count;

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Save_SavesAuthorToDatabase_AuthorList()
    {
      Author newAuthor = new Author("George RR");
      newAuthor.Save();

      List<Author> expected = new List<Author> {newAuthor};
      List<Author> actual = Author.GetAll();

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void DeleteAll_DeletesAllAuthorsFromDatabase_AuthorList()
    {
      Author newAuthor = new Author("George RR");
      newAuthor.Save();

      List<Author> expected = new List<Author> {};
      Author.DeleteAll();
      List<Author> actual = Author.GetAll();

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Find_FindAuthorById_Author()
    {
      Author newAuthor = new Author("George RR");
      newAuthor.Save();

      Author expected = newAuthor;
      Author actual =  Author.Find(newAuthor.GetId());

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Delete_DeleteAuthor_ListAuthors()
    {
      Author newAuthor = new Author("George RR");
      newAuthor.Save();

      Author newAuthor2 = new Author("JK Rowling");
      newAuthor2.Save();
      newAuthor2.Delete();

      List<Author> expected = new List<Author>{newAuthor};
      List<Author> actual = Author.GetAll();

      CollectionAssert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void AddBook_AddBookToAuthor_ListBooks()
    {
      Author newAuthor = new Author("George RR");
      newAuthor.Save();

      DateTime testPublishDate = DateTime.Now;
      Book newBook = new Book("Bible", "History", testPublishDate);
      newBook.Save();

      newAuthor.AddBook(newBook);

      List<Book> expected = new List<Book>{newBook};
      List<Book> actual = newAuthor.GetBooks();

      CollectionAssert.AreEqual(expected,actual);
    }

    [TestMethod]
    public void SearchByAuthor_SearchByAuthorName_ListOfAuthors()
    {
      Author newAuthor = new Author("Robert Jordan");
      newAuthor.Save();

      Author newAuthor2 = new Author("Jordan Robert");
      newAuthor2.Save();

      Author newAuthor3 = new Author("Rob Smith");
      newAuthor3.Save();

      Author newAuthor4 = new Author("Michael");
      newAuthor4.Save();

      List<Author> expected = new List<Author>{newAuthor, newAuthor2, newAuthor3};
      List<Author> actual= Author.SearchByAuthor("Rob");

      foreach (var author in expected)
      {
        Console.WriteLine("EXPECTED: " + author.GetName());
      }
      foreach (var author in actual)
      {
        Console.WriteLine("ACTUAL: " + author.GetName());
      }


      CollectionAssert.AreEqual(expected,actual);


    }


    public void Dispose()
    {
      Book.DeleteAll();
      Author.DeleteAll();
    }

  }

}
