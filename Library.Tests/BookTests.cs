using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using Library.Models;
using System.Linq;

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

    [TestMethod]
    public void SearchByBookTitle_SearchesByBookTitleInDatabase_BookList()
    {
      DateTime publishDate = DateTime.Now;

      Book one = new Book("Eye of the World", "Fantasy", publishDate);
      Book two = new Book("As the World Turns", "Fantasy", publishDate);
      Book three = new Book("World", "Fantasy", publishDate);
      Book four = new Book("Cities and States", "Fantasy", publishDate);
      one.Save();
      two.Save();
      three.Save();
      four.Save();

      List<Book> expected = new List<Book> {one, two, three};
      List<Book> actual = Book.SearchByBookTitle("World");

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetUniqueAuthors_GetUniqueItemsBetween2Lists_ListAuthors()
    {
      DateTime publishDate = DateTime.Now;
      Book bookOne = new Book("Eye of the World", "Fantasy", publishDate);
      bookOne.Save();

      Author authorOne = new Author("Patrick");
      authorOne.Save();
      Author authorTwo = new Author("Robert");
      authorTwo.Save();
      Author authorThree = new Author("Rowling");
      authorThree.Save();

      bookOne.AddAuthor(authorOne);
      bookOne.AddAuthor(authorTwo);


      List<Author> allAuthors = Author.GetAll();
      List<Author> bookAuthors = bookOne.GetAuthors();

      List<Author> expected = new List<Author>{authorThree};
      List<Author> actual = allAuthors.Except(bookAuthors).ToList();

      foreach(var author in actual)
      {
        Console.WriteLine("UNIQUE AUTHOR: " + author.GetName());
      }
      CollectionAssert.AreEqual(expected,actual);
    }

    [TestMethod]
    public void DeleteAuthor_DeletesAuthorFromBook_ListAuthors()
    {
      DateTime publishDate = DateTime.Now;
      Book bookOne = new Book("Eye of the World", "Fantasy", publishDate);
      bookOne.Save();

      Author authorOne = new Author("Patrick");
      authorOne.Save();
      Author authorTwo = new Author("Robert");
      authorTwo.Save();

      bookOne.AddAuthor(authorOne);
      bookOne.AddAuthor(authorTwo);

      bookOne.DeleteAuthor(authorTwo);

      List<Author> expected = new List<Author>{authorOne};
      List<Author> actual = bookOne.GetAuthors();

      CollectionAssert.AreEqual(expected,actual);
    }

    [TestMethod]
    public void AddCopies_AddsCopyOfBookToCopiesTableInDB_BookCount()
    {
      DateTime publishDate = DateTime.Now;
      Book bookOne = new Book("Eye of the World", "Fantasy", publishDate);
      bookOne.Save();
      bookOne.AddCopies(5);

      int expected = 5;
      int actual = bookOne.GetCopiesCount();

      Assert.AreEqual(expected, actual);
    }

    public void Dispose()
    {
      Book.DeleteAll();
      Author.DeleteAll();
      Copy.DeleteAll();
      Patron.DeleteAll();
      Book.DeleteAllCopies();
    }
  }

}
