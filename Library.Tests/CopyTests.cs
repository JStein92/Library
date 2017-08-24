using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using Library.Models;

namespace Library.Tests
{
  [TestClass]
  public class CopyTests : IDisposable
  {
    public CopyTests()
    {
        DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=library_test;";
    }

    [TestMethod]
    public void GetAll_ChecksForEmptyDatabaseBeforeEntries_0()
    {
      int expected = 0;

      int actual = Copy.GetAll().Count;

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Save_SavesCopyToDatabase_CopyList()
    {
      Copy newCopy = new Copy(0);
      newCopy.Save();

      List<Copy> expected = new List<Copy> {newCopy};
      List<Copy> actual = Copy.GetAll();

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void DeleteAll_DeletesAllCopysFromDatabase_CopyList()
    {
      Copy newCopy = new Copy(0);
      newCopy.Save();

      List<Copy> expected = new List<Copy> {};
      Copy.DeleteAll();
      List<Copy> actual = Copy.GetAll();

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Find_FindCopyById_Copy()
    {
      Copy newCopy = new Copy(0);
      newCopy.Save();

      Copy expected = newCopy;
      Copy actual =  Copy.Find(newCopy.GetId());

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Delete_DeleteCopy_ListCopys()
    {
      Copy newCopy = new Copy(0);
      newCopy.Save();

      Copy newCopy2 = new Copy(1);
      newCopy2.Save();
      newCopy2.Delete();

      List<Copy> expected = new List<Copy>{newCopy};
      List<Copy> actual = Copy.GetAll();



      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void CheckoutUpdate_UpdatesCopyInformationInDatabase_Copy()
    {
      Patron newPatron = new Patron("Jon");
      newPatron.Save();

      DateTime publishDate = DateTime.Now;
      Book bookOne = new Book("Eye of the World", "Fantasy", publishDate);
      bookOne.Save();

      Copy newCopy = new Copy(bookOne.GetId());
      newCopy.Save(10);

      newPatron.CheckOutCopy(newCopy.GetId());

      Copy expected = newCopy;
      Copy actual = Copy.Find(newCopy.GetId());

      Console.WriteLine("EXPECTED: " + newCopy.GetCheckoutDate());
      Console.WriteLine("ACTUAL: " + actual.GetCheckoutDate());

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
