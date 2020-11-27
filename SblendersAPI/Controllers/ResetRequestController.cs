using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SblendersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetRequestController : ControllerBase
    {

        // GET api/<ResetRequestController>/5
        [HttpGet("{login}")]
        public Dictionary<string, string> Get(string login)
        {
            using (
             SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
             )
            using (
                SqlCommand idQueryCommand = new SqlCommand("SELECT agenteID, tipoAgenteID, agenteSalt, agenteSenha, agenteToken FROM tbAgente WHERE agenteLogin= @login;", connection)
            )
            using (SqlDataAdapter idQueryAdapter = new SqlDataAdapter(idQueryCommand))
            {
                try
                {
                    idQueryCommand.Parameters.Add(new SqlParameter("@login", login));
                    connection.Open();
                    DataTable idQuery = new DataTable();
                    idQueryAdapter.Fill(idQuery);
                    if (idQuery.Rows.Count < 1)
                    {
                        Response.StatusCode = 403;
                        return new Dictionary<string, string> { { "error", "AUTH_ERROR" } };
                    }
                    else if ((int)idQuery.Rows[0]["tipoAgenteID"] == 1)
                    {
                        string htmlString =$"<h1>Clique neste link para mudar sua senha:</h1><br/><a href='http://localhost:49926/RedefinePass.aspx?target={(int)idQuery.Rows[0]["agenteID"]}&token={(string)idQuery.Rows[0]["agenteToken"]}'>Mudar Senha</a><br/>Esta é uma mensagem automática. Não responda.";
                        MailMessage message = new MailMessage();
                        SmtpClient smtp = new SmtpClient();
                        message.From = new MailAddress("sblenders.fast.food@gmail.com");
                        message.To.Add(new MailAddress(login));
                        message.Subject = "Redefina a senha da sua conta SBLENDERS";
                        message.IsBodyHtml = true;
                        message.Body = htmlString;
                        smtp.Port = 587;
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("sblenders.fast.food@gmail.com", "wbBA6rgyGLQ5dPZ");
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(message);
                        return new Dictionary<string, string> { { "message", "SUCCESS" } }; 
                    }
                    else return null;
                }
                catch(Exception e)
                {
                    Response.StatusCode = 500;
                    return null;
                }
            }
            Response.StatusCode = 500;
            return null;
        }
    }
}
