using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing;
namespace SharePointSPSiteDataQuery
{
    public partial class Qeury : Form
    {
        public Qeury()
        {
            InitializeComponent();
        }

        private void SPSiteDataQuery_Click(object sender, EventArgs e)
        {
            using (SPSite _site = new SPSite("http://home/"))
            {
                using (SPWeb _web = _site.OpenWeb())
                { 
                    SPSiteDataQuery query = new SPSiteDataQuery(); 
                    // Search in doclibs only

                    ////////////////////////////////////////
                    /////// ------------------------- //////
                    /////// BaseType 	        Value //////
                    /////// ------------------------- //////
                    /////// Generic List	    0     //////   
                    /////// Document Library	1     //////
                    /////// Discussion Board	3     //////
                    /////// Survey	            4     //////
                    /////// Issue	            5     //////
                    ////////////////////////////////////////

                    query.Lists = "<Lists BaseType='1' />"; 
                    // Only .doc files
                    query.Query =
                    @"<Where>
          <Eq>
            <FieldRef Name='DocIcon' />
            <Value Type='Computed'>docx</Value>
          </Eq>
        </Where>"; 
                    // Select only needed columns: file reference
                    query.ViewFields = "<FieldRef Name='FileRef' />"; 
                    // Search in all webs of the site collection
                    query.Webs = "<Webs Scope='SiteCollection' />"; 
                    // Perform the query
                    DataTable table = _web.GetSiteData(query); 
                    // Generate an absolute url for each document
                    foreach (DataRow row in table.Rows)
                    {
                        string relativeUrl =
                          row["FileRef"].ToString().Substring(
                            row["FileRef"].ToString().IndexOf("#") + 1);
                        string fullUrl = _site.MakeFullUrl(relativeUrl); 
                        // Write urls to window
                        MessageBox.Show(fullUrl);
                    }
                }
            }




        }

        private void SPCrossListQeuryInfo_Click(object sender, EventArgs e)
        {
            using (SPSite _site = new SPSite("http://home/"))
            {
                using (SPWeb _web = _site.OpenWeb())
                {
                    CrossListQueryInfo query = new CrossListQueryInfo();
                    query.RowLimit = 100;
                    query.WebUrl = _web.ServerRelativeUrl; 
                    query.Query = "<Where><Neq><FieldRef Name=\"ContentType\" /><Value Type=\"Text\"></Value></Neq></Where>";
                   
                    
                    ////////////////////////////////////////
                    /////// ------------------------- //////
                    /////// BaseType 	        Value //////
                    /////// ------------------------- //////
                    /////// Generic List	    0     //////   
                    /////// Document Library	1     //////
                    /////// Discussion Board	3     //////
                    /////// Survey	            4     //////
                    /////// Issue	            5     //////
                    ////////////////////////////////////////

                    // Search in doclibs only
                    query.Lists = "<Lists BaseType='1' />"; 
                    // Only .doc files
                    query.Query =
                    @"<Where>
          <Eq>
            <FieldRef Name='DocIcon' />
            <Value Type='Computed'>docx</Value>
          </Eq>
        </Where>";

                    // Select only needed columns: file reference
                    query.ViewFields = "<FieldRef Name='FileRef' />";

                    // Search in all webs of the site collection
                 
                    query.Webs = "<Webs Scope='SiteCollection' />"; 

                    CrossListQueryCache cache = new CrossListQueryCache(query);
 
                    // Perform the query
                    DataTable table = cache.GetSiteData(_web);
                    // Generate an absolute url for each document
                    foreach (DataRow row in table.Rows)
                    {
                        string relativeUrl =
                          row["FileRef"].ToString().Substring(
                            row["FileRef"].ToString().IndexOf("#") + 1);
                        string fullUrl = _site.MakeFullUrl(relativeUrl);

                        // Write urls to console
                        MessageBox.Show(fullUrl);
                    }
                }
            }
        }

        private void btnSpQuery_Click(object sender, EventArgs e)
        {
            MessageBox.Show("http://sharepointfordeveloper.blogspot.in/2011/10/step-by-step-spquery-list-joins.html");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string siteUrl = "http://home";
            SPWeb _web = new SPSite(siteUrl).OpenWeb();
            var items = _web.Lists["Customer"].GetItems(GetQuery());
            foreach (SPListItem item in items)
            {

                MessageBox.Show(string.Format("{0}----{1}", item["Title"], item["CityTitle"]));

            }

        }

        private SPQuery GetQuery()
        {
            SPQuery _query = new SPQuery();
            _query.Query = "";

            _query.Joins = @"<Join Type='INNER' ListAlias='City'> 
                          <!--List Name: CustomerCity--> 
                          <Eq>
                             <FieldRef Name='City' RefType='ID' /> 
                            <FieldRef List='City' Name='ID' /> 
                          </Eq> 
                        </Join>";

            _query.ProjectedFields = @"<Field Name='CityTitle' Type='Lookup' List='City' ShowField='Title' />
 
                                    <Field Name='CityContentTypeId' Type='Lookup' List='City' ShowField='ContentTypeId' />";


            _query.ViewFields = @" <FieldRef Name='Title' />
 
                                     <FieldRef Name='CityTitle' />";

            return _query;

        } 
    }
}
