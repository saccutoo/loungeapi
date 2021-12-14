using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ELIB.API.Extensions
{
    public static class FormFileExtensions
    {
        public const int FileMaximumBytes = 50 * 1024 * 1024;
        public static List<string> lstAllowExtensions = new List<string>{
            ".doc",
            ".docx",
            ".odt",
            ".pdf",
            ".xls",
            ".xlsx",
            ".ods",
            ".ppt",
            ".pptx",
            ".txt",
            ".7z",
            ".zip"};

        public static bool IsNotExecutableFile(this IFormFile postedFile)
        {
            //-------------------------------------------
            //  Check the pdf is null
            //-------------------------------------------
            if (postedFile == null)
            {
                return false;
            }

            //-------------------------------------------
            //  Check the pdf extension
            //-------------------------------------------
            if (!lstAllowExtensions.Contains(Path.GetExtension(postedFile.FileName).ToLower()))
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.OpenReadStream().CanRead)
                {
                    return false;
                }
                //------------------------------------------
                //check whether the pdf size exceeding the limit or not
                //------------------------------------------ 
                if (postedFile.Length > FileMaximumBytes)
                {
                    return false;
                }

                //byte[] buffer = new byte[FileMaximumBytes];
                //postedFile.OpenReadStream().Read(buffer, 0, FileMaximumBytes);
                //string content = System.Text.Encoding.UTF8.GetString(buffer);
                //if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                //    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                //{
                //    return false;
                //}
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool IsPDFValid(this IFormFile postedFile)
        {
            //-------------------------------------------
            //  Check the pdf is null
            //-------------------------------------------
            if (postedFile == null)
            {
                return false;
            }
            //-------------------------------------------
            //  Check the pdf mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "application/pdf"
                && postedFile.ContentType.ToLower() != "application/octet-stream")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the pdf extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".pdf")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.OpenReadStream().CanRead)
                {
                    return false;
                }
                //------------------------------------------
                //check whether the pdf size exceeding the limit or not
                //------------------------------------------ 
                if (postedFile.Length > FileMaximumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[FileMaximumBytes];
                postedFile.OpenReadStream().Read(buffer, 0, FileMaximumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
