using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApiAutoresV2.DTOs;

namespace WebApiAutoresV2.Servicios
{
    public class HashService
    {
        public ResultadoHash Hash(string textoPlano)
        {
            var sal = new byte[16];
            using(var ramdom = RandomNumberGenerator.Create())
            {
                ramdom.GetBytes(sal);
            }
            return Hash(textoPlano, sal);
        }

        public ResultadoHash Hash(string textoPlano, byte[] sal)
        {
            var llaveDerivada = KeyDerivation.Pbkdf2(
                password: textoPlano,
                salt: sal,
                prf:KeyDerivationPrf.HMACSHA256,
                iterationCount:1000,
                numBytesRequested:32);
            var hash = Convert.ToBase64String(llaveDerivada);
            return new ResultadoHash(){
                Hash = hash,
                Sal = sal
            };
        }
    }
}
