using System;
using System.Collections.Specialized;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.Entities;
using Kesco.Lib.Entities.CashFlow;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Controls.V4;
using Kesco.Lib.Web.Controls.V4.Common;
using Kesco.Lib.Web.Controls.V4.TreeView;
using Kesco.Lib.Web.Settings;

namespace Kesco.App.Web.CashFlowItems
{
    /// <summary>
    ///     Класс объекта страницы
    /// </summary>
    public partial class Search : Page
    {
        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public Search()
        {
            HelpUrl = "hlp/help.htm?id=1";
        }

        /// <summary>
        ///     Ссылка на справку
        /// </summary>
        public override string HelpUrl { get; set; }

        /// <summary>
        ///     Событие пред-загрузки страницы
        /// </summary>
        /// <param name="sender">Объект страницы</param>
        /// <param name="e">Аргументы</param>
        protected void Page_PreInit(object sender, EventArgs e)
        {
            tvCashFlowItem.SetJsonData("SearchData.ashx");
            tvCashFlowItem.SetService("AddCashFlowItem", "EditCashFlowItem", "DeleteCashFlowItem");

            tvCashFlowItem.DbSourceSettings = new TreeViewDbSourceSettings
            {
                ConnectionString = Config.DS_person,
                TableName = "СтатьиДвиженияДенежныхСредств",
                ViewName = "СтатьиДвиженияДенежныхСредств",
                PkField = "КодСтатьиДвиженияДенежныхСредств",
                NameField = "СтатьяДвиженияДенежныхСредств",
                ModifyUserField = "Изменил",
                ModifyDateField = "Изменено",
                RootName = "Статьи движения денежных средств"
            };

            tvCashFlowItem.IsOrderMenu = true;
            tvCashFlowItem.IsLoadData = false;
            tvCashFlowItem.Resizable = false;
            tvCashFlowItem.IsSaveState = true;
            tvCashFlowItem.IsSearchMenu = true;
            tvCashFlowItem.ParamName = "CFITreeState";
            tvCashFlowItem.ClId = ClId;
            tvCashFlowItem.IsEditableInDialog = false;
            tvCashFlowItem.ContextMenuAdd = true;
            tvCashFlowItem.ContextMenuRename = true;
            tvCashFlowItem.ContextMenuDelete = true;
            tvCashFlowItem.ShowTopNodesInSearchResult = false;
            tvCashFlowItem.HelpButtonVisible = true;
            tvCashFlowItem.LikeButtonVisible = true;
            tvCashFlowItem.AddFormTitle = string.Format("{0} {1}", Resx.GetString("lblAddition"),
                Resx.GetString("Cfi_msgCashFlowArticles"));
            tvCashFlowItem.EditFormTitle = string.Format("{0} {1}", Resx.GetString("lblEdit"),
                Resx.GetString("Cfi_msgCashFlowArticles"));

            IsSilverLight = false;

            SetMenuButtons();
        }

        /// <summary>
        ///     Событие загрузки страницы
        /// </summary>
        /// <param name="sender">Объект страницы</param>
        /// <param name="e">Аргументы</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            var loadById = 0;
            if (!Request.QueryString["id"].IsNullEmptyOrZero())
                int.TryParse(Request.QueryString["id"], out loadById);
            else if (!Request.QueryString["idloc"].IsNullEmptyOrZero())
                int.TryParse(Request.QueryString["idloc"], out loadById);
            if (loadById != 0)
                tvCashFlowItem.LoadById = loadById;

            JS.Write(@"cashFlowItem = {{
                AddFormTitle:""{0}"",
                EditFormTitle:""{1}""
                }};",
                tvCashFlowItem.AddFormTitle,
                tvCashFlowItem.EditFormTitle
            );
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
                    ItemId = int.Parse(param["Id"]);
                    JS.Write("AddCashFlowItem('{0}');", ItemId);
                    break;
                case "EditCashFlowItem":
                    ItemId = int.Parse(param["Id"]);
                    JS.Write("EditCashFlowItem('{0}');", ItemId);
                    break;
                case "DeleteCashFlowItem":
                    ItemId = int.Parse(param["Id"]);
                    var entity = new CashFlowItem(ItemId.ToString());
                    try
                    {
                        entity.Delete();
                    }
                    catch (DetailedException e)
                    {
                        ShowMessage(e.Message, Resx.GetString("alertError"), MessageStatus.Error, "", 300, 100);
                        return;
                    }
                    JS.Write("v4_reloadParentNode('{0}', '{1}');", tvCashFlowItem.ClientID, ItemId);
                    break;
                default:
                    base.ProcessCommand(cmd, param);
                    break;
            }
        }

        /// <summary>
        ///     Инициализация/создание кнопок меню
        /// </summary>
        protected void SetMenuButtons()
        {
            // Виды движения
            var btnCashFlowType = new Button
            {
                ID = "btnCashFlowType",
                V4Page = this,
                Text = Resx.GetString("Cfi_lblModesMovement"),
                Title = Resx.GetString("Cfi_lblModesMovement"),
                IconJQueryUI = ButtonIconsEnum.FolderOpen,
                OnClick =
                    "var w = window.open('CashFlowTypeList.aspx','CashFlowTypeList','status=no,toolbar=no,menubar=no,location=no,resizable=yes,scrollbars=yes,height=560,width=1000 ');w.focus();"
            };

            tvCashFlowItem.AddMenuButton(btnCashFlowType);
        }
    }
}