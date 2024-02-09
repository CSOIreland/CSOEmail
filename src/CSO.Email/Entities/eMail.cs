using System.Net.Mail;
using Newtonsoft.Json;

namespace CSO.Email
{
    /// <summary>
    /// Mail Message 
    /// </summary>
    public class eMail : MailMessage
    {
        #region Properties
        #endregion
        public eMail()
        {
        }


        #region Methods
        /// <summary>
        /// Send an Email
        /// </summary>
        /// <param name="configDict"></param>
        /// <param name="LogObject"></param>
        /// <returns></returns>
        public bool Send(IDictionary<string, string> configDict, dynamic LogObject)
        {
            /// <summary>
            /// Swtich on/off the service
            /// </summary>
            bool API_EMAIL_ENABLED = Convert.ToBoolean(configDict["API_EMAIL_ENABLED"]);
           
            /// <summary>
            /// NoReply email address
            /// </summary>
            string API_EMAIL_MAIL_NOREPLY = configDict["API_EMAIL_MAIL_NOREPLY"];

            /// <summary>
            /// Sender email address
            /// </summary>
            string API_EMAIL_MAIL_SENDER = configDict["API_EMAIL_MAIL_SENDER"];

            /// <summary>
            /// Server IP address
            /// </summary>
            string API_EMAIL_SMTP_SERVER = configDict["API_EMAIL_SMTP_SERVER"];

            /// <summary>
            /// Port number
            /// </summary>
            string API_EMAIL_SMTP_PORT = configDict["API_EMAIL_SMTP_PORT"];

            /// <summary>
            /// Flag to indicate if SMTP authentication is required
            /// </summary>
            bool API_EMAIL_SMTP_AUTHENTICATION = Convert.ToBoolean(configDict["API_EMAIL_SMTP_AUTHENTICATION"]);

            /// <summary>
            /// Username if authentication is required
            /// </summary>
            string API_EMAIL_SMTP_USERNAME = configDict["API_EMAIL_SMTP_USERNAME"];

            /// <summary>
            /// Password if authentication is required
            /// </summary>
            string API_EMAIL_SMTP_PASSWORD = configDict["API_EMAIL_SMTP_PASSWORD"];

            /// <summary>
            /// Flag to indicate if SSL is required
            /// </summary>
            bool API_EMAIL_SMTP_SSL = Convert.ToBoolean(configDict["API_EMAIL_SMTP_SSL"]);

            /// <summary>
            /// Template Datetime Mask
            /// </summary>
            //    private readonly string API_EMAIL_DATETIME_MASK = ApiServicesHelper.ApiConfiguration.Settings["API_EMAIL_DATETIME_MASK"];

            LogObject.Info("Email Enabled: " + API_EMAIL_ENABLED);
            LogObject.Info("Email NoReply: " + API_EMAIL_MAIL_NOREPLY);
            LogObject.Info("Email Sender: " + API_EMAIL_MAIL_SENDER);
            LogObject.Info("SMTP Server: " + API_EMAIL_SMTP_SERVER);
            LogObject.Info("SMTP Port: " + API_EMAIL_SMTP_PORT);
            LogObject.Info("SMTP Authentication: " + API_EMAIL_SMTP_AUTHENTICATION);
            LogObject.Info("SMTP Username: " + API_EMAIL_SMTP_USERNAME);
            LogObject.Info("SMTP Password: ********"); // Hide API_EMAIL_SMTP_PAsSSWORD from logs
            LogObject.Info("SMTP SSL: " + API_EMAIL_SMTP_SSL);

            if (!API_EMAIL_ENABLED)
            {
                return false;
            }

            try
            {
                // Initiate new SMTP Client
                SmtpClient smtpClient = new SmtpClient(API_EMAIL_SMTP_SERVER, Convert.ToInt32(API_EMAIL_SMTP_PORT));
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                if (API_EMAIL_SMTP_AUTHENTICATION
                && !string.IsNullOrWhiteSpace(API_EMAIL_SMTP_USERNAME)
                && !string.IsNullOrWhiteSpace(API_EMAIL_SMTP_PASSWORD))
                {
                    // Use authentication if any
                    smtpClient.Credentials = new System.Net.NetworkCredential(API_EMAIL_SMTP_USERNAME, API_EMAIL_SMTP_PASSWORD);
                    smtpClient.UseDefaultCredentials = true;
                }

                if (API_EMAIL_SMTP_SSL)
                {
                    // Use SSL if any
                    smtpClient.EnableSsl = true;
                }

                // Override Sender, From, Reply To for security
                this.ReplyToList.Clear();
                this.ReplyToList.Add(new MailAddress(API_EMAIL_MAIL_NOREPLY));
                this.From = new MailAddress(API_EMAIL_MAIL_SENDER);
                this.Sender = new MailAddress(API_EMAIL_MAIL_SENDER);

                // Set the HTML body
                this.IsBodyHtml = true;

                // Send the mail
                smtpClient.Send(this);

                LogObject.Info("eMail sent");
                return true;
            }
            catch (Exception e)
            {
                LogObject.Fatal(e);
                return false;
            }
        }

        /// <summary>
        /// Parse a Template located in Properties.Resources
        /// </summary>
        /// <param name="template"></param>
        /// <param name="eMail_KeyValuePair"></param>
        /// <param name="LogObject"></param>
        /// <returns></returns>
        public string ParseTemplate(string template, List<eMail_KeyValuePair> eMail_KeyValuePair, dynamic LogObject)
        {

            LogObject.Info("eMail String-Template to parse: " + template);
            LogObject.Info("eMail List to parse: " + JsonConvert.SerializeObject(eMail_KeyValuePair, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            try
            {
                // Parse nodes
                foreach (var item in eMail_KeyValuePair)
                {
                    template = template.Replace(item.key, item.value);
                }

                return template;

            }
            catch (Exception e)
            {
                LogObject.Fatal(e);
                throw;
            }
        }
        #endregion
    }

    public class eMail_KeyValuePair
    {
        #region Properties
        /// <summary>
        /// Key to parse
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// Value to parse
        /// </summary>
        public string value { get; set; }

        #endregion

        /// <summary>
        /// Initialise a blank one 
        /// </summary>
        public eMail_KeyValuePair()
        {
            key = null;
            value = null;
        }
    }

}
