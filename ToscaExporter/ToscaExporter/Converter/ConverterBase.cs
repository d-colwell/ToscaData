using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ToscaExporter.ObjectModel.DataObjects;
using Tricentis.TCCore.Persistency;

namespace ToscaExporter.Converter
{
    public abstract class ConverterBase<T> : IConverter
        where T:PersistableObject
    {
        public virtual ReportableObject Convert(PersistableObject toscaObject)
        {
            T item = (T)toscaObject;
            ReportableObject obj = new ReportableObject();
            obj.Type = typeof(T).Name;
            PopulateLinks(item, obj);
            PopulateProperties(item, obj);
            obj.Hash = GetHash(obj);
            return obj;
        }

        protected abstract void PopulateProperties(T from, ReportableObject to);
        protected abstract void PopulateLinks(T from, ReportableObject to);
        protected byte[] GetHash(ReportableObject obj)
        {
            string objectText = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(objectText);
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(inputBytes);
            return hash;
        }

    }
}
