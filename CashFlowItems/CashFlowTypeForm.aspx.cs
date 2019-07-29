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
    ///     Вид движения денежных средств
    /// </summary>
    public partial class CashFlowTypeForm : EntityPage
    {
        protected CashFlowType cft;
        protected string id;
        protected string idParentPage;
        protected bool IsParentUpdate = true;

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
            if (!V4IsPostBack) V4SetFocus("efName");
        }

        /// <summary>
        ///     Валидация контролов
        /// </summary>
        /// <returns></returns>
        private bool Validation()
        {
            var title = Resx.GetString("Cfi_msgRecordCannotBeSaved");
            if (cft.Id.Length == 0 || cft.Id == "0")
            {
                // Не заполнено поле 'Код'"
                ShowMessage(Resx.GetString("Cfi_msgNotFilledField") + " «" + Resx.GetString("Cfi_lblCode") + "»",
                    title);
                efId.Focus();
                return false;
            }

            if (cft.Name.Length == 0)
            {
                // Не заполнено поле 'Название'"
                ShowMessage(Resx.GetString("Cfi_msgNotFilledField") + " «" + Resx.GetString("Cfi_lblName") + "»",
                    title);
                efName.Focus();
                return false;
            }

            if (cft.Name1С.Length == 0)
            {
                // Не заполнено поле 'Название 1C'"
                ShowMessage(Resx.GetString("Cfi_msgNotFilledField") + " «" + Resx.GetString("Cfi_lblNameIn1C") + "»",
                    title);
                efName1C.Focus();
                return false;
            }

            if (!efId.IsReadOnly)
                if (cft.ExistIdDubl())
                {
                    ShowMessage(Resx.GetString("Cfi_msgIdDubl") + " «" + cft.Id + "»", title);
                    efId.Focus();
                    return false;
                }

            return true;
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
                    //ClearMenuButtons();
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
            var btnReCheck = MenuButtons.Single(btn => btn.ID == "btnReCheck");
            RemoveMenuButton(btnReCheck);

            var btnEdit = MenuButtons.Single(btn => btn.ID == "btnEdit");
            RemoveMenuButton(btnEdit);

            var btnSave = MenuButtons.Single(btn => btn.ID == "btnSave");
            btnSave.Title = Resx.GetString("Cfi_lblOkTooltipCft");

            var btnApply = MenuButtons.Single(btn => btn.ID == "btnApply");
            btnApply.Title = Resx.GetString("Cfi_lblSaveTooltipCft");

            var btnDelete = new Button
            {
                ID = "btnDelete",
                V4Page = this,
                Text = Resx.GetString("cmdDelete"),
                Title = Resx.GetString("Cfi_lblDeleteTooltipCft"),
                IconJQueryUI = ButtonIconsEnum.Delete,
                Width = 105,
                OnClick = string.Format("v4_showConfirm('{0}','{1}','{2}','{3}','{4}', null);",
                    string.Format("{0} «{1}»", Resx.GetString("msgDeleteConfirm"), HttpUtility.HtmlEncode(cft.Name)),
                    Resx.GetString("errDoisserWarrning"),
                    Resx.GetString("CONFIRM_StdCaptionYes"),
                    Resx.GetString("CONFIRM_StdCaptionNo"),
                    string.Format("cmd({0});", HttpUtility.JavaScriptStringEncode("'cmd', 'Delete'"))
                )
            };

            if (!cft.Id.IsNullEmptyOrZero()) AddMenuButton(btnDelete);
        }

        /// <summary>
        ///     инициализация контролов
        /// </summary>
        protected override void EntityFieldInit()
        {
            if (!V4IsPostBack)
            {
                id = Request.QueryString["id"];
                idParentPage = Request.QueryString["idpp"];

                ParentPage = Application[idParentPage] as EntityPage;
                if (ParentPage == null) IsParentUpdate = false;

                if (!string.IsNullOrEmpty(id) && id != "0")
                {
                    cft = new CashFlowType(id);
                    if (cft == null || cft.Id == "0")
                        throw new LogicalException(Resx.GetString("Cfi_ ERRMoveCashInitialized"), "",
                            Assembly.GetExecutingAssembly().GetName(), Priority.Info);
                }
                else
                {
                    cft = new CashFlowType {Id = id};
                }
            }

            Entity = cft;

            efId.BindStringValue = cft.CashFlowTypeIdBind;
            efName.BindStringValue = cft.CashFlowTypeNameBind;
            efName1C.BindStringValue = cft.CashFlowTypeName1CBind;

            efId.IsReadOnly = cft.Id != "0";

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
            if (!Validation())
                return;

            if (cft.IsModified)
            {
                var isNew = !efId.IsReadOnly;
                cft.Save(isNew);

                JS.Write("if (parent.RefreshData) {{ parent.RefreshData({0}); }}", isNew ? "true" : "false");

                if (!needCloseForm)
                {
                    if (isNew)
                        SetCurrentUrlParams(new Dictionary<string, object> {{"id", cft.Id}});
                    RefreshPage();
                }
            }

            if (needCloseForm)
                JS.Write(
                    "parent.cashFlow_RecordsAdd ? parent.CloseDialog(parent.cashFlow_RecordsAdd.form, null, 0) : v4_closeWindow();");
        }

        /// <summary>
        ///     Кнопка: Удалить
        /// </summary>
        private void DeleteData()
        {
            cft.Delete();
            JS.Write("if (parent.RefreshData) {{ parent.RefreshData(false); }}");
            JS.Write(
                "parent.cashFlow_RecordsAdd ? parent.CloseDialog(parent.cashFlow_RecordsAdd.form, null, 0) : v4_closeWindow();");
        }
    }
}