using IS8012_FinalProject.Models;
using IS8012_FinalProject.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.AspNet.Identity;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;

namespace IS8012_FinalProject.Controllers
{
    public class ExpenseController : Controller
    {

        private ExpenseManager ExpenseManager;

        public ExpenseController()
        {
            ExpenseManager = new ExpenseManager();
        }
        // GET: Expense
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetExpenses(string userName)
        {
            if(ModelState.IsValid)
            {
                var results = ExpenseManager.GetExpenseItems(userName);
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.NotFound); ;
        }

        public ActionResult GetExpensesForCategory(string userName, string category)
        {
            if (ModelState.IsValid)
            {
                ExpenseItem[] results;
                if (string.IsNullOrEmpty(category))
                    results = ExpenseManager.GetExpenseItems(userName);
                else
                    results = ExpenseManager.GetExpenseItemsByCategory(userName, category);
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [HttpPost]
        public ActionResult CreateExpense(ExpenseItem item)
        {
            if (ModelState.IsValid)
            {
                this.ExpenseManager.SaveItem(item);
                return Content(JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd hh:mm:ss" }));
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                this.ExpenseManager.DeleteItem(id);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
                return new HttpStatusCodeResult(300);
        }

        [Authorize(Roles = "")]
        public ActionResult ExpenseReport()
        {
            ViewBag.UserName = User.Identity.GetUserName();
            return View();
        }

        [Authorize(Roles = "")]
        public ActionResult CreateReport(DateTime startDate, DateTime endDate)
        {
            ViewBag.Title = "Expense Report";
            string reportForUser = string.Empty;

            var results = ExpenseManager.GetExpensesByDate(User.Identity.GetUserName(), startDate, endDate);
            string jsonString = JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = "MM-dd-yyyy" });
            XmlDocument result = JsonConvert.DeserializeXmlNode("{\"Expense\":" + jsonString + "}", "Expenses");
            var path = Server.MapPath("~/Content/");
            result.Save(path + "\\Expenses.xml");

            /* The below code moves the data from the XML generated to the DataTable via string Lists
             * to display on the expenses on the screen 
             XML --> string Lists --> DataTable*/
            DataTable t = new DataTable();
            t.Columns.Add("ItemID", typeof(String));
            t.Columns.Add("ExpenseName", typeof(String));
            t.Columns.Add("ExpenseCategory", typeof(String));
            t.Columns.Add("Amount(in US. Dollars)", typeof(String));
            t.Columns.Add("ExpenseDate", typeof(String));

            List<String> ItemID = new List<string>();
            List<String> ExpenseName = new List<string>();
            List<String> ExpenseCategory = new List<string>();
            List<String> Amount = new List<string>();
            List<String> ExpenseDate = new List<string>();

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(Server.MapPath("~/Content/") + "\\Expenses.xml");

            /* Looping through the contents of XML using the XmlNodeReader to 
             * move the contents into different String lists*/
            XmlReader xmlReader = new XmlNodeReader(xdoc);
            while (xmlReader.Read())
            {
                if (xmlReader.IsStartElement())
                {
                    String b = xmlReader.Name;
                    switch (xmlReader.Name)
                    {
                        case "Expenses":
                            break;
                        case "Expense":
                            break;
                        case "ItemID":
                            if (xmlReader.Read())
                            {
                                ItemID.Add(xmlReader.Value.Trim());
                            }
                            break;
                        case "ExpenseName":
                            if (xmlReader.Read())
                            {
                                ExpenseName.Add(xmlReader.Value.Trim());
                            }
                            break;
                        case "ExpenseCategory":
                            if (xmlReader.Read())
                            {
                                ExpenseCategory.Add(xmlReader.Value.Trim());
                            }
                            break;
                        case "Amount":
                            if (xmlReader.Read())
                            {
                                Amount.Add(xmlReader.Value.Trim());
                            }
                            break;
                        case "ExpenseDate":
                            if (xmlReader.Read())
                            {
                                ExpenseDate.Add(xmlReader.Value.Trim());
                            }
                            break;
                        case "UserName":
                            if (xmlReader.Read())
                            {
                                reportForUser = xmlReader.Value.Trim();
                            }
                            break;
                    }
                }
            }

            int counter = 0;
            decimal total = 0;

            /* creating a two dimensional array to hold the expenses category wise 
             for the bar graph . The first part of the code identifies the distinct categories. The 
             second part assigns amount to each of the categories*/
            List<string> distinctCategory = new List<string>();
            distinctCategory.AddRange(ExpenseCategory.Distinct());
            string[,] catergoryArray = new string[distinctCategory.Count, 2];
            for (int i = 0; i < distinctCategory.Count; i++)
            {
                catergoryArray[i, 0] = distinctCategory[i];
                catergoryArray[i, 1] = "0";
            }

            /* moving the contents from the string Lists to the DataTable */
            foreach (string line in ItemID)
            {
                DataRow r = t.NewRow();
                r["ItemID"] = counter + 1;
                r["ExpenseName"] = ExpenseName[counter];
                r["ExpenseCategory"] = ExpenseCategory[counter];
                r["Amount(in US. Dollars)"] = Amount[counter];
                r["ExpenseDate"] = ExpenseDate[counter];
                t.Rows.Add(r);

                total += decimal.Parse(Amount[counter]);

                //creating expenses category wise 
                for (int i = 0; i < (catergoryArray.Length) / 2; i++)
                {
                    if (ExpenseCategory[counter].Equals(catergoryArray[i, 0]))
                    {
                        int sum = Convert.ToInt32(catergoryArray[i, 1]) + Convert.ToInt32(Amount[counter]);
                        catergoryArray[i, 1] = sum.ToString();
                        break;
                    }
                }
                counter += 1;
            }

            //CreateCategoryXml creates an Xml on runtime for the differnt Categories and the amonts 
            // This Xml will be further used to generate the bar graph
            CreateCategoryXml(catergoryArray);

            // CreateWordDoc method creates Expense Report between the selected Dates
            // The input for the method is the DataTable, Total amount of all expenses and the user for whom the report is generated
            CreateWordDoc(t, total, reportForUser);

            //Applying xslt on the category xml
            XslCompiledTransform transform = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings();
            settings.EnableScript = true;
            transform.Load(path + "\\Transform.xsl", settings, null);
            transform.Transform(path + "\\Expenses.xml", path + "\\Expenses.html");
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        public void CreateWordDoc(DataTable data, decimal t, string user)
        {
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathDownload = Path.Combine(pathUser, "Downloads");
            //string pathDownload = Server.MapPath("~/Content/");
            decimal total = t;

            try
            {
                //Set the current directory.
                Directory.SetCurrentDirectory(pathDownload);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("The specified directory does not exist. {0}", e);
            }

            WordprocessingDocument doc = WordprocessingDocument.Create("ExpenseReport.docx", WordprocessingDocumentType.Document);
            MainDocumentPart mainDocPart = doc.AddMainDocumentPart();
            mainDocPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
            DocumentFormat.OpenXml.Wordprocessing.Body body = new DocumentFormat.OpenXml.Wordprocessing.Body();
            mainDocPart.Document.Append(body);
            DocumentFormat.OpenXml.Wordprocessing.Table table = new DocumentFormat.OpenXml.Wordprocessing.Table();

            //border
            TableProperties tblProp = new TableProperties(
             new TableBorders(
             new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 10 })
             );
            // Append the TableProperties object to the empty table.
            table.AppendChild<TableProperties>(tblProp);

            //setting header 
            DocumentFormat.OpenXml.Wordprocessing.TableRow tr = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
            DocumentFormat.OpenXml.Wordprocessing.RunProperties rp = new DocumentFormat.OpenXml.Wordprocessing.RunProperties();
            rp.Append(new DocumentFormat.OpenXml.Wordprocessing.Color() { Val = "#FF0000" });
            RunFonts rFont1 = new RunFonts();
            rFont1.Ascii = "Arial";
            rp.Append(rFont1);
            rp.Append(new Bold());
            rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "28" });

            DocumentFormat.OpenXml.Wordprocessing.Run run = new DocumentFormat.OpenXml.Wordprocessing.Run();
            run.RunProperties = rp;
            run.Append(new Text("Expense Report for " + user));
            DocumentFormat.OpenXml.Wordprocessing.Paragraph para = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(run);
            DocumentFormat.OpenXml.Wordprocessing.TableCellProperties tcpp = new DocumentFormat.OpenXml.Wordprocessing.TableCellProperties();
            tcpp.Append(new DocumentFormat.OpenXml.Wordprocessing.TableCellWidth { Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa, Width = "2200" });
            GridSpan gs = new GridSpan();
            gs.Val = 5;
            tcpp.Append(gs);
            DocumentFormat.OpenXml.Wordprocessing.TableCell tc = new DocumentFormat.OpenXml.Wordprocessing.TableCell(tcpp, para);
            tr.Append(tc);
            table.Append(tr);

            DocumentFormat.OpenXml.Wordprocessing.TableRow row_header = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
            foreach (DataColumn column in data.Columns)
            {
                DocumentFormat.OpenXml.Wordprocessing.TableCell cell = new DocumentFormat.OpenXml.Wordprocessing.TableCell();
                cell.Append(new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(column.ToString()))));
                cell.Append(new DocumentFormat.OpenXml.Wordprocessing.TableCellProperties(new DocumentFormat.OpenXml.Wordprocessing.TableCellWidth { Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa, Width = "2200" }));
                row_header.Append(cell);
            }

            table.Append(row_header);

            for (int i = 0; i < data.Rows.Count; ++i)
            {
                DocumentFormat.OpenXml.Wordprocessing.TableRow row = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    DocumentFormat.OpenXml.Wordprocessing.TableCell cell = new DocumentFormat.OpenXml.Wordprocessing.TableCell();
                    cell.Append(new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(data.Rows[i][j].ToString()))));
                    cell.Append(new DocumentFormat.OpenXml.Wordprocessing.TableCellProperties(new DocumentFormat.OpenXml.Wordprocessing.TableCellWidth { Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Dxa, Width = "2200" }));
                    row.Append(cell);
                }
                table.Append(row);
            }
            body.Append(table);

            Run run1 = new Run();
            Paragraph para1 = new Paragraph(run1);
            run1.AppendChild(new Text("The total Expenditure is " + total));
            body.Append(para1);
            doc.MainDocumentPart.Document.Save();
            doc.Dispose();
        }

        public void CreateCategoryXml(string[,] CategoryAndValues)
        {
            string pathDownload = Server.MapPath("~/Content/");
            try
            {
                //Set the current directory.
                Directory.SetCurrentDirectory(pathDownload);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("The specified directory does not exist. {0}", e);
            }

            string fileName = "CategoryData.xml";
            XmlWriter xmlWriter = XmlWriter.Create(fileName);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Expenses");

            for (int i = 0; i < (CategoryAndValues.Length) / 2; i++)
            {
                xmlWriter.WriteStartElement("Expense");
                xmlWriter.WriteStartElement("ExpenseCategory");
                xmlWriter.WriteValue(CategoryAndValues[i, 0]);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("Amount");
                xmlWriter.WriteValue(CategoryAndValues[i, 1]);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

    }
}