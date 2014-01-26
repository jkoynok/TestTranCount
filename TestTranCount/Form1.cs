using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTranCount
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs ev)
        {
            try
            {
                Boolean continueProcessing = true;
                double timeOutFactor = 2;
                int sqlMaxRetryLimit = 3;
                int retryCount = 0;
                int originalSQLTimeout = 0;
                int newSQLTimeout = 0;

                using (var db  = new DataClasses1DataContext())
                {
                    db.Connection.Open();

                    originalSQLTimeout = db.CommandTimeout;
                    newSQLTimeout = originalSQLTimeout;

                    if (true)
                    {
                        //Retry logic
                        while (continueProcessing == true)
                        {
                            db.Transaction = db.Connection.BeginTransaction();
                            db.CommandTimeout = newSQLTimeout;
                     
                            try
                            {
                                db.TestTranCount();
                                db.Transaction.Commit();
                                continueProcessing = false;
                            }
                            catch (System.Data.SqlClient.SqlException e)
                            {
                                retryCount = retryCount + 1;
                                System.Diagnostics.Debug.WriteLine(e.Message);
                                //increase the timeout for the next retry
                                newSQLTimeout = db.CommandTimeout + Convert.ToInt16(db.CommandTimeout * timeOutFactor);
                                db.Transaction.Rollback();
                                if (retryCount >= sqlMaxRetryLimit)
                                {
                                    throw new Exception(e.Message);
                                }
                            }
                            finally
                            {
                                db.Transaction = null;
                            }
                        }
                    }
                }
            }

           catch (Exception x)
            {
                throw x;
            }
        }
        }
    

}