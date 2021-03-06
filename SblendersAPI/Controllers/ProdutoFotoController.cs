﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SblendersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoFotoController : ControllerBase
    {

        // GET: api/ProdutoFoto/5
        [HttpGet("{id}", Name = "GetProductPhoto")]
        public IActionResult Get(int id)
        {
            using (
                SqlConnection connection = new SqlConnection(string.Format("User ID={0}; Password={1}; Initial Catalog={2}; Persist Security Info=True;Data Source={3}", Program.dbLogin, Program.dbPass, "dbSblenders", Program.dbEnv))
            )
            using (
               SqlCommand selectProductPhotoCommand = new SqlCommand("SELECT produtoFoto FROM tbProduto WHERE produtoID = @id", connection)
                )
            {
                connection.Open();
                selectProductPhotoCommand.Parameters.Add(new SqlParameter("@id", id));
                
                object photoFile = selectProductPhotoCommand.ExecuteScalar();
                if(photoFile != DBNull.Value)
                {
                    return File((byte[])photoFile, "image/jpg");
                }
                else
                {
                    return null;
                }
                
                
            }
        }

    }
}
