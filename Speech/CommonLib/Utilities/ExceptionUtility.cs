namespace LiebaoAp.Common.Utilities
{
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    internal static class ExceptionUtility
    {
        public static string GetDetail(this Exception exception)
        {
            var stringBuilder = new StringBuilder(256);

            var printAction = new Action<string>(
                msg => stringBuilder.AppendLine(msg));

            var printWebExceptionAction = new Action<WebException>(
                webException =>
                {
                    printAction("### Web Exception Detail ###");

                    printAction("Status: " + webException.Status);

                    var webResponse = webException.Response;

                    if (webResponse != null)
                    {
                        printAction("Response Uri: " + webResponse.ResponseUri);

                        var headers = string.Join(
                            ";",
                            webResponse.Headers.AllKeys.Select(
                                key => string.Format("{0}={1}", key, webResponse.Headers[key])));
                        printAction("Response Headers: " + headers);

                        var responseStream = webResponse.GetResponseStream();

                        if (responseStream != null && responseStream.CanRead)
                        {
                            using (var streamReader = new StreamReader(responseStream))
                            {
                                printAction("Response Body: " + streamReader.ReadToEnd());
                            }
                        }

                        webResponse.Close();
                    }
                });

            var printSqlExceptionAction = new Action<SqlException>(
                sqlException =>
                {
                    printAction("### SQL Exception Detail ###");

                    printAction("ClientConnectionId: " + sqlException.ClientConnectionId);
                    printAction("Server: " + sqlException.Server);
                    printAction("Class: " + sqlException.Class);
                    printAction("State: " + sqlException.State);

                    for (var i = 0; i < sqlException.Errors.Count; i++)
                    {
                        printAction("SQL Error: #" + i.ToString("D2"));

                        var sqlError = sqlException.Errors[i];

                        printAction("Error Message: " + sqlError.Message);
                        printAction("Error Number: " + sqlError.Number);
                        printAction("LineNumber: " + sqlError.LineNumber);
                        printAction("Source: " + sqlError.Source);
                        printAction("Procedure: " + sqlError.Procedure);
                    }
                });

            var printExceptionAction = new Action<Exception>(
                ex =>
                {
                    printAction("Exception Type: " + ex.GetType().AssemblyQualifiedName);
                    printAction("Exception Message: " + ex.Message);
                    printAction("Exception StackTrace: " + ex.StackTrace);

                    var webException = ex as WebException;
                    if (webException != null)
                    {
                        printWebExceptionAction(webException);
                    }

                    var sqlException = ex as SqlException;
                    if (sqlException != null)
                    {
                        printSqlExceptionAction(sqlException);
                    }
                });

            var aggregrateException = exception as AggregateException;

            var currentException = aggregrateException != null ? aggregrateException.Flatten() : exception;

            printExceptionAction(currentException);

            if (currentException.InnerException != null)
            {
                printAction("==========Inner Exceptions==========");

                currentException = currentException.InnerException;

                while (currentException != null)
                {
                    printAction(null);
                    printExceptionAction(currentException);

                    currentException = currentException.InnerException;
                }
            }

            return stringBuilder.ToString();
        }
    }
}