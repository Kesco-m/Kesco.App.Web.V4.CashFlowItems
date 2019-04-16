using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Web;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.Entities.CashFlowItem;
using Kesco.Lib.Log;
using Kesco.Lib.Web.Controls.V4;
using Kesco.Lib.Web.Controls.V4.Common;

namespace Kesco.App.Web.CashFlowItems
{
    /// <summary>
    ///     Форма средства движения денежных средств
    /// </summary>
    public partial class CashFlowItemForm : EntityPage
    {
        protected CashFlowItem cfi;
        protected string id;
        protected string idParentPage;
        protected bool IsParentUpdate = true;
        protected string parentid;

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
            if (!V4IsPostBack) V4SetFocus("tbCashFlowItemName");
        }

        /// <summary>
        ///     Проверка корректности вводимых полей
        /// </summary>
        protected bool ValidateCashFlowItem(out List<string> errors)
        {
            errors = new List<string>();

            if (tbCashFlowItemName.Value.IsNullEmptyOrZero())
                errors.Add(Resx.GetString("Cfi_msgCashFlowItemNotFilled"));

            if (tbCashFlowItemType.Value.IsNullEmptyOrZero())
                errors.Add(Resx.GetString("Cfi_msgCashFlowTypeNotFilled"));

            return errors.Count <= 0;
        }

        /// <summary>
        ///     Подготовка данных для отрисовки заголовка страницы(панели с кнопками)
        /// </summary>
        /// <returns></returns>
        protected string RenderDocumentHeader()
        {
            using (var w = new StringWriter())
            {
                try
                {
                    ClearMenuButtons();
                    SetMenuButtons();
                    RenderButtons(w);
                }
                catch (Exception e)
                {
                    var dex = new DetailedException("Не удалось сформировать кнопки формы: " + e.Message, e);
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
            var btnAdd = new Button
            {
                ID = "btnSave",
                V4Page = this,
                Text = Resx.GetString("cmdSave") + "&nbsp;(F2)",
                Title = Resx.GetString("cmdSave"),
                Width = 125,
                IconJQueryUI = ButtonIconsEnum.Save,
                OnClick = "cmd('cmd', 'SaveData');"
            };

            var btnRefresh = new Button
            {
                ID = "btnRefresh",
                V4Page = this,
                Text = Resx.GetString("cmdRefresh"),
                Title = Resx.GetString("cmdRefreshTitle"),
                IconJQueryUI = ButtonIconsEnum.Refresh,
                Width = 105,
                OnClick = "cmd('cmd', 'RefreshData');"
            };

            var btnDelete = new Button
            {
                ID = "btnDelete",
                V4Page = this,
                Text = Resx.GetString("cmdDelete"),
                Title = Resx.GetString("cmdDeleteTitle"),
                IconJQueryUI = ButtonIconsEnum.Delete,
                Width = 105,
                OnClick = string.Format("v4_showConfirm('{0}','{1}','{2}','{3}','{4}', null);",
                    string.Format("{0} «{1}»", Resx.GetString("msgDeleteConfirm"), HttpUtility.HtmlEncode(cfi.Name)),
                    Resx.GetString("errDoisserWarrning"),
                    Resx.GetString("CONFIRM_StdCaptionYes"),
                    Resx.GetString("CONFIRM_StdCaptionNo"),
                    string.Format("cmd({0});", HttpUtility.JavaScriptStringEncode("'cmd', 'DeleteData'"))
                )
            };

            var btnClose = new Button
            {
                ID = "btnClose",
                V4Page = this,
                Text = Resx.GetString("cmdClose"),
                Title = Resx.GetString("cmdCloseTitleApp"),
                IconJQueryUI = ButtonIconsEnum.Close,
                Width = 105,
                OnClick = IsParentUpdate ? "parent.Records_Close(idp, null);" : "window.close();"
            };

            if (!cfi.Id.IsNullEmptyOrZero())
            {
                Button[] buttons = {btnAdd, btnRefresh, btnDelete, btnClose};
                AddMenuButton(buttons);
            }
            else
            {
                Button[] buttons = {btnAdd, btnRefresh, btnClose};
                AddMenuButton(buttons);
            }
        }

        /// <summary>
        ///     инициализация контролов
        /// </summary>
        protected override void EntityFieldInit()
        {
            if (!V4IsPostBack)
            {
                id = Request.QueryString["id"];
                parentid = Request.QueryString["parentid"];
                idParentPage = Request.QueryString["idpp"];

                ParentPage = Application[idParentPage] as EntityPage;
                if (ParentPage == null) IsParentUpdate = false;

                if (!string.IsNullOrEmpty(id) && id != "0")
                {
                    cfi = new CashFlowItem(id);
                    if (cfi == null || cfi.Id == "0")
                        throw new LogicalException(Resx.GetString("Cfi_ ERRMoveCashInitialized"), "",
                            Assembly.GetExecutingAssembly().GetName(), Priority.Info);
                }
                else
                {
                    cfi = new CashFlowItem {Id = id, Parent = parentid};
                    efChanged.ChangedByID = null;
                }
            }

            Entity = cfi;

            tbCashFlowItemName.BindStringValue = cfi.CashFlowItemNameBind;
            tbCashFlowItemType.BindStringValue = cfi.CashFlowTypeIdBind;

            efChanged.SetChangeDateTime = cfi.ChangedTime;
            efChanged.ChangedByID = cfi.ChangedId;

            base.EntityFieldInit();
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
                    JS.Write("$('#btnRefresh').attr('disabled', 'disabled');Wait.render(true);");
                    JS.Write(
                        "setTimeout(function(){{$('#btnRefresh').removeAttr('disabled'); Wait.render(false);}}, 2000);");
                    RefreshData();
                    break;
                case "SaveData":
                    List<string> validList;
                    if (ValidateCashFlowItem(out validList))
                        SaveData();
                    else
                        RenderErrors(validList, "<br/> " + Resx.GetString("Cfi_msgRecordCannotBeSaved"));
                    break;
                case "DeleteData":
                    DeleteData();
                    break;
            }
        }

        /// <summary>
        ///     Сформировать сообщение об ошибках
        /// </summary>
        public void RenderErrors(List<string> li, string text = null)
        {
            using (var w = new StringWriter())
            {
                foreach (var l in li)
                    w.Write("<div style='white-space: nowrap;'>{0}</div>", l);

                ShowMessage(w + text, Resx.GetString("errIncorrectlyFilledField"), MessageStatus.Error, "", 500, 200);
            }
        }

        /// <summary>
        ///     Кнопка: Сохранить
        /// </summary>
        private void SaveData()
        {
            var reloadParentForm = true;
            var isNew = cfi.Id.IsNullEmptyOrZero();
            cfi.Save(isNew);
            JS.Write("parent.Records_Save('','{0}','{1}');", reloadParentForm, isNew);
        }

        /// <summary>
        ///     Обновление данных формы из объекта
        /// </summary>
        private void RefreshData()
        {
            ClearCacheObjects();
            RefreshNtf();
        }

        /// <summary>
        ///     Очистка всех данных формы
        /// </summary>
        private void DeleteData()
        {
            cfi.Delete();
            JS.Write("parent.Records_Save();");
        }
    }
}