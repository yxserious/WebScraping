using System.Globalization;
using CsvHelper;
using HtmlAgilityPack;

namespace WebScraping
{
    class Book
    {
        public string? Title { get; set; }
        public string? Price { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            //starting point empty list string object
            //add all the links to this object and return them

            List<string> bookLinks = GetBookLinks(url: "http://books.toscrape.com/catalogue/category/books/mystery_3/index.html");
            Console.WriteLine(format: "Found {0} links", arg0: bookLinks.Count);
            List<Book> books = GetBookDetails(urls: bookLinks);
            exportToCSV(books);
        }

        static void exportToCSV(List<Book>books)
        {
            //create streamwriter and send the csv file name as parameter
            //use writer record function to write all the books in single line of code
            using (StreamWriter writer = new StreamWriter(path: "./books.csv")) 
            using( CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(books);
            }

        }

        static HtmlDocument GetDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            return doc;
        }

        static List<Book> GetBookDetails (List<string> urls)
        {
            List<Book> books = new List<Book>();
            foreach (string url in urls)
            {
                HtmlDocument document = GetDocument(url);
                string titleXPath = "//h1";
                string priceXPath = "//div[contains(@class,\"product_main\")]/p[@class=\"price_color\"]";
                Book book = new Book();
                book.Title = document.DocumentNode.SelectSingleNode(xpath: titleXPath).InnerText;
                book.Price = document.DocumentNode.SelectSingleNode(xpath: priceXPath).InnerText;
                books.Add(book);
            }
            return books;
        }

        static List<string> GetBookLinks(string url)
        {
            List<string> bookLinks = new List<string>();

            HtmlDocument doc = GetDocument(url);

            HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes(xpath: "//h3/a");

            //using the consturctor to get the URI object with an absolute URL
            Uri baseUri = new Uri(uriString: url);

            foreach(var node in linkNodes)
            {
                string href = node.Attributes[name: "href"].Value;
                bookLinks.Add(new Uri(baseUri, relativeUri:href).AbsoluteUri);
            }

            return bookLinks;
        }

    }
}
