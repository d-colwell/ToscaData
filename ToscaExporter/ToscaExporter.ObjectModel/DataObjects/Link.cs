﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToscaExporter.ObjectModel.DataObjects
{
    public class Link
    {
        public LinkDescription LinkDescription { get; set; }
        public string[] LinkedObjects { get; set; }
    }
}
