using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities;
using Kesco.Lib.Entities.CashFlow;
using Kesco.Lib.Web.Controls.V4.Common;
using Kesco.Lib.Web.Settings;

namespace Kesco.App.Web.CashFlowItems
{
    /// <summary>
    ///     Виды движения денежных средств
    /// </summary>
    public partial class CashFlowTypeList : EntityPage
    {
        /// <summary>
        ///     Нужно ли возвращать значение
        /// </summary>
        protected bool _returnState;

        /// <summary>
        ///     Значение поиска из строки запроса
        /// </summary>
        protected string _searchText;

        /// <summary>
        ///     Свойство указывающее, что у пользователя есть права на добавление
        /// </summary>
        protected bool HasInsert { get; set; }

        /// <summary>
        ///     Свойство указывающее, что у пользователя есть права на обновление
        /// </summary>
        protected bool HasUpdate { get; set; }

        /// <summary>
        ///     Свойство указывающее, что у пользователя есть права на удаление
        /// </summary>
        protected bool HasDelete { get; set; }

        /// <summary>
        ///     Задание ссылки на справку
        /// </summary>
        public override string HelpUrl { get; set; }

        protected override void EntityInitialization(Entity copy = null)
        {
        }

        /// <summary>
        ///     Событие загрузки страницы
        /// </summary>
        /// <param name="sender">Объект страницы</param>
        /// <param name="e">Аргументы</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!V4IsPostBack)
            {
                if (Request.QueryString["return"] != null) _returnState = true;
                _searchText = Request.QueryString["Search"];
                JS.Write(@"domain='{0}';", Config.domain);
            }

            JS.Write(@"cashflowtype = {{
                editaction:""{0}"",
                addaction:""{1}"",
                title:""{2}""
                }};",
                Resx.GetString("lblEdit"),
                Resx.GetString("lblAddition"),
                Resx.GetString("Cfi_msgTypeCashFlow")
            );

            InitPermissions();
            InitDataGrid();
            FillDataGrid(false);
        }

        /// <summary>
        ///     Проинициализировать права доступа
        /// </summary>
        protected void InitPermissions()
        {
            var sqlParams = new Dictionary<string, object> {{"@TableName", "ВидыДвиженийДенежныхСредств"}};

            var dt = DBManager.GetData(SQLQueries.SELECT_ПраваНаТаблицу, Config.DS_resource, CommandType.Text,
                sqlParams);

            if (dt.Rows.Count > 0)
            {
                HasInsert = Convert.ToBoolean(dt.Rows[0]["PermOnInsert"]);
                HasUpdate = Convert.ToBoolean(dt.Rows[0]["PermOnUpdate"]);
                HasDelete = Convert.ToBoolean(dt.Rows[0]["PermOnDelete"]);
            }
        }

        /// <summary>
        ///     Инициализация DataGrid
        /// </summary>
        protected void InitDataGrid()
        {
            GridCashFlowType.EmptyDataString = Resx.GetString("");
            GridCashFlowType.EmptyDataNtfStatus = NtfStatus.Error;
            GridCashFlowType.ShowGroupPanel = false;
            GridCashFlowType.ExistServiceColumn = HasInsert || HasUpdate || HasDelete;
            GridCashFlowType.ExistServiceColumnDetail = false;

            if (_returnState)
            {
                GridCashFlowType.ExistServiceColumnReturn = true;
                GridCashFlowType.SetServiceColumnReturn("returnValue",
                    new List<string> {"КодВидаДвиженияДенежныхСредств", "ВидДвиженияДенежныхСредств"},
                    Resx.GetString("ppBtnChoose"));
            }

            if (HasInsert)
                GridCashFlowType.SetServiceColumnAdd("AddCashFlowType", Resx.GetString("Cfi_lblAddCashFlowType"));

            if (HasUpdate)
                GridCashFlowType.SetServiceColumnEdit("EditCashFlowType",
                    new List<string> {"КодВидаДвиженияДенежныхСредств"},
                    Resx.GetString("Cfi_lblEditCashFlowType"));

            if (HasDelete)
                GridCashFlowType.SetServiceColumnDelete("DeleteCashFlowType",
                    new List<string> {"КодВидаДвиженияДенежныхСредств"},
                    new List<string> {"ВидДвиженияДенежныхСредств"}, Resx.GetString("Cfi_lblDeleteCashFlowType"));
        }

        /// <summary>
        ///     Заполнение DataGrid
        /// </summary>
        /// <param name="isAddNew">Признак добавления новой записи</param>
        protected void FillDataGrid(bool isAddNew)
        {
            var isSearch = !string.IsNullOrEmpty(_searchText);

            var sqlParams = new Dictionary<string, object>();

            if (isSearch) sqlParams.Add("@Название", _searchText);

            var sql = isSearch
                ? SQLQueries.SELECT_ВидыДвиженийДенежныхСредств_Фильтр
                : SQLQueries.SELECT_ВидыДвиженийДенежныхСредств_ID;

            GridCashFlowType.SetDataSource(sql, Config.DS_resource,
                CommandType.Text, sqlParams);

            #region Настройка параметров колонок, общих для все видов грида

            var dictHeaderAlias = new Dictionary<string, string>
            {
                {"КодВидаДвиженияДенежныхСредств", Resx.GetString("Cfi_lblCode")}, // Код
                {"ВидДвиженияДенежныхСредств", Resx.GetString("Cfi_lblName")}, // Название
                {"Название1С", Resx.GetString("Cfi_lblNameIn1C")} // Название в 1С
            };

            #endregion

            GridCashFlowType.Settings.SetColumnHeaderAlias(dictHeaderAlias);

            GridCashFlowType.RefreshGridData();
            var currentPage = GridCashFlowType.GеtCurrentPage();

            if (isAddNew) GridCashFlowType.GoToLastPage();
            else GridCashFlowType.GoToPage(currentPage);
        }

        /// <summary>
        ///     Обработка клиентских команд
        /// </summary>
        /// <param name="cmd">Команды</param>
        /// <param name="param">Параметры</param>
        protected override void ProcessCommand(string cmd, NameValueCollection param)
        {
            switch (cmd)
            {
                case "RefreshDataGrid":
                    var isAddNew = param["IsAddNew"] == "true";
                    FillDataGrid(isAddNew);
                    break;
                case "DeleteCashFlowType":
                    ItemId = int.Parse(param["Id"]);
                    var entity = new CashFlowType(ItemId.ToString());
                    entity.Delete();
                    FillDataGrid(false);
                    break;
                default:
                    base.ProcessCommand(cmd, param);
                    break;
            }
        }
    }
}