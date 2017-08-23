using Microsoft.AspNetCore.Mvc;
using Library.Models;
using System.Collections.Generic;
using System;

namespace Library.Controllers
{
  public class HomeController : Controller
  {
    [HttpGet("/")]
    public ActionResult Index()
    {
      return View();
    }

    [HttpGet("/authors")]
    public ActionResult Authors()
    {
      return View(Author.GetAll());
    }

    [HttpGet("/books")]
    public ActionResult Books()
    {
      return View(Book.GetAll());
    }

    [HttpGet("/bookForm")]
    public ActionResult BookForm()
    {

      return View(Author.GetAll());
    }

    [HttpPost("/book-add-confirmation")]
    public ActionResult BookAddConfirmation()
    {
      string bookTitle = Request.Form["title"];
      string bookGenre = Request.Form["genre"];
      DateTime publishDate = DateTime.Parse(Request.Form["publish-date"]);
      Book newBook = new Book(bookTitle,bookGenre,publishDate);
      newBook.Save();

      string authorValues = (Request.Form["authors"]);
      string[] split = authorValues.Split(',');

      foreach(var authorId in split)
      {
        newBook.AddAuthor(Author.Find(int.Parse(authorId)));
      }

      return View(newBook);
    }

    [HttpGet("/authorForm")]
    public ActionResult AuthorForm()
    {
      return View();
    }

    [HttpPost("/author-add-confirmation")]
    public ActionResult AuthorAddConfirmation()
    {
      Author newAuthor = new Author(Request.Form["author-name"]);
      newAuthor.Save();

      return View(newAuthor);

    }

    [HttpPost("/delete-book/{id}")]
    public ActionResult BookDeleted(int id)
    {
      Book foundBook = Book.Find(id);
      foundBook.Delete();

      return View("Books", Book.GetAll());
    }

    [HttpPost("/book-search-results")]
    public ActionResult BooksSearchResults()
    {
      string userSearch = Request.Form["book-search"];
      List<Book> searchResults = Book.SearchByBookTitle(userSearch);
      return View("Books", searchResults);
    }

    [HttpPost("/author-search-results")]
    public ActionResult AuthorSearchResults()
    {
      string userSearch = Request.Form["author-search"];
      List<Author> searchResults = Author.SearchByAuthor(userSearch);
      return View("Authors", searchResults);
    }

    [HttpGet("/book-details/{id}")]
    public ActionResult BookDetails(int id)
    {
      Dictionary<string, object> model = new Dictionary<string, object> {};

      Book newBook = Book.Find(id);
      List<Author> allAuthors = Author.GetAll();

      model.Add("book", newBook);
      model.Add("authors", allAuthors);

      return View(model);
    }

    [HttpPost("/book-update/{id}")]
    public ActionResult BookDetailsUpdate(int id)
    {
      Book foundBook = Book.Find(id);

      string newTitle = Request.Form["title"];
      string newGenre = Request.Form["genre"];
      DateTime newPubDate = DateTime.Parse(Request.Form["publish-date"]);

      foundBook.Update(newTitle, newGenre, newPubDate);

      return View("Books", Book.GetAll());
    }
  }

}
