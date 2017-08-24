using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using Library.Models;

namespace Library.Tests
{
  [TestClass]
  public class PatronTests : IDisposable
  {
    public PatronTests()
    {
        DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=library_test;";
    }

    [TestMethod]
    public void GetAll_ChecksForEmptyDatabaseBeforeEntries_0()
    {
      int expected = 0;

      int actual = Patron.GetAll().Count;

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Save_SavesPatronToDatabase_PatronList()
    {
      Patron newPatron = new Patron("Jon");
      newPatron.Save();

      List<Patron> expected = new List<Patron> {newPatron};
      List<Patron> actual = Patron.GetAll();

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void DeleteAll_DeletesAllPatronsFromDatabase_PatronList()
    {
      Patron newPatron = new Patron("Jon");
      newPatron.Save();

      List<Patron> expected = new List<Patron> {};
      Patron.DeleteAll();
      List<Patron> actual = Patron.GetAll();

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Find_FindPatronById_Patron()
    {
      Patron newPatron = new Patron("Jon");
      newPatron.Save();

      Patron expected = newPatron;
      Patron actual =  Patron.Find(newPatron.GetId());

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Delete_DeletePatron_ListPatrons()
    {
      Patron newPatron = new Patron("Jon");
      newPatron.Save();

      Patron newPatron2 = new Patron("Susan");
      newPatron2.Save();
      newPatron2.Delete();

      List<Patron> expected = new List<Patron>{newPatron};
      List<Patron> actual = Patron.GetAll();

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void CheckOutCopy_ChecksOutCopyOfBook_ListOfBooks()
    {
      Patron newPatron = new Patron("Jon");
      newPatron.Save();

      DateTime publishDate = DateTime.Now;
      Book bookOne = new Book("Eye of the World", "Fantasy", publishDate);
      bookOne.Save();

      Copy newCopy = new Copy(bookOne.GetId());
      newCopy.Save(10);

      newPatron.CheckOutCopy(newCopy.GetId());

      List<Copy> actual = newPatron.GetCopies();
      List<Copy> expected = new List<Copy>{newCopy};

      CollectionAssert.AreEqual(expected,actual);
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
