using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace Library.Models
{
  public class Copy
  {
    private int _id;
    private int _bookId;
    private bool _checkedOut = false;
    private DateTime _checkoutDate;
    private DateTime _dueDate;

    public Copy(int bookId, bool checkedOut = false, DateTime checkoutDate = default(DateTime), DateTime dueDate = default(DateTime), int id = 0)
    {
      _id = id;
      _bookId = bookId;
      _checkedOut = checkedOut;
      _checkoutDate = checkoutDate;
      _dueDate = dueDate;
    }

    public int GetId()
    {
      return _id;
    }
    public int GetBookId()
    {
      return _bookId;
    }
    public bool GetCheckedOut()
    {
      return _checkedOut;
    }
    public DateTime GetCheckoutDate()
    {
      return _checkoutDate;
    }
    public DateTime GetDueDate()
    {
      return _dueDate;
    }

    public override bool Equals(System.Object otherCopy)
    {
      if(!(otherCopy is Copy))
      {
        return false;
      }
      else
      {
        Copy newCopy = (Copy) otherCopy;
        bool idEquality = this.GetId() == newCopy.GetId();
        bool bookIdEquality = this.GetBookId() == newCopy.GetBookId();
        bool checkedOutEquality = this.GetCheckedOut() == newCopy.GetCheckedOut();

        return (idEquality && bookIdEquality && checkedOutEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetBookId().GetHashCode();
    }

    public void Save(int copiesToAdd = 1)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO copies (book_id, checkout_date, due_date) VALUES (@bookId, @checkoutDate, @dueDate);";

      MySqlParameter idParameter = new MySqlParameter();
      idParameter.ParameterName = "@bookId";
      idParameter.Value = _bookId;
      cmd.Parameters.Add(idParameter);

      MySqlParameter checkoutDate = new MySqlParameter();
      checkoutDate.ParameterName = "@checkoutDate";
      checkoutDate.Value = _checkoutDate;
      cmd.Parameters.Add(checkoutDate);

      MySqlParameter dueDate = new MySqlParameter();
      dueDate.ParameterName = "@dueDate";
      dueDate.Value = _dueDate;
      cmd.Parameters.Add(dueDate);

      for (int i = 0; i < copiesToAdd; i++)
      {
        cmd.ExecuteNonQuery();

      }

      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Copy> GetAll()
    {
      List<Copy> allCopies = new List<Copy> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM copies;";

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        int bookId = rdr.GetInt32(1);
        bool checkedOut = (bool)rdr.GetBoolean(2);
        DateTime checkoutDate = rdr.GetDateTime(3);
        DateTime dueDate = rdr.GetDateTime(4);

        Copy newCopy = new Copy(bookId, checkedOut, checkoutDate, dueDate, id);
        allCopies.Add(newCopy);
      }
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return allCopies;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO copies (book_id, checkout_date, due_date) VALUES (@bookId, @checkoutDate, @dueDate);";

      MySqlParameter bookId = new MySqlParameter();
      bookId.ParameterName = "@bookId";
      bookId.Value = _bookId;
      cmd.Parameters.Add(bookId);

      MySqlParameter checkoutDate = new MySqlParameter();
      checkoutDate.ParameterName = "@checkoutDate";
      checkoutDate.Value = _checkoutDate;
      cmd.Parameters.Add(checkoutDate);

      MySqlParameter dueDate = new MySqlParameter();
      dueDate.ParameterName = "@dueDate";
      dueDate.Value = _dueDate;
      cmd.Parameters.Add(dueDate);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM copies; DELETE FROM copies_patrons;";

      cmd.ExecuteNonQuery();
      conn.Close();

      if(conn != null)
      {
        conn.Dispose();
      }
    }

    public static Copy Find (int id)
    {

      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM copies WHERE id = @thisId;";

      MySqlParameter idParameter = new MySqlParameter();
      idParameter.ParameterName = "@thisId";
      idParameter.Value = id;
      cmd.Parameters.Add(idParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      int myId = 0;
      int bookId = 0;
      bool checkedOut = false;
      DateTime checkoutDate = DateTime.Now;
      DateTime dueDate = DateTime.Now;

      while(rdr.Read())
      {
        myId = rdr.GetInt32(0);
        bookId = rdr.GetInt32(1);
        checkedOut = rdr.GetBoolean(2);
        checkoutDate = rdr.GetDateTime(3);
        dueDate = rdr.GetDateTime(4);
      }
      Copy foundCopy = new Copy(bookId, checkedOut, checkoutDate, dueDate, myId);
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return foundCopy;
    }

    public void CheckoutUpdate()
    {
      DateTime now = DateTime.Now;
      // var tempDate = now.ToString();
      // Console.WriteLine("STRING: " + tempDate);

      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE copies SET checked_out = @checkedOut, checkout_date = @checkoutDate, due_date = @dueDate WHERE id = @thisId;";

      MySqlParameter checkedOut = new MySqlParameter();
      checkedOut.ParameterName = "@checkedOut";
      checkedOut.Value = true;
      cmd.Parameters.Add(checkedOut);

      MySqlParameter checkoutDate = new MySqlParameter();
      checkoutDate.ParameterName = "@checkoutDate";
      checkoutDate.Value = now;
      cmd.Parameters.Add(checkoutDate);

      MySqlParameter dueDate = new MySqlParameter();
      dueDate.ParameterName = "@dueDate";
      dueDate.Value = now.AddDays(7);
      cmd.Parameters.Add(dueDate);

      MySqlParameter idParameter = new MySqlParameter();
      idParameter.ParameterName = "@thisId";
      idParameter.Value = _id;
      cmd.Parameters.Add(idParameter);

      _checkedOut = true;
      _checkoutDate = now;
      _dueDate = now.AddDays(7);

      cmd.ExecuteNonQuery();
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }

    }

    public string GetBookTitle()
    {
      return Book.Find(_bookId).GetTitle();
    }

    public static Copy FindAvailableCopy(int bookId)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText =@"SELECT * FROM copies WHERE book_id = @bookId AND checked_out = @checkedOut LIMIT 1;";

      MySqlParameter bookIdParameter  = new MySqlParameter();
      bookIdParameter.ParameterName = "@bookId";
      bookIdParameter.Value = bookId;
      cmd.Parameters.Add(bookIdParameter);

      MySqlParameter checkedOutParameter  = new MySqlParameter();
      checkedOutParameter.ParameterName = "@checkedOut";
      checkedOutParameter.Value = false;
      cmd.Parameters.Add(checkedOutParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      int myId = 0;
      int myBookId = 0;
      bool checkedOut = false;
      DateTime checkoutDate = DateTime.Now;
      DateTime dueDate = DateTime.Now;

      while(rdr.Read())
      {
        myId = rdr.GetInt32(0);
        myBookId = rdr.GetInt32(1);
        checkedOut = rdr.GetBoolean(2);
        checkoutDate = rdr.GetDateTime(3);
        dueDate = rdr.GetDateTime(4);
      }
      Copy foundCopy = new Copy(myBookId, checkedOut, checkoutDate, dueDate, myId);
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return foundCopy;
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM copies WHERE id = @thisId; DELETE FROM copies_patrons WHERE copy_id = @thisId;";

      MySqlParameter idParameter = new MySqlParameter();
      idParameter.ParameterName = "@thisId";
      idParameter.Value = _id;
      cmd.Parameters.Add(idParameter);

      cmd.ExecuteNonQuery();

      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }

  }
}
