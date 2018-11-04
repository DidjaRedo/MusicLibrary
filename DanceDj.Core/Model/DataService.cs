﻿using System;

namespace DanceDj.Model
{
    public class DataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback) {
            // Use this to connect to the actual data service

            var item = new DataItem("./data/library.json");
            callback(item, null);
        }
    }
}