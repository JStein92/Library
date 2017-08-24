using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace Library.Models
{
  public class Patron
  {
    private int _id;
    private string _name;

    public Patron(string name, int id = 0)
    {
      _id = id;
      _name = name;
    }

    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _name;
    }

    public override bool Equals(System.Object otherPatron)
    {
      if(!(otherPatron is Patron))
      {
        return false;
      }
      else
      {
        Patron newPatron = (Patron) otherPatron;
        bool idEquality = this.GetId() == newPatron.GetId();
        bool nameEquality = this.GetName() == newPatron.GetName();
        return (idEquality && nameEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetName().GetHashCode();
    }

    public static List<Patron> GetAll()
    {
      List<Patron> allPatrons = new List<Patron> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM patrons;";

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);

        Patron newPatron = new Patron(name, id);
        allPatrons.Add(newPatron);
      }
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return allPatrons;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO patrons (name) VALUES (@name);";

      MySqlParameter patronName = new MySqlParameter();
      patronName.ParameterName = "@name";
      patronName.Value = _name;
      cmd.Parameters.Add(patronName);

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
      cmd.CommandText = @"DELETE FROM patrons; DELETE FROM copies_patrons;";

      cmd.ExecuteNonQuery();
      conn.Close();

      if(conn != null)
      {
        conn.Dispose();
      }
    }

    public static Patron Find (int id)
    {

      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM patrons WHERE id = @thisId;";

      MySqlParameter idParameter = new MySqlParameter();
      idParameter.ParameterName = "@thisId";
      idParameter.Value = id;
      cmd.Parameters.Add(idParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      int myId = 0;
      string name = "";

      while(rdr.Read())
      {
        myId = rdr.GetInt32(0);
        name = rdr.GetString(1);
      }
      Patron newPatron = new Patron(name, myId);
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return newPatron;
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM patrons WHERE id = @thisId; DELETE FROM copies_patrons WHERE patron_id = @thisId;";

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

    public void CheckOutCopy(int copyId)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO copies_patrons (copy_id, patron_id) VALUES (@copyId, @thisId);";

      MySqlParameter copyIdParameter = new MySqlParameter();
      copyIdParameter.ParameterName = "@copyId";
      copyIdParameter.Value = copyId;
      cmd.Parameters.Add(copyIdParameter);

      MySqlParameter patronId = new MySqlParameter();
      patronId.ParameterName = "@thisId";
      patronId.Value = _id;
      cmd.Parameters.Add(patronId);

      Copy.Find(copyId).CheckoutUpdate();

      cmd.ExecuteNonQuery();
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }

    // public void ReturnCopy(int id)
    // {
    //   MySqlConnection conn = DB.Connection();
    //   conn.Open();
    //
    //   var cmd = conn.CreateCommand() as MySqlCommand;
    //   cmd.CommandText = @"UPDATE copies SET checked_out = @checkedOut WHERE id = @copyID;";
    //
    //   MySqlParameter checkedOut = new MySqlParameter();
    //   checkedOut.ParameterName = "@checkedOut";
    //   checkedOut.Value = false;
    //   cmd.Parameters.Add(checkedOut);
    //
    //   MySqlParameter copyId = new MySqlParameter();
    //   copyId.ParameterName = "@copyId";
    //   copyId.Value = id;
    //   cmd.Parameters.Add(copyId);
    //
    //   cmd.ExecuteNonQuery();
    //   conn.Close();
    //   if(conn != null)
    //   {
    //     conn.Dispose();
    //   }
    // }

    public List<Copy> GetCopies()
    {
      List<Copy> patronCopies = new List<Copy>{};
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT copies.* FROM patrons

      JOIN copies_patrons ON(patrons.id = copies_patrons.patron_id)
      JOIN copies ON (copies_patrons.copy_id = copies.id)

      WHERE patrons.id = @thisId";

      MySqlParameter patronId = new MySqlParameter();
      patronId.ParameterName = "@thisId";
      patronId.Value = _id;
      cmd.Parameters.Add(patronId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        int bookId = rdr.GetInt32(1);
        bool checkedOut = (bool)rdr.GetBoolean(2);
        DateTime checkoutDate = rdr.GetDateTime(3);
        DateTime dueDate = rdr.GetDateTime(4);

        Copy newCopy = new Copy(bookId, checkedOut, checkoutDate, dueDate, id);
        patronCopies.Add(newCopy);
      }

      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }

      return patronCopies;
    }



  }
}
