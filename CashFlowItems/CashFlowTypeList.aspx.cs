using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.Entities;
using Kesco.Lib.Entities.Resources;
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
        private bool _returnState;

        /// <summary>
        ///     Задание ссылки на справку
        /// </summary>
        public override string HelpUrl { get; set; }

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

            FillDataGrid(false);
        }

        /// <summary>
        ///     Заполнение Grid-а
        /// </summary>
        /// <param name="isAddNew">Признак добавления новой записи</param>
        private void FillDataGrid(bool isAddNew)
        {
            GridCashFlowType.EmptyDataString = Resx.GetString("");
            GridCashFlowType.EmptyDataNtfStatus = NtfStatus.Error;

            #region Настройка параметров колонок, общих для все видов грида

            var dictHeaderAlias = new Dictionary<string, string>
            {
                {"КодВидаДвиженияДенежныхСредств", Resx.GetString("Cfi_lblCode")}, // Код
                {"ВидДвиженияДенежныхСредств", Resx.GetString("Cfi_lblName")}, // Название
                {"Название1С", Resx.GetString("Cfi_lblNameIn1C")} // Название в 1С
            };

            #endregion

            GridCashFlowType.ShowGroupPanel = false;
            GridCashFlowType.ExistServiceColumn = true;
            GridCashFlowType.ExistServiceColumnDetail = false;

            if (_returnState)
            {
                GridCashFlowType.ExistServiceColumnReturn = true;
                GridCashFlowType.SetServiceColumnReturn("_return",
                    new List<string> {"КодВидаДвиженияДенежныхСредств", "ВидДвиженияДенежныхСредств"},
                    Resx.GetString("ppBtnChoose"));
            }

            var sqlParams = new Dictionary<string, object>();
            //sqlParams.Add("@Код", Id);
            GridCashFlowType.SetDataSource(SQLQueries.SELECT_ВидыДвиженийДенежныхСредств_ID, Config.DS_resource,
                CommandType.Text, sqlParams);

            GridCashFlowType.Settings.SetColumnHeaderAlias(dictHeaderAlias);

            GridCashFlowType.SetServiceColumnAdd("_add", Resx.GetString("lblAdd"));
            GridCashFlowType.SetServiceColumnEdit("_edit", new List<string> {"КодВидаДвиженияДенежныхСредств"},
                Resx.GetString("TTN_btnEditPosition"));
            GridCashFlowType.SetServiceColumnDelete("_delete", new List<string> {"КодВидаДвиженияДенежныхСредств"},
                new List<string> {"ВидДвиженияДенежныхСредств"}, Resx.GetString("TTN_btnDeletePosition"));


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
                case "RefreshData":
                    RefreshData(param["isNew"] == "True");
                    //JS.Write("Records_Close({0}); $('#addResource').focus();", param["ctrlFocus"]);
                    JS.Write("Records_Close(null, 0);");
                    break;
                case "Delete":
                    DeleteData(param["Id"]);
                    break;
                default:
                    base.ProcessCommand(cmd, param);
                    break;
            }
        }

        /// <summary>
        ///     Удаление
        /// </summary>
        /// <param name="Id"></param>
        private void DeleteData(string Id)
        {
            var cft = new CashFlowType(Id);
            cft.Delete();
            RefreshData(false);
        }

        /// <summary>
        ///     Обновление
        /// </summary>
        public void RefreshData(bool isAddNew)
        {
            FillDataGrid(isAddNew);
        }
    }
}