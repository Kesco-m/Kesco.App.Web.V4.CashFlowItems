using System;
using System.Collections.Specialized;
using System.IO;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.Entities.CashFlowItem;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Controls.V4;
using Kesco.Lib.Web.Controls.V4.Common;
using Kesco.Lib.Web.Settings;

namespace Kesco.App.Web.CashFlowItems
{
    /// <summary>
    ///     Класс объекта страницы
    /// </summary>
    public partial class Search : EntityPage
    {
        // Идентификатор записи
        protected int CashFlowItemId;
        private int _loadById;

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public Search()
        {
            HelpUrl = "hlp/help.htm?id=1";
        }

        /// <summary>
        ///     Задание ссылки на справку
        /// </summary>
        public override string HelpUrl { get; set; }

        /// <summary>
        ///     Формирует основыной функционал страницы: подписи, меню, заголовок, title
        /// </summary>
        protected string RenderDocumentHeader()
        {
            using (var w = new StringWriter())
            {
                try
                {
                    ClearMenuButtons();
                    if (ReturnId.IsNullEmptyOrZero())
                        SetMenuButtons();
                    RenderButtons(w);
                }
                catch (Exception e)
                {
                    var dex = new DetailedException(Resx.GetString("TTN_errFailedGenerateButtons") + ": " + e.Message,
                        e);
                    Logger.WriteEx(dex);
                    throw dex;
                }

                return w.ToString();
            }
        }

        /// <summary>
        ///     Инициализация/создание кнопок меню
        /// </summary>
        private void SetMenuButtons()
        {
            var btnCashFlowType = new Button
            {
                ID = "btnCashFlowType",
                V4Page = this,
                Text = Resx.GetString("Cfi_lblModesMovement"), //Виды Движения
                Title = Resx.GetString("Cfi_lblModesMovement"),
                IconJQueryUI = ButtonIconsEnum.FolderOpen,
                OnClick = "cmd('cmd', 'CashFlowType')"
            };

            var buttons = new[] {btnCashFlowType};
            AddMenuButton(buttons);
        }

        /// <summary>
        ///     Событие пред-загрузки страницы
        /// </summary>
        /// <param name="sender">Объект страницы</param>
        /// <param name="e">Аргументы</param>
        protected void Page_PreInit(object sender, EventArgs e)
        {
            tvCashFlowItem.SetJsonData("SearchData.ashx");
            tvCashFlowItem.SetService("AddCashFlowItem", "EditCashFlowItem", "DeleteCashFlowItem");
            tvCashFlowItem.SetDataSource("Справочники.dbo.СтатьиДвиженияДенежныхСредств",
                "СтатьиДвиженияДенежныхСредств", Config.DS_resource, "КодСтатьиДвиженияДенежныхСредств",
                "СтатьяДвиженияДенежныхСредств", "");

            tvCashFlowItem.IsLoadData = false;
            tvCashFlowItem.IsSaveState = true;
            tvCashFlowItem.IsSearchMenu = true;

            tvCashFlowItem.ParamName = "CfiTreeState";
            tvCashFlowItem.ClId = ClId;

            tvCashFlowItem.IsContextMenu = true;
            tvCashFlowItem.ContextMenuAdd = true;
            tvCashFlowItem.ContextMenuRename = true;
            tvCashFlowItem.ContextMenuDelete = true;
            tvCashFlowItem.IsOrderMenu = true;
            tvCashFlowItem.Resizable = false;

            IsSilverLight = false;

            if (!Request.QueryString["id"].IsNullEmptyOrZero())
                int.TryParse(Request.QueryString["id"], out _loadById);
            else if (!Request.QueryString["idloc"].IsNullEmptyOrZero())
                int.TryParse(Request.QueryString["idloc"], out _loadById);
            if (_loadById != 0)
                tvCashFlowItem.LoadById = _loadById;
        }

        /// <summary>
        ///     Событие загрузки страницы
        /// </summary>
        /// <param name="sender">Объект страницы</param>
        /// <param name="e">Аргументы</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            JS.Write(@"cashflowitem = {{
                editaction:""{0}"",
                addaction:""{1}"",
                title:""{2}""
            }};",
                Resx.GetString("lblEdit"),
                Resx.GetString("lblAddition"),
                Resx.GetString("Cfi_msgCashFlowArticles")
            );

            IsRememberWindowProperties = true;
            WindowParameters = new WindowParameters("CFISrchWndLeft", "CFISrchWndTop", "CFISrchWidth", "CFISrchHeight");
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
                case "AddCashFlowItem":
                    CashFlowItemId = int.Parse(param["Id"]);
                    JS.Write("cashFlowItem_add({0});", CashFlowItemId);
                    break;
                case "EditCashFlowItem":
                    CashFlowItemId = int.Parse(param["Id"]);
                    JS.Write("cashFlowItem_edit({0});", CashFlowItemId);
                    break;
                case "DeleteCashFlowItem":
                    CashFlowItemId = int.Parse(param["Id"]);
                    var cfi = new CashFlowItem(CashFlowItemId.ToString());
                    try
                    {
                        cfi.Delete();
                        JS.Write("v4_reloadNode('{0}');", tvCashFlowItem.ClientID);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(ex.Message, Resx.GetString("alertError"));
                    }
                    break;
                case "RefreshData":
                    JS.Write("Records_Close(null, 0);");
                    JS.Write("v4_reloadNode('{0}');", tvCashFlowItem.ClientID);
                    break;
                case "CashFlowType":
                    JS.Write(
                        "window.open('{0}','_blank','status=no,toolbar=no,menubar=no,location=no,resizable=yes,scrollbars=yes,height=560,width=1000 ');",
                        "CashFlowTypeList.aspx");
                    break;
                default:
                    base.ProcessCommand(cmd, param);
                    break;
            }
        }
    }
}