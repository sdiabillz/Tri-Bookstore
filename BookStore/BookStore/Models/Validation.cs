using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace BookStore.Models
{

    public class checkSingularDates : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContex)
        {

            string date = null;
            if (value != null)
            {
                date = value.ToString();
                DateTime p;

                string[] formats = { "MM/dd/yyyy", "M/d/yyyy", "M/dd/yyyy", "MM/d/yyyy", "MM-dd-yyyy", "M-d-yyyy", "M-dd-yyyy", "MM-d-yyyy" };
                string[] formats2 = { "yyyy/MM/dd", "yyyy/M/d", "yyyy/M/dd", "yyyy/MM/d", "yyyy-MM-dd", "yyyy-M-d", "yyyy-M-dd", "yyyy-MM-d" };
                string[] formats3 = { "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy", "dd-MM-yyyy", "d-M-yyyy", "dd-M-yyyy", "d-MM-yyyy" };

                // validating format first
                if (!DateTime.TryParseExact(date, formats2, new CultureInfo("en-US"),
                            DateTimeStyles.None, out p))
                {
                    DateTime temp;
                    //validate date value
                    if (DateTime.TryParse(date, out temp))
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        int year = Int16.Parse((date.Substring(0, 4)));
                        if (DateTime.IsLeapYear(year))
                        {
                            return ValidationResult.Success;
                        }
                        else
                        {
                            return new ValidationResult("The Date entered is invalid");
                        }
                    }
                }
                else if (!DateTime.TryParseExact(date, formats, new CultureInfo("en-US"),
                            DateTimeStyles.None, out p))
                {
                    DateTime temp;
                    //validate date value
                    if (DateTime.TryParse(date, out temp))
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        int year = Int16.Parse((date.Substring(0, 4)));
                        if (DateTime.IsLeapYear(year))
                        {
                            return ValidationResult.Success;
                        }
                        else
                        {
                            return new ValidationResult("The Date entered is invalid");
                        }
                    }
                }
                else if (!DateTime.TryParseExact(date, formats3, new CultureInfo("en-US"),
                       DateTimeStyles.None, out p))
                {
                    DateTime temp;
                    //validate date value
                    if (DateTime.TryParse(date, out temp))
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        int year = Int16.Parse((date.Substring(0, 4)));
                        if (DateTime.IsLeapYear(year))
                        {
                            return ValidationResult.Success;
                        }
                        else
                        {
                            return new ValidationResult("The Date entered is invalid");
                        }

                    }
                }
                else
                {
                    return new ValidationResult("The Date Format must be YYYY-MM-DD|| MM-DD-YYYY|| DD-MM-YYYY");
                }

            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}