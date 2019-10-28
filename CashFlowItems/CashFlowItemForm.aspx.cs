using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Controls;
using Kesco.Lib.Entities;
using Kesco.Lib.Entities.CashFlow;
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
        ///     Сформировать сообщение об ошибках
        /// </summary>
        protected void RenderErrors(List<string> li, string text = null)
        {
            using (var w = new StringWriter())
            {
                foreach (var l in li)
                    w.Write("<div style='white-space: nowrap;'>{0}</div>", l);

                ShowMessage(w + text, Resx.GetString("errIncorrectlyFilledField"), MessageStatus.Error, "", 500, 200);
            }
        }

        /// <summary>
        ///     Подготовка данных для отрисовки заголовка страницы (панели с кнопками)
        /// </summary>
        /// <returns></returns>
        protected string RenderDocumentHeader()
        {
            using (var w = new StringWriter())
            {
                try
                {
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
        protected void SetMenuButtons()
        {
            var btnReCheck = MenuButtons.Single(x => x.ID == "btnReCheck");
            RemoveMenuButton(btnReCheck);

            var btnEdit = MenuButtons.Single(x => x.ID == "btnEdit");
            RemoveMenuButton(btnEdit);

            var btnSave = MenuButtons.Single(x => x.ID == "btnSave");
            btnSave.Title = Resx.GetString("Cfi_lblOkTooltip");

            var btnApply = MenuButtons.Single(x => x.ID == "btnApply");
            btnApply.Title = Resx.GetString("Cfi_lblSaveTooltip");

            var btnDelete = new Button
            {
                ID = "btnDelete",
                V4Page = this,
                Text = Resx.GetString("cmdDelete"),
                Title = Resx.GetString("Cfi_lblDeleteTooltip"),
                IconJQueryUI = ButtonIconsEnum.Delete,
                Width = 105,
                OnClick = string.Format("v4_showConfirm('{0}','{1}','{2}','{3}','{4}', null);",
                    string.Format("{0} «{1}»", Resx.GetString("msgDeleteConfirm"), HttpUtility.HtmlEncode(cfi.Name)),
                    Resx.GetString("errDoisserWarrning"),
                    Resx.GetString("CONFIRM_StdCaptionYes"),
                    Resx.GetString("CONFIRM_StdCaptionNo"),
                    string.Format("cmd({0});", HttpUtility.JavaScriptStringEncode("'cmd', 'Delete'"))
                )
            };

            if (!cfi.Id.IsNullEmptyOrZero()) AddMenuButton(btnDelete);
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
                case "Save":
                    SaveData(true);
                    break;
                case "Apply":
                    SaveData(false);
                    break;
                case "Delete":
                    DeleteData();
                    break;
                default:
                    base.ProcessCommand(cmd, param);
                    break;
            }
        }

        /// <summary>
        ///     Кнопка: OK / Сохранить
        /// </summary>
        private void SaveData(bool needCloseForm)
        {
            List<string> validList;
            if (!ValidateCashFlowItem(out validList))
            {
                RenderErrors(validList, "<br/> " + Resx.GetString("Cfi_msgRecordCannotBeSaved"));
                return;
            }

            if (cfi.IsModified)
            {
                var isNew = cfi.Id.IsNullEmptyOrZero();

                try
                {
                    cfi.Save(isNew);
                }
                catch (DetailedException e)
                {
                    ShowMessage(e.Message, Resx.GetString("alertError"), MessageStatus.Error, "", 300,100);
                    return;
                }

                JS.Write("if (parent.RefreshTreeView) {{ parent.RefreshTreeView('{0}', {1}); }}", cfi.Id, isNew ? "true" : "false");

                if (!needCloseForm)
                {
                    if (isNew)
                        SetCurrentUrlParams(new Dictionary<string, object> {{"id", cfi.Id}});
                    RefreshPage();
                }
            }

            if (needCloseForm) JS.Write("parent.cashFlowItem_dialogShow ? parent.CloseDialog(parent.cashFlowItem_dialogShow.form, null, 0) : v4_closeWindow();");
        }

        /// <summary>
        ///     Кнопка: Удалить
        /// </summary>
        private void DeleteData()
        {
            try
            {
                cfi.Delete();
            }
            catch (DetailedException e)
            {
                ShowMessage(e.Message, Resx.GetString("alertError"), MessageStatus.Error, "", 300, 100);
                return;
            }
            JS.Write("if (parent.RefreshTreeView) {{ parent.RefreshTreeView('{0}', true); }}", cfi.Id);
            JS.Write("parent.cashFlowItem_dialogShow ? parent.CloseDialog(parent.cashFlowItem_dialogShow.form, null, 0) : v4_closeWindow();");
        }
    }
}