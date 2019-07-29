using System.Data;
using System.Text;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.Entities;
using Kesco.Lib.Web.Controls.V4.TreeView;

namespace Kesco.App.Web.Inventory
{
    /// <summary>
    ///     Сводное описание для SearchData
    /// </summary>
    public sealed class SearchData : TreeViewDataHandler
    {
        /// <summary>
        ///     Получение строки запроса по переданным параметрам
        /// </summary>
        /// <param name="orderByField">Поле сортировки</param>
        /// <param name="orderByDirection">Направление сортировки</param>
        /// <param name="searchText">Строка поиска</param>
        /// <param name="searchParam">Параметры поиска</param>
        /// <param name="openList">Список открытых нодов</param>
        /// <returns></returns>
        protected override string GetTreeData_Sql(string orderByField = "L", string orderByDirection = "ASC",
            string searchText = "", string searchParam = "", string openList = "")
        {
            if (orderByField != "L") orderByField = dbSource.NameField;

            var orderBy = orderByField + " " + orderByDirection;

            if (string.IsNullOrEmpty(searchText))
            {
                if (openList.IsNullEmptyOrZero())
                    return string.Format(SQLQueries.SELECT_СтатьиДвиженияДенежныхСредствДанныеДляДерева, orderBy,
                        string.IsNullOrEmpty(RootIds) ? "-1" : RootIds);
                return string.Format(SQLQueries.SELECT_СтатьиДвиженияДенежныхСредствДанныеДляДерева_State, orderBy,
                    openList);
            }

            searchText = searchParam == "1" ? searchText + "%" : "%" + searchText + "%";
            return string.Format(SQLQueries.SELECT_СтатьиДвиженияДенежныхСредствДанныеДляДерева_Фильтр, orderBy,
                searchText, string.IsNullOrEmpty(RootIds) ? "-1" : RootIds);
        }

        /// <summary>
        ///     Вывод иконок до названия
        /// </summary>
        /// <param name="dt">DataRow</param>
        /// <returns>возвращаямая строка, содержащая готовую разметку</returns>
        protected override string GetPrefixIcon(DataRow dt)
        {
            var iconsString = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(ReturnId) && ReturnId == "1")
                iconsString.AppendFormat(
                    "<img style='border:none' src='/styles/BackToList.gif' title='Выбрать значение' onclick='v4_returnValue({0},\"{1}\");'>&nbsp;",
                    dt["Id"], dt["Text"]);

            if (dt["ЕстьДети"].ToString().Equals("1"))
                iconsString.Append("<img style='border:none' src='/styles/File.gif'>&nbsp;");
            else
                iconsString.Append("<img style='border:none' src='/styles/Folder.gif'>&nbsp;");

            return iconsString.ToString();
        }

        /// <summary>
        ///     Вывод иконок до названия
        /// </summary>
        /// <param name="dt">DataRowView</param>
        /// <returns>возвращаямая строка, содержащая готовую разметку</returns>
        protected override string GetPrefixIcon(DataRowView dt)
        {
            return base.GetPrefixIcon(dt) + GetPrefixIcon(dt.Row);
        }
    }
}