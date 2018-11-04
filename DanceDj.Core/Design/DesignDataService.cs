using System;
using DanceDj.Mvvm.Model;

namespace DanceDj.Mvvm.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback) {
            // Use this to create design time data

            var item = new DataItem("./data/library.json");
            callback(item, null);
        }
    }
}